using LogicMonitor.Datamart.Interfaces;
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
			await SyncTopLevelDimensionsAsync(_configuration.DimensionSyncHaltOnError, cancellationToken)
				.ConfigureAwait(false);

			// Second level
			await SyncSecondLevelDimensionsAsync(_configuration.DimensionSyncHaltOnError, cancellationToken)
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
		if (_types?.Contains(nameof(DeviceDataSourceInstance)) ?? true)
		{
			foreach (var dataSourceSpecification in _configuration.DataSources)
			{
				await _datamartClient
					.SyncDeviceDataSourcesAndInstancesAsync(
						dataSourceSpecification,
						Logger,
						cancellationToken
					)
					.ConfigureAwait(false);
				Logger.LogInformation($"Syncing {nameof(DeviceDataSourceInstance)}s for DataSource '{{DataSource}}' complete.", dataSourceSpecification.Name);
			}
		}
	}

	private async Task SyncSecondLevelDimensionsAsync(bool haltOnError, CancellationToken cancellationToken)
	{
		if (_types?.Contains(nameof(Device)) ?? true)
		{
			await _datamartClient
			.AddOrUpdate<Device, DeviceStoreItem>(
				context => context.Devices,
				haltOnError,
				Logger,
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
				cancellationToken)
			.ConfigureAwait(false);
		}
	}

	private async Task SyncTopLevelDimensionsAsync(bool haltOnError, CancellationToken cancellationToken)
	{
		if (_types?.Contains(nameof(CollectorGroup)) ?? true)
		{
			await _datamartClient
			.AddOrUpdate<CollectorGroup, CollectorGroupStoreItem>(
				context => context.CollectorGroups,
				haltOnError,
				Logger,
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
				cancellationToken).ConfigureAwait(false);
			Logger.LogInformation($"Syncing {nameof(ConfigSource)}s complete.");
		}

		if (_types?.Contains(nameof(EventSource)) ?? true)
		{
			await _datamartClient
			.AddOrUpdate<EventSource, EventSourceStoreItem>(
				context => context.EventSources,
				haltOnError,
				Logger,
				cancellationToken)
			.ConfigureAwait(false);
		}

		if (_types?.Contains(nameof(DeviceGroup)) ?? true)
		{
			await _datamartClient
			.AddOrUpdate<DeviceGroup, DeviceGroupStoreItem>(
				context => context.DeviceGroups,
				haltOnError,
				Logger,
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
				cancellationToken)
			.ConfigureAwait(false);
		}
	}
}
