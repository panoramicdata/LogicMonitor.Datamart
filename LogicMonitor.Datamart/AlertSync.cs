namespace LogicMonitor.Datamart;

internal class AlertSync : LoopInterval
{
	private readonly DatamartClient _datamartClient;
	private readonly DateTimeOffset _startDateTimeUtc;

	public AlertSync(
		DatamartClient datamartClient,
		DateTimeOffset startDateTimeUtc,
		ILoggerFactory loggerFactory)
		: base(nameof(AlertSync), loggerFactory)
	{
		_datamartClient = datamartClient;
		_startDateTimeUtc = startDateTimeUtc;
	}

	public async Task TruncateAlerts(CancellationToken cancellationToken)
	{
		using var context = new Context(_datamartClient.DbContextOptions);
		// Truncate the table
		await context
			.Database
			.ExecuteSqlRawAsync("TRUNCATE TABLE [Alerts]", cancellationToken)
			.ConfigureAwait(false);
	}

	public override Task ExecuteAsync(CancellationToken cancellationToken)
		=> DifferentialLoopTaskAsync(cancellationToken);

	internal async Task<UpdateAlertStats> DifferentialLoopTaskAsync(CancellationToken cancellationToken)
	{
		var nowSecondsSinceEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

		List<int> databaseDeviceIds;
		// Run the test against one instance of the context
		using (var context = new Context(_datamartClient.DbContextOptions))
		{
			// Ordered for predictability
			databaseDeviceIds = await context
						.Devices
						.Select(d => d.LogicMonitorId)
						.OrderBy(id => id)
						.ToListAsync(cancellationToken: cancellationToken)
						.ConfigureAwait(false);
		}

		var updateAlertStats = await UpdateDeviceAlerts(nowSecondsSinceEpoch, databaseDeviceIds, cancellationToken)
			.ConfigureAwait(false);

		// TODO - Non-device alerts

		return updateAlertStats;
	}

