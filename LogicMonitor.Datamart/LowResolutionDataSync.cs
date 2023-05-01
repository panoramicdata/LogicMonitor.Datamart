using LogicMonitor.Api.Data;
using LogicMonitor.Api.Time;
using PanoramicData.NCalcExtensions;

namespace LogicMonitor.Datamart;

internal class LowResolutionDataSync : LoopInterval
{
	private static readonly TimeSpan EightHours = TimeSpan.FromHours(8);

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
		Logger.LogInformation("Data sync started...");

		using var context = new Context(_datamartClient.DbContextOptions);
		// Use the database as a reference for what should be loaded in to ensure referential integrity between the data and the DeviceDataSourceInstance

		// Get the configured DataSource names
		Logger.LogInformation("Getting reference data...");
		var deviceDataSourceNames = _configuration.DataSources
			.ConvertAll(dsci => dsci.Name);

		// Get the database DataSources for those names
		var matchingDatabaseDataSources = await context
			.DataSources
			.Where(ds => deviceDataSourceNames.Contains(ds.Name))
			.ToListAsync(cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		// Get the LogicMonitor Ids for those DataSources
		var dataSourceIds = matchingDatabaseDataSources
			.ConvertAll(ds => ds.LogicMonitorId);

		// Get the database instances for those DataSources, excluding ones where LastWentMissingUtc is set
		var databaseDeviceDataSourceInstanceDataPoints = await context
			.DeviceDataSourceInstanceDataPoints
			.Include(ddsidp => ddsidp.DeviceDataSourceInstance!.DeviceDataSourceInstanceDataPoints)
			.Include(ddsidp => ddsidp.DeviceDataSourceInstance!.DeviceDataSource!.DataSource)
			.Include(ddsidp => ddsidp.DeviceDataSourceInstance!.DeviceDataSource!.Device)
			.Include(ddsidp => ddsidp.DataSourceDataPoint)
			.Where(ddsi =>
				ddsi.DeviceDataSourceInstance!.LastWentMissing == null
				&& dataSourceIds.Contains(ddsi.DeviceDataSourceInstance!.DeviceDataSource!.DataSource!.LogicMonitorId)
			)
			// To make debugging a little more deterministic, order by the Device and then its instances
			.OrderBy(ddsi => ddsi.DeviceDataSourceInstance!.DeviceDataSourceId)
			.ThenBy(ddsi => ddsi.LogicMonitorId)
			.ToListAsync(cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		// If there aren't any, log and return
		if (!databaseDeviceDataSourceInstanceDataPoints.Any())
		{
			Logger.LogWarning(
				"Found no DeviceDataSourceInstanceDataPoints in the databases for DeviceDataSource names {Names}. Check dimensions have been synced.",
				string.Join(", ", deviceDataSourceNames));
			return;
		}
		// We have the database deviceDataSourceInstances for the configured DataSources

		// Clear out the PortalClient Cache, otherwise we remember all the data values for no reason
		_datamartClient.ClearCache();
		var oldCacheState = _datamartClient.UseCache;
		_datamartClient.UseCache = false;

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

		foreach (var databaseDeviceDataSourceInstanceGroup in databaseDeviceDataSourceInstanceDataPoints.GroupBy(ddsidp => ddsidp.DeviceDataSourceInstance))
		{
			DeviceDataSourceInstanceStoreItem databaseDeviceDataSourceInstance = databaseDeviceDataSourceInstanceGroup.Key!;
			var lastAggregationHourWrittenUtc = databaseDeviceDataSourceInstance.DataCompleteTo is null
				? configuration.StartDateTimeUtc
				: databaseDeviceDataSourceInstance.DataCompleteTo.Value;

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

			string dataSourceName = databaseDeviceDataSourceInstance!.DeviceDataSource!.DataSource!.Name;

			// Get the configuration for this DataSourceName
			var dataSourceConfigurationItem = configuration
				.DataSources
				.SingleOrDefault(dsci =>
				{
					return dsci.Name == dataSourceName;
				})
				?? throw new InvalidOperationException($"Could not find configuration for DataSource {dataSourceName}.");

			while (endDateTime < utcNow)
			{
				foreach (var databaseDeviceDataSourceInstanceDataPoint in databaseDeviceDataSourceInstanceGroup)
				{
					string dataPointName = databaseDeviceDataSourceInstanceDataPoint.DataSourceDataPoint!.Name;

					TimeSeriesDataAggregationStoreItem bulkWriteModel;

					var dataPointStoreItem = dataPointStoreItems
						.SingleOrDefault(dp => dp.Name == dataPointName && dp.DataSource!.Name == dataSourceConfigurationItem.Name);

					var graphData = await datamartClient
						.GetGraphDataAsync(
							new DeviceDataSourceInstanceGraphDataRequest
							{
								DeviceDataSourceInstanceId = databaseDeviceDataSourceInstance.LogicMonitorId,
								StartDateTime = startDateTime.UtcDateTime,
								EndDateTime = endDateTime.UtcDateTime,
								TimePeriod = TimePeriod.Zoom,
								DataSourceGraphId = -1,
							},
							cancellationToken)
						.ConfigureAwait(false);

					if (string.IsNullOrWhiteSpace(databaseDeviceDataSourceInstanceDataPoint.DataSourceDataPoint.Calculation))
					{
						var line = graphData
							.Lines
							.SingleOrDefault(dp =>
							{
								return dp.Legend == dataPointName;
							});

						if (line is null || dataPointStoreItem is null)
						{
							continue;
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
						};
					}
					else
					{
						var expression = new ExtendedExpression(databaseDeviceDataSourceInstanceDataPoint.DataSourceDataPoint.Calculation);
						var lineData = graphData.TimeStamps.Select((ts, index) =>
						{
							expression.Parameters.Clear();
							foreach (var line in graphData.Lines)
							{
								expression.Parameters.Add(line.Legend, line.Data[index]);
							}

							return expression.Evaluate() as double?;
						})
						.ToArray();

						// Calculate and sort non-null values
						var sortedNonNullValues = lineData
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
							DataCount = lineData.Count(d => d.HasValue),
							NoDataCount = lineData.Count(d => !d.HasValue),
							Sum = lineData.Sum(d => d ?? 0),
							SumSquared = lineData.Sum(d => d.HasValue ? d.Value * d.Value : 0),
							Max = lineData.Where(d => d != null).DefaultIfEmpty(null).Max(),
							Min = lineData.Where(d => d != null).DefaultIfEmpty(null).Min(),
							First = lineData.Where(d => d != null).DefaultIfEmpty(null).First(),
							Last = lineData.Where(d => d != null).DefaultIfEmpty(null).Last(),
							FirstWithData = lineData.Where(d => d != null).DefaultIfEmpty(null).First(),
							LastWithData = lineData.Where(d => d != null).DefaultIfEmpty(null).Last(),
							Centile05 = CalculatePercentile(sortedNonNullValues, 5),
							Centile10 = CalculatePercentile(sortedNonNullValues, 10),
							Centile25 = CalculatePercentile(sortedNonNullValues, 25),
							Centile75 = CalculatePercentile(sortedNonNullValues, 75),
							Centile90 = CalculatePercentile(sortedNonNullValues, 90),
							Centile95 = CalculatePercentile(sortedNonNullValues, 95),
							NormalCount = CountAtAlertLevel(
								lineData,
								dataPointStoreItem.GlobalAlertExpression,
								CountAlertLevel.Normal
							),
							WarningCount = CountAtAlertLevel(
								lineData,
								dataPointStoreItem.GlobalAlertExpression,
								CountAlertLevel.Warning
							),
							ErrorCount = CountAtAlertLevel(
								lineData,
								dataPointStoreItem.GlobalAlertExpression,
								CountAlertLevel.Error
							),
							CriticalCount = CountAtAlertLevel(
								lineData,
								dataPointStoreItem.GlobalAlertExpression,
								CountAlertLevel.Critical
							),
						};
					}


					aggregationsToWrite.Add(bulkWriteModel);
				}

				databaseDeviceDataSourceInstance.DataCompleteTo = endDateTime;
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

		logger.LogInformation("Syncing data complete.");
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
