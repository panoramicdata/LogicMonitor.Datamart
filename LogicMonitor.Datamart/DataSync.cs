using LogicMonitor.Datamart.Config;
using LogicMonitor.Datamart.Extensions;
using LogicMonitor.Datamart.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LogicMonitor.Datamart
{
	internal class DataSync : LoopInterval
	{
		private const int BatchSize = 2;

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
			Logger.LogInformation($"Data sync started");
			var configurationLevelAggregationDuration = TimeSpan.FromMinutes(_configuration.AggregationDurationMinutes);

			using (var context = new Context(_datamartClient.DbContextOptions))
			{
				// Use the database as a reference for what should be loaded in to ensure referential integrity between the data and the DeviceDataSourceInstance

				// Get the configured DataSource names
				Logger.LogInformation($"Getting reference data");
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

				try
				{
					if (false)
					{
						await OldMethod(
							_datamartClient,
							_configuration,
							Logger,
							databaseDeviceDataSourceInstances,
							matchingDatabaseDataSources,
							configurationLevelAggregationDuration,
							cancellationToken)
							.ConfigureAwait(false);
					}
					else
					{
						await NewMethod(
							_datamartClient,
							_configuration,
							Logger,
							databaseDeviceDataSourceInstances,
							matchingDatabaseDataSources,
							configurationLevelAggregationDuration,
							cancellationToken)
							.ConfigureAwait(false);
					}
				}
				finally
				{
					_datamartClient.UseCache = oldCacheState;
				}
			}
		}

		private async Task NewMethod(
			DatamartClient datamartClient,
			Configuration configuration,
			ILogger logger,
			List<DeviceDataSourceInstanceStoreItem> databaseDeviceDataSourceInstances,
			List<DataSourceStoreItem> matchingDatabaseDataSources,
			TimeSpan configurationLevelAggregationDuration,
			CancellationToken cancellationToken)
		{
			// To ignore a period of uncertainty whether the Collector has been
			// able to publish its measurement data to the LogicMonitor API,
			// we consider "now" to be X hours ago.
			// This is "B" in the diagram below.
			var utcNow = DateTimeOffset.UtcNow;
			var lateArrivingDataWindowStart = utcNow.AddHours(-configuration.LateArrivingDataWindowHours);

			var stopwatch = new Stopwatch();

			// Determine the aggregation duration at the datasource level
			var dataSourceAggregationDuration = configurationLevelAggregationDuration;

			// Get data for each instance
			logger.LogInformation("Syncing each DeviceDataSourceInstance...");
			foreach (var databaseDeviceDataSourceInstanceGroup in databaseDeviceDataSourceInstances.GroupBy(ddsi => ddsi.LastMeasurementUpdatedTimeSeconds).Batch(BatchSize))
			{
				var instanceIdList = databaseDeviceDataSourceInstanceGroup
					.Select(g => g.Id)
					.ToList();

				var rangeDescription = $"Syncing data for {databaseDeviceDataSourceInstanceGroup.Count()} instances for time range starting {databaseDeviceDataSourceInstanceGroup.Key:yyyy-MM-dd HH:mm:ss}: {string.Join(",", instanceIdList)}...";
				logger.LogInformation(rangeDescription);

				try
				{
					stopwatch.Restart();
					var totalRowsLoadedFromApi = 0;

					//var dataSourceName = matchingDatabaseDataSources
					//	.Single(ds => ds.Id == databaseDeviceDataSourceInstance.DataSourceId)
					//	.Name;

					//var dataSourceConfigurationItem = configuration
					//	.DataSources
					//	.SingleOrDefault(dsci => dsci.Name == dataSourceName);

					// A: The last time we got measurement up to for this DeviceDataSourceInstance
					var lastUpdatedDateTimeUtc = DateTimeOffset
						.FromUnixTimeSeconds(databaseDeviceDataSourceInstanceGroup.Key);

					// If we have never fetched data, determine the minimum data fetch date
					if (lastUpdatedDateTimeUtc < configuration.StartDateTimeUtc)
					{
						lastUpdatedDateTimeUtc = configuration.StartDateTimeUtc;
					}
					var timeCursor = lastUpdatedDateTimeUtc;
					// A is now calculated

					// .........---------|----:----:-|-----NOW
					//                   A           B
					// A is where we have data in the database up to
					// B is where we want to fetch it up to
					// The maximum data that can be retrieved from LM at once with full resolution is 8 hours
					// We fetch the data in blocks of no more than 8 hours.

					// Determine the last datetimeoffset we want to get data up to
					var blockIndex = 0;
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
						var instancesFetchDataResponse = await datamartClient.GetFetchDataResponseAsync(instanceIdList,
							blockStart,
							blockEnd,
							cancellationToken
							).ConfigureAwait(false);

						var rowsRetrieved = instancesFetchDataResponse.InstanceFetchDataResponses.Sum(r => r.Timestamps.Length);
						logger.LogDebug($"Loaded {rowsRetrieved} entries.");
						if (rowsRetrieved > 0)
						{
							totalRowsLoadedFromApi += rowsRetrieved;

							// Create a new DbContext to clear out tracked objects
							using (var dataContext = new Context(datamartClient.DbContextOptions))
							{
								// Iterate over the retrieved DeviceDataSourceInstances
								foreach (var instanceFetchDataResponse in instancesFetchDataResponse.InstanceFetchDataResponses)
								{
									// Get the configuration for this DataSourceName
									var dataSourceConfigurationItem = configuration.DataSources.SingleOrDefault(dsci => dsci.Name == instanceFetchDataResponse.DataSourceName);

									var deviceDataSourceInstanceIdAsInt = int.Parse(instanceFetchDataResponse.DeviceDataSourceInstanceId);

									// Process only the configured DataPoints to retrieve
									// Add data to the context for each of the dataPointNames
									foreach (var dataPointModel in dataSourceConfigurationItem.DataPoints)
									{
										// Get the index into the timestamps and values
										var dataPointIndex = Array.FindIndex(instanceFetchDataResponse.DataPoints, dpName => dpName == dataPointModel.Name);

										var data = instanceFetchDataResponse.Timestamps.Zip(
											instanceFetchDataResponse.DataValues.Select(v => v[dataPointIndex]),
											(timeStampMs, value)
												=> new DeviceDataSourceInstanceDataStoreItem
												{
													DateTime = DateTimeOffset.FromUnixTimeMilliseconds(timeStampMs).UtcDateTime,
													DataPointName = dataPointModel.Name,
													Value = (double?)value, // TODO - Is this right?
													DeviceDataSourceInstanceId = deviceDataSourceInstanceIdAsInt
												})
												.ToList();
										// Data:                   :-------------------------------:
										// Data fetched is a block :---.---.---.---.---.---.---.---:
										//... where the maximum size is 8 hours, with an integer number data aggregation chunks
										// We need to aggregate this in blocks of aggregationDuration

										var databaseDataPoint = await dataContext
											.DataSourceDataPoints
											.SingleOrDefaultAsync(dp => dp.Name == dataPointModel.Name && dp.DataSource.Name == instanceFetchDataResponse.DataSourceName)
											.ConfigureAwait(false);

										// Aggregate it in blocks of DataAggregationDuration
										var aggregationTimeCursor = blockStart;
										aggregationTimeCursor += dataSourceAggregationDuration;
										var deviceDataSourceInstanceAggregatedDataStoreItems = data
											.GroupBy(d => ((int)(d.DateTime - blockStart).TotalSeconds) / ((int)dataSourceAggregationDuration.TotalSeconds))
											.Select(chunkedData => new DeviceDataSourceInstanceAggregatedDataStoreItem
											{
												DeviceDataSourceInstanceId = deviceDataSourceInstanceIdAsInt,
												DataPointId = databaseDataPoint.DatamartId,
												Hour = (blockStart + TimeSpan.FromSeconds(chunkedData.Key * dataSourceAggregationDuration.TotalSeconds)).UtcDateTime,
												DataCount = chunkedData.Count(d => d.Value != null),
												NoDataCount = chunkedData.Count(d => d.Value == null),
												Sum = chunkedData.Sum(d => d.Value ?? 0),
												SumSquared = chunkedData.Sum(d => d.Value == null ? 0 : d.Value * d.Value),
												Max = chunkedData.Max(d => d.Value),
												Min = chunkedData.Min(d => d.Value)
											})
											.ToList();
										dataContext.DeviceDataSourceInstanceAggregatedData.AddRange(deviceDataSourceInstanceAggregatedDataStoreItems);
										// Increment the blockIndex
										blockIndex++;
									}

									var databaseDeviceDataSourceInstance = await dataContext.DeviceDataSourceInstances.SingleAsync(ddi => ddi.Id == deviceDataSourceInstanceIdAsInt);

									databaseDeviceDataSourceInstance.LastAggregationHourWrittenUtc = databaseDeviceDataSourceInstance.LastAggregationHourWrittenUtc = blockEnd.UtcDateTime;
									await dataContext
										.SaveChangesAsync()
										.ConfigureAwait(false);
								}
							}
						}

						// Move onto the next block
						timeCursor += EightHours;
					}
				}
				catch (Exception e)
				{
					logger.LogWarning(e, $"{rangeDescription} failed due to {e.Message}");
				}
			}
			logger.LogInformation($"Syncing data complete.");
		}

		private static async Task OldMethod(
			DatamartClient datamartClient,
			Configuration configuration,
			ILogger logger,
			List<DeviceDataSourceInstanceStoreItem> databaseDeviceDataSourceInstances,
			List<DataSourceStoreItem> matchingDatabaseDataSources,
			TimeSpan configurationLevelAggregationDuration,
			CancellationToken cancellationToken)
		{
			// To ignore a period of uncertainty whether the Collector has been
			// able to publish its measurement data to the LogicMonitor API,
			// we consider "now" to be X hours ago.
			// This is "B" in the diagram below.
			var utcNow = DateTimeOffset.UtcNow;
			var lateArrivingDataWindowStart = utcNow.AddHours(-configuration.LateArrivingDataWindowHours);

			var stopwatch = new Stopwatch();

			// Get data for each instance
			logger.LogInformation("Syncing each DeviceDataSourceInstance...");
			foreach (var databaseDeviceDataSourceInstance in databaseDeviceDataSourceInstances)
			{
				try
				{
					logger.LogInformation($"Syncing data for device: {databaseDeviceDataSourceInstance.DeviceId}, dataSource {databaseDeviceDataSourceInstance.DataSourceId}, instance {databaseDeviceDataSourceInstance.DisplayName}");
					stopwatch.Restart();
					var totalRowsLoadedFromApi = 0;

					var dataSourceName = matchingDatabaseDataSources
						.Single(ds => ds.Id == databaseDeviceDataSourceInstance.DataSourceId)
						.Name;

					var dataSourceConfigurationItem = configuration
						.DataSources
						.SingleOrDefault(dsci => dsci.Name == dataSourceName);

					// Determine the aggregation duration at the datasource level
					var dataSourceAggregationDuration = configurationLevelAggregationDuration;

					// A: The last time we got measurement up to for this DeviceDataSourceInstance
					var lastUpdatedDateTimeUtc = DateTimeOffset
						.FromUnixTimeSeconds(databaseDeviceDataSourceInstance.LastMeasurementUpdatedTimeSeconds);

					// If we have never fetched data, determine the minimum data fetch date
					if (lastUpdatedDateTimeUtc < configuration.StartDateTimeUtc)
					{
						lastUpdatedDateTimeUtc = configuration.StartDateTimeUtc;
					}
					var timeCursor = lastUpdatedDateTimeUtc;
					// A is now calculated

					// .........---------|----:----:-|-----NOW
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
						var rawData = await datamartClient.GetRawDataSetAsync(
							databaseDeviceDataSourceInstance.DeviceId.Value,
							databaseDeviceDataSourceInstance.DeviceDataSourceId,
							databaseDeviceDataSourceInstance.Id,
							blockStart.UtcDateTime + OneSecond,
							blockEnd.UtcDateTime,
							cancellationToken
							).ConfigureAwait(false);

						var rowsRetrieved = rawData.UtcTimeStamps?.Count ?? 0;
						logger.LogDebug($"Loaded {rowsRetrieved} entries for DeviceDataSourceInstance {instanceLogId}.");
						if (rowsRetrieved > 0)
						{
							totalRowsLoadedFromApi += rowsRetrieved;

							using (var dataContext = new Context(datamartClient.DbContextOptions))
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
											SumSquared = chunkedData.Sum(d => d.Value == null ? 0 : d.Value * d.Value),
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
				catch (Exception e)
				{
					logger.LogWarning(e, $"Syncing data for device: {databaseDeviceDataSourceInstance.DeviceId}, dataSource {databaseDeviceDataSourceInstance.DataSourceId}, instance {databaseDeviceDataSourceInstance.DisplayName} failed due to {e.Message}");
				}
			}
			logger.LogInformation($"Syncing data complete.");
		}
	}
}
