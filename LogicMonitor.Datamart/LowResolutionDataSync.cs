using LogicMonitor.Api.Data;
using LogicMonitor.Api.ScheduledDownTimes;
using LogicMonitor.Api.Time;
using LogicMonitor.Datamart.Interfaces;
using LogicMonitor.Datamart.Notifications;
using PanoramicData.NCalcExtensions;
using System.Globalization;

namespace LogicMonitor.Datamart;

internal class LowResolutionDataSync(
	DatamartClient datamartClient,
	Configuration configuration,
	ILoggerFactory loggerFactory,
	INotificationReceiver? notificationReceiver,
	ITimeProviderService timeProviderService) : LoopInterval(nameof(LowResolutionDataSync), loggerFactory)
{
	private const int DeviceDownTimeWindowSeconds = 3000;

	private readonly DatamartClient _datamartClient = datamartClient;
	private readonly Configuration _configuration = configuration;
	private readonly ITimeProviderService _timeProviderService = timeProviderService;
	private readonly INotificationReceiver _notificationReceiver = notificationReceiver ?? new NullNotificationReceiver();

	public override async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		Logger.LogInformation("Data sync started for {DatabaseName}...", _configuration.DatabaseName);

		if (_configuration.AggregationReset == true)
		{
			await AggregationResetAsync(cancellationToken)
				.ConfigureAwait(false);
		}

		List<DataSourceStoreItem> matchingDatabaseDataSourcesNotTracked;
		Dictionary<int, ResourceStoreItem> allDatabaseDevicesByLogicMonitorIdNotTracked;

		using (var contextForReferenceDataNotTracked = _datamartClient.GetContext())
		{
			// Use the database as a reference for what should be loaded in to ensure referential integrity between the data and the DeviceDataSourceInstance
			// Get the configured DataSource names
			Logger.LogInformation(
				"Getting reference data for {DatabaseName}: DataSources...",
				_configuration.DatabaseName);

			var configurationDataSourceNames = _configuration.DataSources.ConvertAll(ds => ds.Name);

			// Get the database DataSources for those that are configured
			matchingDatabaseDataSourcesNotTracked = await contextForReferenceDataNotTracked
				.DataSources
				.AsNoTracking()
				.Where(ds => configurationDataSourceNames.Contains(ds.Name))
				.ToListAsync(cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			Logger.LogInformation(
				"Getting reference data for {DatabaseName}: DataSources found {DatabaseDataSourceCount} in db",
				_configuration.DatabaseName,
				matchingDatabaseDataSourcesNotTracked.Count);

			// Get a list of devices
			allDatabaseDevicesByLogicMonitorIdNotTracked = await contextForReferenceDataNotTracked
				.Devices
				.AsNoTracking()
				.ToDictionaryAsync(
					d => d.LogicMonitorId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}

		// Clear out the PortalClient Cache, otherwise we remember all the data values for no reason
		_datamartClient.ClearCache();
		var oldCacheState = _datamartClient.UseCache;
		_datamartClient.UseCache = false;
		try
		{
			// Get the LogicMonitor Ids for those DataSources
			var dataSourceCount = matchingDatabaseDataSourcesNotTracked.Count;
			var dataSourceIndex = 0;
			var dataSourceStopwatch = new Stopwatch();
			var deviceStopwatch = new Stopwatch();
			var notificationStopwatch = Stopwatch.StartNew();
			var totalDurationMsByDeviceLogicMonitorId = new Dictionary<int, long>();
			var overallCacheStats = new CacheStats("Overall");

			foreach (var matchingDatabaseDataSourceNotTracked in matchingDatabaseDataSourcesNotTracked)
			{
				try
				{
					Logger.BeginScope(
						"Syncing DataSource {DataSourceName} ({DataSourceIndex}/{DataSourceCount})... ",
						matchingDatabaseDataSourceNotTracked.Name,
						dataSourceIndex + 1,
						dataSourceCount
					);

					// Log the start of the sync
					await _notificationReceiver
						.SetStageNameAsync($"Syncing DataSource {matchingDatabaseDataSourceNotTracked.Name}", cancellationToken)
						.ConfigureAwait(false);

					Logger.LogInformation(
						"Getting DeviceDataSourceInstanceDataPoints for {DatabaseName}: DataSource {DataSourceName} ({DataSourceIndex}/{DataSourceCount})... ",
						_configuration.DatabaseName,
						matchingDatabaseDataSourceNotTracked.Name,
						dataSourceIndex + 1,
						dataSourceCount
					);

					var dataSourceCacheStats = new CacheStats($"DataSource {matchingDatabaseDataSourceNotTracked.Name}");
					dataSourceIndex++;
					await ProcessDataSourceAsync(
						allDatabaseDevicesByLogicMonitorIdNotTracked,
						dataSourceIndex,
						dataSourceCount,
						dataSourceStopwatch,
						deviceStopwatch,
						notificationStopwatch,
						totalDurationMsByDeviceLogicMonitorId,
						matchingDatabaseDataSourceNotTracked,
						dataSourceCacheStats,
						cancellationToken)
						.ConfigureAwait(false);

					// Update the overall cache stats
					overallCacheStats.Add(dataSourceCacheStats);
				}
				catch (Exception e)
				{
					Logger.LogError(
						e,
						"An error occurred syncing data for DataSource {DataSourceName} ({DataSourceIndex}/{DataSourceCount}): {Message}.",
						matchingDatabaseDataSourceNotTracked.Name,
						dataSourceIndex + 1,
						dataSourceCount,
						e.Message
					);
				}
			}

			overallCacheStats.Log(Logger);

			// Log the next call
			await UpdateDeviceSyncDuration(
				totalDurationMsByDeviceLogicMonitorId,
				cancellationToken)
				.ConfigureAwait(false);
		}
		finally
		{
			_datamartClient.UseCache = oldCacheState;
		}
	}

	private async Task AggregationResetAsync(CancellationToken cancellationToken)
	{
		using var context = _datamartClient.GetContext();

		// Do the equivalent of a TRUNCATE TABLE for TimeSeriesDataDataAggregations.  This is a postgres database
		Logger.LogInformation("Aggregation reset: Truncating TimeSeriesDataAggregations table...");
		await context
			.Database
			.ExecuteSqlRawAsync("TRUNCATE TABLE \"TimeSeriesDataAggregations\";", cancellationToken)
			.ConfigureAwait(false);

		// Set DeviceDataSourceInstanceDataPointStoreItem.DataCompleteTo to null for all DeviceDataSourceInstanceDataPointStoreItems
		Logger.LogInformation("Aggregation reset: Setting DeviceDataSourceInstanceDataPoints.DataCompleteTo to null...");
		await context
			.Database
			.ExecuteSqlRawAsync("UPDATE \"DeviceDataSourceInstanceDataPoints\" SET \"DataCompleteTo\" = NULL;", cancellationToken)
			.ConfigureAwait(false);

		Logger.LogInformation("Aggregation reset: Complete.");
	}

	private async Task ProcessDataSourceAsync(
		Dictionary<int, ResourceStoreItem> allDatabaseDevicesByLogicMonitorIdNotTracked,
		int dataSourceIndex,
		int dataSourceCount,
		Stopwatch dataSourceStopwatch,
		Stopwatch deviceStopwatch,
		Stopwatch notificationStopwatch,
		Dictionary<int, long> totalDurationMsByDeviceLogicMonitorId,
		DataSourceStoreItem matchingDatabaseDataSourceNotTracked,
		CacheStats dataSourceCacheStats,
		CancellationToken cancellationToken)
	{
		dataSourceStopwatch.Restart();

		var dataSourceLogicMonitorId = matchingDatabaseDataSourceNotTracked.LogicMonitorId;
		var dataSourceName = matchingDatabaseDataSourceNotTracked.Name;
		try
		{
			var failedDeviceDisplayNames = new List<string>();

			await _notificationReceiver
				.SetStageNameAsync($"Syncing TimeSeriesDataAggregations for {dataSourceName} ({dataSourceIndex}/{dataSourceCount})", cancellationToken)
				.ConfigureAwait(false);

			var configDataSource = _configuration.DataSources
				.First(dsci => dsci.Name == dataSourceName);

			// Only sync the requested devices
			// If no devices are specified in the configuration, sync all devices using the AppliesTo on the DataSource
			var appliesTo = string.IsNullOrWhiteSpace(configDataSource.AppliesTo)
				? matchingDatabaseDataSourceNotTracked.AppliesTo
				: configDataSource.AppliesTo;

			var appliesToMatches = await _datamartClient
				.GetAppliesToAsync(appliesTo, cancellationToken)
				.ConfigureAwait(false);
			var appliesToDeviceIds = appliesToMatches.Select(a => a.Id).ToHashSet();

			// Filter the devices to only those that match the AppliesTo
			var databaseDevicesThatMatchTheAppliesToNotTracked = allDatabaseDevicesByLogicMonitorIdNotTracked
				.Where(kvp => appliesToDeviceIds.Contains(kvp.Key))
				.Select(kvp => kvp.Value)
				.ToList();

			var deviceCount = databaseDevicesThatMatchTheAppliesToNotTracked.Count;
			await _notificationReceiver
				.SetItemCountAsync(deviceCount, cancellationToken)
				.ConfigureAwait(false);

			var deviceIndex = 0;
			foreach (var databaseDeviceNotTracked in databaseDevicesThatMatchTheAppliesToNotTracked)
			{
				deviceIndex++;

				if (notificationStopwatch.Elapsed.TotalSeconds > 5)
				{
					Logger.LogInformation(
						"Getting DeviceDataSourceInstanceDataPoints for {DatabaseName}: DataSource {DataSourceName} ({DataSourceIndex}/{DataSourceCount}): Device {DeviceIndex}/{DeviceCount}...",
						_configuration.DatabaseName,
						dataSourceName,
						dataSourceIndex,
						dataSourceCount,
						deviceIndex,
						deviceCount
					);

					await _notificationReceiver
						.SetItemIndexAsync(deviceIndex + 1, cancellationToken)
						.ConfigureAwait(false);

					notificationStopwatch.Restart();
				}

				var deviceCacheStats = new CacheStats($"Device {databaseDeviceNotTracked.DisplayName}");
				await ProcessDeviceAsync(
					deviceStopwatch,
					totalDurationMsByDeviceLogicMonitorId,
					dataSourceLogicMonitorId,
					failedDeviceDisplayNames,
					deviceIndex,
					deviceCount,
					databaseDeviceNotTracked,
					deviceCacheStats,
					cancellationToken)
				.ConfigureAwait(false);

				// Update the dataSource cache stats
				dataSourceCacheStats.Add(deviceCacheStats);
			}

			LogDeviceFailures(dataSourceName, failedDeviceDisplayNames);
			dataSourceCacheStats.Log(Logger);
		}
		finally
		{
			Logger.LogInformation(
				"Aggregations written for {DatabaseName}: DataSource {DataSourceName} in ({DataSourceDuration}ms).",
					_configuration.DatabaseName,
					dataSourceName,
					dataSourceStopwatch.ElapsedMilliseconds
			);

			// Update the DataSource's LastTimeSeriesDataSyncDurationMs
			using var context = _datamartClient.GetContext();
			// Use ExecuteUpdate to efficiently update this DataSource's LastTimeSeriesDataSyncDurationMs
			await context.DataSources
				.Where(ds => ds.Id == matchingDatabaseDataSourceNotTracked.Id)
				.ExecuteUpdateAsync(
					setters => setters.SetProperty(ds => ds.LastTimeSeriesDataSyncDurationMs, dataSourceStopwatch.ElapsedMilliseconds),
					cancellationToken);
		}
	}

	private void LogDeviceFailures(string dataSourceName, List<string> failedDeviceDisplayNames)
	{
		if (failedDeviceDisplayNames.Count == 0)
		{
			Logger.LogInformation(
				"Aggregations written for {DatabaseName}: {DataSourceName} ({ErrorCount} failed devices).",
				_configuration.DatabaseName,
				dataSourceName,
				failedDeviceDisplayNames.Count
			);
		}
		else
		{
			Logger.LogError(
				"Aggregations written for {DatabaseName}: {DataSourceName} ({ErrorCount} failed devices). Failed devices (up to 10): {FailedDeviceDisplayNames}",
				_configuration.DatabaseName,
				dataSourceName,
				failedDeviceDisplayNames.Count,
				failedDeviceDisplayNames.Count switch
				{
					1 => failedDeviceDisplayNames[0],
					_ => $"{string.Join(", ", failedDeviceDisplayNames.Take(10))}"
				}
			);
		}
	}

	private async Task ProcessDeviceAsync(
		Stopwatch deviceStopwatch,
		Dictionary<int, long> totalDurationMsByDeviceLogicMonitorId,
		int dataSourceLogicMonitorId,
		List<string> failedDeviceDisplayNames,
		int deviceIndex,
		int deviceCount,
		ResourceStoreItem databaseDeviceNotTracked,
		CacheStats deviceCacheStats,
		CancellationToken cancellationToken)
	{
		deviceStopwatch.Restart();

		// Use a fresh context for each device to avoid tracking too many objects unnecessarily
		using (var context = _datamartClient.GetContext())
		{
			// Get the list of DeviceDataSourceInstanceDataPoints
			var databaseDeviceDataSourceInstanceDataPoints =
				await context
					.DeviceDataSourceInstanceDataPoints
					.Include(ddsidp => ddsidp.DeviceDataSourceInstance!.DeviceDataSource!.DataSource)
					.Include(ddsidp => ddsidp.DeviceDataSourceInstance!.DeviceDataSource!.Device)
					.Include(ddsidp => ddsidp.DataSourceDataPoint)
					.Where(ddsi =>
						ddsi.DeviceDataSourceInstance!.DeviceDataSource!.DeviceId == databaseDeviceNotTracked.Id
						&& ddsi.DeviceDataSourceInstance!.LastWentMissing == null
						&& ddsi.DeviceDataSourceInstance!.DeviceDataSource!.DataSource!.LogicMonitorId == dataSourceLogicMonitorId
					)
					// To make debugging a little more deterministic, order by the Device and then its instances
					.OrderBy(ddsi => ddsi.DeviceDataSourceInstance!.DeviceDataSourceId)
					.ThenBy(ddsi => ddsi.LogicMonitorId)
					.ToListAsync(cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			var databaseDeviceDataSourceInstanceDataPointsCount = databaseDeviceDataSourceInstanceDataPoints.Count;

			// Continue if there aren't any
			if (databaseDeviceDataSourceInstanceDataPointsCount == 0)
			{
				Logger.LogInformation(
					"No Device DataSource Instance DataPoints found in the database for Device {DeviceId}. Continuing.",
					databaseDeviceNotTracked.Id.ToString());

				return;
			}
			// We have the database deviceDataSourceInstances for the configured DataSources

			try
			{
				await GetAndWriteAggregationsAsync(
					_datamartClient,
					context,
					_configuration,
					Logger,
					databaseDeviceNotTracked,
					databaseDeviceDataSourceInstanceDataPoints,
					deviceCacheStats,
					cancellationToken)
				.ConfigureAwait(false);

				Logger.LogDebug(
					"Writing aggregations for {DatabaseName}: {DeviceName} ({DeviceIndex}/{DeviceCount}) complete.",
					_configuration.DatabaseName,
					databaseDeviceNotTracked.DisplayName,
					deviceIndex,
					deviceCount
					);
			}
			catch (Exception ex)
			{
				// This can happen if the device has been deleted from LogicMonitor
				Logger.LogInformation(
					ex,
					"Writing aggregations for {DatabaseName}: {DeviceName} ({DeviceIndex}/{DeviceCount}) failed: '{Message}' ||| {StackTrace}",
					_configuration.DatabaseName,
					databaseDeviceNotTracked.DisplayName,
					deviceIndex,
					deviceCount,
					ex.Message,
					ex.StackTrace
				);
				failedDeviceDisplayNames.Add(databaseDeviceNotTracked.DisplayName);
			}
		}

		// Update the total duration for this device across all DataSources
		if (totalDurationMsByDeviceLogicMonitorId.ContainsKey(databaseDeviceNotTracked.LogicMonitorId))
		{
			totalDurationMsByDeviceLogicMonitorId[databaseDeviceNotTracked.LogicMonitorId] += deviceStopwatch.ElapsedMilliseconds;
		}
		else
		{
			totalDurationMsByDeviceLogicMonitorId[databaseDeviceNotTracked.LogicMonitorId] = deviceStopwatch.ElapsedMilliseconds;
		}
	}

	/// <summary>
	/// Update all the Device LastTimeSeriesDataSyncDurationMs values from the Dictionary
	/// </summary>
	private async Task UpdateDeviceSyncDuration(Dictionary<int, long> totalDurationMsByDeviceLogicMonitorId, CancellationToken cancellationToken)
	{
		Logger.LogInformation(
				"Updating total Device sync durations in the database {DatabaseName}",
				_configuration.DatabaseName
			);

		// Get all the devices tracked on the context
		using var context = _datamartClient.GetContext();
		var allDevicesByLogicMonitorId = await context
			.Devices
			.ToDictionaryAsync(d => d.LogicMonitorId, cancellationToken)
			.ConfigureAwait(false);

		// Update the Device in the context to set its LastTimeSeriesDataSyncDurationMs to the overall time spent on this device for all DataSources.
		foreach (var kvp in totalDurationMsByDeviceLogicMonitorId)
		{
			allDevicesByLogicMonitorId[kvp.Key].LastTimeSeriesDataSyncDurationMs = kvp.Value;
		}

		// All Devices have been updated, save changes
		await context
			.SaveChangesAsync(cancellationToken)
			.ConfigureAwait(false);

		Logger.LogInformation(
			"Updating total Device sync durations in the database {DatabaseName} - Complete",
			_configuration.DatabaseName
			);
	}

	public static double? CalculatePercentile(double[] values, int n)
	{
		// return null if the list is empty
		if (values.Length == 0)
		{
			return null;
		}

		// calculate the index of the value at the nth percentile
		var index = n / 100.0 * (values.Length - 1);

		// check if the index is an integer
		if (index % 1 == 0)
		{
			// if the index is an integer, return the corresponding value
			return values[(int)index];
		}
		else
		{
			// if the index is not an integer, interpolate between the two closest values
			var floorIndex = (int)Math.Floor(index);
			var ceilIndex = (int)Math.Ceiling(index);

			var floorValue = values[floorIndex];
			var ceilValue = values[ceilIndex];

			return floorValue + ((index - floorIndex) * (ceilValue - floorValue));
		}
	}

	private async Task GetAndWriteAggregationsAsync(
		DatamartClient datamartClient,
		Context context,
		Configuration configuration,
		ILogger logger,
		ResourceStoreItem deviceNotTracked,
		List<ResourceDataSourceInstanceDataPointStoreItem> databaseDeviceDataSourceInstanceDataPoints,
		CacheStats cacheStats,
		CancellationToken cancellationToken)
	{
		// To ignore a period of uncertainty whether the Collector has been
		// able to publish its measurement data to the LogicMonitor API,
		// we consider "now" to be X hours ago.
		// This is "B" in the diagram below.
		var utcNow = _timeProviderService.UtcOffsetNow;

		var aggregationsToWrite = new List<TimeSeriesDataAggregationStoreItem>();

		var stopwatch = new Stopwatch();

		var dataSourceDataPointStoreItemsNotTracked = await context
			.DataSourceDataPoints
			.Include(dsdp => dsdp.DataSource)
			.AsNoTracking()
			.ToListAsync(cancellationToken)
			.ConfigureAwait(false);

		await ResetForResyncAsync(
			context,
			logger,
			databaseDeviceDataSourceInstanceDataPoints,
			dataSourceDataPointStoreItemsNotTracked,
			cancellationToken)
			.ConfigureAwait(false);

		// Disable caching
		var oldCacheState = datamartClient.UseCache;
		datamartClient.UseCache = false;

		foreach (var databaseDeviceDataSourceInstanceDataPointGroup in
			databaseDeviceDataSourceInstanceDataPoints
			.GroupBy(ddsidp => ddsidp.DeviceDataSourceInstance!.LogicMonitorId))
		{
			try
			{
				var graphDataCache = new Dictionary<string, GraphData>();
				
				// MS-21395: Cache for Device and DeviceGroup SDTs to reduce API calls
				// Key format: "Device_{deviceId}" or "DeviceGroup_{groupId}"
				var sdtCache = new Dictionary<string, List<ScheduledDownTimeHistory>>();

				foreach (var databaseDeviceDataSourceInstanceDataPoint in databaseDeviceDataSourceInstanceDataPointGroup)
				{
					try
					{
						var lastAggregationHourWrittenUtc =
							(databaseDeviceDataSourceInstanceDataPoint.DataCompleteTo is null
								? configuration.StartDateTimeUtc
								: databaseDeviceDataSourceInstanceDataPoint.DataCompleteTo.Value)
							.ToUniversalTime();

						// Ensure that this is on a month boundary
						lastAggregationHourWrittenUtc = new DateTimeOffset(
							lastAggregationHourWrittenUtc.Year,
							lastAggregationHourWrittenUtc.Month,
							1, 0, 0, 0,
							TimeSpan.Zero);

						var startDateTimeUtc = lastAggregationHourWrittenUtc;
						var endDateTimeUtc = lastAggregationHourWrittenUtc.AddMonths(1);
						var endDateTimePlusOffset = endDateTimeUtc.AddMinutes(configuration.MinutesOffset);

						if (endDateTimePlusOffset >= utcNow)
						{
							Logger.LogInformation(
								"Skipped writing aggregations because the end date time + minutes offset ({EndDateTimePlusOffset}) (from configuration) was greater than UTC now ({UtcNow}). " +
								"Start date UTC: {StartDateUtc}. " +
								"End date UTC: {EndDateUtc}. " +
								"Minutes Offset: {MinutesOffset}. " +
								"End date UTC with offset: {EndDateUtcWithOffset}. " +
								"Database: {DatabaseName}. " +
								"DeviceDataSourceInstanceDataPointId: {DeviceDataSourceInstanceDataPointId}. " +
								"DataSourceDataPointId: {DataSourceDataPointId}. " +
								"DeviceDataSourceInstanceId: {DeviceDataSourceInstanceId}.",
								endDateTimePlusOffset,
								utcNow,
								startDateTimeUtc,
								endDateTimeUtc,
								configuration.MinutesOffset,
								endDateTimeUtc.AddMinutes(configuration.MinutesOffset),
								configuration.DatabaseName,
								databaseDeviceDataSourceInstanceDataPoint?.Id,
								databaseDeviceDataSourceInstanceDataPoint?.DataSourceDataPointId,
								databaseDeviceDataSourceInstanceDataPoint?.DeviceDataSourceInstanceId);

							continue;
						}

						var dataSourceName = databaseDeviceDataSourceInstanceDataPoint
							.DeviceDataSourceInstance!
							.DeviceDataSource!
							.DataSource!
							.Name;

						var dataPointName = databaseDeviceDataSourceInstanceDataPoint.DataSourceDataPoint!.Name;

						// Get the configuration for this DataSourceName
						var dataSourceConfigurationItem = configuration
							.DataSources
							.SingleOrDefault(dsci => dsci.Name == dataSourceName)
							?? throw new InvalidOperationException($"Could not find configuration for DataSource {dataSourceName}.");

						var dataSourceDataPointStoreItemNotTracked = dataSourceDataPointStoreItemsNotTracked
							.SingleOrDefault(dp =>
								dp.Name == dataPointName
								&& dp.DataSource!.Name == dataSourceConfigurationItem.Name
							)
							?? throw new InvalidOperationException($"Could not find DataPoint {dataPointName} for DataSource {dataSourceName}.");

						// Build up the aggregations to write
						while (endDateTimeUtc.AddMinutes(configuration.MinutesOffset) < utcNow)
						{
							// RM-16049 Add an offset from the start and end times
							var startDateTimeUtcWithOffset = startDateTimeUtc.AddMinutes(configuration.MinutesOffset);
							var endDateTimeUtcWithOffset = endDateTimeUtc.AddMinutes(configuration.MinutesOffset);

							var deviceDataSourceInstanceId = databaseDeviceDataSourceInstanceDataPoint.DeviceDataSourceInstance!.LogicMonitorId;
							var cacheKey = deviceDataSourceInstanceId + "_" + startDateTimeUtcWithOffset.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture) + "_" + endDateTimeUtcWithOffset.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);
							if (!graphDataCache.TryGetValue(cacheKey, out var graphData))
							{
								graphData = graphDataCache[cacheKey] = await GetGraphDataAsync(
								datamartClient,
								deviceDataSourceInstanceId,
								startDateTimeUtcWithOffset,
								endDateTimeUtcWithOffset,
								logger,
								cancellationToken);

								cacheStats.AddMiss();
							}
							else
							{
								logger.LogDebug(
									"Using cached data for DeviceDataSourceInstance {DeviceDataSourceInstanceId} ({Start:yyyy-MM-dd HH:mm:ss} .. {End:yyyy-MM-dd HH:mm:ss})...",
									deviceDataSourceInstanceId,
									startDateTimeUtcWithOffset,
									endDateTimeUtcWithOffset);

								cacheStats.AddHit();
							}

							var bulkWriteModel =
								await GetTimeSeriesDataAggregationStoreItem(
									datamartClient,
									deviceNotTracked,
									databaseDeviceDataSourceInstanceDataPoint,
									dataPointName,
									dataSourceDataPointStoreItemNotTracked,
									startDateTimeUtc,
									endDateTimeUtc,
									graphData,
									configuration.ExcludeSdtPeriods,
									sdtCache,
									Logger,
									cancellationToken)
								.ConfigureAwait(false);

							if (bulkWriteModel is null)
							{
								continue;
							}

							aggregationsToWrite.Add(bulkWriteModel);

							databaseDeviceDataSourceInstanceDataPoint.DataCompleteTo = endDateTimeUtc;
							startDateTimeUtc = startDateTimeUtc.AddMonths(1);
							endDateTimeUtc = endDateTimeUtc.AddMonths(1);
						}

						// Write out the aggregations
						await context
							.BulkInsertAsync(aggregationsToWrite, cancellationToken: cancellationToken)
							.ConfigureAwait(false);

						await context
							.SaveChangesAsync(cancellationToken)
							.ConfigureAwait(false);

						aggregationsToWrite.Clear();
					}
					catch (Exception ex)
					{
						logger.LogError(ex, "Error syncing data for instance datapoint with id {DeviceDataSourceInstanceDataPointId}: {Message}.", databaseDeviceDataSourceInstanceDataPoint.Id, ex.Message);
					}
				}
			}
			catch (Exception exception)
			{
				logger.LogError(
					exception,
					"An exception occurred syncing data for a database Device Data Source Instance. " +
					"Database GUID: {DatabaseGuid}. " +
					"Device DataSource Instance Name: {DeviceDataSourceInstanceName}. " +
					"DataSource DataPoint Name: {DataSourceDataPointName}. " +
					"Device DataSourceInstance LogicMonitor ID: {DataSourceInstanceLogicMonitorId}. " +
					"The exception was: {Message}",
					databaseDeviceDataSourceInstanceDataPointGroup.FirstOrDefault()?.Id.ToString() ?? string.Empty,
					databaseDeviceDataSourceInstanceDataPointGroup.FirstOrDefault()?.DeviceDataSourceInstance?.Name ?? string.Empty,
					databaseDeviceDataSourceInstanceDataPointGroup.FirstOrDefault()?.DataSourceDataPoint?.Name ?? string.Empty,
					databaseDeviceDataSourceInstanceDataPointGroup.FirstOrDefault()?.DeviceDataSourceInstance?.LogicMonitorId.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
					exception.Message);

				// Go to the next group
			}
		}

		// Re-enable caching
		datamartClient.UseCache = oldCacheState;
	}

	internal static async Task<GraphData> GetGraphDataAsync(
		DatamartClient datamartClient,
		int resourceDataSourceInstanceId,
		DateTimeOffset startDateTimeOffset,
		DateTimeOffset endDateTimeOffset,
		ILogger logger,
		CancellationToken cancellationToken
	)
	{
		logger.LogDebug(
			"Getting graph data for DeviceDataSourceInstance {DeviceDataSourceInstanceId} ({Start:yyyy-MM-dd HH:mm:ss} .. {End:yyyy-MM-dd HH:mm:ss})...",
			resourceDataSourceInstanceId,
			startDateTimeOffset,
			endDateTimeOffset);

		try
		{
			return await datamartClient
				.GetGraphDataAsync(
					new ResourceDataSourceInstanceGraphDataRequest
					{
						ResourceDataSourceInstanceId = resourceDataSourceInstanceId,
						StartDateTime = startDateTimeOffset.UtcDateTime,
						EndDateTime = endDateTimeOffset.UtcDateTime,
						TimePeriod = TimePeriod.Zoom,
						DataSourceGraphId = -1,
					},
					cancellationToken)
				.ConfigureAwait(false);
		}
		catch (Exception exception)
		{
			logger.LogError(
				exception,
				"Error getting graph data for DeviceDataSourceInstance {DeviceDataSourceInstanceId} ({Start:yyyy-MM-dd HH:mm:ss} .. {End:yyyy-MM-dd HH:mm:ss})...",
				resourceDataSourceInstanceId,
				startDateTimeOffset,
				endDateTimeOffset);

			throw;
		}
	}

	/// <summary>
	/// Creates a TimeSeriesDataAggregationStoreItem from graph data.
	/// MS-21395: Supports excluding SDT (Scheduled Down Time) periods from aggregations when excludeStdPeriods is true.
	/// </summary>
	internal static async Task<TimeSeriesDataAggregationStoreItem?> GetTimeSeriesDataAggregationStoreItem(
		DatamartClient datamartClient,
		ResourceStoreItem deviceNotTracked,
		ResourceDataSourceInstanceDataPointStoreItem databaseDeviceDataSourceInstanceDataPoint,
		string dataPointName,
		DataSourceDataPointStoreItem dataPointStoreItemNotTracked,
		DateTimeOffset startDateTimeUtc,
		DateTimeOffset endDateTimeUtc,
		GraphData graphData,
		bool excludeStdPeriods,
		Dictionary<string, List<ScheduledDownTimeHistory>> sdtCache,
		ILogger logger,
		CancellationToken cancellationToken)
	{
		try
		{
			// Remove all DataPoint values before the device was added to LogicMonitor
			foreach (var graphDataLine in graphData.Lines)
			{
				graphDataLine.Data =
					[.. graphDataLine.Data
						.Where((dp, index) =>
						{
							return graphData.TimeStamps[index] >= deviceNotTracked.CreatedOnSeconds * 1000;
						})
					];
			}

			graphData.TimeStamps = [.. graphData.TimeStamps.Where(ts => ts >= deviceNotTracked.CreatedOnSeconds * 1000)];

			// MS-21395: Create paired timestamp/value data to ensure synchronization
			List<(long timestamp, double? value)> pairedData;
			
			if (string.IsNullOrWhiteSpace(databaseDeviceDataSourceInstanceDataPoint.DataSourceDataPoint!.Calculation))
			{
				// No calculation - get data directly from the graph
				var line = graphData
					.Lines
					.SingleOrDefault(dp => dp.Legend == dataPointName);

				if (line is null || dataPointStoreItemNotTracked is null)
				{
					throw new FormatException($"Could not find DataPoint '{dataPointName}' for DataSource '{databaseDeviceDataSourceInstanceDataPoint.DataSourceDataPoint.DataSource.Name}'");
				}

				// Pair timestamps with values
				pairedData = graphData.TimeStamps
					.Select((ts, index) => (ts, line.Data[index]))
					.ToList();
			}
			else
			{
				// Calculation exists - evaluate expression for each timestamp
				var expression = new ExtendedExpression(databaseDeviceDataSourceInstanceDataPoint.DataSourceDataPoint.Calculation);
				
				pairedData = graphData.TimeStamps
					.Select((ts, index) =>
					{
						expression.Parameters.Clear();
						foreach (var line in graphData.Lines)
						{
							expression.Parameters.Add(line.Legend, line.Data[index]);
						}

						var calculatedValue = expression.Evaluate() as double?;
						return (ts, calculatedValue);
					})
					.ToList();
			}

			// MS-21395: Transform paired data into TimeSeriesDataPoint structure with SDT information
			List<TimeSeriesDataPoint> timeSeriesDataPoints;
			
			if (excludeStdPeriods)
			{
				// Can be removed once validated
				logger.LogInformation(
					"Excluding SDT periods for DeviceDataSourceInstanceDataPointId {DeviceDataSourceInstanceDataPointId} ({Start:yyyy-MM-dd HH:mm:ss} .. {End:yyyy-MM-dd HH:mm:ss})...",
					databaseDeviceDataSourceInstanceDataPoint.Id,
					startDateTimeUtc,
					endDateTimeUtc);

				// Get SDT periods only if we need to exclude them (with caching)
				var sdtPeriods =
					await GetHistoricalSdtPeriodsAsync(
						datamartClient,
						deviceNotTracked,
						databaseDeviceDataSourceInstanceDataPoint.DeviceDataSourceInstance,
						startDateTimeUtc,
						endDateTimeUtc,
						sdtCache,
						logger,
						cancellationToken)
					.ConfigureAwait(false);

				timeSeriesDataPoints = new List<TimeSeriesDataPoint>();

				foreach (var (timestamp, value) in pairedData)
				{
					// Check if timestamp falls within any SDT period
					var isInSdt = sdtPeriods.Any(sdt => 
						timestamp >= sdt.StartTimestampMs && 
						timestamp <= sdt.EndTimestampMs);
					
					timeSeriesDataPoints.Add(new TimeSeriesDataPoint
					{
						IsInSdt = isInSdt,
						Timestamp = timestamp,
						Value = value
					});
				}
			}
			else
			{
				// Can be removed once validated
				logger.LogInformation("Skipping SDT exclusion for DeviceDataSourceInstanceDataPointId {DeviceDataSourceInstanceDataPointId} ({Start:yyyy-MM-dd HH:mm:ss} .. {End:yyyy-MM-dd HH:mm:ss})...",
					databaseDeviceDataSourceInstanceDataPoint.Id,
					startDateTimeUtc,
					endDateTimeUtc);

				// Performance optimization: Skip SDT checking entirely when not needed
				timeSeriesDataPoints = pairedData
					.Select(pair => new TimeSeriesDataPoint
					{
						Value = pair.value,
						Timestamp = pair.timestamp,
						IsInSdt = false
					})
					.ToList();
			}

			// MS-21395: Filter out SDT periods when configured
			var effectiveDataPoints = excludeStdPeriods
				? timeSeriesDataPoints.Where(dp => !dp.IsInSdt).ToList()
				: timeSeriesDataPoints;

			// Extract values for calculations
			var effectiveValues = effectiveDataPoints.Select(dp => dp.Value).ToList();

			// Calculate and sort non-null values for percentile calculations
			var sortedNonNullValues = effectiveValues
				.Where(v => v.HasValue)
				.Select(v => v!.Value)
				.OrderBy(v => v)
				.ToArray();

			var bulkWriteModel = new TimeSeriesDataAggregationStoreItem
			{
				Id = Guid.NewGuid(),
				DeviceDataSourceInstanceDataPointId = databaseDeviceDataSourceInstanceDataPoint.Id,
				PeriodStart = startDateTimeUtc.UtcDateTime,
				PeriodEnd = endDateTimeUtc.UtcDateTime,
				DataCount = effectiveValues.Count(d => d.HasValue),
				NoDataCount = effectiveValues.Count(d => !d.HasValue),
				Sum = effectiveValues.Sum(d => d ?? 0),
				SumSquared = effectiveValues.Sum(d => d.HasValue ? d.Value * d.Value : 0),
				Max = effectiveValues.Where(d => d != null).DefaultIfEmpty(null).Max(),
				Min = effectiveValues.Where(d => d != null).DefaultIfEmpty(null).Min(),
				First = effectiveValues.DefaultIfEmpty(null).First(),
				Last = effectiveValues.DefaultIfEmpty(null).Last(),
				FirstWithData = effectiveValues.Where(d => d != null).DefaultIfEmpty(null).First(),
				LastWithData = effectiveValues.Where(d => d != null).DefaultIfEmpty(null).Last(),
				Centile05 = CalculatePercentile(sortedNonNullValues, 5),
				Centile10 = CalculatePercentile(sortedNonNullValues, 10),
				Centile25 = CalculatePercentile(sortedNonNullValues, 25),
				Centile50 = CalculatePercentile(sortedNonNullValues, 50),
				Centile75 = CalculatePercentile(sortedNonNullValues, 75),
				Centile90 = CalculatePercentile(sortedNonNullValues, 90),
				Centile95 = CalculatePercentile(sortedNonNullValues, 95),
				NormalCount = CountAtAlertLevel(
					effectiveValues,
					dataPointStoreItemNotTracked.GlobalAlertExpression,
					CountAlertLevel.Normal
				),
				WarningCount = CountAtAlertLevel(
					effectiveValues,
					dataPointStoreItemNotTracked.GlobalAlertExpression,
					CountAlertLevel.Warning
				),
				ErrorCount = CountAtAlertLevel(
					effectiveValues,
					dataPointStoreItemNotTracked.GlobalAlertExpression,
					CountAlertLevel.Error
				),
				CriticalCount = CountAtAlertLevel(
					effectiveValues,
					dataPointStoreItemNotTracked.GlobalAlertExpression,
					CountAlertLevel.Critical
				),

				// MS-21394 DataMagic: PercentUpTime availability calculation should calculate downtime based on up-time after no data
				AvailabilityPercent2 = CalculatePercentageAvailabilityNew(
					effectiveValues,
					dataPointStoreItemNotTracked.PercentageAvailabilityCalculation
				),

				// IMPORTANT! This must be calculated last as this process can reverse the array.
				AvailabilityPercent = CalculatePercentageAvailability(
					effectiveValues,
					dataPointStoreItemNotTracked.PercentageAvailabilityCalculation
				)
			};

			return bulkWriteModel;
		}
		catch (Exception exception)
		{
			logger.LogError(
				exception,
				"An error occurred getting time series data aggregations. " +
				"Data point name: {DataPointName}. " +
				"DataSourceDataPointStoreItem Id: {DataSourceDataPointStoreItemId}. " +
				"The error was due to: {Message}",
				dataPointStoreItemNotTracked.Id.ToString(),
				dataPointName,
				exception.Message);

			throw;
		}
	}

	private static async Task ResetForResyncAsync(
		Context context,
		ILogger logger,
		List<ResourceDataSourceInstanceDataPointStoreItem> databaseDeviceDataSourceInstanceDataPoints,
		List<DataSourceDataPointStoreItem> dataPointStoreItemsNotTracked,
		CancellationToken cancellationToken
	)
	{
		if (databaseDeviceDataSourceInstanceDataPoints.Count == 0)
		{
			return;
		}

		var firstDeviceDataSourceInstanceDataPointStoreItem = databaseDeviceDataSourceInstanceDataPoints[0];

		// Get the list of DataSourceDataPoints that we're re-syncing
		var resyncDataPointStoreItemsNotTracked = dataPointStoreItemsNotTracked
			.Where(dsdp => dsdp.ResyncTimeSeriesData)
			.ToList();

		if (resyncDataPointStoreItemsNotTracked.Count == 0)
		{
			return;
		}

		logger.LogInformation(
			"Re-syncing {ResyncDataPointStoreItemCount} DataPoints for {Device} / {DataSource}...",
			resyncDataPointStoreItemsNotTracked.Count,
			firstDeviceDataSourceInstanceDataPointStoreItem.DeviceDataSourceInstance!.DeviceDataSource!.Device!.DisplayName,
			firstDeviceDataSourceInstanceDataPointStoreItem.DataSourceDataPoint!.DataSource.Name
		);

		var databaseDeviceDataSourceInstanceDataPointsToResync = databaseDeviceDataSourceInstanceDataPoints
			.Where(ddsidp => resyncDataPointStoreItemsNotTracked.Any(dsdp => dsdp.Id == ddsidp.DataSourceDataPointId))
			.ToList();

		// Reset their Time cursor
		foreach (var databaseDeviceDataSourceInstanceDataPoint in databaseDeviceDataSourceInstanceDataPointsToResync)
		{
			databaseDeviceDataSourceInstanceDataPoint.DataCompleteTo = null;

			// Remove any related aggregations
			var aggregationStoreItemsToRemove = await context
				.TimeSeriesDataAggregations
				.Where(tsda => tsda.DeviceDataSourceInstanceDataPointId == databaseDeviceDataSourceInstanceDataPoint.Id)
				.ExecuteDeleteAsync(cancellationToken)
				.ConfigureAwait(false);
		}

		// Save Changes
		await context
			.SaveChangesAsync(cancellationToken)
			.ConfigureAwait(false);
	}

	public static double? CalculatePercentageAvailability(
		List<double?> values,
		string percentageAvailabilityCalculation
	)
	{
		if (string.IsNullOrWhiteSpace(percentageAvailabilityCalculation))
		{
			return null;
		}

		if (percentageAvailabilityCalculation != "PercentUpTime")
		{
			return null;
		}

		if (values.Count == 0)
		{
			return null;
		}

		var reversedValues = values.ToArray().Reverse();
		//var previousButOneDoubleValue = double.NaN;
		var previousDoubleValue = double.NaN;
		var upTimeCount = 0;
		var downTimeCount = 0;
		var ambiguousCount = 0;
		var isAmbiguous = true;
		foreach (var value in reversedValues)
		{
			if (value is double doubleValue)
			{
				//previousButOneDoubleValue = previousDoubleValue;
				previousDoubleValue = doubleValue;
				upTimeCount += ambiguousCount;
				ambiguousCount = 0;
				upTimeCount++;
				isAmbiguous = false;
			}
			else
			{
				//double projectedValue = !double.IsNaN(previousDoubleValue) && !double.IsNaN(previousButOneDoubleValue)
				//	? previousDoubleValue + (previousDoubleValue - previousButOneDoubleValue)
				//	: double.NaN;

				if (
					double.IsNaN(previousDoubleValue)
					|| previousDoubleValue < DeviceDownTimeWindowSeconds
					//|| projectedValue < DeviceDownTimeWindowSeconds
					)
				{
					if (isAmbiguous)
					{
						ambiguousCount++;
					}
					else
					{
						downTimeCount++;
					}
				}
				else
				{
					upTimeCount++;
				}
			}
		}

		// This should be a double, var would incorrectly infer int
		double totalCount = upTimeCount + downTimeCount;

		if (totalCount == 0)
		{
			// If there aren't any values, return null.
			return null;
		}

		return 100 * upTimeCount / totalCount;
	}

	// MS-21394 DataMagic: PercentUpTime availability calculation should calculate downtime based on up-time after no data
	public static double? CalculatePercentageAvailabilityNew(
		List<double?> values,
		string percentageAvailabilityCalculation
	)
	{
		if (string.IsNullOrWhiteSpace(percentageAvailabilityCalculation))
		{
			return null;
		}

		if (percentageAvailabilityCalculation != "PercentUpTime")
		{
			return null;
		}

		if (values.Count == 0 || values.All(v => v is null))
		{
			return null;
		}

		var doubleValues = values.Select(v => v as double?).ToList();
		var gradient = CalculatePercentUptimeGradient(doubleValues);

		if (Math.Abs(gradient) < 1e-6)
		{
			return null;
		}

		var (totalDowntime, totalExpectedDataPoints) = CalculateDowntime(doubleValues, gradient);
		var totalTime = totalExpectedDataPoints;
		var uptime = totalTime - totalDowntime;

		if (totalTime == 0)
		{
			return 0d;
		}

		return 100d * uptime / totalTime;
	}

	public static double? CalculatePercentageAvailabilityOld(
		double?[] values,
		string percentageAvailabilityCalculation
	)
	{
		if (string.IsNullOrWhiteSpace(percentageAvailabilityCalculation))
		{
			return null;
		}

		if (percentageAvailabilityCalculation != "PercentUpTime")
		{
			return null;
		}

		var reversedValues = values.Reverse();
		var previousDoubleValue = double.NaN;
		var upTimeCount = 0;
		var downTimeCount = 0;
		foreach (var value in reversedValues)
		{
			if (value is double doubleValue)
			{
				previousDoubleValue = doubleValue;
				upTimeCount++;
			}
			else
			{
				if (double.IsNaN(previousDoubleValue) || previousDoubleValue < DeviceDownTimeWindowSeconds)
				{
					downTimeCount++;
				}
				else
				{
					upTimeCount++;
				}
			}
		}

		var totalCount = upTimeCount + downTimeCount;

		if (totalCount == 0)
		{
			// If there aren't any values, return 100%.
			return 100;
		}

		return 100 * upTimeCount / totalCount;
	}

	/// <summary>
	/// TODO - implement this
	/// </summary>
	/// <param name="data"></param>
	/// <param name="effectiveAlertExpression"></param>
	/// <param name="countAlertLevel"></param>
	/// <returns></returns>
	private static int? CountAtAlertLevel(
		List<double?> data,
		string effectiveAlertExpression,
		CountAlertLevel countAlertLevel
	)
	{
		if (string.IsNullOrEmpty(effectiveAlertExpression))
		{
			return countAlertLevel == CountAlertLevel.Normal ? data.Count(d => d.HasValue) : 0;
		}

		// The alert expression is in the form "> 1 2 3", where the first symbol represents the
		// type of the inequality, the first value represents the warning level, the optional next value
		// represents the error level, and the optional next value represents the critical level.
		// Count the data values that meet the expression at the count alert level

		var symbol = effectiveAlertExpression.Split(' ')[0];

		var alertLevels = effectiveAlertExpression
			.Split(' ')
			.Skip(1)
			.Where(x => double.TryParse(x, out var _))
			.Select(double.Parse)
			.ToArray();

		var criticalLevel = alertLevels.Length == 3 ? alertLevels[2] : (double?)null;
		var errorLevel = alertLevels.Length >= 2 ? alertLevels[1] : (double?)null;
		var warningLevel = alertLevels.Length >= 1 ? alertLevels[0] : (double?)null;

		switch (countAlertLevel)
		{
			case CountAlertLevel.Critical:
				if (criticalLevel == null)
				{
					return 0;
				}

				return symbol switch
				{
					">" => data.Count(d => d.HasValue && d.Value > criticalLevel),
					">=" => data.Count(d => d.HasValue && d.Value >= criticalLevel),
					"<" => data.Count(d => d.HasValue && d.Value < criticalLevel),
					"<=" => data.Count(d => d.HasValue && d.Value <= criticalLevel),
					"=" => data.Count(d => d.HasValue && d.Value == criticalLevel),
					"!=" => data.Count(d => d.HasValue && d.Value != criticalLevel),
					_ => null,
				};
			case CountAlertLevel.Error:
				return symbol switch
				{
					">" => data.Count(d => d.HasValue && (criticalLevel == null || d.Value <= criticalLevel) && d.Value > errorLevel),
					">=" => data.Count(d => d.HasValue && (criticalLevel == null || d.Value < criticalLevel) && d.Value >= errorLevel),
					"<" => data.Count(d => d.HasValue && (criticalLevel == null || d.Value >= criticalLevel) && d.Value < errorLevel),
					"<=" => data.Count(d => d.HasValue && (criticalLevel == null || d.Value > criticalLevel) && d.Value <= errorLevel),
					"=" => data.Count(d => d.HasValue && (criticalLevel == null || d.Value != criticalLevel) && d.Value == errorLevel),
					"!=" => data.Count(d => d.HasValue && (criticalLevel == null || d.Value == criticalLevel) && d.Value != errorLevel),
					_ => null,
				};
			case CountAlertLevel.Warning:
				if (alertLevels.Length == 0)
				{
					return 0;
				}

				return symbol switch
				{
					">" => data.Count(d => d.HasValue && (criticalLevel == null || d.Value <= criticalLevel) && (errorLevel == null || d.Value <= errorLevel) && d.Value > warningLevel),
					">=" => data.Count(d => d.HasValue && (criticalLevel == null || d.Value < criticalLevel) && (errorLevel == null || d.Value < errorLevel) && d.Value >= warningLevel),
					"<" => data.Count(d => d.HasValue && (criticalLevel == null || d.Value >= criticalLevel) && (errorLevel == null || d.Value >= errorLevel) && d.Value < warningLevel),
					"<=" => data.Count(d => d.HasValue && (criticalLevel == null || d.Value > criticalLevel) && (errorLevel == null || d.Value > errorLevel) && d.Value <= warningLevel),
					"=" => data.Count(d => d.HasValue && (criticalLevel == null || d.Value != criticalLevel) && (errorLevel == null || d.Value != errorLevel) && d.Value == warningLevel),
					"!=" => data.Count(d => d.HasValue && (criticalLevel == null || d.Value == criticalLevel) && (errorLevel == null || d.Value == errorLevel) && d.Value != warningLevel),
					_ => null,
				};
			case CountAlertLevel.Normal:
				if (alertLevels.Length == 0)
				{
					return data.Count(d => d.HasValue);
				}

				return symbol switch
				{
					">" => data.Count(d => d.HasValue && (criticalLevel == null || d.Value <= criticalLevel) && (errorLevel == null || d.Value <= errorLevel) && (warningLevel == null || d.Value <= warningLevel)),
					">=" => data.Count(d => d.HasValue && (criticalLevel == null || d.Value < criticalLevel) && (errorLevel == null || d.Value < errorLevel) && (errorLevel == null || d.Value < warningLevel)),
					"<" => data.Count(d => d.HasValue && (criticalLevel == null || d.Value >= criticalLevel) && (errorLevel == null || d.Value >= errorLevel) && (errorLevel == null || d.Value >= warningLevel)),
					"<=" => data.Count(d => d.HasValue && (criticalLevel == null || d.Value > criticalLevel) && (errorLevel == null || d.Value > errorLevel) && (errorLevel == null || d.Value > warningLevel)),
					"=" => data.Count(d => d.HasValue && (criticalLevel == null || d.Value != criticalLevel) && (errorLevel == null || d.Value != errorLevel) && (errorLevel == null || d.Value != warningLevel)),
					"!=" => data.Count(d => d.HasValue && (criticalLevel == null || d.Value == criticalLevel) && (errorLevel == null || d.Value == errorLevel) && (errorLevel == null || d.Value == warningLevel)),
					_ => null,
				};
			default:
				throw new ArgumentException($"Unexpected {nameof(CountAlertLevel)} '{countAlertLevel}'");
		}
	}

	private static double CalculatePercentUptimeGradient(List<double?> values)
	{
		double gradientSum = 0;
		var gradientCount = 0;
		double? prev = null;
		for (var i = 0; i < values.Count; i++)
		{
			if (values[i] is double v)
			{
				if (prev.HasValue && v > prev.Value)
				{
					gradientSum += v - prev.Value;
					gradientCount++;
				}

				prev = v;
			}
		}

		return gradientCount > 0 ? gradientSum / gradientCount : 0;
	}

	private static (int totalDowntime, int totalExpectedDataPoints) CalculateDowntime(List<double?> values, double gradient)
	{
		var totalDowntime = 0;
		var totalExpectedDataPoints = 0;
		double? previousValue = null;

		for (var i = 0; i < values.Count; i++)
		{
			var value = values[i];
			if (value is double doubleValue)
			{
				if (previousValue.HasValue)
				{
					// Explicitly handle counter resets
					if (doubleValue < previousValue.Value)
					{
						// Counter reset detected, skip gap/downtime calculation for this pair
						previousValue = doubleValue;
						continue;
					}

					var expectedNext = previousValue.Value + gradient;
					// Use a tolerance for floating-point comparison
					if (doubleValue > previousValue.Value && Math.Abs(doubleValue - expectedNext) > gradient * 1.1)
					{
						var gap = (int)Math.Round((doubleValue - previousValue.Value) / gradient) - 1;
						if (gap > 0)
						{
							var projectedStart = doubleValue - gap * gradient - gradient;
							var downtimeForGap = gap + (int)Math.Round((previousValue.Value - projectedStart) / gradient);
							totalDowntime += downtimeForGap;
							totalExpectedDataPoints += gap;
						}
					}
				}

				previousValue = doubleValue;
			}
			else
			{
				totalDowntime++;
				totalExpectedDataPoints++;
			}
		}
		// Add actual data points to total expected
		totalExpectedDataPoints += values.Count(v => v is double);
		return (totalDowntime, totalExpectedDataPoints);
	}

	#region SDT Handling

	/// <summary>
	/// Gets the SDT (Scheduled Down Time) periods for a device data source instance within the specified time range.
	/// MS-21395: This method retrieves ScheduledDownTimeHistory from the LogicMonitor API at multiple levels:
	/// - Device Data Source Instance level
	/// - Device Data Source level
	/// - Device level (cached)
	/// - All Device Group levels up to the root (cached)
	/// </summary>
	/// <param name="deviceNotTracked">The device to get SDT periods for</param>
	/// <param name="resourceDataSourceInstanceStoreItem">The device data source instance</param>
	/// <param name="startDateTimeUtc">Start of the time range</param>
	/// <param name="endDateTimeUtc">End of the time range</param>
	/// <param name="sdtCache">Cache for Device and DeviceGroup SDTs</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>List of SDT periods as (start timestamp in ms, end timestamp in ms) tuples</returns>
	private async static Task<List<(long StartTimestampMs, long EndTimestampMs)>> GetHistoricalSdtPeriodsAsync(
		DatamartClient datamartClient,
		ResourceStoreItem deviceNotTracked,
		ResourceDataSourceInstanceStoreItem? resourceDataSourceInstanceStoreItem,
		DateTimeOffset startDateTimeUtc,
		DateTimeOffset endDateTimeUtc,
		Dictionary<string, List<ScheduledDownTimeHistory>> sdtCache,
		ILogger logger,
		CancellationToken cancellationToken)
	{
		logger.LogInformation("Getting historical SDTs for Device ID {DeviceId} ({Start:yyyy-MM-dd HH:mm:ss} .. {End:yyyy-MM-dd HH:mm:ss})...",
			deviceNotTracked.LogicMonitorId,
			startDateTimeUtc,
			endDateTimeUtc);

		// MS-21395 - Retrieve all historical SDT periods that overlap with the time range
		var allSdtHistories = new List<ScheduledDownTimeHistory>();
		var deviceId = deviceNotTracked.LogicMonitorId;
		
		try
		{
			using var context = datamartClient.GetContext();

			if (resourceDataSourceInstanceStoreItem is null)
			{
				// Log a warning to help with logging / issues...
				logger.LogWarning(
					"ResourceDataSourceInstance (DeviceDataSourceInstance) is null for Device ID {DeviceId}. Skipping SDT retrieval.",
					deviceId);

				return [];
			}

			// Find the ResourceDataSourceStoreItem (DeviceDataSource)
			if (context.
					DeviceDataSources.
					SingleOrDefault(dds => dds.Id == resourceDataSourceInstanceStoreItem.DeviceDataSourceId)
					is not ResourceDataSourceStoreItem resourceDataSource)
			{
				// Log a warning to help with logging / issues...
				logger.LogWarning(
					"Could not find ResourceDataSource (DeviceDataSource) for DeviceDataSourceInstance ID {DeviceDataSourceInstanceId}. Skipping SDT retrieval.",
					resourceDataSourceInstanceStoreItem.Id);

				return [];
			}
				
			var resourceDataSourceInstanceLmId = resourceDataSourceInstanceStoreItem.LogicMonitorId;
			var resourceDataSourceLmId = resourceDataSource.LogicMonitorId;

			// Device Data Source Instance historical SDTs (not cached - instance-specific)
			var deviceDataSourceInstanceSdts = 
				await GetHistoricSdtsForDeviceDataSourceInstance(
					datamartClient,
					deviceId,
					resourceDataSourceLmId,
					resourceDataSourceInstanceLmId,
					cancellationToken)
				.ConfigureAwait(false);

			// Add them
			allSdtHistories.AddRange(deviceDataSourceInstanceSdts);

			// Device Data Source historical SDTs (not cached - data source-specific)
			var deviceDataSourceSdts =
				await GetHistoricSdtsForDeviceDataSource(
					datamartClient,
					deviceId,
					resourceDataSourceLmId,
					cancellationToken)
				.ConfigureAwait(false);

			// Add them
			allSdtHistories.AddRange(deviceDataSourceSdts);

			// Device historical SDTs (cached)
			var deviceCacheKey = $"Device_{deviceId}";
			if (!sdtCache.TryGetValue(deviceCacheKey, out var deviceSdts))
			{
				deviceSdts = await datamartClient
					.GetResourceHistorySdtsAsync(
						deviceId,
						cancellationToken)
					.ConfigureAwait(false);
				
				sdtCache[deviceCacheKey] = deviceSdts;

				logger.LogDebug(
					"Retrieved and cached Device SDTs for Device ID {DeviceId} (cache miss)",
					deviceId);
			}
			else
			{
				logger.LogDebug(
					"Using cached Device SDTs for Device ID {DeviceId} (cache hit)",
					deviceId);
			}

			// Add them
			allSdtHistories.AddRange(deviceSdts);
			
			// Device Group level SDTs (up to root) - cached
			if (!string.IsNullOrWhiteSpace(deviceNotTracked.DeviceGroupIdsString))
			{
				var deviceGroupIds = deviceNotTracked.DeviceGroupIdsString
					.Split(',')
					.Select(id => int.TryParse(id, out var parsed) ? parsed : 0)
					.Where(id => id > 0);
				
				foreach (var deviceGroupId in deviceGroupIds)
				{
					var groupSdts =
						await GetHistoricSdtsForDeviceGroupToRootAsync(
							datamartClient,
							deviceGroupId,
							sdtCache,
							logger,
							cancellationToken)
						.ConfigureAwait(false);

					// Add them
					allSdtHistories.AddRange(groupSdts);
				}
			}
			
			// Processing - convert SDT histories to timestamp tuples, filtering by date range
			var sEpochSeconds = startDateTimeUtc.ToUnixTimeSeconds();
			var eEpochSeconds = endDateTimeUtc.ToUnixTimeSeconds();
			
			var sdtPeriods = allSdtHistories
				.Where(sdt => 
					// SDT overlaps with our time range
					sdt.ApproximateStartEpoch < eEpochSeconds && 
					sdt.ApproximateEndEpoch > sEpochSeconds)
				.Select(sdt => (
					StartTimestampMs: sdt.ApproximateStartEpoch * 1000L,
					EndTimestampMs: sdt.ApproximateEndEpoch * 1000L
				))
				.Distinct()
				.ToList();
			
			return sdtPeriods;
		}
		catch (Exception ex)
		{
			logger.LogError("Error retrieving SDTs for device ID {DeviceId}. Returning empty SDT list. Failure: {Message}", deviceId, ex.Message);
			return [];
		}
	}
	
	/// <summary>
	/// Gets historical SDTs for a device group and all parent groups up to root.
	/// MS-21395: Helper method to traverse the device group hierarchy with caching.
	/// </summary>
	/// <param name="deviceGroupId">The device group ID to start from</param>
	/// <param name="sdtCache">Cache for DeviceGroup SDTs</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>List of SDT histories for the group and all parent groups</returns>
	private async static Task<List<ScheduledDownTimeHistory>> GetHistoricSdtsForDeviceGroupToRootAsync(
		DatamartClient datamartClient,
		int deviceGroupId,
		Dictionary<string, List<ScheduledDownTimeHistory>> sdtCache,
		ILogger logger,
		CancellationToken cancellationToken)
	{
		var sdts = new List<ScheduledDownTimeHistory>();
		
		try
		{
			// Get SDTs for current group (cached)
			var groupCacheKey = $"DeviceGroup_{deviceGroupId}";
			if (!sdtCache.TryGetValue(groupCacheKey, out var groupSdts))
			{
				groupSdts = await datamartClient
					.GetResourceGroupHistorySdtsAsync(
						deviceGroupId,
						cancellationToken)
					.ConfigureAwait(false);
				
				sdtCache[groupCacheKey] = groupSdts;
				
				logger.LogDebug(
					"Retrieved and cached DeviceGroup SDTs for DeviceGroup ID {DeviceGroupId} (cache miss)",
					deviceGroupId);
			}
			else
			{
				logger.LogDebug(
					"Using cached DeviceGroup SDTs for DeviceGroup ID {DeviceGroupId} (cache hit)",
					deviceGroupId);
			}

			sdts.AddRange(groupSdts);
		}
		catch (Exception ex)
		{
			logger.LogError("Error retrieving SDTs for device group ID {DeviceGroupId}. Stopping traversal. Failure: {Message}", deviceGroupId, ex.Message);
			return sdts;
		}
		
		// Traverse up to root (parentId = 1)
		while (deviceGroupId != 1)
		{
			try
			{
				// Fetch the device group to get parent ID
				var deviceGroup =
					await datamartClient
						.GetAsync<ResourceGroup>(
						deviceGroupId,
						cancellationToken)
					.ConfigureAwait(false);
				
				if (deviceGroup == null || deviceGroup.ParentId == deviceGroupId)
				{
					break; // Avoid infinite loop
				}
				
				deviceGroupId = deviceGroup.ParentId;
				
				// Get parent group SDTs (cached)
				var parentGroupCacheKey = $"DeviceGroup_{deviceGroupId}";
				if (!sdtCache.TryGetValue(parentGroupCacheKey, out var parentSdts))
				{
					parentSdts = await datamartClient
						.GetResourceGroupHistorySdtsAsync(
							deviceGroupId,
							cancellationToken)
						.ConfigureAwait(false);
					
					sdtCache[parentGroupCacheKey] = parentSdts;
					
					logger.LogDebug(
						"Retrieved and cached DeviceGroup SDTs for parent DeviceGroup ID {DeviceGroupId} (cache miss)",
						deviceGroupId);
				}
				else
				{
					logger.LogDebug(
						"Using cached DeviceGroup SDTs for parent DeviceGroup ID {DeviceGroupId} (cache hit)",
						deviceGroupId);
				}
				
				sdts.AddRange(parentSdts);
			}
			catch (Exception ex)
			{
				logger.LogError("Error retrieving SDTs for device group ID {DeviceGroupId}. Stopping traversal. Error: {Message}", deviceGroupId, ex.Message);
				break;
			}
		}
		return sdts;
	}

	private async static Task<List<ScheduledDownTimeHistory>> GetHistoricSdtsForDeviceDataSource(
		DatamartClient datamartClient,
		int deviceId,
		int deviceDataSourceId,
		CancellationToken cancellationToken)
		=> await datamartClient
			.GetResourceDataSourceHistorySdtsAsync(
				deviceId,
				deviceDataSourceId,
				cancellationToken)
			.ConfigureAwait(false);

	private async static Task<List<ScheduledDownTimeHistory>> GetHistoricSdtsForDeviceDataSourceInstance(
		DatamartClient datamartClient,
		int deviceId,
		int deviceDataSourceId,
		int deviceDataSourceInstanceId,
		CancellationToken cancellationToken)
		=> await datamartClient
			.GetResourceDataSourceInstanceHistorySdtsAsync(
				deviceId,
				deviceDataSourceId,
				deviceDataSourceInstanceId,
				cancellationToken)
			.ConfigureAwait(false);

	#endregion
}