	private async Task<UpdateAlertStats> UpdateDeviceAlerts(
		long nowSecondsSinceEpoch,
		List<int> databaseDeviceIds,
		CancellationToken cancellationToken)
	{
		Logger.LogInformation(
			"Loading alerts for {DatabaseDeviceIdCount} devices...",
			databaseDeviceIds.Count);

		// Record stats
		var updateAlertStats = new UpdateAlertStats();

		// DataSource alerts
		long timeCursor;
		var deviceAlertCount = 0;
		const int pageSize = 300;
		const int maxNewAlerts = 10000;
		var alertsToBulkInsert = new Dictionary<string, AlertStoreItem>();
		var deviceIndex = 0;

		var stopwatch = new Stopwatch();

		using (var monitorObjectGroupContext = new Context(_datamartClient.DbContextOptions))
		{
			using var memoryCache = new MemoryCache(new MemoryCacheOptions());
			foreach (var deviceId in databaseDeviceIds)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					break;
				}

				try
				{
					using var context = new Context(_datamartClient.DbContextOptions);
					// Get the device
					var device = await context.Devices.SingleOrDefaultAsync(d => d.LogicMonitorId == deviceId, cancellationToken: cancellationToken).ConfigureAwait(false);

					// Build up the list of alerts in memory, then delete the table contents and re-add.
					alertsToBulkInsert.Clear();

					stopwatch.Restart();
					deviceIndex++;
					Logger.LogDebug(
						"Retrieving DataSource alerts for {DatamartClientAccountName} : Id={DeviceId}, CurrentDisplayName={DeviceDisplayName}",
						_datamartClient.AccountName,
						deviceId,
						device.DisplayName);

					// Get the  alerts

					timeCursor = Math.Max(device.LastAlertClosedTimeSeconds, _startDateTimeUtc.ToUnixTimeSeconds());
					var timeCursorLastTime = timeCursor;
					deviceAlertCount = 0;

					while (timeCursor <= nowSecondsSinceEpoch && !cancellationToken.IsCancellationRequested)
					{
						var alertFilter = new Filter<Alert>
						{
							FilterItems = new List<FilterItem<Alert>>
										{
											new Eq<Alert>(nameof(Alert.IsCleared), "*"),
											new Gt<Alert>(nameof(Alert.EndOnSeconds), timeCursor),
										},
							Order = new Order<Alert>
							{
								Property = nameof(Alert.EndOnSeconds),
								Direction = OrderDirection.Asc
							},
							Take = pageSize
						};
						var alertsThisTime = await _datamartClient.GetAllAsync(alertFilter, $"device/devices/{deviceId}/alerts", cancellationToken).ConfigureAwait(false);

						deviceAlertCount += alertsThisTime.Count;

						Logger.LogDebug(
							"Processing DataSource alerts for {DatamartClientAccountName} : Id={DeviceId}, CurrentDisplayName={DeviceDisplayName}",
							_datamartClient.AccountName,
							deviceId,
							device.DisplayName);
						var dataProcessingStopwatch = Stopwatch.StartNew();

						// A structure for reducing the alerts that came in so we only have 1 record per Id, the last one is the only one of interest
						var reducedAlerts = new Dictionary<string, Alert>();
						foreach (var networkAlert in alertsThisTime)
						{
							reducedAlerts[networkAlert.Id] = networkAlert;
						}

						var sqlFetch = new Stopwatch();
						var sqlSave = new Stopwatch();

						// We either need to update an existing alert, or bulk insert this one
						foreach (var networkAlert in reducedAlerts.Values)
						{
							sqlFetch.Start();
							// Is it already in the database?
							var databaseAlert = await context
								.Alerts
								.SingleOrDefaultAsync(asi => asi.LogicMonitorId == networkAlert.Id, cancellationToken: cancellationToken)
								.ConfigureAwait(false);
							sqlFetch.Stop();

							if (databaseAlert == null)
							{
								// No.  We will bulk insert
								var newStoreItem = DatamartClient.MapperInstance.Map<Alert, AlertStoreItem>(networkAlert);
								var utcNow = DateTimeOffset.UtcNow;
								newStoreItem.DatamartCreatedUtc = utcNow.UtcDateTime;
								newStoreItem.DatamartLastModifiedUtc = utcNow.UtcDateTime;
								await SetMonitorObjectGroupIdsAsync(monitorObjectGroupContext, networkAlert, newStoreItem, memoryCache).ConfigureAwait(false);
								alertsToBulkInsert[networkAlert.Id] = newStoreItem;
								updateAlertStats.New++;
							}
							else
							{
								// Yes. We update now with the fields that CAN change.
								databaseAlert.AckComment = networkAlert.AckComment.Truncate(50);
								databaseAlert.AckedBy = networkAlert.AckedBy.Truncate(50);
								databaseAlert.AckedOnSeconds = networkAlert.AckedOnSeconds;
								databaseAlert.ClearValue = networkAlert.ClearValue.Truncate(50);
								databaseAlert.EndOnSeconds = networkAlert.EndOnSeconds;
								databaseAlert.IsCleared = networkAlert.IsCleared;
								sqlSave.Start();
								await context
									.SaveChangesAsync(cancellationToken)
									.ConfigureAwait(false);
								sqlSave.Stop();
								updateAlertStats.Updated++;
							}
						}

						var message = $"Processed DataSource alerts for {_datamartClient.AccountName} : Id={deviceId}, CurrentDisplayName={device.DisplayName}; {reducedAlerts.Count}(of {alertsThisTime.Count}) " +
							$"dbGet({sqlFetch.ElapsedMilliseconds:N0}ms) dbSave({sqlSave.ElapsedMilliseconds:N0}ms) in {dataProcessingStopwatch.ElapsedMilliseconds:N0}ms " +
							$"from {DateTimeOffset.FromUnixTimeSeconds(timeCursor).UtcDateTime}...)";
						Logger.LogDebug("{Message}", message);

						// Update the timeCursor to point to the highest value observed in the data

						if (alertsThisTime.Count > 0)
						{
							// The timeCursor should be at least as high as it was and if there are any later EndOnSeconds, we'll update to that. This will ignore the 0's coming back from OpenAlerts.
							timeCursor = Math.Max(timeCursor, alertsThisTime.Max(a => a.EndOnSeconds));
						}

						if (alertsThisTime.Count < pageSize)
						{
							// not a full page so we got as much as we can for this time period, leave the loop
							break;
						}

						// It's possible that there are more than the max buffer count of alerts in a single second.
						// If this is the case, give up on that second and move to the next one.
						if (timeCursor == timeCursorLastTime)
						{
							// There is a situation where if there are more than 300 open alerts
							if (alertsThisTime.Max(a => a.EndOnSeconds) == 0)
							{
								// All alerts are still open and there are no alerts in front of them to process, so we can just set the timeCursor to nowSecondsSinceEpoch
								Logger.LogDebug(
									"All alerts received have EndOnSeconds==0. Moving timeCursor to 'now': {NowSecondsSinceEpoch} ({DateTime})",
									nowSecondsSinceEpoch,
									DateTimeOffset.FromUnixTimeSeconds(nowSecondsSinceEpoch));
								timeCursor = nowSecondsSinceEpoch;
								// We're done looping, nothing else to do on this device for the moment
								break;
							}
							else
							{
								// We probably haven't got all the possible alerts for the timeCursor (>300), but it's the best we can do
								// move on by 1 second and get another batch
								Logger.LogDebug("All alerts received have same EndOnSeconds (>0). Incrementing timeCursor by 1 second.");
								timeCursor++;
							}
						}

						timeCursorLastTime = timeCursor;

						// If we've got a lot of new values then break out the loop so that writes can go ahead
						// This will also help limit the total amount of RAM used
						if (alertsToBulkInsert.Values.Count > maxNewAlerts)
						{
							Logger.LogDebug("Already got {AlertsToBulkInsertValuesCount}, going to write out...", alertsToBulkInsert.Values.Count);
							break;
						}
					}

					if (alertsToBulkInsert.Values.Count > 0)
					{
						await BulkInsertAlertsAsync(_datamartClient.DbContextOptions, alertsToBulkInsert.Values.ToList()).ConfigureAwait(false);
						alertsToBulkInsert.Clear();
					}
					// Update the device
					device.LastAlertClosedTimeSeconds = timeCursor;
					await context
						.SaveChangesAsync(cancellationToken)
						.ConfigureAwait(false);

					Logger.LogInformation(
						"Retrieved DataSource alerts for {DatamartClientAccountName} : Id={DeviceId}, CurrentDisplayName={DeviceDisplayName} ({DeviceIndex}/{DatabaseDeviceIdsCount}). Retrieved {DeviceAlertCount} in {StopwatchElapsedTotalSeconds:N1}s",
						_datamartClient.AccountName,
						deviceId,
						device.DisplayName,
						deviceIndex,
						databaseDeviceIds.Count,
						deviceAlertCount,
						stopwatch.Elapsed.TotalSeconds
						);
				}
				catch (Exception e)
				{
					Logger.LogWarning(
						"Failed to retrieve alerts for {DatamartClientAccountName} : Id={DeviceId} due to {Message}",
						_datamartClient.AccountName,
						deviceId,
						e.Message
						);
				}
			}
		}
		// TODO Alerts where the MonitorObjectType is not device.
		return updateAlertStats;
	}

	private async Task SetMonitorObjectGroupIdsAsync(Context monitorObjectGroupContext, Alert networkAlert, AlertStoreItem alertStoreItem, MemoryCache cache)
	{
		//var monitorObjectGroups = new[] {
		//	alertStoreItem.MonitorObjectGroup0Id,
		//	alertStoreItem.MonitorObjectGroup1Id,
		//	alertStoreItem.MonitorObjectGroup2Id,
		//	alertStoreItem.MonitorObjectGroup3Id,
		//	alertStoreItem.MonitorObjectGroup4Id,
		//	alertStoreItem.MonitorObjectGroup5Id,
		//	alertStoreItem.MonitorObjectGroup6Id,
		//	alertStoreItem.MonitorObjectGroup7Id,
		//	alertStoreItem.MonitorObjectGroup8Id,
		//	alertStoreItem.MonitorObjectGroup9Id
		//};

		//for (var i = 0; i < networkAlert.MonitorObjectGroups.Count; i++)
		//{
		//	monitorObjectGroups[i] = await GetMonitorObjectGroupIdAsync(monitorObjectGroupContext, networkAlert, i).ConfigureAwait(false);
		//}

		if (networkAlert.MonitorObjectGroups.Count > 0)
		{
			alertStoreItem.MonitorObjectGroup0Id = await GetMonitorObjectGroupIdAsync(cache, monitorObjectGroupContext, networkAlert, 0).ConfigureAwait(false);
		}

		if (networkAlert.MonitorObjectGroups.Count > 1)
		{
			alertStoreItem.MonitorObjectGroup1Id = await GetMonitorObjectGroupIdAsync(cache, monitorObjectGroupContext, networkAlert, 1).ConfigureAwait(false);
		}

		if (networkAlert.MonitorObjectGroups.Count > 2)
		{
			alertStoreItem.MonitorObjectGroup2Id = await GetMonitorObjectGroupIdAsync(cache, monitorObjectGroupContext, networkAlert, 2).ConfigureAwait(false);
		}

		if (networkAlert.MonitorObjectGroups.Count > 3)
		{
			alertStoreItem.MonitorObjectGroup3Id = await GetMonitorObjectGroupIdAsync(cache, monitorObjectGroupContext, networkAlert, 3).ConfigureAwait(false);
		}

		if (networkAlert.MonitorObjectGroups.Count > 4)
		{
			alertStoreItem.MonitorObjectGroup4Id = await GetMonitorObjectGroupIdAsync(cache, monitorObjectGroupContext, networkAlert, 4).ConfigureAwait(false);
		}

		if (networkAlert.MonitorObjectGroups.Count > 5)
		{
			alertStoreItem.MonitorObjectGroup5Id = await GetMonitorObjectGroupIdAsync(cache, monitorObjectGroupContext, networkAlert, 5).ConfigureAwait(false);
		}

		if (networkAlert.MonitorObjectGroups.Count > 6)
		{
			alertStoreItem.MonitorObjectGroup6Id = await GetMonitorObjectGroupIdAsync(cache, monitorObjectGroupContext, networkAlert, 6).ConfigureAwait(false);
		}

		if (networkAlert.MonitorObjectGroups.Count > 7)
		{
			alertStoreItem.MonitorObjectGroup7Id = await GetMonitorObjectGroupIdAsync(cache, monitorObjectGroupContext, networkAlert, 7).ConfigureAwait(false);
		}

		if (networkAlert.MonitorObjectGroups.Count > 8)
		{
			alertStoreItem.MonitorObjectGroup8Id = await GetMonitorObjectGroupIdAsync(cache, monitorObjectGroupContext, networkAlert, 8).ConfigureAwait(false);
		}

		if (networkAlert.MonitorObjectGroups.Count > 9)
		{
			alertStoreItem.MonitorObjectGroup9Id = await GetMonitorObjectGroupIdAsync(cache, monitorObjectGroupContext, networkAlert, 9).ConfigureAwait(false);
		}
	}

	private async Task<Guid> GetMonitorObjectGroupIdAsync(MemoryCache cache, Context monitorObjectGroupContext, Alert networkAlert, int index)
	{
		var key = $"{networkAlert.MonitorObjectType}:{networkAlert.MonitorObjectGroups[index].FullPath}";
		var result = await cache.GetOrCreateAsync(key, async _ =>
		{
			var databaseEntry = await monitorObjectGroupContext.MonitorObjectGroups.SingleOrDefaultAsync(g => g.MonitoredObjectType == networkAlert.MonitorObjectType && g.FullPath == networkAlert.MonitorObjectGroups[index].FullPath).ConfigureAwait(false);
			if (databaseEntry == null)
			{
				Logger.LogDebug(
					"Adding new MonitorObjectGroup {NetworkAlertMonitorObjectType}:{NetworkAlertMonitorObjectGroupsFullPath}",
					networkAlert.MonitorObjectType,
					networkAlert.MonitorObjectGroups[index].FullPath);
				databaseEntry = new MonitorObjectGroupStoreItem
				{
					MonitoredObjectType = networkAlert.MonitorObjectType,
					FullPath = networkAlert.MonitorObjectGroups[index].FullPath
				};
				monitorObjectGroupContext.MonitorObjectGroups.Add(databaseEntry);
				await monitorObjectGroupContext.SaveChangesAsync().ConfigureAwait(false);
				Logger.LogInformation(
					"Added new MonitorObjectGroup {DatabaseEntryMonitoredObjectType}:{DatabaseEntryFullPath} with id {DatabaseEntryId}",
					databaseEntry.MonitoredObjectType,
					databaseEntry.FullPath,
					databaseEntry.Id
					);
			}

			return databaseEntry.Id;
		}).ConfigureAwait(false);
		return result;
	}

	private async Task AlterIndexes(Context context, bool enabled)
	{
		var stopwatch = Stopwatch.StartNew();
		var indexAction = enabled ? "REBUILD" : "DISABLE";
		Logger.LogDebug("Alert index {IndexAction}...", indexAction);
		foreach (var column in new[] {
					"InternalId",
					"Id",
					"MonitorObjectId",
					"MonitorObjectName",
					"MonitorObjectType",
					"MonitorObjectGroup0",
					"MonitorObjectGroup1",
					"MonitorObjectGroup2",
					"MonitorObjectGroup3",
					"MonitorObjectGroup4",
					"MonitorObjectGroup5",
					"MonitorObjectGroup6",
					"MonitorObjectGroup7",
					"MonitorObjectGroup8",
					"MonitorObjectGroup9",
					"DataPointId",
					"DataPointName",
					"InstanceId",
					"InstanceName",
					"Severity",
					"IsCleared",
					"ResourceId",
					"ResourceTemplateId",
					"ResourceTemplateName",
					"ResourceTemplateType",
					"ResourceId",
					"ResourceTemplateId",
					"ResourceTemplateName",
					"ResourceTemplateType",
					"StartOnSeconds",
					"EndOnSeconds",
					"StartOnSeconds",
					"FasterPercentageAvailability"
				})
		{
			var sql = "ALTER INDEX IX_Alerts_" + column + " ON [Alerts] " + indexAction;
			await context
			  .Database
			  .ExecuteSqlRawAsync(sql)
			  .ConfigureAwait(false);
		}

		Logger.LogInformation(
			"Alert index action {IndexAction} complete after {StopwatchElapsedSeconds:N1}s",
			indexAction,
			stopwatch.Elapsed.Seconds);
	}

	internal async Task BulkInsertAlertsAsync(DbContextOptions<Context> contextOptions, List<AlertStoreItem> alertStoreItems)
	{
		Logger.LogDebug(
			"Bulk inserting {AlertStoreItemCount} alerts...",
			alertStoreItems.Count);
		var stopwatch = Stopwatch.StartNew();
		switch (_datamartClient.DatabaseType)
		{
			case DatabaseType.SqlServer:
				// Bulk insert
				using (var context = new Context(contextOptions))
				{
					await context.BulkInsertAsync(
						alertStoreItems,
						new BulkConfig
						{
							BulkCopyTimeout = 0,
							BatchSize = 10000,
						},
						n => Logger.LogDebug(
							"Bulk inserted {ItemNumber}/{AlertStoreItemCount}",
							(int)(n * alertStoreItems.Count),
							alertStoreItems.Count))
						.ConfigureAwait(false);
				}

				break;
			case DatabaseType.Postgres:
			case DatabaseType.InMemory:
				// Add and save in chunks to avoid over usage of memory. This can be done using proper bulk insert once the ef core libraries can be updated.
				const int BatchSize = 1000;
				for (var batch = 0; batch * BatchSize < alertStoreItems.Count; batch++)
				{
					Logger.LogDebug(
						"Bulk inserting batch {BatchNumber} of up to {BatchSize} alerts...",
						batch + 1,
						BatchSize);

					using var context = new Context(contextOptions);
					context.Alerts.AddRange(alertStoreItems.Skip(batch * BatchSize).Take(BatchSize));
					await context.SaveChangesAsync().ConfigureAwait(false);
				}

				break;
			default:
				throw new NotSupportedException($"The Database type {_datamartClient.DatabaseType} is not supported for bulk inserts");
		}

		Logger.LogInformation(
			"Bulk inserted {AlertStoreItemsCount} alerts; complete after {StopwatchElapsedTotalSeconds:N1}s",
			alertStoreItems.Count,
			stopwatch.Elapsed.TotalSeconds);
	}
}
