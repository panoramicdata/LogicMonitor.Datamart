using LogicMonitor.Api.Data;
using LogicMonitor.Api.Time;
using PanoramicData.NCalcExtensions;

namespace LogicMonitor.Datamart;

internal class LowResolutionDataSync : LoopInterval
{
	private static readonly TimeSpan EightHours = TimeSpan.FromHours(8);
	private const int DeviceDownTimeWindowSeconds = 300;

	private readonly DatamartClient _datamartClient;
	private readonly Configuration _configuration;

	public LowResolutionDataSync(
		DatamartClient datamartClient,
		Configuration configuration,
		ILoggerFactory loggerFactory)
		: base(nameof(LowResolutionDataSync), loggerFactory)
	{
		_datamartClient = datamartClient;
		_configuration = configuration;
	}

	public override async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		Logger.LogInformation(
			"Data sync started for {DatabaseName}...",
			_configuration.DatabaseName
			);

		using var context = new Context(_datamartClient.DbContextOptions);
		// Use the database as a reference for what should be loaded in to ensure referential integrity between the data and the DeviceDataSourceInstance

		// Get the configured DataSource names
		Logger.LogInformation(
			"Getting reference data for {DatabaseName}: DataSources...",
			_configuration.DatabaseName
		);
		var dataSourceNames = _configuration.DataSources
			.ConvertAll(dsci => dsci.Name);

