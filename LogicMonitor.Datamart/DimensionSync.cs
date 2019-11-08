using AutoMapper;
using LogicMonitor.Api.Alerts;
using LogicMonitor.Api.Collectors;
using LogicMonitor.Api.Devices;
using LogicMonitor.Api.Filters;
using LogicMonitor.Api.LogicModules;
using LogicMonitor.Api.Settings;
using LogicMonitor.Api.Websites;
using LogicMonitor.Datamart.Config;
using LogicMonitor.Datamart.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LogicMonitor.Datamart
{
	internal class DimensionSync : LoopInterval
	{
		private readonly DatamartClient _datamartClient;
		private readonly Configuration _configuration;

		public DimensionSync(
			DatamartClient datamartClient,
			Configuration configuration,
			ILoggerFactory loggerFactory)
			: base(nameof(DimensionSync), loggerFactory)
		{
			_datamartClient = datamartClient;
			_configuration = configuration;
		}

		public override async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			// Top level
			await _datamartClient.AddOrUpdate<AlertRule, AlertRuleStoreItem>(context => context.AlertRules, cancellationToken).ConfigureAwait(false);
			await _datamartClient.AddOrUpdate<DataSource, DataSourceStoreItem>(context => context.DataSources, cancellationToken).ConfigureAwait(false);
			await _datamartClient.AddOrUpdate<DeviceGroup, DeviceGroupStoreItem>(context => context.DeviceGroups, cancellationToken).ConfigureAwait(false);
			await _datamartClient.AddOrUpdate<CollectorGroup, CollectorGroupStoreItem>(context => context.CollectorGroups, cancellationToken).ConfigureAwait(false);
			await _datamartClient.AddOrUpdate<ConfigSource, ConfigSourceStoreItem>(context => context.ConfigSources, cancellationToken).ConfigureAwait(false);
			await _datamartClient.AddOrUpdate<EscalationChain, EscalationChainStoreItem>(context => context.EscalationChains, cancellationToken).ConfigureAwait(false);
			await _datamartClient.AddOrUpdate<EventSource, EventSourceStoreItem>(context => context.EventSources, cancellationToken).ConfigureAwait(false);
			await _datamartClient.AddOrUpdate<WebsiteGroup, WebsiteGroupStoreItem>(context => context.WebsiteGroups, cancellationToken).ConfigureAwait(false);

			// Second level
			await _datamartClient.AddOrUpdate<Device, DeviceStoreItem>(context => context.Devices, cancellationToken).ConfigureAwait(false);
			await _datamartClient.AddOrUpdate<Collector, CollectorStoreItem>(context => context.Collectors, cancellationToken).ConfigureAwait(false);
			await _datamartClient.AddOrUpdate<Website, WebsiteStoreItem>(context => context.Websites, cancellationToken).ConfigureAwait(false);

			// Process each DataSource
			foreach (var dataSourceSpecification in _configuration.DataSources)
			{
				try
				{
					await SyncDeviceDataSourcesAndInstancesAsync(dataSourceSpecification, cancellationToken).ConfigureAwait(false);
				}
				catch(Exception e)
				{
					Logger.LogWarning(e.Message, e);
				}
			}
		}

		private async Task SyncDeviceDataSourcesAndInstancesAsync(DataSourceConfigurationItem dataSourceSpecification, CancellationToken cancellationToken)
		{
			var dataSourceName = dataSourceSpecification.Name;
			Logger.LogDebug($"Syncing {dataSourceName}");

			// Get the DataSource
			var dataSource = await _datamartClient.GetByNameAsync<DataSource>(dataSourceName, cancellationToken).ConfigureAwait(false);
			if (dataSource == null)
			{
				throw new InvalidOperationException($"DataSource {dataSourceName} does not exist.");
			}
			// We have the DataSource

			// Get the Devices that match the appliesTo function on the DataSource
			var appliesToMatches = await _datamartClient.GetAppliesToAsync(dataSource.AppliesTo, cancellationToken).ConfigureAwait(false);

			Logger.LogInformation($"Syncing {dataSourceName} instances for {appliesToMatches.Count} devices");

			using (var context = new Context(_datamartClient.DbContextOptions))
			{
				// Not all of these will have instances
				foreach (var appliesToMatch in appliesToMatches)
				{
					// Get the device
					var device = await _datamartClient.GetAsync<Device>(appliesToMatch.Id, cancellationToken).ConfigureAwait(false);

					// Get the DeviceDataSource
					var deviceDataSource = await _datamartClient.GetDeviceDataSourceByDeviceIdAndDataSourceIdAsync(device.Id, dataSource.Id, cancellationToken).ConfigureAwait(false);
					if (dataSource == null)
					{
						Logger.LogTrace($"No DeviceDataSource for Device:{device.DisplayName}, DataSource:{dataSource.Name}");
						continue;
					}
					// We have a DeviceDataSource
					Logger.LogTrace($"DeviceDataSource fetched for Device:{device.DisplayName}, DataSource:{dataSource.Name}");

					// Ensure that this DeviceDataSource exists in the database
					var databaseDeviceDataSource = await context
						.DeviceDataSources
						.SingleOrDefaultAsync(dds => dds.DeviceId == deviceDataSource.DeviceId && dds.DataSourceId == deviceDataSource.DataSourceId)
						.ConfigureAwait(false);
					if (databaseDeviceDataSource == null)
					{
						// Add it to the database
						context.DeviceDataSources.Add(DatamartClient.MapperInstance.Map<DeviceDataSourceStoreItem>(deviceDataSource));
					}
					// It is now in the database context

					// Fetch the DeviceDataSourceInstances
					var deviceDataSourceInstances = await _datamartClient
						.GetAllDeviceDataSourceInstancesAsync(
							device.Id,
							deviceDataSource.Id,
							null,
							cancellationToken)
						.ConfigureAwait(false);

					foreach (var deviceDataSourceInstance in deviceDataSourceInstances)
					{
						Logger.LogDebug($"Device {device.DisplayName}, Instance: {deviceDataSourceInstance.Name}");

						// Ensure that this DeviceDataSourceInstance exists in the database
						var databaseDeviceDataSourceInstance = await context
							.DeviceDataSourceInstances
							.SingleOrDefaultAsync(dddsi => dddsi.Id == deviceDataSourceInstance.Id)
							.ConfigureAwait(false);
						if (databaseDeviceDataSourceInstance == null)
						{
							// Add it to the database
							context.DeviceDataSourceInstances.Add(DatamartClient.MapperInstance.Map<DeviceDataSourceInstanceStoreItem>(deviceDataSourceInstance));
						}
						// It is now in the database context
					}
				}
				var added = context.ChangeTracker.Entries().Count(e => e.State == EntityState.Added);
				var modified = context.ChangeTracker.Entries().Count(e => e.State == EntityState.Modified);
				var total = context.ChangeTracker.Entries().Count();
				var rowsAffected = await context.SaveChangesAsync().ConfigureAwait(false);
				Logger.LogInformation($"Sync completed for {dataSourceName}; Total {total}; Added {added}; Modified {modified}.");
			}
		}
	}
}
