using LogicMonitor.Api.Data;
using LogicMonitor.Api.Time;
using LogicMonitor.Datamart.Interfaces;
using LogicMonitor.Datamart.Notifications;
using LogicMonitor.Datamart.Services;
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
	private static readonly TimeSpan EightHours = TimeSpan.FromHours(8);
	private const int DeviceDownTimeWindowSeconds = 3000;

	private readonly DatamartClient _datamartClient = datamartClient;
	private readonly Configuration _configuration = configuration;
	private readonly ITimeProviderService _timeProviderService = timeProviderService;
	private readonly INotificationReceiver _notificationReceiver = notificationReceiver ?? new NullNotificationReceiver();

	public override async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		Logger.LogInformation(
			"Data sync started for {DatabaseName}...",
			_configuration.DatabaseName
			);

		if (_configuration.AggregationReset == true)
		{
			await AggregationResetAsync(cancellationToken)
				.ConfigureAwait(false);
		}

		List<DataSourceStoreItem> matchingDatabaseDataSourcesNotTracked;
		Dictionary<int, DeviceStoreItem> allDatabaseDevicesByLogicMonitorIdNotTracked;
		using (var contextForReferenceDataNotTracked = _datamartClient.GetContext())
		{
			// Use the database as a reference for what should be loaded in to ensure referential integrity between the data and the DeviceDataSourceInstance
			// Get the configured DataSource names
			Logger.LogInformation(
				"Getting reference data for {DatabaseName}: DataSources...",
				_configuration.DatabaseName
			);

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
				matchingDatabaseDataSourcesNotTracked.Count
			);

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

		// Do the equivalent of a TRUNCATE TABLE for TimeSeriesDataAggregations.  This is a postgres database
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
		Dictionary<int, DeviceStoreItem> allDatabaseDevicesByLogicMonitorIdNotTracked,
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
		DeviceStoreItem? databaseDeviceNotTracked,
		CacheStats deviceCacheStats,
		CancellationToken cancellationToken)
	{
		deviceStopwatch.Restart();

		// Use a fresh context for each device to avoid tracking too many objects unnecessarily
		using (var context = _datamartClient.GetContext())
		{
			// Get the list of DeviceDataSourceInstanceDataPoints
			var databaseDeviceDataSourceInstanceDataPoints = await context
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

		// Update the total duration for this device across all datasources
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

		// Update the Device in the context to set its LastTimeSeriesDataSyncDurationMs to the overall time spent on this device for all datasources.
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
		DeviceStoreItem deviceNotTracked,
		List<DeviceDataSourceInstanceDataPointStoreItem> databaseDeviceDataSourceInstanceDataPoints,
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
		foreach (var databaseDeviceDataSourceInstanceDataPointGroup in databaseDeviceDataSourceInstanceDataPoints.GroupBy(ddsidp => ddsidp.DeviceDataSourceInstance!.LogicMonitorId))
		{
			var graphDataCache = new Dictionary<string, GraphData>();

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

					if (endDateTimeUtc.AddMinutes(configuration.MinutesOffset) >= utcNow)
					{
						Logger.LogInformation(
							"Skipped writing aggregations because the end date time + minutes offset (from configuration) was greater than UTC now. " +
							"Start date UTC: {StartDateUtc}. " +
							"End date UTC: {EndDateUtc}. " +
							"Minutes Offset: {MinutesOffset}. " +
							"End date UTC with offset: {EndDateUtcWithOffset}. " +
							"Database: {DatabaseName}. " +
							"DeviceDataSourceInstanceDataPointId: {DeviceDataSourceInstanceDataPointId}. " +
							"DataSourceDataPointId: {DataSourceDataPointId}. " +
							"DeviceDataSourceInstanceId: {DeviceDataSourceInstanceId}.", 
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
						);

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

						var bulkWriteModel = GetTimeSeriesDataAggregationStoreItem(
							deviceNotTracked,
							databaseDeviceDataSourceInstanceDataPoint,
							dataPointName,
							dataSourceDataPointStoreItemNotTracked,
							startDateTimeUtc,
							endDateTimeUtc,
							graphData
						);

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

		// Re-enable caching
		datamartClient.UseCache = oldCacheState;
	}

	internal static async Task<GraphData> GetGraphDataAsync(
		DatamartClient datamartClient,
		int deviceDataSourceInstanceId,
		DateTimeOffset startDateTimeOffset,
		DateTimeOffset endDateTimeOffset,
		ILogger logger,
		CancellationToken cancellationToken
	)
	{
		logger.LogDebug(
			"Getting graph data for DeviceDataSourceInstance {DeviceDataSourceInstanceId} ({Start:yyyy-MM-dd HH:mm:ss} .. {End:yyyy-MM-dd HH:mm:ss})...",
			deviceDataSourceInstanceId,
			startDateTimeOffset,
			endDateTimeOffset);

		return await datamartClient
			.GetGraphDataAsync(
				new DeviceDataSourceInstanceGraphDataRequest
				{
					DeviceDataSourceInstanceId = deviceDataSourceInstanceId,
					StartDateTime = startDateTimeOffset.UtcDateTime,
					EndDateTime = endDateTimeOffset.UtcDateTime,
					TimePeriod = TimePeriod.Zoom,
					DataSourceGraphId = -1,
				},
				cancellationToken)
			.ConfigureAwait(false);
	}

	internal static TimeSeriesDataAggregationStoreItem? GetTimeSeriesDataAggregationStoreItem(
		DeviceStoreItem deviceNotTracked,
		DeviceDataSourceInstanceDataPointStoreItem databaseDeviceDataSourceInstanceDataPoint,
		string dataPointName,
		DataSourceDataPointStoreItem? dataPointStoreItemNotTracked,
		DateTimeOffset startDateTimeUtc,
		DateTimeOffset endDateTimeUtc,
		GraphData graphData)
	{
		TimeSeriesDataAggregationStoreItem bulkWriteModel;

		// Remove all DataPoint values before the device was added to LogicMonitor
		foreach (var graphDataLine in graphData.Lines)
		{
			graphDataLine.Data = graphDataLine.Data
			.Where((dp, index) =>
			{
				return graphData.TimeStamps[index] >= deviceNotTracked.CreatedOnSeconds * 1000;
			})
				.ToList();
		}

		graphData.TimeStamps = graphData.TimeStamps
			.Where(ts => ts >= deviceNotTracked.CreatedOnSeconds * 1000)
			.ToList();

		Line? line;
		if (string.IsNullOrWhiteSpace(databaseDeviceDataSourceInstanceDataPoint.DataSourceDataPoint!.Calculation))
		{
			line = graphData
				.Lines
				.SingleOrDefault(dp =>
				{
					return dp.Legend == dataPointName;
				});

			if (line is null || dataPointStoreItemNotTracked is null)
			{
				throw new FormatException($"Could not find DataPoint '{dataPointName}' for DataSource '{databaseDeviceDataSourceInstanceDataPoint.DataSourceDataPoint.DataSource.Name}'");
			}
		}
		else
		{
			var expression = new ExtendedExpression(databaseDeviceDataSourceInstanceDataPoint.DataSourceDataPoint.Calculation);
			line = new Line();
			line.Data = graphData.TimeStamps.Select((ts, index) =>
			{
				expression.Parameters.Clear();
				foreach (var line in graphData.Lines)
				{
					expression.Parameters.Add(line.Legend, line.Data[index]);
				}

				return expression.Evaluate() as double?;
			})
			.ToList();
		}

		// Calculate and sort non-null values
		var sortedNonNullValues = line.Data
			.Where(v => v.HasValue)
			.Select(v => v.Value)
			.OrderBy(v => v)
			.ToArray();

		bulkWriteModel = new TimeSeriesDataAggregationStoreItem
		{
			Id = Guid.NewGuid(),
			DeviceDataSourceInstanceDataPointId = databaseDeviceDataSourceInstanceDataPoint.Id,
			PeriodStart = startDateTimeUtc.UtcDateTime,
			PeriodEnd = endDateTimeUtc.UtcDateTime,
			DataCount = line.Data.Count(d => d.HasValue),
			NoDataCount = line.Data.Count(d => !d.HasValue),
			Sum = line.Data.Sum(d => d ?? 0),
			SumSquared = line.Data.Sum(d => d.HasValue ? d.Value * d.Value : 0),
			Max = line.Data.Where(d => d != null).DefaultIfEmpty(null).Max(),
			Min = line.Data.Where(d => d != null).DefaultIfEmpty(null).Min(),
			First = line.Data.DefaultIfEmpty(null).First(),
			Last = line.Data.DefaultIfEmpty(null).Last(),
			FirstWithData = line.Data.Where(d => d != null).DefaultIfEmpty(null).First(),
			LastWithData = line.Data.Where(d => d != null).DefaultIfEmpty(null).Last(),
			Centile05 = CalculatePercentile(sortedNonNullValues, 5),
			Centile10 = CalculatePercentile(sortedNonNullValues, 10),
			Centile25 = CalculatePercentile(sortedNonNullValues, 25),
			Centile50 = CalculatePercentile(sortedNonNullValues, 50),
			Centile75 = CalculatePercentile(sortedNonNullValues, 75),
			Centile90 = CalculatePercentile(sortedNonNullValues, 90),
			Centile95 = CalculatePercentile(sortedNonNullValues, 95),
			NormalCount = CountAtAlertLevel(
				line.Data,
				dataPointStoreItemNotTracked.GlobalAlertExpression,
				CountAlertLevel.Normal
			),
			WarningCount = CountAtAlertLevel(
				line.Data,
				dataPointStoreItemNotTracked.GlobalAlertExpression,
				CountAlertLevel.Warning
			),
			ErrorCount = CountAtAlertLevel(
				line.Data,
				dataPointStoreItemNotTracked.GlobalAlertExpression,
				CountAlertLevel.Error
			),
			CriticalCount = CountAtAlertLevel(
				line.Data,
				dataPointStoreItemNotTracked.GlobalAlertExpression,
				CountAlertLevel.Critical
			),
			// IMPORTANT! This must be calculated last as this process can reverse the array.
			AvailabilityPercent = CalculatePercentageAvailability(
				line.Data,
				dataPointStoreItemNotTracked.PercentageAvailabilityCalculation
			),
		};

		return bulkWriteModel;
	}

	private static async Task ResetForResyncAsync(
		Context context,
		ILogger logger,
		List<DeviceDataSourceInstanceDataPointStoreItem> databaseDeviceDataSourceInstanceDataPoints,
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
}
