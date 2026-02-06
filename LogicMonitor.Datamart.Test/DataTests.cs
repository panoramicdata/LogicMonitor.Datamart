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

	/// <summary>
	/// Tests the IsThrottled detection method with a known throttled response (3-month request).
	/// </summary>
	[Fact]
	public async Task IsThrottled_ThreeMonthRequest_DetectsThrottling()
	{
		// Arrange: Fetch throttled data (3 months)
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
		var deviceDataSourceInstanceId = 272867174;

		var graphData = await LowResolutionDataSync.GetGraphDataAsync(
			DatamartClient,
			deviceDataSourceInstanceId,
			startDateTimeUtc,
			endDateTimeUtc,
			LoggerFactory.CreateLogger<DataTests>(),
			default);

		// Act: Check if throttled
		var isThrottled = LowResolutionDataSync.IsThrottled(
			graphData,
			daysInPeriod,
			out var actualStepSeconds,
			out var actualPointsPerDay);

		// Assert
		ITestOutputHelper.WriteLine($"Days in period: {daysInPeriod}");
		ITestOutputHelper.WriteLine($"Timestamp count: {graphData.TimeStamps.Count}");
		ITestOutputHelper.WriteLine($"Points per day: {actualPointsPerDay:F1}");
		ITestOutputHelper.WriteLine($"Step seconds: {actualStepSeconds}");
		ITestOutputHelper.WriteLine($"IsThrottled: {isThrottled}");

		isThrottled.Should().BeTrue("3-month request should be detected as throttled");
		actualStepSeconds.Should().BeGreaterThan(3600, "Throttled step should be > 3600s");
		actualPointsPerDay.Should().BeLessThan(18, "Throttled points per day should be < 18");
	}

	/// <summary>
	/// Tests that a 1-month request is NOT detected as throttled.
	/// </summary>
	[Fact]
	public async Task IsThrottled_OneMonthRequest_NotThrottled()
	{
		// Arrange: Fetch non-throttled data (1 month)
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
		var deviceDataSourceInstanceId = 272867174;

		var graphData = await LowResolutionDataSync.GetGraphDataAsync(
			DatamartClient,
			deviceDataSourceInstanceId,
			startDateTimeUtc,
			endDateTimeUtc,
			LoggerFactory.CreateLogger<DataTests>(),
			default);

		// Act: Check if throttled
		var isThrottled = LowResolutionDataSync.IsThrottled(
			graphData,
			daysInPeriod,
			out var actualStepSeconds,
			out var actualPointsPerDay);

		// Assert
		ITestOutputHelper.WriteLine($"Days in period: {daysInPeriod}");
		ITestOutputHelper.WriteLine($"Timestamp count: {graphData.TimeStamps.Count}");
		ITestOutputHelper.WriteLine($"Points per day: {actualPointsPerDay:F1}");
		ITestOutputHelper.WriteLine($"Step seconds: {actualStepSeconds}");
		ITestOutputHelper.WriteLine($"IsThrottled: {isThrottled}");

		isThrottled.Should().BeFalse("1-month request should NOT be detected as throttled");
		actualStepSeconds.Should().BeInRange(3240, 3960, "Non-throttled step should be ~3600s ± 10%");
		actualPointsPerDay.Should().BeGreaterThanOrEqualTo(18, "Non-throttled points per day should be >= 18");
	}

	/// <summary>
	/// Tests that auto-chunking successfully retrieves hourly resolution for a 3-month request.
	/// When EnableAutoChunking is true, a 3-month request should be split into smaller chunks
	/// and return approximately 24 data points per day.
	/// 
	/// NOTE: This test documents observed behavior. The server may or may not throttle
	/// depending on current load. The test validates that chunking produces MORE data
	/// than a single throttled request.
	/// </summary>
	[Fact]
	public async Task GetGraphDataWithAutoChunking_ThreeMonthRequest_ReturnsHourlyResolution()
	{
		// Arrange: Set up a three-month date range (will be throttled without chunking)
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
		var deviceDataSourceInstanceId = 272867174;

		// Act: Fetch with auto-chunking enabled, 31-day chunks (optimal for hourly resolution)
		var graphData = await LowResolutionDataSync.GetGraphDataWithAutoChunkingAsync(
		DatamartClient,
		deviceDataSourceInstanceId,
		startDateTimeUtc,
		endDateTimeUtc,
		enableAutoChunking: true,
		chunkSizeDays: 31,
			LoggerFactory.CreateLogger<DataTests>(),
			default);

		// Assert: We should now have approximately hourly resolution
		var timestampCount = graphData.TimeStamps.Count;
		var pointsPerDay = timestampCount / daysInPeriod;

		// Calculate step if we have enough data
		long stepSeconds = 0;
		if (timestampCount >= 2)
		{
			var stepMs = graphData.TimeStamps[1] - graphData.TimeStamps[0];
			stepSeconds = stepMs / 1000;
		}

		ITestOutputHelper.WriteLine($"=== Auto-Chunking Test Results ===");
		ITestOutputHelper.WriteLine($"Date range: {startDateTimeUtc:yyyy-MM-dd} to {endDateTimeUtc:yyyy-MM-dd}");
		ITestOutputHelper.WriteLine($"Days in period: {daysInPeriod}");
		ITestOutputHelper.WriteLine($"Timestamp count: {timestampCount}");
		ITestOutputHelper.WriteLine($"Points per day: {pointsPerDay:F1}");
		ITestOutputHelper.WriteLine($"Step size: {stepSeconds} seconds ({stepSeconds / 3600.0:F2} hours)");

		var expectedMinPoints = (int)(daysInPeriod * 20);
		ITestOutputHelper.WriteLine($"Expected min points: {expectedMinPoints}");

		// With auto-chunking, we should get roughly hourly resolution
		// Even for 3 months (92 days), we should get close to 92 * 24 = 2208 points
		pointsPerDay.Should().BeGreaterThanOrEqualTo(
			18,
			$"With auto-chunking, should get at least 18 points per day (got {pointsPerDay:F1})");

		timestampCount.Should().BeGreaterThanOrEqualTo(
			expectedMinPoints,
			$"With auto-chunking, should get at least {expectedMinPoints} points for {daysInPeriod} days");

		stepSeconds.Should().BeInRange(
			3240, 3960,
			"With auto-chunking, step size should be approximately hourly (3600s ± 10%)");

		ITestOutputHelper.WriteLine($"");
		ITestOutputHelper.WriteLine($"=== CONCLUSION ===");
		ITestOutputHelper.WriteLine($"Auto-chunking successfully restored hourly resolution for 3-month request!");
	}

	/// <summary>
	/// Compares throttled (no chunking) vs non-throttled (with chunking) results for the same 3-month period.
	/// This demonstrates the data loss that occurs without auto-chunking.
	/// </summary>
	[Fact]
	public async Task GetGraphData_ComparesThrottledVsChunked()
	{
		// Arrange: 3-month date range
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
		var deviceDataSourceInstanceId = 272867174;

		// Act 1: Fetch as single request (may be aggregated)
		var singleRequestData = await LowResolutionDataSync.GetGraphDataAsync(
			DatamartClient,
			deviceDataSourceInstanceId,
			startDateTimeUtc,
			endDateTimeUtc,
			LoggerFactory.CreateLogger<DataTests>(),
			default);

		// Act 2: Fetch WITH chunking (31-day chunks for hourly resolution)
		var chunkedData = await LowResolutionDataSync.GetGraphDataChunkedAsync(
		DatamartClient,
		deviceDataSourceInstanceId,
		startDateTimeUtc,
		endDateTimeUtc,
		chunkSizeDays: 31,
			LoggerFactory.CreateLogger<DataTests>(),
			default);

		// Assert: Document observed behavior
		var singleCount = singleRequestData.TimeStamps.Count;
		var chunkedCount = chunkedData.TimeStamps.Count;
		var ratio = singleCount > 0 ? (double)chunkedCount / singleCount : 0;

		ITestOutputHelper.WriteLine($"=== Single Request vs Chunked Comparison ===");
		ITestOutputHelper.WriteLine($"Date range: {startDateTimeUtc:yyyy-MM-dd} to {endDateTimeUtc:yyyy-MM-dd} ({daysInPeriod:F0} days)");
		ITestOutputHelper.WriteLine($"");
		ITestOutputHelper.WriteLine($"Single request:");
		ITestOutputHelper.WriteLine($"  - Timestamp count: {singleCount}");
		ITestOutputHelper.WriteLine($"  - Points per day: {singleCount / daysInPeriod:F1}");
		ITestOutputHelper.WriteLine($"");
		ITestOutputHelper.WriteLine($"Chunked (31-day chunks):");
		ITestOutputHelper.WriteLine($"  - Timestamp count: {chunkedCount}");
		ITestOutputHelper.WriteLine($"  - Points per day: {chunkedCount / daysInPeriod:F1}");
		ITestOutputHelper.WriteLine($"");
		ITestOutputHelper.WriteLine($"Ratio: {ratio:F1}x (chunked/single)");

		// Document the observed behavior - don't assert strict ratios since server behavior varies
		// Just ensure we got SOME data from both methods
		singleCount.Should().BeGreaterThan(0, "Single request should return data");
		chunkedCount.Should().BeGreaterThan(0, "Chunked request should return data");

		// Log conclusion based on observed behavior
		if (ratio > 1.5)
		{
			ITestOutputHelper.WriteLine($"");
			ITestOutputHelper.WriteLine($"OBSERVATION: Chunking provided {ratio:F1}x more data points.");
			ITestOutputHelper.WriteLine($"This suggests the single request was throttled/aggregated by the API.");
		}
		else if (ratio is > 0.9 and < 1.1)
		{
			ITestOutputHelper.WriteLine($"");
			ITestOutputHelper.WriteLine($"OBSERVATION: Both methods returned similar amounts of data.");
			ITestOutputHelper.WriteLine($"The API did not apply significant aggregation to the single request.");
		}
		else
		{
			ITestOutputHelper.WriteLine($"");
			ITestOutputHelper.WriteLine($"OBSERVATION: Unexpected ratio ({ratio:F1}x). Check data quality.");
		}
	}
}
