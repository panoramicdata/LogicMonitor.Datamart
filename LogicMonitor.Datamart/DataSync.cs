using LogicMonitor.Datamart.Config;
using LogicMonitor.Datamart.Models;
using Microsoft.Data.SqlClient;
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
		private static readonly TimeSpan EightHours = TimeSpan.FromHours(8);

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
			Logger.LogInformation("Data sync started...");
			var configurationLevelAggregationDuration = TimeSpan.FromMinutes(_configuration.AggregationDurationMinutes);

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
				.ToListAsync()
				.ConfigureAwait(false);

			// Get the LogicMonitor Ids for those DataSources
			var dataSourceIds = matchingDatabaseDataSources
				.ConvertAll(ds => ds.Id);

			// Get the database instances for those DataSources, excluding ones where LastWentMissingUtc is set
			var databaseDeviceDataSourceInstances = await context
				.DeviceDataSourceInstances
				.Where(ddsi =>
					ddsi.LastWentMissingUtc == null
					&& ddsi.DataSourceId.HasValue
					&& dataSourceIds.Contains(ddsi.DataSourceId.Value)
				)
				// To make debugging a little more deterministic, order by the Device and then its instances
				.OrderBy(ddsi => ddsi.DeviceId).ThenBy(ddsi => ddsi.Id)
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
				await GetAndWriteAggregations(
					_datamartClient,
					_configuration,
					Logger,
					databaseDeviceDataSourceInstances,
					configurationLevelAggregationDuration,
					cancellationToken)
					.ConfigureAwait(false);
			}
			finally
			{
				_datamartClient.UseCache = oldCacheState;
			}
		}

		private async Task GetAndWriteAggregations(
			DatamartClient datamartClient,
			Configuration configuration,
			ILogger logger,
			List<DeviceDataSourceInstanceStoreItem> databaseDeviceDataSourceInstances,
			TimeSpan configurationLevelAggregationDuration,
			CancellationToken cancellationToken)
		{
			// To ignore a period of uncertainty whether the Collector has been
			// able to publish its measurement data to the LogicMonitor API,
			// we consider "now" to be X hours ago.
			// This is "B" in the diagram below.
			var utcNow = DateTimeOffset.UtcNow;
			var lateArrivingDataWindowStart = utcNow.AddHours(-configuration.LateArrivingDataWindowHours);

			var aggregationsToWrite = new List<DeviceDataSourceInstanceAggregatedDataBulkWriteModel>();

			var stopwatch = new Stopwatch();

			// Determine the aggregation duration at the datasource level
			var dataSourceAggregationDuration = configurationLevelAggregationDuration;

			// Get data for each instance
			logger.LogInformation($"Syncing {databaseDeviceDataSourceInstances.Count} DeviceDataSourceInstances...");
			foreach (var databaseDeviceDataSourceInstanceGroup in
					databaseDeviceDataSourceInstances
					.GroupBy(ddsi => ddsi.LastAggregationHourWrittenUtc ?? DateTime.MinValue)
				)
			{
				var lastAggregationHourWrittenUtc = new DateTimeOffset(databaseDeviceDataSourceInstanceGroup.Key, TimeSpan.Zero);

				// Handle that groups of LastAggregationHourWrittenUtc need to be batched to deal with the LogicMonitor restriction

				foreach (var databaseDeviceDataSourceInstanceGroupBatch in databaseDeviceDataSourceInstanceGroup
					.Select((item, itemIndex) => (item, itemIndex))
					.GroupBy(x => x.itemIndex / _configuration.DeviceDataSourceInstanceBatchSize))
				{
					var batchIndex = databaseDeviceDataSourceInstanceGroupBatch.Key;

					var instanceIdList = databaseDeviceDataSourceInstanceGroupBatch
						.Select(t => t.item.Id)
						.ToList();

					var rangeDescription = $"Batch {batchIndex + 1}: {instanceIdList.Count} instances starting {databaseDeviceDataSourceInstanceGroup.Key:yyyy-MM-dd HH:mm:ss}: {string.Join(",", instanceIdList)}...";
					logger.LogDebug(rangeDescription);

					try
					{
						stopwatch.Restart();
						var totalRowsLoadedFromApi = 0;

						// A: The last time we got measurement up to for this DeviceDataSourceInstance
						var lastUpdatedDateTimeUtc = lastAggregationHourWrittenUtc;

						// If we have never fetched data, determine the minimum data fetch date
						if (lastUpdatedDateTimeUtc < configuration.StartDateTimeUtc)
						{
							lastUpdatedDateTimeUtc = configuration.StartDateTimeUtc.UtcDateTime;
						}

						// Due to limitations on the DataFetch Logicmonitor.Api endpoint, we can only go back a max of 24 hours
						// If lastUpdatedDateTimeUtc < 23 hours ago then set to 23 hours ago
						const int MaxHoursBack = 23;
						if (lastUpdatedDateTimeUtc < DateTimeOffset.UtcNow.AddHours(-MaxHoursBack))
						{
							var originalLastUpdatedDateTimeUtc = lastUpdatedDateTimeUtc;
							var hoursAgo23 = DateTimeOffset.UtcNow.AddHours(-MaxHoursBack);
							lastUpdatedDateTimeUtc = DateTimeOffset.UtcNow.Date.AddHours(-MaxHoursBack);
							while (lastUpdatedDateTimeUtc < hoursAgo23)
							{
								// Increment by the aggregation duration until we're within the window
								lastUpdatedDateTimeUtc = lastUpdatedDateTimeUtc.Add(dataSourceAggregationDuration);
							}
							logger.LogDebug($"lastUpdatedDateTimeUtc {originalLastUpdatedDateTimeUtc} is more than {MaxHoursBack} hours ago so setting to {lastUpdatedDateTimeUtc}.");
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
								// If we've genuinely done nothing, then log it so terminating after this is shown to be intentional
								if (blockIndex == 0)
								{
									logger.LogDebug($"BlockIndex is 0, nothing to do for batch {batchIndex + 1}.");
								}
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
							//if (rowsRetrieved > 0)
							totalRowsLoadedFromApi += rowsRetrieved;

							// Create a new DbContext to clear out tracked objects
							using var dataContext = new Context(datamartClient.DbContextOptions);
							using var dbConnection = dataContext.Database.GetDbConnection();
							using var sqlConnection = new SqlConnection(dbConnection.ConnectionString);
							await sqlConnection.OpenAsync().ConfigureAwait(false);
							aggregationsToWrite.Clear();

							// Iterate over the retrieved DeviceDataSourceInstances
							foreach (var instanceFetchDataResponse in instancesFetchDataResponse.InstanceFetchDataResponses)
							{
								// Get the configuration for this DataSourceName
								var dataSourceConfigurationItem = configuration.DataSources.SingleOrDefault(dsci => dsci.Name == instanceFetchDataResponse.DataSourceName);

								var deviceDataSourceInstanceIdAsInt = int.Parse(instanceFetchDataResponse.DeviceDataSourceInstanceId);

								if (instanceFetchDataResponse.Timestamps.Length > 0)
								{
									// Process only the configured DataPoints to retrieve
									// Add data to the context for each of the dataPointNames
									foreach (var dataPointModel in dataSourceConfigurationItem.DataPoints)
									{
										// Get the index into the timestamps and values
										var dataPointIndex = Array.FindIndex(instanceFetchDataResponse.DataPoints, dpName => dpName == dataPointModel.Name);

										if (dataPointIndex == -1)
										{
											// We have a datapoint in our configuration that isn't being returned for this DataSource, therefore we cant write it out
											continue;
										}

										// Validate the result is good to zip up
										if (instanceFetchDataResponse.Timestamps.Length != instanceFetchDataResponse.DataValues.Length)
										{
											logger.LogError($"Expected count of {nameof(instanceFetchDataResponse.Timestamps)} ({instanceFetchDataResponse.Timestamps.Length}) and count of {nameof(instanceFetchDataResponse.DataValues)} ({instanceFetchDataResponse.DataValues.Length}) to match.");
											// We've logged, try the next DataPoint
											continue;
										}

										var data = instanceFetchDataResponse.Timestamps.Zip(
											instanceFetchDataResponse.DataValues.Select(v => v[dataPointIndex]),
											(timeStampMs, value)
												=> new
												{
													DateTime = DateTimeOffset.FromUnixTimeMilliseconds(timeStampMs).UtcDateTime,
													DataPointName = dataPointModel.Name,
													Value = (double?)(value is string ? null : value),
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
											.Select(chunkedData =>
											{
												var periodStart = (blockStart + TimeSpan.FromSeconds(chunkedData.Key * dataSourceAggregationDuration.TotalSeconds)).UtcDateTime;
												return new DeviceDataSourceInstanceAggregatedDataBulkWriteModel
												{
													DeviceDataSourceInstanceId = deviceDataSourceInstanceIdAsInt,
													DataPointId = databaseDataPoint.DatamartId,
													PeriodStart = periodStart,
													PeriodEnd = periodStart.Add(dataSourceAggregationDuration),
													DataCount = chunkedData.Count(d => d.Value != null),
													NoDataCount = chunkedData.Count(d => d.Value == null),
													Sum = chunkedData.Sum(d => d.Value ?? 0),
													SumSquared = chunkedData.Sum(d => d.Value == null ? 0 : d.Value.Value * d.Value.Value),
													Max = chunkedData.Max(d => d.Value),
													Min = chunkedData.Min(d => d.Value)
												};
											})
											.ToList();
										aggregationsToWrite.AddRange(deviceDataSourceInstanceAggregatedDataStoreItems);
										// Increment the blockIndex
										blockIndex++;
									}
								}

								// We always want to write something out about where we've attempted to get data until

								// TODO write out aggregationsToWrite using bulk write in a transaction with the progress on day boundaries

								if (aggregationsToWrite.Count > 0)
								{
									foreach (var blockToWrite in aggregationsToWrite.GroupBy(a => a.PeriodStart.Date))
									{
										await AggregationWriter.WriteAggregations(
											sqlConnection,
											configuration.SqlCommandTimeoutSeconds,
											configuration.SqlBulkCopyTimeoutSeconds,
											deviceDataSourceInstanceIdAsInt,
											blockToWrite.Key,
											blockToWrite,
											logger);
									}
								}
								else
								{
									await AggregationWriter.WriteProgressBoundaryAsync(
										sqlConnection,
										configuration.SqlCommandTimeoutSeconds,
										deviceDataSourceInstanceIdAsInt,
										blockEnd.UtcDateTime,
										null);
								}
							}
						}
					}
					catch (Exception e)
					{
						logger.LogWarning(e, $"{rangeDescription} failed due to {e}");
					}
				}
			}
			logger.LogInformation("Syncing data complete.");
		}
	}
}
