namespace LogicMonitor.Datamart.Test;

public class DataTests : TestWithOutput
{
	public DataTests(ITestOutputHelper iTestOutputHelper)
	 : base(iTestOutputHelper)
	{
	}

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
				LogicMonitorId = 268954440
			},
			DataSourceDataPoint = new()
			{
			}
		};

		var endDateTime = new DateTimeOffset(DateTimeOffset.UtcNow.Date, TimeSpan.Zero);
		var startDateTime = endDateTime.AddHours(-8);

		string dataPointName = "PercentUpTime";

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
			default)
			.ConfigureAwait(false);

		result.Should().NotBeNull();
		result.PeriodStart.Should().Be(startDateTime);
		result.AvailabilityPercent.Should().BeApproximately(50, 5);
	}

	[Fact]
	public async void LowResolutionDataSync_RunsSuccessfully()
	{
		await new LowResolutionDataSync(
				DatamartClient,
				Configuration,
				LoggerFactory)
			.ExecuteAsync(default)
			.ConfigureAwait(false);
	}
}
