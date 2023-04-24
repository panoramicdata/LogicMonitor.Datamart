namespace LogicMonitor.Datamart;

internal class DimensionSync : LoopInterval
{
	private readonly List<string> _types;
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

	public DimensionSync(
		DatamartClient datamartClient,
		Configuration configuration,
		List<string> types,
		ILoggerFactory loggerFactory)
		: base(nameof(DimensionSync), loggerFactory)
	{
		_types = types;
		_datamartClient = datamartClient;
		_configuration = configuration;
	}

	public override async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		try
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
		if (_types is null || _types.Contains(nameof(DeviceDataSourceInstance)))
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
	}

	private async Task SyncSecondLevelDimensionsAsync(CancellationToken cancellationToken)
	{
		if (_types is null || _types.Contains(nameof(Device)))
		{
			await _datamartClient
			.AddOrUpdate<Device, DeviceStoreItem>(context => context.Devices, Logger, cancellationToken)
			.ConfigureAwait(false);
		}

		if (_types is null || _types.Contains(nameof(Website)))
		{
			await _datamartClient
			.AddOrUpdate<Website, WebsiteStoreItem>(context => context.Websites, Logger, cancellationToken)
			.ConfigureAwait(false);
		}

		if (_types is null || _types.Contains(nameof(Collector)))
		{
			await _datamartClient
			.AddOrUpdate<Collector, CollectorStoreItem>(context => context.Collectors, Logger, cancellationToken)
			.ConfigureAwait(false);
		}
	}

	private async Task SyncTopLevelDimensionsAsync(CancellationToken cancellationToken)
	{
		if (_types is null || _types.Contains(nameof(CollectorGroup)))
		{
			await _datamartClient
			.AddOrUpdate<CollectorGroup, CollectorGroupStoreItem>(
				context => context.CollectorGroups,
				Logger,
				cancellationToken)
			.ConfigureAwait(false);
		}

		if (_types is null || _types.Contains(nameof(EscalationChain)))
		{
			await _datamartClient
			.AddOrUpdate<EscalationChain, EscalationChainStoreItem>(
				context => context.EscalationChains,
				Logger,
				cancellationToken
			)
			.ConfigureAwait(false);
		}

		if (_types is null || _types.Contains(nameof(AlertRule)))
		{

			await _datamartClient
			.AddOrUpdate<AlertRule, AlertRuleStoreItem>(
				context => context.AlertRules,
				Logger,
				cancellationToken)
			.ConfigureAwait(false);
		}

		if (_types is null || _types.Contains(nameof(DataSource)))
		{
			await _datamartClient
			.AddOrUpdate<DataSource, DataSourceStoreItem>(
				context => context.DataSources,
				Logger,
				cancellationToken)
			.ConfigureAwait(false);
		}

		if (_types is null || _types.Contains(nameof(ConfigSource)))
		{
			await _datamartClient
			.AddOrUpdate<ConfigSource, ConfigSourceStoreItem>(
				context => context.ConfigSources,
				Logger,
				cancellationToken).ConfigureAwait(false);
		}

		if (_types is null || _types.Contains(nameof(EventSource)))
		{
			await _datamartClient
			.AddOrUpdate<EventSource, EventSourceStoreItem>(
				context => context.EventSources,
				Logger,
				cancellationToken)
			.ConfigureAwait(false);
		}

		if (_types is null || _types.Contains(nameof(DeviceGroup)))
		{
			await _datamartClient
			.AddOrUpdate<DeviceGroup, DeviceGroupStoreItem>(
				context => context.DeviceGroups,
				Logger,
				cancellationToken)
			.ConfigureAwait(false);
		}

		if (_types is null || _types.Contains(nameof(WebsiteGroup)))
		{
			await _datamartClient
			.AddOrUpdate<WebsiteGroup, WebsiteGroupStoreItem>(
				context => context.WebsiteGroups,
				Logger,
				cancellationToken)
			.ConfigureAwait(false);
		}
	}
}
