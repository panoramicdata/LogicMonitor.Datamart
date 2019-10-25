using LogicMonitor.Datamart.Config;
using LogicMonitor.Datamart.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LogicMonitor.Datamart
{
	internal class DataSync : LoopInterval
	{
		private static readonly TimeSpan EightHours = TimeSpan.FromHours(8);
		private static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);

		private readonly DatamartClient _datamartClient;
		private readonly Configuration _configuration;

		public DataSync(
			DatamartClient datamartClient,
			Configuration configuration,
			ILoggerFactory loggerFactory)
			: base(nameof(DataSync), loggerFactory)
		{
			_datamartClient = datamartClient;
			_configuration = configuration;
		}

		public override async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			var configurationLevelAggregationDuration = TimeSpan.FromMinutes(_configuration.AggregationDurationMinutes);

			using (var context = new Context(_datamartClient.DbContextOptions))
			{
				// Use the database as a reference for what should be loaded in to ensure referential integrity between the data and the DeviceDataSourceInstance

				// Get the configured DataSource names
				var deviceDataSourceNames = _configuration.DataSources
					.Select(dsci => dsci.Name)
					.ToList();

				// Get the database DataSources for those names
				var matchingDatabaseDataSources = await context
					.DataSources
					.Where(ds => deviceDataSourceNames.Contains(ds.Name))
					.ToListAsync()
					.ConfigureAwait(false);

				// Get the LogicMonitor Ids for those DataSources
				var dataSourceIds = matchingDatabaseDataSources
					.Select(ds => ds.Id)
					.ToList();

				// Get the database instances for those datasources
				var databaseDeviceDataSourceInstances = await context
					.DeviceDataSourceInstances
					.Where(ddsi =>
						ddsi.DataSourceId.HasValue
						&& dataSourceIds.Contains(ddsi.DataSourceId.Value)
					)
					.ToListAsync()
					.ConfigureAwait(false);

				// If there aren't any, log and return
				if (databaseDeviceDataSourceInstances.Count == 0)
				{
					Logger.LogWarning($"Found no DeviceDataSourceInstances in the databases for DeviceDataSource names {string.Join(", ", deviceDataSourceNames)}. Check dimensions have been synced.");
					return;
				}
				// We have the database deviceDataSourceInstances for the configured DataSources

				// Clear out the PortalClient Cache, otherwise we remember all the datavalues for no reason
				_datamartClient.ClearCache();
				var oldCacheState = _datamartClient.UseCache;
				_datamartClient.UseCache = false;

				// To ignore a period of uncertainty whether the Collector has been
				// able to publish its measurement data to the LogicMonitor API,
				// we consider "now" to be X hours ago.
				// This is "B" in the diagram below.
				var utcNow = DateTimeOffset.UtcNow;
				var lateArrivingDataWindowStart = utcNow.AddHours(-_configuration.LateArrivingDataWindowHours);
				var lateArrivingDataWindowStartSeconds = lateArrivingDataWindowStart.ToUnixTimeSeconds();

				var stopwatch = new Stopwatch();

				// Get data for each instance
				try
				{
					foreach (var databaseDeviceDataSourceInstance in databaseDeviceDataSourceInstances)
					{
						stopwatch.Restart();
						var totalRowsLoadedFromApi = 0;

						var dataSourceName = matchingDatabaseDataSources
							.Single(ds => ds.Id == databaseDeviceDataSourceInstance.DataSourceId)
							.Name;

						var dataSourceConfigurationItem = _configuration
							.DataSources
							.SingleOrDefault(dsci => dsci.Name == dataSourceName);

						// Determine the aggregation duration at the datasource level
						var dataSourceAggregationDuration = dataSourceConfigurationItem?.AggregationDurationMinutes != null
							? TimeSpan.FromMinutes(dataSourceConfigurationItem.AggregationDurationMinutes.Value)
							: configurationLevelAggregationDuration;

						// A: The last time we got measurement up to for this DeviceDataSourceInstance
						var lastUpdatedDateTimeUtc = DateTimeOffset
							.FromUnixTimeSeconds(databaseDeviceDataSourceInstance.LastMeasurementUpdatedTimeSeconds);

						// If we have never fetched data, determine the minimum data fetch date
						if (lastUpdatedDateTimeUtc < _configuration.StartDateTimeUtc)
						{
							lastUpdatedDateTimeUtc = _configuration.StartDateTimeUtc;
						}
						var timeCursor = lastUpdatedDateTimeUtc;
						// A is now calculated

						// .........---------|----:----:-|-----
						//                   A           B
						// A is where we have data in the database up to
						// B is where we want to fetch it up to
						// The maximum data that can be retrieved from LM at once with full resolution is 8 hours
						// We fetch the data in blocks of no more than 8 hours.

						// Determine the last datetimeoffset we want to get data up to
						var blockIndex = 0;
						var instanceLogId = $"{databaseDeviceDataSourceInstance.DeviceDisplayName}:{databaseDeviceDataSourceInstance.Id}";
						while (timeCursor < lateArrivingDataWindowStart)
						{
							var blockStart = timeCursor;
							while (timeCursor - blockStart < EightHours)
							{
								if (timeCursor + dataSourceAggregationDuration >= lateArrivingDataWindowStart)
								{
									break;
								}
								timeCursor += dataSourceAggregationDuration;
							}
							// We have a block of time to fetch, starting at blockStart and ending at timeCursor.

							// Is the block zero length?
							if (timeCursor == blockStart)
							{
								break;
							}

							var blockEnd = timeCursor;

							// Fetch the data and loop
							var rawData = await _datamartClient.GetRawDataSetAsync(
								databaseDeviceDataSourceInstance.DeviceId.Value,
								databaseDeviceDataSourceInstance.DeviceDataSourceId,
								databaseDeviceDataSourceInstance.Id,
								blockStart.UtcDateTime + OneSecond,
								blockEnd.UtcDateTime,
								cancellationToken
								).ConfigureAwait(false);

							var rowsRetrieved = rawData.UtcTimeStamps?.Count ?? 0;
							Logger.LogDebug($"Loaded {rowsRetrieved} entries for DeviceDataSourceInstance {instanceLogId}.");
							if (rowsRetrieved > 0)
							{
								totalRowsLoadedFromApi += rowsRetrieved;

								using (var dataContext = new Context(_datamartClient.DbContextOptions))
								{
									// Add data to the context for each of the dataPointNames
									foreach (var dataPointModel in dataSourceConfigurationItem.DataPoints)
									{
										var dataPointIndex = rawData.DataPoints.FindIndex(dpName => dpName == dataPointModel.Name);
										var data = rawData.UtcTimeStamps.Zip(
											rawData.Values.Select(v => v[dataPointIndex]),
											(timeStampMs, value)
												=> new DeviceDataSourceInstanceDataStoreItem
												{
													DateTime = DateTimeOffset.FromUnixTimeMilliseconds(timeStampMs).UtcDateTime,
													DataPointName = dataPointModel.Name,
													Value = value,
													DeviceDataSourceInstanceId = databaseDeviceDataSourceInstance.Id
												})
												.ToList();
										// Data:                   :-------------------------------:
										// Data fetched is a block :---.---.---.---.---.---.---.---:
										//... where the maximum size is 8 hours, with an integer number data aggregation chunks
										// We need to aggregate this in blocks of aggregationDuration

										var databaseDataPoint = await dataContext
											.DataSourceDataPoints
											.SingleOrDefaultAsync(dp => dp.Name == dataPointModel.Name && dp.DataSource.Id == databaseDeviceDataSourceInstance.DataSourceId)
											.ConfigureAwait(false);

										// Aggregate it in blocks of DataAggregationDuration
										var aggregationTimeCursor = blockStart;
										aggregationTimeCursor += dataSourceAggregationDuration;
										var deviceDataSourceInstanceAggregatedDataStoreItems = data
											.GroupBy(d => ((int)(d.DateTime - blockStart).TotalSeconds) / ((int)dataSourceAggregationDuration.TotalSeconds))
											.Select(chunkedData => new DeviceDataSourceInstanceAggregatedDataStoreItem
											{
												DeviceDataSourceInstanceId = databaseDeviceDataSourceInstance.Id,
												DataPointId = databaseDataPoint.DatamartId,
												Hour = (blockStart + TimeSpan.FromSeconds(chunkedData.Key * dataSourceAggregationDuration.TotalSeconds)).UtcDateTime,
												DataCount = chunkedData.Count(d => d.Value != null),
												NoDataCount = chunkedData.Count(d => d.Value == null),
												Sum = chunkedData.Sum(d => d.Value ?? 0),
												SumSquared = chunkedData.Sum(d => Math.Pow(d.Value ?? 0, 2)),
												Max = chunkedData.Max(d => d.Value),
												Min = chunkedData.Min(d => d.Value)
											})
											.ToList();
										dataContext.DeviceDataSourceInstanceAggregatedData.AddRange(deviceDataSourceInstanceAggregatedDataStoreItems);
										// Increment the blockIndex
										blockIndex++;
									}
									databaseDeviceDataSourceInstance.LastAggregationHourWrittenUtc = databaseDeviceDataSourceInstance.LastAggregationHourWrittenUtc = blockEnd.UtcDateTime;
									await dataContext
										.SaveChangesAsync()
										.ConfigureAwait(false);
								}
							}

							// Move onto the next block
							timeCursor += EightHours;
						}
					}
				}
				finally
				{
					_datamartClient.UseCache = oldCacheState;
				}
			}
		}

		//		private async Task PerformAggregationsAsync(int deviceDataSourceInstanceId)
		//		{
		//			using (var context = new Context(_datamartClient.DbContextOptions))
		//			{
		//				// Get deviceDataSourceInstance row
		//				var ddsi = context.DeviceDataSourceInstances.SingleOrDefault(i => i.Id == deviceDataSourceInstanceId);

		//				// StartHour = Find out which aggregated hour we last wrote and add an hour, or start at the configure beginning
		//				var startHour = ddsi.LastAggregationHourWrittenUtc.HasValue
		//					? new DateTimeOffset(ddsi.LastAggregationHourWrittenUtc.Value, TimeSpan.Zero).AddHours(1)
		//					: _startDateTimeUtc.UtcDateTime;

		//				// EndHour = The one prior to the last hour we wrote source data
		//				var endHour = DateTimeOffset.FromUnixTimeSeconds(ddsi.LastMeasurementUpdatedTimeSeconds);
		//				// Move to the start of the end hour
		//				endHour = new DateTimeOffset(endHour.Year, endHour.Month, endHour.Day, endHour.Hour, 0, 0, TimeSpan.Zero);

		//				try
		//				{
		//					// Insert rows from StartDate to EndHour
		//					var rowsAffected = await context.Database.ExecuteSqlRawAsync($@"
		//begin transaction;

		//insert into DeviceDataSourceInstanceAggregatedData
		//(Hour, DeviceDataSourceInstanceId, DataPointName, Min, Max, Sum, SumSquared, DataCount, NoDataCount)
		//select
		//dateadd(hour, datediff(hour, 0, DateTime), 0) as [Hour],
		//DeviceDataSourceInstanceId,
		//DataPointName,
		//min(Value) as [Min],
		//max(Value) as [Max],
		//sum(Value) as [Sum],
		//sum(POWER(Value, 2)) as SumSquared,
		//sum(case when Value is not null then 1 else 0 end) [DataCount],
		//sum(case when Value is null then 1 else 0 end) [NoDataCount]
		//from DeviceDataSourceInstanceData
		//where [DateTime] >= {startHour.UtcDateTime} and [DateTime] < {endHour.UtcDateTime} and DeviceDataSourceInstanceId = {deviceDataSourceInstanceId}
		//group by
		//DeviceDataSourceInstanceId,
		//DataPointName,
		//dateadd(hour, datediff(hour, 0, DateTime), 0);

		//update DeviceDataSourceInstances set LastAggregationHourWrittenUtc =
		//(
		//	select max(Hour) from DeviceDataSourceInstanceAggregatedData
		//	where DeviceDataSourceInstanceId = {deviceDataSourceInstanceId}
		//)
		//where Id = {deviceDataSourceInstanceId};

		//commit;
		//")
		//						.ConfigureAwait(false);
		//				}
		//#pragma warning disable CA1031 // Do not catch general exception types
		//				catch (Exception ex)
		//				{
		//					Logger.LogError(ex, $"A problem occurred while writing aggregated data for DeviceDataSourceInstance#{deviceDataSourceInstanceId} ex.Message");
		//				}
		//#pragma warning restore CA1031 // Do not catch general exception types
		//			}
		//		}
	}
}
