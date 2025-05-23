﻿using LogicMonitor.Datamart.Interfaces;
using LogicMonitor.Datamart.Notifications;

namespace LogicMonitor.Datamart;

internal class DimensionSync : LoopInterval
{
	private readonly List<string>? _types;
	private readonly INotificationReceiver _notificationReceiver;
	private readonly DatamartClient _datamartClient;
	private readonly Configuration _configuration;

	public DimensionSync(
		DatamartClient datamartClient,
		Configuration configuration,
		ILoggerFactory loggerFactory,
		INotificationReceiver? notificationReceiver)
		: base(nameof(DimensionSync), loggerFactory)
	{
		_notificationReceiver = notificationReceiver ?? new NullNotificationReceiver();
		_datamartClient = datamartClient;
		_configuration = configuration;
	}

	public DimensionSync(
		DatamartClient datamartClient,
		Configuration configuration,
		List<string> types,
		ILoggerFactory loggerFactory,
		INotificationReceiver? notificationReceiver)
		: base(nameof(DimensionSync), loggerFactory)
	{
		_types = types;
		_datamartClient = datamartClient;
		_configuration = configuration;
		_notificationReceiver = notificationReceiver ?? new NullNotificationReceiver();
	}

	public override async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		try
		{
			await _notificationReceiver
				.SetStageNameAsync("Applying migrations to database", cancellationToken)
				.ConfigureAwait(false);

			await _datamartClient
				.EnsureDatabaseCreatedAndSchemaUpdatedAsync(cancellationToken)
				.ConfigureAwait(false);

			// Top level
			await SyncTopLevelDimensionsAsync(
				_configuration.DimensionSyncHaltOnError,
				cancellationToken)
				.ConfigureAwait(false);

			// Second level
			await SyncSecondLevelDimensionsAsync(
				_configuration.DimensionSyncHaltOnError,
				cancellationToken)
				.ConfigureAwait(false);

			// DeviceDataSources and DeviceDataSourceInstances
			await SyncThirdLevelDimensionsAsync(cancellationToken)
				.ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			Logger.LogError(
				ex,
				$"An unexpected error occurred during the {nameof(DimensionSync)}: '{{Message}}' using '{{Configuration}}'.  Stack trace: {{StackTrace}}",
				ex.Message,
				_configuration.ToString(),
				ex.StackTrace
			);
		}
	}

	private async Task SyncThirdLevelDimensionsAsync(CancellationToken cancellationToken)
	{
		// ResourceDataSourceInstances
		if (_types?.Contains(nameof(ResourceDataSourceInstance)) ?? true)
		{
			var stopwatch = new Stopwatch();
			var dataSourceIndex = 0;
			foreach (var dataSourceSpecification in _configuration.DataSources)
			{
				try
				{
					dataSourceIndex++;
					var dataSourceCount = _configuration.DataSources.Count;
					Logger.LogInformation(
						$"Syncing {nameof(ResourceDataSourceInstance)}s for DataSource {{DataSource}} ({{DataSourceIndex}}/{{DataSourceCount}})...",
						dataSourceSpecification.Name,
						dataSourceIndex,
						dataSourceCount);

					await _notificationReceiver
						.SetStageNameAsync($"DataSource {dataSourceSpecification.Name} ({dataSourceIndex}/{dataSourceCount})", cancellationToken)
						.ConfigureAwait(false);

					stopwatch.Restart();

					await _datamartClient
						.SyncDeviceLogicModuleSourcesAndInstancesAsync(
							dataSourceSpecification,
							dataSourceIndex,
							dataSourceCount,
							_notificationReceiver,
							Logger,
							cancellationToken
						)
						.ConfigureAwait(false);

					Logger.LogInformation(
						$"Syncing {nameof(ResourceDataSourceInstance)}s for DataSource '{{DataSource}}' complete after {{TimeSeconds}}s.",
						dataSourceSpecification.Name,
						stopwatch.Elapsed.TotalSeconds);
				}
				catch (Exception ex)
				{
					Logger.LogError(
						ex,
						$"Unable to sync {nameof(ResourceDataSourceInstance)}s for DataSource '{{DataSource}}': {{Message}}",
						dataSourceSpecification.Name,
						ex.Message);
					if (_configuration.DimensionSyncHaltOnError)
					{
						throw;
					}
				}
			}

			// ResourceConfigSources
			var configSourceIndex = 0;
			var configSourceCount = _configuration.ConfigSources.Count;
			foreach (var configSourceSpecification in _configuration.ConfigSources)
			{
				try
				{
					configSourceIndex++;
					Logger.LogInformation(
						$"Syncing {nameof(ResourceDataSourceInstance)}s for ConfigSource {{ConfigSource}} ({{ConfigSourceIndex}}/{{ConfigSourceCount}})...",
						configSourceSpecification.Name,
						configSourceIndex,
						configSourceCount);
					await _notificationReceiver
						.SetStageNameAsync($"ConfigSource {configSourceSpecification.Name} ({configSourceIndex}/{configSourceCount})", cancellationToken)
						.ConfigureAwait(false);

					stopwatch.Restart();
					await _datamartClient
						.SyncDeviceLogicModuleSourcesAndInstancesAsync(
							configSourceSpecification,
							configSourceIndex,
							configSourceCount,
							_notificationReceiver,
							Logger,
							cancellationToken
						)
						.ConfigureAwait(false);
					Logger.LogInformation(
						$"Syncing {nameof(ResourceDataSourceInstance)}s for ConfigSource '{{ConfigSource}}' complete after {{TimeSeconds}}s.",
						configSourceSpecification.Name,
						stopwatch.Elapsed.TotalSeconds);
				}
				catch (Exception ex)
				{
					Logger.LogError(
						ex,
						$"Unable to sync {nameof(ResourceDataSourceInstance)}s for ConfigSource '{{ConfigSource}}': {{Message}}",
						configSourceSpecification.Name,
						ex.Message);
					if (_configuration.DimensionSyncHaltOnError)
					{
						throw;
					}
				}
			}
		}
	}

	private async Task SyncSecondLevelDimensionsAsync(
		bool haltOnError,
		CancellationToken cancellationToken)
	{
		if (_types?.Contains(nameof(Resource)) ?? true)
		{
			await _datamartClient
			.AddOrUpdate<Resource, ResourceStoreItem>(
				context => context.Devices,
				haltOnError,
				Logger,
				_notificationReceiver,
				cancellationToken)
			.ConfigureAwait(false);
		}

		if (_types?.Contains(nameof(Website)) ?? true)
		{
			await _datamartClient
			.AddOrUpdate<Website, WebsiteStoreItem>(
				context => context.Websites,
				haltOnError,
				Logger,
				_notificationReceiver,
				cancellationToken)
			.ConfigureAwait(false);
		}

		if (_types?.Contains(nameof(Collector)) ?? true)
		{
			await _datamartClient
			.AddOrUpdate<Collector, CollectorStoreItem>(
				context => context.Collectors,
				haltOnError,
				Logger,
				_notificationReceiver,
				cancellationToken)
			.ConfigureAwait(false);
		}
	}

	private async Task SyncTopLevelDimensionsAsync(
		bool haltOnError,
		CancellationToken cancellationToken)
	{
		if (_types?.Contains(nameof(CollectorGroup)) ?? true)
		{
			await _datamartClient
			.AddOrUpdate<CollectorGroup, CollectorGroupStoreItem>(
				context => context.CollectorGroups,
				haltOnError,
				Logger,
				_notificationReceiver,
				cancellationToken)
			.ConfigureAwait(false);
		}

		if (_types?.Contains(nameof(EscalationChain)) ?? true)
		{
			await _datamartClient
				.AddOrUpdate<EscalationChain, EscalationChainStoreItem>(
					context => context.EscalationChains,
					haltOnError,
					Logger,
					_notificationReceiver,
					cancellationToken
				)
				.ConfigureAwait(false);
		}

		if (_types?.Contains(nameof(AlertRule)) ?? true)
		{
			await _datamartClient
				.AddOrUpdate<AlertRule, AlertRuleStoreItem>(
					context => context.AlertRules,
					haltOnError,
					Logger,
					_notificationReceiver,
					cancellationToken)
				.ConfigureAwait(false);
		}

		if (_types?.Contains(nameof(DataSource)) ?? true)
		{
			await _datamartClient
				.AddOrUpdate<DataSource, DataSourceStoreItem>(
					context => context.DataSources,
					haltOnError,
					Logger,
					_notificationReceiver,
					cancellationToken)
				.ConfigureAwait(false);
		}

		if (_types?.Contains(nameof(ConfigSource)) ?? true)
		{
			await _datamartClient
				.AddOrUpdate<ConfigSource, ConfigSourceStoreItem>(
					context => context.ConfigSources,
					haltOnError,
					Logger,
					_notificationReceiver,
					cancellationToken)
				.ConfigureAwait(false);
			Logger.LogInformation($"Syncing {nameof(ConfigSource)}s complete.");
		}

		if (_types?.Contains(nameof(EventSource)) ?? true)
		{
			await _datamartClient
				.AddOrUpdate<EventSource, EventSourceStoreItem>(
					context => context.EventSources,
					haltOnError,
					Logger,
					_notificationReceiver,
					cancellationToken)
				.ConfigureAwait(false);
		}

		if (_types?.Contains(nameof(Integration)) ?? true)
		{
			// It's possible that new Integrations are not supported.
			// For now, we'll just log an error and continue.
			try
			{
				await _datamartClient
					.AddOrUpdate<Integration, IntegrationStoreItem>(
						context => context.Integrations,
						haltOnError,
						Logger,
						_notificationReceiver,
						cancellationToken)
					.ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, "Unable to sync Integrations {Message}.", ex.Message);
			}
		}

		if (_types?.Contains(nameof(ResourceGroup)) ?? true)
		{
			await _datamartClient
				.AddOrUpdate<ResourceGroup, ResourceGroupStoreItem>(
					context => context.DeviceGroups,
					haltOnError,
					Logger,
					_notificationReceiver,
					cancellationToken)
				.ConfigureAwait(false);
		}

		if (_types?.Contains(nameof(WebsiteGroup)) ?? true)
		{
			await _datamartClient
				.AddOrUpdate<WebsiteGroup, WebsiteGroupStoreItem>(
					context => context.WebsiteGroups,
					haltOnError,
					Logger,
					_notificationReceiver,
					cancellationToken)
				.ConfigureAwait(false);
			Logger.LogInformation($"Syncing {nameof(ConfigSource)}s complete.");
		}


		if (_types?.Contains(nameof(LogicModuleUpdate)) ?? true)
		{
			await _datamartClient
				.AddOrUpdateLogicModuleUpdates(
					context => context.LogicModuleUpdates,
					haltOnError,
					Logger,
					_notificationReceiver,
					cancellationToken)
				.ConfigureAwait(false);
		}
	}
}
