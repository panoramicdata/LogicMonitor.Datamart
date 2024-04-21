namespace LogicMonitor.Datamart;

internal class DimensionSync : LoopInterval
{
	private readonly List<string> _types;
	private readonly DatamartClient _datamartClient;
	private readonly Configuration _configuration;
	private readonly bool _throwOnError;

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
		if (_types is null || _types.Contains(nameof(DeviceDataSourceInstance)))
		{
			foreach (var dataSourceSpecification in _configuration.DataSources)
			{
				try
				{
					Logger.LogInformation($"Syncing {nameof(DeviceDataSourceInstance)}s for DataSource '{{DataSource}}'...", dataSourceSpecification.Name);
					await _datamartClient
						.SyncDeviceDataSourcesAndInstancesAsync(
							dataSourceSpecification,
							Logger,
							cancellationToken
						)
						.ConfigureAwait(false);
					Logger.LogInformation($"Syncing {nameof(DeviceDataSourceInstance)}s for DataSource '{{DataSource}}' complete.", dataSourceSpecification.Name);
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
					Logger.LogWarning(
						e,
						$"Error while syncing {nameof(DeviceDataSourceInstance)}s for DataSource '{{DataSource}}' : {{Message}}\n {{StackTrace}}",
						dataSourceSpecification.Name,
						e.Message,
						e.StackTrace);
				}
			}
		}
	}

	private async Task SyncSecondLevelDimensionsAsync(bool dimensionSyncHaltOnError, CancellationToken cancellationToken)
	{
		if (_types is null || _types.Contains(nameof(Device)))
		{
			try
			{
				Logger.LogInformation($"Syncing {nameof(Device)}s...");
				await _datamartClient
				.AddOrUpdate<Device, DeviceStoreItem>(context => context.Devices, Logger, cancellationToken)
				.ConfigureAwait(false);
				Logger.LogInformation($"Syncing {nameof(Device)}s complete.");
			}
			catch (Exception e)
			{
				Logger.LogError(
					e,
					$"Could not sync {nameof(Device)}s due to {{Message}}", e.Message
				);
				if (dimensionSyncHaltOnError)
				{
					throw;
				}
			}
		}

		if (_types is null || _types.Contains(nameof(Website)))
		{
			try
			{
				Logger.LogInformation($"Syncing {nameof(Website)}s...");
				await _datamartClient
				.AddOrUpdate<Website, WebsiteStoreItem>(context => context.Websites, Logger, cancellationToken)
				.ConfigureAwait(false);
				Logger.LogInformation($"Syncing {nameof(Website)}s complete.");
			}
			catch (Exception e)
			{
				Logger.LogError(
					e,
					$"Could not sync {nameof(Website)}s due to {{Message}}", e.Message
				);
				if (dimensionSyncHaltOnError)
				{
					throw;
				}
			}
		}

		if (_types is null || _types.Contains(nameof(Collector)))
		{
			try
			{
				Logger.LogInformation($"Syncing {nameof(Collector)}s...");
				await _datamartClient
				.AddOrUpdate<Collector, CollectorStoreItem>(context => context.Collectors, Logger, cancellationToken)
				.ConfigureAwait(false);
				Logger.LogInformation($"Syncing {nameof(Collector)}s complete.");
			}
			catch (Exception e)
			{
				Logger.LogError(
					e,
					$"Could not sync {nameof(Collector)}s due to {{Message}}", e.Message
				);
				if (dimensionSyncHaltOnError)
				{
					throw;
				}
			}
		}
	}

	private async Task SyncTopLevelDimensionsAsync(bool haltOnError, CancellationToken cancellationToken)
	{
		if (_types is null || _types.Contains(nameof(CollectorGroup)))
		{
			try
			{
				Logger.LogInformation($"Syncing {nameof(CollectorGroup)}s...");
				await _datamartClient
				.AddOrUpdate<CollectorGroup, CollectorGroupStoreItem>(
					context => context.CollectorGroups,
					Logger,
					cancellationToken)
				.ConfigureAwait(false);
				Logger.LogInformation($"Syncing {nameof(CollectorGroup)}s complete.");
			}
			catch (Exception e)
			{
				Logger.LogError(
					e,
					$"Could not sync {nameof(CollectorGroup)}s due to {{Message}}", e.Message
				);
				if (haltOnError)
				{
					throw;
				}
			}
		}

		if (_types is null || _types.Contains(nameof(EscalationChain)))
		{
			try
			{
				Logger.LogInformation($"Syncing {nameof(EscalationChain)}s...");
				await _datamartClient
					.AddOrUpdate<EscalationChain, EscalationChainStoreItem>(
						context => context.EscalationChains,
						Logger,
						cancellationToken
					)
					.ConfigureAwait(false);
				Logger.LogInformation($"Syncing {nameof(EscalationChain)}s complete.");
			}
			catch (Exception e)
			{
				Logger.LogError(
					e,
					$"Could not sync {nameof(EscalationChain)}s due to {{Message}}", e.Message
				);
				if (haltOnError)
				{
					throw;
				}
			}
		}

		if (_types is null || _types.Contains(nameof(AlertRule)))
		{
			try
			{
				Logger.LogInformation($"Syncing {nameof(AlertRule)}s...");
				await _datamartClient
					.AddOrUpdate<AlertRule, AlertRuleStoreItem>(
						context => context.AlertRules,
						Logger,
						cancellationToken)
					.ConfigureAwait(false);
				Logger.LogInformation($"Syncing {nameof(AlertRule)}s complete.");
			}
			catch (Exception e)
			{
				Logger.LogError(
					e,
					$"Could not sync {nameof(AlertRule)}s due to {{Message}}", e.Message
				);
				if (haltOnError)
				{
					throw;
				}
			}
		}

		if (_types is null || _types.Contains(nameof(DataSource)))
		{
			try
			{
				Logger.LogInformation($"Syncing {nameof(DataSource)}s...");
				await _datamartClient
					.AddOrUpdate<DataSource, DataSourceStoreItem>(
						context => context.DataSources,
						Logger,
						cancellationToken)
					.ConfigureAwait(false);
				Logger.LogInformation($"Syncing {nameof(DataSource)}s complete.");
			}
			catch (Exception e)
			{
				Logger.LogError(
					e,
					$"Could not sync {nameof(DataSource)}s due to {{Message}}", e.Message
				);
				if (haltOnError)
				{
					throw;
				}
			}
		}

		if (_types is null || _types.Contains(nameof(ConfigSource)))
		{
			try
			{
				Logger.LogInformation($"Syncing {nameof(ConfigSource)}s...");
				await _datamartClient
				.AddOrUpdate<ConfigSource, ConfigSourceStoreItem>(
					context => context.ConfigSources,
					Logger,
					cancellationToken).ConfigureAwait(false);
				Logger.LogInformation($"Syncing {nameof(ConfigSource)}s complete.");
			}
			catch (Exception e)
			{
				Logger.LogError(
					e,
					$"Could not sync {nameof(ConfigSource)}s due to {{Message}}", e.Message
				);
				if (haltOnError)
				{
					throw;
				}
			}
		}

		if (_types is null || _types.Contains(nameof(EventSource)))
		{
			try
			{
				Logger.LogInformation($"Syncing {nameof(EventSource)}s...");
				await _datamartClient
				.AddOrUpdate<EventSource, EventSourceStoreItem>(
					context => context.EventSources,
					Logger,
					cancellationToken)
				.ConfigureAwait(false);
				Logger.LogInformation($"Syncing {nameof(EventSource)}s complete.");
			}
			catch (Exception e)
			{
				Logger.LogError(
					e,
					$"Could not sync {nameof(EventSource)}s due to {{Message}}", e.Message
				);
				if (haltOnError)
				{
					throw;
				}
			}

		}

		if (_types is null || _types.Contains(nameof(DeviceGroup)))
		{
			try
			{
				Logger.LogInformation($"Syncing {nameof(DeviceGroup)}s...");
				await _datamartClient
				.AddOrUpdate<DeviceGroup, DeviceGroupStoreItem>(
					context => context.DeviceGroups,
					Logger,
					cancellationToken)
				.ConfigureAwait(false);
				Logger.LogInformation($"Syncing {nameof(DeviceGroup)}s complete.");
			}
			catch (Exception e)
			{
				Logger.LogError(
					e,
					$"Could not sync {nameof(DeviceGroup)}s due to {{Message}}", e.Message
				);
				if (haltOnError)
				{
					throw;
				}
			}
		}

		if (_types is null || _types.Contains(nameof(WebsiteGroup)))
		{
			try
			{
				Logger.LogInformation($"Syncing {nameof(WebsiteGroup)}s...");
				await _datamartClient
				.AddOrUpdate<WebsiteGroup, WebsiteGroupStoreItem>(
					context => context.WebsiteGroups,
					Logger,
					cancellationToken)
				.ConfigureAwait(false);
				Logger.LogInformation($"Syncing {nameof(ConfigSource)}s complete.");
			}
			catch (Exception e)
			{
				Logger.LogError(
					e,
					$"Could not sync {nameof(ConfigSource)}s due to {{Message}}", e.Message
				);
				if (haltOnError)
				{
					throw;
				}
			}
		}


		if (_types is null || _types.Contains(nameof(LogicModuleUpdate)))
		{
			try
			{
				Logger.LogInformation($"Syncing {nameof(LogicModuleUpdate)}s...");
				await _datamartClient
				.AddOrUpdateLogicModuleUpdates(
					context => context.LogicModuleUpdates,
					Logger,
					cancellationToken)
				.ConfigureAwait(false);
				Logger.LogInformation($"Syncing {nameof(LogicModuleUpdate)}s complete.");
			}
			catch (Exception e)
			{
				Logger.LogError(
					e,
					$"Could not sync {nameof(LogicModuleUpdate)}s due to {{Message}}", e.Message
				);
				if (haltOnError)
				{
					throw;
				}
			}
		}
	}
}
