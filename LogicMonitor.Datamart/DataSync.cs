using Humanizer;
using Humanizer.Localisation;
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
		private readonly DatamartClient _datamartClient;
		private readonly Dictionary<string, List<string>> _dataSourceSpecifications;
		private readonly int _lateArrivingDataWindowHours;
		private readonly DateTimeOffset _startDateTimeUtc;

		public DataSync(
			DatamartClient datamartClient,
			Dictionary<string, List<string>> dataSourceSpecifications,
			DateTimeOffset startDateTimeUtc,
			int lateArrivingDataWindowHours,
			ILogger<DataSync> logger)
			: base(nameof(DataSync), logger)
		{
			_datamartClient = datamartClient;
			_dataSourceSpecifications = dataSourceSpecifications;
			_lateArrivingDataWindowHours = lateArrivingDataWindowHours;
			_startDateTimeUtc = startDateTimeUtc;
		}

		public override async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			using (var context = new Context(_datamartClient.DbContextOptions))
			{
				// Use the database as a reference for what should be loaded in to ensure referential integrity between the data and the DeviceDataSourceInstance

				var deviceDataSourceNames = _dataSourceSpecifications.Keys.ToList();
				var matchingDatabaseDataSources = await context.DataSources.Where(ds => deviceDataSourceNames.Contains(ds.Name)).ToListAsync().ConfigureAwait(false);
				var dataSourceIds = matchingDatabaseDataSources.Select(ds => ds.Id).ToList();

				var deviceDataSourceInstances = await context.DeviceDataSourceInstances
					.Where(ddsi =>
						ddsi.DataSourceId.HasValue
						&& dataSourceIds.Contains(ddsi.DataSourceId.Value)
					)
					.ToListAsync()
					.ConfigureAwait(false);

				//var endDateTimeUtc = DateTime.UtcNow;

				if (deviceDataSourceInstances.Count == 0)
				{
					Logger.LogWarning($"Found no DeviceDataSourceInstances in the databases for DeviceDataSource names {string.Join(", ", deviceDataSourceNames)}. Check dimensions have been synced.");
					return;
				}

				// Clear out the PortalClient Cache, otherwise we remember all the datavalues for no reason
				_datamartClient.ClearCache();
				var oldCacheState = _datamartClient.UseCache;
				_datamartClient.UseCache = false;

				// This is the most recent point in time we should retrieve data to
				var utcNow = DateTimeOffset.UtcNow;

				// Configure the LateArrivingDataWindow
				var lateArrivingDataWindowStart = utcNow.AddHours(-_lateArrivingDataWindowHours);
				var lateArrivingDataWindowStartSeconds = lateArrivingDataWindowStart.ToUnixTimeSeconds();

				var stopwatch = new Stopwatch();

				var deviceDataSourceInstanceNumber = 1;

				// Get data for each instance
				try
				{
					foreach (var deviceDataSourceInstance in deviceDataSourceInstances)
					{
						stopwatch.Restart();
						var totalRowsLoadedFromApi = 0;

						var dataSourceName = matchingDatabaseDataSources
							.Single(ds => ds.Id == deviceDataSourceInstance.DataSourceId)
							.Name;

						// Load the last time we got measurement for this DeviceDataSourceInstance and add a second to start after the last retrieved value
						var lastUpdatedDateTimeUtc = DateTimeOffset.FromUnixTimeSeconds(deviceDataSourceInstance.LastMeasurementUpdatedTimeSeconds + 1).UtcDateTime;

						// Loop until 0 rows retrieved
						var rowsRetrieved = 0;

						var dataWindowStartUtc = lastUpdatedDateTimeUtc > _startDateTimeUtc ? lastUpdatedDateTimeUtc : _startDateTimeUtc;
						// Save the date for logging
						var initialDataWindowStartUtc = dataWindowStartUtc;
						DateTimeOffset endDateTimeUtc;

						var instanceLogId = $"{deviceDataSourceInstance.DeviceDisplayName}:{deviceDataSourceInstance.Id}";
						Logger.LogDebug($"Loading data for DeviceDataSourceInstance {instanceLogId} from {initialDataWindowStartUtc.UtcDateTime} to {utcNow.UtcDateTime} - data window: {(utcNow - initialDataWindowStartUtc).Humanize(maxUnit: TimeUnit.Day, minUnit: TimeUnit.Minute, precision: 7)}");

						do
						{
							// Use a new context each time so we don't accumulate too much EF tracking data
							using (var dataContext = new Context(_datamartClient.DbContextOptions))
							{
								const int maxWindowDurationHours = 8;

								var maxWindowEndUtc = dataWindowStartUtc.AddHours(maxWindowDurationHours);
								// Get the least of the max window and now
								endDateTimeUtc = maxWindowEndUtc > utcNow ? utcNow : maxWindowEndUtc;

								Logger.LogTrace($"Loading data from {dataWindowStartUtc.UtcDateTime} to {endDateTimeUtc.UtcDateTime} - duration {endDateTimeUtc - dataWindowStartUtc}");

								var rawData = await _datamartClient.GetRawDataSetAsync(
									deviceDataSourceInstance.DeviceId.Value,
									deviceDataSourceInstance.DeviceDataSourceId,
									deviceDataSourceInstance.Id,
									dataWindowStartUtc.UtcDateTime,
									endDateTimeUtc.UtcDateTime,
									cancellationToken
									).ConfigureAwait(false);

								rowsRetrieved = rawData.UtcTimeStamps?.Count ?? 0;
								Logger.LogDebug($"Loaded {rowsRetrieved} entries for DeviceDataSourceInstance {instanceLogId}.");
								if (rowsRetrieved > 0)
								{
									totalRowsLoadedFromApi += rowsRetrieved;

									// Add data to the context for each of the dataPointNames
									foreach (var dataPointName in _dataSourceSpecifications[dataSourceName])
									{
										var dataPointIndex = rawData.DataPoints.FindIndex(dpName => dpName == dataPointName);
										var data = rawData.UtcTimeStamps.Zip(
											rawData.Values.Select(v => v[dataPointIndex]),
											(timeStampMs, value)
												=> new DeviceDataSourceInstanceDataStoreItem
												{
													DateTime = DateTimeOffset.FromUnixTimeMilliseconds(timeStampMs).UtcDateTime,
													DataPointName = dataPointName,
													Value = value,
													DeviceDataSourceInstanceId = deviceDataSourceInstance.Id
												})
												.ToList();

										dataContext.DeviceDataSourceInstanceData.AddRange(data);
									}

									// Save the data rows
									// TODO - This should be a bulk insert to avoid the tracking and checking for updating timestamps
									await dataContext.SaveChangesAsync().ConfigureAwait(false);

									// Update lastUpdatedDateTimeUtc and save it for this DeviceDataSourceInstance
									deviceDataSourceInstance.LastMeasurementUpdatedTimeSeconds = rawData.UtcTimeStamps.Max() / 1000;
									await context.SaveChangesAsync().ConfigureAwait(false);
								}
								else
								{
									// No data
								}

								// Move to next time window
								dataWindowStartUtc = dataWindowStartUtc.AddHours(maxWindowDurationHours).AddSeconds(1);
							}
						} while (endDateTimeUtc < utcNow);

						// We've reached the end of the data so take the most recent of 7 days ago and the last measurement. This way we won't keep looking for very old data for instances we haven't seen anything
						if (deviceDataSourceInstance.LastMeasurementUpdatedTimeSeconds < lateArrivingDataWindowStartSeconds)
						{
							Logger.LogDebug($"Didn't see anything newer than the configured LateArrivingDataWindow ({_lateArrivingDataWindowHours} hours); updating to {lateArrivingDataWindowStart.UtcDateTime} ({lateArrivingDataWindowStartSeconds})");
							deviceDataSourceInstance.LastMeasurementUpdatedTimeSeconds = lateArrivingDataWindowStartSeconds;
							await context.SaveChangesAsync().ConfigureAwait(false);
						}
						Logger.LogInformation($"Loaded {totalRowsLoadedFromApi} data entries in {stopwatch.Elapsed.TotalSeconds:N1}s for DeviceDataSourceInstance {instanceLogId} ({deviceDataSourceInstanceNumber++}/{deviceDataSourceInstances.Count}) from {initialDataWindowStartUtc.UtcDateTime} to {utcNow.UtcDateTime} - data window: {(utcNow - initialDataWindowStartUtc).Humanize(maxUnit: TimeUnit.Day, minUnit: TimeUnit.Minute, precision: 7)}");

						await PerformAggregationsAsync(deviceDataSourceInstance.Id).ConfigureAwait(false);
					}
				}
				finally
				{
					_datamartClient.UseCache = oldCacheState;
				}
			}
		}

		private async Task PerformAggregationsAsync(int deviceDataSourceInstanceId)
		{
			using (var context = new Context(_datamartClient.DbContextOptions))
			{
				// Get deviceDataSourceInstance row
				var ddsi = context.DeviceDataSourceInstances.SingleOrDefault(i => i.Id == deviceDataSourceInstanceId);

				// StartHour = Find out which aggregated hour we last wrote and add an hour, or start at the configure beginning
				var startHour = ddsi.LastAggregationHourWrittenUtc.HasValue
					? new DateTimeOffset(ddsi.LastAggregationHourWrittenUtc.Value, TimeSpan.Zero).AddHours(1)
					: _startDateTimeUtc.UtcDateTime;

				// EndHour = The one prior to the last hour we wrote source data
				var endHour = DateTimeOffset.FromUnixTimeSeconds(ddsi.LastMeasurementUpdatedTimeSeconds);
				// Move to the start of the end hour
				endHour = new DateTimeOffset(endHour.Year, endHour.Month, endHour.Day, endHour.Hour, 0, 0, TimeSpan.Zero);

				try
				{
					// Insert rows from StartDate to EndHour
					var rowsAffected = await context.Database.ExecuteSqlCommandAsync($@"
begin transaction;

insert into DeviceDataSourceInstanceAggregatedData
(Hour, DeviceDataSourceInstanceId, DataPointName, Min, Max, Sum, SumSquared, DataCount, NoDataCount)
select
dateadd(hour, datediff(hour, 0, DateTime), 0) as [Hour],
DeviceDataSourceInstanceId,
DataPointName,
min(Value) as [Min],
max(Value) as [Max],
sum(Value) as [Sum],
sum(POWER(Value, 2)) as SumSquared,
sum(case when Value is not null then 1 else 0 end) [DataCount],
sum(case when Value is null then 1 else 0 end) [NoDataCount]
from DeviceDataSourceInstanceData
where [DateTime] >= {startHour.UtcDateTime} and [DateTime] < {endHour.UtcDateTime} and DeviceDataSourceInstanceId = {deviceDataSourceInstanceId}
group by
DeviceDataSourceInstanceId,
DataPointName,
dateadd(hour, datediff(hour, 0, DateTime), 0);

update DeviceDataSourceInstances set LastAggregationHourWrittenUtc =
(
	select max(Hour) from DeviceDataSourceInstanceAggregatedData
	where DeviceDataSourceInstanceId = {deviceDataSourceInstanceId}
)
where Id = {deviceDataSourceInstanceId};

commit;
")
						.ConfigureAwait(false);
				}
#pragma warning disable CA1031 // Do not catch general exception types
				catch (Exception ex)
				{
					Logger.LogError(ex, $"A problem occurred while writing aggregated data for DeviceDataSourceInstance#{deviceDataSourceInstanceId} ex.Message");
				}
#pragma warning restore CA1031 // Do not catch general exception types
			}
		}
	}
}
