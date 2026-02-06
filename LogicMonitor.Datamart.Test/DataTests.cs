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
		);

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

	/// <summary>
	/// Validates that a one-month data request returns approximately hourly resolution data.
	/// This test guards against LogicMonitor API adaptive aggregation issues.
	/// 
	/// Expected behavior:
	/// - For date ranges ≤40 days, the API should return 3600s (hourly) step
	/// - This means ~24 data points per day, or ~672-744 points per month
	/// 
	/// If this test fails with significantly fewer points (e.g., ~240 instead of ~720),
	/// it indicates the API is applying adaptive aggregation, which occurs when:
	/// - Date ranges exceed 42-43 days
	/// - The "1000 point rule" is being applied
	/// </summary>
	[Fact]
	public async Task GetGraphData_MonthlyRequest_ReturnsHourlyResolution()
	{
		// Arrange: Set up a one-month date range (28-31 days, well under 40-day threshold)
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

		var daysInPeriod = (endDateTimeUtc - startDateTimeUtc).TotalDays;

		// Use a known DeviceDataSourceInstance ID from the test configuration
		var deviceDataSourceInstanceId = 272867174;

		// Act: Fetch graph data for one month
		var graphData = await LowResolutionDataSync.GetGraphDataAsync(
			DatamartClient,
			deviceDataSourceInstanceId,
			startDateTimeUtc,
			endDateTimeUtc,
			LoggerFactory.CreateLogger<DataTests>(),
			default);

		// Assert: Validate we received approximately hourly resolution
		var timestampCount = graphData.TimeStamps.Count;

		// Expected: ~24 data points per day for hourly resolution
		var expectedMinPoints = (int)(daysInPeriod * 20);  // Allow 20 points/day minimum (some gaps OK)
		var expectedMaxPoints = (int)(daysInPeriod * 30);  // Allow up to 30 points/day (accounts for slight variations)

		// Log the actual values for diagnostics
		ITestOutputHelper.WriteLine($"Date range: {startDateTimeUtc:yyyy-MM-dd} to {endDateTimeUtc:yyyy-MM-dd}");
		ITestOutputHelper.WriteLine($"Days in period: {daysInPeriod}");
		ITestOutputHelper.WriteLine($"Timestamp count: {timestampCount}");
		ITestOutputHelper.WriteLine($"Points per day: {timestampCount / daysInPeriod:F1}");
		ITestOutputHelper.WriteLine($"Expected range: {expectedMinPoints} - {expectedMaxPoints}");

		// Calculate step size if we have multiple timestamps
		if (timestampCount >= 2)
		{
			var firstTimestamp = graphData.TimeStamps[0];
			var secondTimestamp = graphData.TimeStamps[1];
			var stepMs = secondTimestamp - firstTimestamp;
			var stepSeconds = stepMs / 1000;
			ITestOutputHelper.WriteLine($"Step size: {stepSeconds} seconds ({stepSeconds / 3600.0:F2} hours)");

			// Verify step is approximately hourly (3600s ± 10%)
			stepSeconds.Should().BeInRange(3240, 3960, "Step size should be approximately hourly (3600s ± 10%)");
		}

		// Verify we got enough data points (at least 20 per day on average)
		timestampCount.Should().BeGreaterThanOrEqualTo(
			expectedMinPoints,
			$"Expected at least {expectedMinPoints} data points for {daysInPeriod} days " +
			$"(~20+ per day for hourly resolution). Got {timestampCount} which is " +
			$"{timestampCount / daysInPeriod:F1} per day. This may indicate API adaptive aggregation.");
	}

	/// <summary>
	/// Demonstrates LogicMonitor API adaptive aggregation behavior for requests exceeding 42-43 days.
	/// 
	/// This test documents the "1000 point rule" - for date ranges ≥45 days, LogicMonitor
	/// caps data at ~1000 points regardless of duration, scaling the step size accordingly.
	/// 
	/// Expected behavior for 90-day request:
	/// - Step size increases to ~7800s (~2.2 hours) instead of 3600s (hourly)
	/// - Points per day drops from ~24 to ~11
	/// - Total points ~1000 instead of ~2160
	/// 
	/// This test validates the documented boundary behavior and serves as a regression
	/// guard to detect if LogicMonitor changes their aggregation algorithm.
	/// </summary>
	[Fact]
	public async Task GetGraphData_ThreeMonthRequest_DemonstratesAdaptiveAggregation()
	{
		// Arrange: Set up a three-month date range (90+ days, well over 42-43 day threshold)
		var utcNow = DateTimeOffset.UtcNow;
		var endDateTimeUtc = new DateTimeOffset(
			utcNow.Year,
			utcNow.Month,
			1,
			0,
			0,
			0,
			TimeSpan.Zero);
		var startDateTimeUtc = endDateTimeUtc.AddMonths(-3);

		var daysInPeriod = (endDateTimeUtc - startDateTimeUtc).TotalDays;

		// Use a known DeviceDataSourceInstance ID from the test configuration
		var deviceDataSourceInstanceId = 272867174;

		// Act: Fetch graph data for three months
		var graphData = await LowResolutionDataSync.GetGraphDataAsync(
			DatamartClient,
			deviceDataSourceInstanceId,
			startDateTimeUtc,
			endDateTimeUtc,
			LoggerFactory.CreateLogger<DataTests>(),
			default);

		// Assert: Validate we observe adaptive aggregation behavior
		var timestampCount = graphData.TimeStamps.Count;
		var pointsPerDay = timestampCount / daysInPeriod;

		// Log the actual values for diagnostics
		ITestOutputHelper.WriteLine($"=== Three-Month Request (Adaptive Aggregation Test) ===");
		ITestOutputHelper.WriteLine($"Date range: {startDateTimeUtc:yyyy-MM-dd} to {endDateTimeUtc:yyyy-MM-dd}");
		ITestOutputHelper.WriteLine($"Days in period: {daysInPeriod}");
		ITestOutputHelper.WriteLine($"Timestamp count: {timestampCount}");
		ITestOutputHelper.WriteLine($"Points per day: {pointsPerDay:F1}");

		// Calculate step size if we have multiple timestamps
		long stepSeconds = 0;
		if (timestampCount >= 2)
		{
			var firstTimestamp = graphData.TimeStamps[0];
			var secondTimestamp = graphData.TimeStamps[1];
			var stepMs = secondTimestamp - firstTimestamp;
			stepSeconds = stepMs / 1000;
			ITestOutputHelper.WriteLine($"Step size: {stepSeconds} seconds ({stepSeconds / 3600.0:F2} hours)");
		}

		// Document expected vs actual
		var expectedHourlyPoints = (int)(daysInPeriod * 24);
		var reductionFactor = (double)expectedHourlyPoints / timestampCount;
		ITestOutputHelper.WriteLine($"");
		ITestOutputHelper.WriteLine($"=== Adaptive Aggregation Analysis ===");
		ITestOutputHelper.WriteLine($"Expected points (hourly): {expectedHourlyPoints}");
		ITestOutputHelper.WriteLine($"Actual points: {timestampCount}");
		ITestOutputHelper.WriteLine($"Reduction factor: {reductionFactor:F1}x fewer points");
		ITestOutputHelper.WriteLine($"Expected hourly step: 3600 seconds");
		ITestOutputHelper.WriteLine($"Actual step: {stepSeconds} seconds");

		// Verify adaptive aggregation is occurring (step > 3600s)
		// For 90 days, we expect step to be approximately 7800s (~2.2 hours)
		stepSeconds.Should().BeGreaterThan(
			3600,
			"For requests >42 days, LogicMonitor should apply adaptive aggregation with step > 3600s");

		// Verify we're getting approximately 1000 points (the "1000 point rule")
		// Allow range of 800-1200 to account for variations
		timestampCount.Should().BeInRange(
			800,
			1200,
			$"For requests >42 days, LogicMonitor caps at ~1000 points. Got {timestampCount}.");

		// Verify points per day is significantly less than 24 (hourly)
		pointsPerDay.Should().BeLessThan(
			15,
			$"For requests >42 days, points per day should be significantly less than 24. Got {pointsPerDay:F1}.");

		ITestOutputHelper.WriteLine($"");
		ITestOutputHelper.WriteLine($"=== CONCLUSION ===");
		ITestOutputHelper.WriteLine($"Adaptive aggregation confirmed: {reductionFactor:F1}x data reduction");
		ITestOutputHelper.WriteLine($"This is why LowResolutionDataSync uses monthly chunks (≤31 days)");
	}
}