		// Get the database DataSources for those names
		var matchingDatabaseDataSources = await context
			.DataSources
			.Where(ds => dataSourceNames.Contains(ds.Name))
			.ToListAsync(cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		Logger.LogInformation(
			"Getting reference data for {DatabaseName}: DataSources - found {DatabaseDataSourceCount}",
			_configuration.DatabaseName,
			matchingDatabaseDataSources.Count
		);

		// Get the LogicMonitor Ids for those DataSources
		var dataSourceIds = matchingDatabaseDataSources
			.ConvertAll(ds => ds.LogicMonitorId);

		// Get the database instances for those DataSources, excluding ones where LastWentMissingUtc is set
		Logger.LogInformation(
			"Getting reference data for {DatabaseName}: DeviceDataSourceInstanceDataPoints...",
			_configuration.DatabaseName
		);
		// Clear out the PortalClient Cache, otherwise we remember all the data values for no reason
		_datamartClient.ClearCache();
		var oldCacheState = _datamartClient.UseCache;
		_datamartClient.UseCache = false;

		try
		{
			// IMPORTANT: This must be done per device, as doing all at once causes database timeouts

			// Get a list of devices
			var devices = await context
				.Devices
				.ToListAsync(cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			var deviceCount = devices.Count;
			var deviceIndex = 0;
			var failedDeviceDisplayNames = new List<string>();
			// For each device, get the list of DeviceDataSourceInstanceDataPoints
			foreach (var device in devices)
			{
				deviceIndex++;

				Logger.LogInformation(
					"Getting DeviceDataSourceInstanceDataPoints for {DatabaseName}: {DeviceName} ({DeviceIndex}/{DeviceCount})...",
					_configuration.DatabaseName,
					device.DisplayName,
					deviceIndex,
					deviceCount
				);

				var databaseDeviceDataSourceInstanceDataPoints = await context
					.DeviceDataSourceInstanceDataPoints
					.Include(ddsidp => ddsidp.DeviceDataSourceInstance!.DeviceDataSource!.DataSource)
					.Include(ddsidp => ddsidp.DeviceDataSourceInstance!.DeviceDataSource!.Device)
					.Include(ddsidp => ddsidp.DataSourceDataPoint)
					.Where(ddsi =>
						ddsi.DeviceDataSourceInstance!.DeviceDataSource!.DeviceId == device.Id
						&& ddsi.DeviceDataSourceInstance!.LastWentMissing == null
						&& dataSourceIds.Contains(ddsi.DeviceDataSourceInstance!.DeviceDataSource!.DataSource!.LogicMonitorId)
					)
					// To make debugging a little more deterministic, order by the Device and then its instances
					.OrderBy(ddsi => ddsi.DeviceDataSourceInstance!.DeviceDataSourceId)
					.ThenBy(ddsi => ddsi.LogicMonitorId)
					.ToListAsync(cancellationToken: cancellationToken)
					.ConfigureAwait(false);

				var databaseDeviceDataSourceInstanceDataPointsCount = databaseDeviceDataSourceInstanceDataPoints.Count;

				Logger.LogDebug(
					"Getting DeviceDataSourceInstanceDataPoints for {DatabaseName}: {DeviceName} ({DeviceIndex}/{DeviceCount}) complete.  Found {DeviceDataSourceInstanceDataPointCount}.",
					_configuration.DatabaseName,
					device.DisplayName,
					deviceIndex,
					deviceCount,
					databaseDeviceDataSourceInstanceDataPointsCount
					);

				// If there aren't any, log and return
				if (databaseDeviceDataSourceInstanceDataPointsCount == 0)
				{
					continue;
				}
				// We have the database deviceDataSourceInstances for the configured DataSources

				Logger.LogInformation(
					"Writing aggregations for {DatabaseName}: {DeviceName} ({DeviceIndex}/{DeviceCount}) for {DatabaseDeviceDataSourceInstanceDataPointsCount} DeviceDataSourceInstanceDataPoints...",
					_configuration.DatabaseName,
					device.DisplayName,
					deviceIndex,
					deviceCount,
					databaseDeviceDataSourceInstanceDataPointsCount
					);
				try
				{
					await GetAndWriteAggregationsAsync(
						_datamartClient,
						context,
						_configuration,
						Logger,
						databaseDeviceDataSourceInstanceDataPoints,
						cancellationToken)
						.ConfigureAwait(false);
					Logger.LogDebug(
						"Writing aggregations for {DatabaseName}: {DeviceName} ({DeviceIndex}/{DeviceCount}) complete.",
						_configuration.DatabaseName,
						device.DisplayName,
						deviceIndex,
						deviceCount
						);
				}
				catch (Exception ex)
				{
					Logger.LogError(
						ex,
						"Writing aggregations for {DatabaseName}: {DeviceName} ({DeviceIndex}/{DeviceCount}) failed: '{Message}' ||| {StackTrace}",
						_configuration.DatabaseName,
						device.DisplayName,
						deviceIndex,
						deviceCount,
						ex.Message,
						ex.StackTrace
					);
					failedDeviceDisplayNames.Add(device.DisplayName);
				}
			}

			if (failedDeviceDisplayNames.Count == 0)
			{
				Logger.LogInformation(
					"Aggregations written for {DatabaseName}: ({ErrorCount} failed devices).",
					_configuration.DatabaseName,
					failedDeviceDisplayNames.Count
				);
			}
			else
			{
				Logger.LogError(
					"Aggregations written for {DatabaseName}: ({ErrorCount} failed devices). Failed devices (up to 10): {FailedDeviceDisplayNames}",
					_configuration.DatabaseName,
					failedDeviceDisplayNames.Count,
					failedDeviceDisplayNames.Count switch
					{
						1 => failedDeviceDisplayNames[0],
						_ => $"{string.Join(", ", failedDeviceDisplayNames.Take(10))}"
					}
				);
			}
		}
		finally
		{
			_datamartClient.UseCache = oldCacheState;
		}
	}

	public static double? CalculatePercentile(double[] values, int n)
	{
		// return null if the list is empty
		if (values.Length == 0)
		{
			return null;
		}

		// calculate the index of the value at the nth percentile
		double index = ((n / 100.0) * (values.Length - 1));

		// check if the index is an integer
		if (index % 1 == 0)
		{
			// if the index is an integer, return the corresponding value
			return values[(int)index];
		}
		else
		{
			// if the index is not an integer, interpolate between the two closest values
			int floorIndex = (int)Math.Floor(index);
			int ceilIndex = (int)Math.Ceiling(index);

			double floorValue = values[floorIndex];
			double ceilValue = values[ceilIndex];

			return floorValue + ((index - floorIndex) * (ceilValue - floorValue));
		}
	}


	private async Task GetAndWriteAggregationsAsync(
		DatamartClient datamartClient,
		Context context,
		Configuration configuration,
		ILogger logger,
		List<DeviceDataSourceInstanceDataPointStoreItem> databaseDeviceDataSourceInstanceDataPoints,
		CancellationToken cancellationToken)
	{
		// To ignore a period of uncertainty whether the Collector has been
		// able to publish its measurement data to the LogicMonitor API,
		// we consider "now" to be X hours ago.
		// This is "B" in the diagram below.
		var utcNow = DateTimeOffset.UtcNow;

		var aggregationsToWrite = new List<TimeSeriesDataAggregationStoreItem>();

		var stopwatch = new Stopwatch();

		// Get data for each instance
		logger.LogInformation(
			"Syncing {DeviceDataSourceInstanceDataPointCount} DataPoints across {InstanceCount} DeviceDataSourceInstances...",
			databaseDeviceDataSourceInstanceDataPoints.Count,
			databaseDeviceDataSourceInstanceDataPoints.Select(ddsidp => ddsidp.DeviceDataSourceInstanceId).Distinct().Count()
		);

		var dataPointStoreItems = await context
			.DataSourceDataPoints
			.Include(dsdp => dsdp.DataSource)
			.AsNoTracking()
			.ToListAsync(cancellationToken)
			.ConfigureAwait(false);

		await ResetForResyncAsync(
			context,
			logger,
			databaseDeviceDataSourceInstanceDataPoints,
			dataPointStoreItems,
			cancellationToken)
			.ConfigureAwait(false);

		// Disable caching
		var oldCacheState = datamartClient.UseCache;
		datamartClient.UseCache = false;

		foreach (var databaseDeviceDataSourceInstanceDataPoint in databaseDeviceDataSourceInstanceDataPoints)
		{
			var lastAggregationHourWrittenUtc = databaseDeviceDataSourceInstanceDataPoint.DataCompleteTo is null
				? configuration.StartDateTimeUtc
				: databaseDeviceDataSourceInstanceDataPoint.DataCompleteTo.Value;

			// Ensure that this is on a month boundary
			lastAggregationHourWrittenUtc = new DateTimeOffset(
				lastAggregationHourWrittenUtc.Year,
				lastAggregationHourWrittenUtc.Month,
				1, 0, 0, 0,
				TimeSpan.Zero);

			var startDateTime = lastAggregationHourWrittenUtc;
			var endDateTime = lastAggregationHourWrittenUtc.AddMonths(1);

			if (endDateTime >= utcNow)
			{
				continue;
			}

			string dataSourceName = databaseDeviceDataSourceInstanceDataPoint.DeviceDataSourceInstance!.DeviceDataSource!.DataSource!.Name;
			string dataPointName = databaseDeviceDataSourceInstanceDataPoint.DataSourceDataPoint!.Name;

			// Get the configuration for this DataSourceName
			var dataSourceConfigurationItem = configuration
				.DataSources
				.SingleOrDefault(dsci =>
				{
					return dsci.Name == dataSourceName;
				})
				?? throw new InvalidOperationException($"Could not find configuration for DataSource {dataSourceName}.");

			var dataPointStoreItem = dataPointStoreItems
				.SingleOrDefault(dp => dp.Name == dataPointName && dp.DataSource!.Name == dataSourceConfigurationItem.Name);

			while (endDateTime < utcNow)
			{
				TimeSeriesDataAggregationStoreItem bulkWriteModel;

				var graphData = await datamartClient
					.GetGraphDataAsync(
						new DeviceDataSourceInstanceGraphDataRequest
						{
							DeviceDataSourceInstanceId = databaseDeviceDataSourceInstanceDataPoint.DeviceDataSourceInstance.LogicMonitorId,
							StartDateTime = startDateTime.UtcDateTime,
							EndDateTime = endDateTime.UtcDateTime,
							TimePeriod = TimePeriod.Zoom,
							DataSourceGraphId = -1,
						},
						cancellationToken)
					.ConfigureAwait(false);

				Line? line;
				if (string.IsNullOrWhiteSpace(databaseDeviceDataSourceInstanceDataPoint.DataSourceDataPoint.Calculation))
				{
					line = graphData
						.Lines
						.SingleOrDefault(dp =>
						{
							return dp.Legend == dataPointName;
						});

					if (line is null || dataPointStoreItem is null)
					{
						continue;
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
					.ToArray();
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
					PeriodStart = startDateTime.UtcDateTime,
					PeriodEnd = endDateTime.UtcDateTime,
					DataCount = line.Data.Count(d => d.HasValue),
					NoDataCount = line.Data.Count(d => !d.HasValue),
					Sum = line.Data.Sum(d => d ?? 0),
					SumSquared = line.Data.Sum(d => d.HasValue ? d.Value * d.Value : 0),
					Max = line.Data.Where(d => d != null).DefaultIfEmpty(null).Max(),
					Min = line.Data.Where(d => d != null).DefaultIfEmpty(null).Min(),
					First = line.Data.Where(d => d != null).DefaultIfEmpty(null).First(),
					Last = line.Data.Where(d => d != null).DefaultIfEmpty(null).Last(),
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
						dataPointStoreItem.GlobalAlertExpression,
						CountAlertLevel.Normal
					),
					WarningCount = CountAtAlertLevel(
						line.Data,
						dataPointStoreItem.GlobalAlertExpression,
						CountAlertLevel.Warning
					),
					ErrorCount = CountAtAlertLevel(
						line.Data,
						dataPointStoreItem.GlobalAlertExpression,
						CountAlertLevel.Error
					),
					CriticalCount = CountAtAlertLevel(
						line.Data,
						dataPointStoreItem.GlobalAlertExpression,
						CountAlertLevel.Critical
					),
					// IMPORTANT! This must be calculated last as this process can reverse the array.
					AvailabilityPercent = CalculatePercentageAvailability(
						line.Data,
						dataPointStoreItem.PercentageAvailabilityCalculation
					),
				};

				aggregationsToWrite.Add(bulkWriteModel);

				databaseDeviceDataSourceInstanceDataPoint.DataCompleteTo = endDateTime;
				startDateTime = startDateTime.AddMonths(1);
				endDateTime = endDateTime.AddMonths(1);
			}

			await context
				.BulkInsertAsync(aggregationsToWrite, cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			await context
				.SaveChangesAsync(cancellationToken)
				.ConfigureAwait(false);

			aggregationsToWrite.Clear();
		}

		// Re-enable caching
		datamartClient.UseCache = oldCacheState;

		logger.LogInformation("Syncing data complete.");
	}

	private static async Task ResetForResyncAsync(
		Context context,
		ILogger logger,
		List<DeviceDataSourceInstanceDataPointStoreItem> databaseDeviceDataSourceInstanceDataPoints,
		List<DataSourceDataPointStoreItem> dataPointStoreItems,
		CancellationToken cancellationToken
	)
	{
		// Get the list of DataSourceDataPoints that we're re-syncing
		var resyncDataPointStoreItems = dataPointStoreItems
			.Where(dsdp => dsdp.ResyncTimeSeriesData)
			.ToList();

		if (resyncDataPointStoreItems.Count > 0)
		{
			logger.LogInformation(
				"Re-syncing {ResyncDataPointStoreItemCount} DataPoints...",
				resyncDataPointStoreItems.Count
			);

			var databaseDeviceDataSourceInstanceDataPointsToResync = databaseDeviceDataSourceInstanceDataPoints
				.Where(ddsidp => resyncDataPointStoreItems.Any(dsdp => dsdp.Id == ddsidp.DataSourceDataPointId))
				.ToList();

			// Reset their Time cursor
			foreach (var databaseDeviceDataSourceInstanceDataPoint in databaseDeviceDataSourceInstanceDataPointsToResync)
			{
				databaseDeviceDataSourceInstanceDataPoint.DataCompleteTo = null;

				// Remove any related aggregations using the Bulk extensions
				var aggregationStoreItemsToRemove = await context
					.TimeSeriesDataAggregations
					.Where(tsda => tsda.DeviceDataSourceInstanceDataPointId == databaseDeviceDataSourceInstanceDataPoint.Id)
					.BatchDeleteAsync(cancellationToken)
					.ConfigureAwait(false);
			}

			// Save Changes
			await context
				.SaveChangesAsync(cancellationToken)
				.ConfigureAwait(false);
		}
	}

	public static double? CalculatePercentageAvailability(
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
		var ambiguousCount = 0;
		var isAmbiguous = true;
		foreach (var value in reversedValues)
		{
			if (value is double doubleValue)
			{
				previousDoubleValue = doubleValue;
				upTimeCount += ambiguousCount;
				ambiguousCount = 0;
				upTimeCount++;
				isAmbiguous = false;
			}
			else
			{
				if (double.IsNaN(previousDoubleValue) || previousDoubleValue < DeviceDownTimeWindowSeconds)
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

		var totalCount = upTimeCount + downTimeCount;

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
		double?[] data,
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
