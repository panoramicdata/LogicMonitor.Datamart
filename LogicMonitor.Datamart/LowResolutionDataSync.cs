using LogicMonitor.Api.Data;
using LogicMonitor.Api.Time;

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
		var databaseDeviceDataSourceInstances = await context
			.DeviceDataSourceInstances
			.Include(ddsi => ddsi.DeviceDataSource.DataSource)
			.Include(ddsi => ddsi.DeviceDataSource.Device)
			.Where(ddsi =>
				ddsi.LastWentMissingUtc == null
				&& dataSourceIds.Contains(ddsi.DeviceDataSource.DataSource.LogicMonitorId)
			)
			// To make debugging a little more deterministic, order by the Device and then its instances
			.OrderBy(ddsi => ddsi.DeviceDataSourceId)
			.ThenBy(ddsi => ddsi.LogicMonitorId)
			.ToListAsync(cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		// If there aren't any, log and return
		if (!databaseDeviceDataSourceInstances.Any())
		{
			Logger.LogWarning(
				"Found no DeviceDataSourceInstances in the databases for DeviceDataSource names {Names}. Check dimensions have been synced.",
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
				databaseDeviceDataSourceInstances,
				cancellationToken)
				.ConfigureAwait(false);
		}
		finally
		{
			_datamartClient.UseCache = oldCacheState;
		}
	}

	public static double? CalculatePercentile(double?[] values, int n)
	{
		// remove null values from the list
		var nonNullValues = values
			.Where(v => v.HasValue)
			.Select(v => v.Value)
			.OrderBy(v => v)
			.ToList();

		// return null if the list is empty
		if (nonNullValues.Count == 0)
		{
			return null;
		}

		// calculate the index of the value at the nth percentile
		double index = ((n / 100.0) * (nonNullValues.Count - 1));

		// check if the index is an integer
		if (index % 1 == 0)
		{
			// if the index is an integer, return the corresponding value
			return nonNullValues[(int)index];
		}
		else
		{
			// if the index is not an integer, interpolate between the two closest values
			int floorIndex = (int)Math.Floor(index);
			int ceilIndex = (int)Math.Ceiling(index);

			double floorValue = nonNullValues[floorIndex];
			double ceilValue = nonNullValues[ceilIndex];

			return floorValue + ((index - floorIndex) * (ceilValue - floorValue));
		}
	}


	private async Task GetAndWriteAggregationsAsync(
		DatamartClient datamartClient,
		Context context,
		Configuration configuration,
		ILogger logger,
		List<DeviceDataSourceInstanceStoreItem> databaseDeviceDataSourceInstances,
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
			"Syncing {InstanceCount} DeviceDataSourceInstances...",
			databaseDeviceDataSourceInstances.Count
		);


		var dataPointStoreItems = await context
			.DataSourceDataPoints
			.Include(dsdp => dsdp.DataSource)
			.AsNoTracking()
			.ToListAsync(cancellationToken)
			.ConfigureAwait(false);

		foreach (var databaseDeviceDataSourceInstance in databaseDeviceDataSourceInstances)
		{
			var lastAggregationHourWrittenUtc = databaseDeviceDataSourceInstance.DataCompleteToUtc is null
				? configuration.StartDateTimeUtc
				: new DateTimeOffset(databaseDeviceDataSourceInstance.DataCompleteToUtc.Value, TimeSpan.Zero);

			// Ensure that this is on a month boundary
			lastAggregationHourWrittenUtc = new DateTimeOffset(
				lastAggregationHourWrittenUtc.Year,
				lastAggregationHourWrittenUtc.Month,
				1, 0, 0, 0,
				TimeSpan.Zero);

			var startDateTime = lastAggregationHourWrittenUtc;
			var endDateTime = lastAggregationHourWrittenUtc.AddMonths(1);

			while (endDateTime < DateTime.UtcNow)
			{
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

				// Get the configuration for this DataSourceName
				var dataSourceConfigurationItem = configuration
					.DataSources
					.SingleOrDefault(dsci => dsci.Name == graphData.DataSourceName);

				foreach (var dataPoint in dataSourceConfigurationItem.DataPoints)
				{
					var line = graphData
						.Lines
						.SingleOrDefault(dp => dp.Legend == dataPoint.Name);

					var dataPointStoreItem = dataPointStoreItems
						.SingleOrDefault(dp => dp.Name == dataPoint.Name && dp.DataSource!.Name == dataSourceConfigurationItem.Name);

					if (line is null || dataPointStoreItem is null)
					{
						continue;
					}

					var bulkWriteModel = new TimeSeriesDataAggregationStoreItem
					{
						Id = Guid.NewGuid(),
						DataPointId = dataPointStoreItem.Id,
						PeriodStart = startDateTime.UtcDateTime,
						PeriodEnd = endDateTime.UtcDateTime,
						DataCount = line.Data.Count(d => d.HasValue),
						NoDataCount = line.Data.Count(d => !d.HasValue),
						Sum = line.Data.Sum(d => d.HasValue ? d.Value : 0),
						SumSquared = line.Data.Sum(d => d.HasValue ? d.Value * d.Value : 0),
						Max = line.Data.Where(d => d != null).DefaultIfEmpty(null).Max(),
						Min = line.Data.Where(d => d != null).DefaultIfEmpty(null).Min(),
						First = line.Data.Where(d => d != null).DefaultIfEmpty(null).First(),
						Last = line.Data.Where(d => d != null).DefaultIfEmpty(null).Last(),
						FirstWithData = line.Data.Where(d => d != null).DefaultIfEmpty(null).First(),
						LastWithData = line.Data.Where(d => d != null).DefaultIfEmpty(null).Last(),
						Centile05 = CalculatePercentile(line.Data, 5),
						Centile10 = CalculatePercentile(line.Data, 10),
						Centile25 = CalculatePercentile(line.Data, 25),
						Centile75 = CalculatePercentile(line.Data, 75),
						Centile90 = CalculatePercentile(line.Data, 90),
						Centile95 = CalculatePercentile(line.Data, 95),
						NormalCount = CountAtAlertLevel(
							line.Data,
							databaseDeviceDataSourceInstance.EffectiveAlertExpression,
							CountAlertLevel.Normal
						),
						WarningCount = CountAtAlertLevel(
							line.Data,
							databaseDeviceDataSourceInstance.EffectiveAlertExpression,
							CountAlertLevel.Warning
						),
						ErrorCount = CountAtAlertLevel(
							line.Data,
							databaseDeviceDataSourceInstance.EffectiveAlertExpression,
							CountAlertLevel.Error
						),
						CriticalCount = CountAtAlertLevel(
							line.Data,
							databaseDeviceDataSourceInstance.EffectiveAlertExpression,
							CountAlertLevel.Critical
						),
					};
					aggregationsToWrite.Add(bulkWriteModel);
				}

				databaseDeviceDataSourceInstance.DataCompleteToUtc = endDateTime.UtcDateTime;
				startDateTime = startDateTime.AddMonths(1);
				endDateTime = endDateTime.AddMonths(1);
			}

			await context
				.BulkInsertAsync(aggregationsToWrite, cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			aggregationsToWrite.Clear();

			await context
				.SaveChangesAsync(cancellationToken)
				.ConfigureAwait(false);
		}

		logger.LogInformation("Syncing data complete.");
	}

	/// <summary>
	/// TODO - implement this
	/// </summary>
	/// <param name="data"></param>
	/// <param name="effectiveAlertExpression"></param>
	/// <param name="normal"></param>
	/// <returns></returns>
	private int CountAtAlertLevel(double?[] data, string effectiveAlertExpression, CountAlertLevel normal)
		=> 0;
}
