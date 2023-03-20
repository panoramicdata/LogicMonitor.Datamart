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
		await SyncTopLevelDimensionsAsync(cancellationToken)
			.ConfigureAwait(false);

		// Second level
		await SyncSecondLevelDimensionsAsync(cancellationToken)
			.ConfigureAwait(false);

		// DeviceDataSources and DeviceDataSourceInstances
		await SyncThirdLevelDimensionsAsync(cancellationToken)
			.ConfigureAwait(false);
	}

	private async Task SyncThirdLevelDimensionsAsync(CancellationToken cancellationToken)
	{
		foreach (var dataSourceSpecification in _configuration.DataSources)
		{
			try
			{
				await _datamartClient
					.SyncDeviceDataSourcesAndInstancesAsync(
						dataSourceSpecification,
						Logger,
						cancellationToken
					)
					.ConfigureAwait(false);
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

	private async Task SyncSecondLevelDimensionsAsync(CancellationToken cancellationToken)
	{
		await _datamartClient
			.AddOrUpdate<Device, DeviceStoreItem>(context => context.Devices, Logger, cancellationToken)
			.ConfigureAwait(false);
		await _datamartClient
			.AddOrUpdate<Website, WebsiteStoreItem>(context => context.Websites, Logger, cancellationToken)
			.ConfigureAwait(false);
		await _datamartClient
			.AddOrUpdate<Collector, CollectorStoreItem>(context => context.Collectors, Logger, cancellationToken)
			.ConfigureAwait(false);
	}

	private async Task SyncTopLevelDimensionsAsync(CancellationToken cancellationToken)
	{
		await _datamartClient
			.AddOrUpdate<CollectorGroup, CollectorGroupStoreItem>(
				context => context.CollectorGroups,
				Logger,
				cancellationToken)
			.ConfigureAwait(false);

		await _datamartClient
			.AddOrUpdate<EscalationChain, EscalationChainStoreItem>(
				context => context.EscalationChains,
				Logger,
				cancellationToken
			)
			.ConfigureAwait(false);

		await _datamartClient
			.AddOrUpdate<AlertRule, AlertRuleStoreItem>(
				context => context.AlertRules,
				Logger,
				cancellationToken)
			.ConfigureAwait(false);
		await _datamartClient
			.AddOrUpdate<DataSource, DataSourceStoreItem>(
				context => context.DataSources,
				Logger,
				cancellationToken)
			.ConfigureAwait(false);
		await _datamartClient
			.AddOrUpdate<ConfigSource, ConfigSourceStoreItem>(
				context => context.ConfigSources,
				Logger,
				cancellationToken).ConfigureAwait(false);
		await _datamartClient
			.AddOrUpdate<EventSource, EventSourceStoreItem>(
				context => context.EventSources,
				Logger,
				cancellationToken)
			.ConfigureAwait(false);
		await _datamartClient
			.AddOrUpdate<DeviceGroup, DeviceGroupStoreItem>(
				context => context.DeviceGroups,
				Logger,
				cancellationToken)
			.ConfigureAwait(false);
		await _datamartClient
			.AddOrUpdate<WebsiteGroup, WebsiteGroupStoreItem>(
				context => context.WebsiteGroups,
				Logger,
				cancellationToken)
			.ConfigureAwait(false);
	}
}
