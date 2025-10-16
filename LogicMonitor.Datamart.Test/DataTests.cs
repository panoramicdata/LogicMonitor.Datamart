using LogicMonitor.Api.ScheduledDownTimes;
using LogicMonitor.Datamart.Services;

namespace LogicMonitor.Datamart.Test;

public class DataTests(ITestOutputHelper iTestOutputHelper) : TestWithOutput(iTestOutputHelper)
{
	[Fact]
	public async Task GetData_Succeeds()
	{
		var device = new ResourceStoreItem
		{
			CreatedOnSeconds = 0
		};

		var deviceDataSourceInstanceDataPoint = new ResourceDataSourceInstanceDataPointStoreItem
		{
			DeviceDataSourceInstance = new()
			{
				LogicMonitorId = 78121413
			},
			DataSourceDataPoint = new()
			{
			}
		};
		var utcNow = DateTimeOffset.UtcNow;
		var endDateTimeUtc = new DateTimeOffset(
			utcNow.Year,
			utcNow.Month,
			1,
			0,
			0,
			0,
			TimeSpan.Zero);
		var startDateTimeUtc = endDateTimeUtc.AddMonths(-1);

		var dataPointName = "Uptime";

		var dataPointStoreItem = new DataSourceDataPointStoreItem
		{
			GlobalAlertExpression = string.Empty,
			PercentageAvailabilityCalculation = "PercentUpTime",
			MeasurementUnit = "Seconds",
		};

		var graphData = await LowResolutionDataSync.GetGraphDataAsync(
			DatamartClient,
			deviceDataSourceInstanceDataPoint.DeviceDataSourceInstance.LogicMonitorId,
			startDateTimeUtc,
			endDateTimeUtc,
			LoggerFactory.CreateLogger<DataTests>(),
			default);

		// MS-21395: Create empty SDT cache for test
		var sdtCache = new Dictionary<string, List<ScheduledDownTimeHistory>>();

		var result = await LowResolutionDataSync.GetTimeSeriesDataAggregationStoreItem(
			DatamartClient,
			device,
			deviceDataSourceInstanceDataPoint,
			dataPointName,
			dataPointStoreItem,
			startDateTimeUtc,
			endDateTimeUtc,
			graphData,
			Configuration.ExcludeSdtPeriods,
			sdtCache,
			LoggerFactory.CreateLogger<DataTests>(),
			default
		).ConfigureAwait(false);

		result.Should().NotBeNull();
		result!.PeriodStart.Should().Be(startDateTimeUtc);
		result.AvailabilityPercent.Should().BeApproximately(70, 5);
	}

	[Fact]
	public Task LowResolutionDataSync_RunsSuccessfully()
		=> new LowResolutionDataSync(
				DatamartClient,
				Configuration,
				LoggerFactory,
				TestNotificationReceiver,
				new TimeProviderService())
			.ExecuteAsync(default);


	[Fact]
	public async Task LowResolutionDataSync_ResettingAggregations_RunsSuccessfully()
	{
		var tps = new TimeProviderService();
		tps.SetDateTimeNow(Configuration.FakeExecutionTime);

		Configuration.AggregationReset = true;
		await new LowResolutionDataSync(
				DatamartClient,
				Configuration,
				LoggerFactory,
				TestNotificationReceiver,
				tps)
			.ExecuteAsync(default)
			.ConfigureAwait(true);
	}
}
