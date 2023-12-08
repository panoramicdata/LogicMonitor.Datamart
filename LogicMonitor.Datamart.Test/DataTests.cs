namespace LogicMonitor.Datamart.Test;

public class DataTests(ITestOutputHelper iTestOutputHelper) : TestWithOutput(iTestOutputHelper)
{
	[Fact]
	public async void GetData_Succeeds()
	{
		var device = new DeviceStoreItem
		{
			CreatedOnSeconds = 0
		};

		var deviceDataSourceInstanceDataPoint = new DeviceDataSourceInstanceDataPointStoreItem
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
		var endDateTime = new DateTimeOffset(utcNow.Year,
			utcNow.Month,
			1,
			0,
			0,
			0,
			TimeSpan.Zero);
		var startDateTime = endDateTime.AddMonths(-1);

		var dataPointName = "Uptime";

		var dataPointStoreItem = new DataSourceDataPointStoreItem
		{
			GlobalAlertExpression = string.Empty,
			PercentageAvailabilityCalculation = "PercentUpTime",
			MeasurementUnit = "Seconds",
		};

		var result = await LowResolutionDataSync.GetTimeSeriesDataAggregationStoreItemAsync(
			DatamartClient,
			device,
			deviceDataSourceInstanceDataPoint,
			startDateTime,
			endDateTime,
			dataPointName,
			dataPointStoreItem,
			LoggerFactory.CreateLogger<DataTests>(),
			default)
			.ConfigureAwait(true);

		result.Should().NotBeNull();
		result!.PeriodStart.Should().Be(startDateTime);
		result.AvailabilityPercent.Should().BeApproximately(70, 5);
	}

	[Fact]
	public async void LowResolutionDataSync_RunsSuccessfully()
	{
		await new LowResolutionDataSync(
				DatamartClient,
				Configuration,
				LoggerFactory)
			.ExecuteAsync(default)
			.ConfigureAwait(true);
	}
}
