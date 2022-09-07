namespace LogicMonitor.Datamart;

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
		await _datamartClient.AddOrUpdate<AlertRule, AlertRuleStoreItem>(context => context.AlertRules, Logger, cancellationToken).ConfigureAwait(false);
		await _datamartClient.AddOrUpdate<DataSource, DataSourceStoreItem>(context => context.DataSources, Logger, cancellationToken).ConfigureAwait(false);
		await _datamartClient.AddOrUpdate<DeviceGroup, DeviceGroupStoreItem>(context => context.DeviceGroups, Logger, cancellationToken).ConfigureAwait(false);
		await _datamartClient.AddOrUpdate<CollectorGroup, CollectorGroupStoreItem>(context => context.CollectorGroups, Logger, cancellationToken).ConfigureAwait(false);
		await _datamartClient.AddOrUpdate<ConfigSource, ConfigSourceStoreItem>(context => context.ConfigSources, Logger, cancellationToken).ConfigureAwait(false);
		await _datamartClient.AddOrUpdate<EscalationChain, EscalationChainStoreItem>(context => context.EscalationChains, Logger, cancellationToken).ConfigureAwait(false);
		await _datamartClient.AddOrUpdate<EventSource, EventSourceStoreItem>(context => context.EventSources, Logger, cancellationToken).ConfigureAwait(false);
		await _datamartClient.AddOrUpdate<WebsiteGroup, WebsiteGroupStoreItem>(context => context.WebsiteGroups, Logger, cancellationToken).ConfigureAwait(false);

		// Second level
		await _datamartClient.AddOrUpdate<Device, DeviceStoreItem>(context => context.Devices, Logger, cancellationToken).ConfigureAwait(false);
		await _datamartClient.AddOrUpdate<Collector, CollectorStoreItem>(context => context.Collectors, Logger, cancellationToken).ConfigureAwait(false);
		await _datamartClient.AddOrUpdate<Website, WebsiteStoreItem>(context => context.Websites, Logger, cancellationToken).ConfigureAwait(false);

		// Process each DataSource
		foreach (var dataSourceSpecification in _configuration.DataSources)
		{
			try
			{
				await SyncDeviceDataSourcesAndInstancesAsync(dataSourceSpecification, cancellationToken).ConfigureAwait(false);
			}
			catch (Exception e) when (e is OperationCanceledException || e is TaskCanceledException)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					// We're done, don't loop any more
					return;
				}
				// If it was anything else then re-throw
				throw;
			}
			catch (Exception e)
			{
				Logger.LogWarning(e, "{Message}", e.Message);
			}
		}
	}

	private async Task SyncDeviceDataSourcesAndInstancesAsync(DataSourceConfigurationItem dataSourceSpecification, CancellationToken cancellationToken)
	{
		var dataSourceName = dataSourceSpecification.Name;

		// Get the DataSource
		var dataSource = await _datamartClient.GetByNameAsync<DataSource>(dataSourceName, cancellationToken).ConfigureAwait(false);
		if (dataSource is null)
		{
			throw new InvalidOperationException($"DataSource {dataSourceName} does not exist.");
		}
		// We have the DataSource

		// Get the Devices that match the appliesTo function on the DataSource
		var appliesToMatches = await _datamartClient.GetAppliesToAsync(dataSource.AppliesTo, cancellationToken).ConfigureAwait(false);

		Logger.LogDebug(
			"Syncing {DataSourceName} instances for {AppliesToMatchesCount} devices",
			dataSourceName,
			appliesToMatches.Count);

		using var context = new Context(_datamartClient.DbContextOptions);
		var markedMissing = 0;

		// Not all of these will have instances
		foreach (var appliesToMatch in appliesToMatches)
		{
			// Get the device
			var device = await _datamartClient
				.GetAsync<Device>(appliesToMatch.Id, cancellationToken)
				.ConfigureAwait(false);

			// Get the DeviceDataSource
			var deviceDataSource = await _datamartClient
				.GetDeviceDataSourceByDeviceIdAndDataSourceIdAsync(device.Id, dataSource.Id, cancellationToken)
				.ConfigureAwait(false);
			if (deviceDataSource is null)
			{
				Logger.LogTrace(
					"No DeviceDataSource for Device:{DeviceDisplayName}, DataSource:{DataSourceName}",
					device.DisplayName,
					dataSource.Name);
				continue;
			}
			// We have a DeviceDataSource
			Logger.LogTrace(
				"DeviceDataSource fetched for Device:{DeviceDisplayName}, DataSource:{DataSourceName}",
				device.DisplayName,
				dataSource.Name);

			// Ensure that this DeviceDataSource exists in the database
			var databaseDeviceDataSource = await context
				.DeviceDataSources
				.SingleOrDefaultAsync(dds =>
						dds.DeviceId == deviceDataSource.DeviceId
						&& dds.DataSourceId == deviceDataSource.DataSourceId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);
			if (databaseDeviceDataSource == null)
			{
				// Add it to the database
				context.DeviceDataSources.Add(DatamartClient.MapperInstance.Map<DeviceDataSourceStoreItem>(deviceDataSource));
			}
			else
			{
				// Update the existing entry
				deviceDataSource = DatamartClient.MapperInstance.Map(databaseDeviceDataSource, deviceDataSource);
			}
			// It is now in the database context

			// Fetch the DeviceDataSourceInstances
			var apiDeviceDataSourceInstances = await _datamartClient
				.GetAllDeviceDataSourceInstancesAsync(
					device.Id,
					deviceDataSource.Id,
					null,
					cancellationToken)
				.ConfigureAwait(false);

			foreach (var apiDeviceDataSourceInstance in apiDeviceDataSourceInstances)
			{
				Logger.LogDebug(
					"Device {DeviceDisplayName}, Instance: {ApiDeviceDataSourceInstanceName}",
					device.DisplayName,
					apiDeviceDataSourceInstance.Name);

				// Ensure that this DeviceDataSourceInstance exists in the database
				var databaseDeviceDataSourceInstance = await context
					.DeviceDataSourceInstances
					.SingleOrDefaultAsync(dddsi => dddsi.Id == apiDeviceDataSourceInstance.Id, cancellationToken: cancellationToken)
					.ConfigureAwait(false);
				if (databaseDeviceDataSourceInstance == null)
				{
					// Add it to the database
					context.DeviceDataSourceInstances.Add(DatamartClient.MapperInstance.Map<DeviceDataSourceInstanceStoreItem>(apiDeviceDataSourceInstance));
				}
				else
				{
					// Update - including clearing the LastWentMissingUtc field
					// Update the existing entry using AutoMapper
					databaseDeviceDataSourceInstance = DatamartClient.MapperInstance.Map(apiDeviceDataSourceInstance, databaseDeviceDataSourceInstance);
					databaseDeviceDataSourceInstance.LastWentMissingUtc = null;
				}
				// It is now in the database context
			}

			// It's possible that there are entries in the database that are no longer brought back from the API, due to instances being deleted by Active Discovery/manual deletion
			// Get all database instances where the
			var databaseDeviceDataSourceInstanceIdsThatShouldHaveComeBackFromApi = new HashSet<int>(await context
					.DeviceDataSourceInstances
					.Where(ddsi => ddsi.DeviceId == device.Id && ddsi.DataSourceId == dataSource.Id && ddsi.LastWentMissingUtc == null)
					.Select(ddsi => ddsi.Id)
					.ToListAsync(cancellationToken: cancellationToken)
					.ConfigureAwait(false));

			var apiDeviceDataSourceInstanceIds = new HashSet<int>(apiDeviceDataSourceInstances.Select(ddsi => ddsi.Id));

			var deviceDatasourceInstanceIdsToMarkMissing = databaseDeviceDataSourceInstanceIdsThatShouldHaveComeBackFromApi.Except(apiDeviceDataSourceInstanceIds).ToList();
			if (deviceDatasourceInstanceIdsToMarkMissing.Count > 0)
			{
				foreach (var deviceDatasourceInstanceIdToMarkMissing in deviceDatasourceInstanceIdsToMarkMissing)
				{
					// Get the entry to modify from the context and update it
					var databaseDeviceDataSourceInstance = await context
						.DeviceDataSourceInstances
						.SingleOrDefaultAsync(dddsi => dddsi.Id == deviceDatasourceInstanceIdToMarkMissing, cancellationToken: cancellationToken)
						.ConfigureAwait(false);

					if (databaseDeviceDataSourceInstance is null)
					{
						continue;
					}

					databaseDeviceDataSourceInstance.LastWentMissingUtc = DateTime.UtcNow;
				}

				markedMissing += deviceDatasourceInstanceIdsToMarkMissing.Count;
			}

			// Check for any that were NOT in the entries that came back from the API
		}

		var added = context.ChangeTracker.Entries().Count(e => e.State == EntityState.Added);
		var modified = context.ChangeTracker.Entries().Count(e => e.State == EntityState.Modified);
		var total = context.ChangeTracker.Entries().Count();
		var rowsAffected = await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
		Logger.LogInformation(
			"Sync completed for {DataSourceName}; Total {Total}; Added {Added}; Modified {Modified} ({MarkedMissing:N0} MarkedMissing).",
			dataSourceName,
			total,
			added,
			modified,
			markedMissing);
	}
}
