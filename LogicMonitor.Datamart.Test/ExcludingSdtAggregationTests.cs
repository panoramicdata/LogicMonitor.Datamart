namespace LogicMonitor.Datamart.Test;

/// <summary>
/// Tests for MS-22557: ExcludingSdt aggregation columns
/// Tests the logic that calculates aggregations for data excluding SDT periods
/// </summary>
public class ExcludingSdtAggregationTests(ITestOutputHelper testOutputHelper) : TestWithOutput(testOutputHelper)
{
	#region Test Data Helpers

	private static List<TimeSeriesDataPoint> CreateTestDataPoints((double? value, bool isInSdt)[] dataPoints) =>
		[.. dataPoints.Select((dp, index) => new TimeSeriesDataPoint
		{
			Value = dp.value,
			IsInSdt = dp.isInSdt,
			Timestamp = 1000L * index
		})];

	private static List<double?> GetAllValues(List<TimeSeriesDataPoint> dataPoints) =>
		[.. dataPoints.Select(dp => dp.Value)];

	private static List<double?> GetExcludingSdtValues(List<TimeSeriesDataPoint> dataPoints) =>
		[.. dataPoints.Where(dp => !dp.IsInSdt).Select(dp => dp.Value)];

	private static double[] GetSortedNonNullValues(List<double?> values) =>
		[.. values.Where(v => v.HasValue).Select(v => v!.Value).OrderBy(v => v)];

	#endregion

	#region Data Count Tests

	[Fact]
	public void DataCount_AllValues_IncludesSdtPeriods()
	{
		// Arrange - Mix of SDT and non-SDT data points
		var dataPoints = CreateTestDataPoints(
		[
			(10.0, false),  // Non-SDT with value
			(20.0, true),   // SDT with value
			(30.0, false),  // Non-SDT with value
			(null, true),   // SDT without value
			(40.0, true)    // SDT with value
		]);

		var allValues = GetAllValues(dataPoints);

		// Act
		var dataCount = allValues.Count(d => d.HasValue);
		var noDataCount = allValues.Count(d => !d.HasValue);

		// Assert - Should count ALL data points
		dataCount.Should().Be(4);   // 4 with values (including SDT)
		noDataCount.Should().Be(1); // 1 without value
	}

	[Fact]
	public void DataCountExcludingSdt_ExcludesSdtPeriods()
	{
		// Arrange - Mix of SDT and non-SDT data points
		var dataPoints = CreateTestDataPoints(
		[
			(10.0, false),  // Non-SDT with value -> COUNT
			(20.0, true),   // SDT with value -> EXCLUDE
			(30.0, false),  // Non-SDT with value -> COUNT
			(null, true),   // SDT without value -> EXCLUDE
			(40.0, true),   // SDT with value -> EXCLUDE
			(null, false)   // Non-SDT without value -> COUNT (as NoData)
		]);

		var excludingSdtValues = GetExcludingSdtValues(dataPoints);

		// Act
		var dataCountExcludingSdt = excludingSdtValues.Count(d => d.HasValue);
		var noDataCountExcludingSdt = excludingSdtValues.Count(d => !d.HasValue);

		// Assert - Should only count non-SDT data points
		dataCountExcludingSdt.Should().Be(2);   // Only non-SDT with values
		noDataCountExcludingSdt.Should().Be(1); // Non-SDT without value
	}

	#endregion

	#region Sum and SumSquared Tests

	[Fact]
	public void Sum_AllValues_IncludesSdtPeriods()
	{
		// Arrange
		var dataPoints = CreateTestDataPoints(
		[
			(10.0, false),  // Non-SDT
			(20.0, true),   // SDT
			(30.0, false)   // Non-SDT
		]);

		var allValues = GetAllValues(dataPoints);

		// Act
		var sum = allValues.Sum(d => d ?? 0);

		// Assert - Should sum ALL values including SDT
		sum.Should().Be(60.0); // 10 + 20 + 30
	}

	[Fact]
	public void SumExcludingSdt_ExcludesSdtPeriods()
	{
		// Arrange
		var dataPoints = CreateTestDataPoints(
		[
			(10.0, false),  // Non-SDT -> Include
			(20.0, true),   // SDT -> Exclude
			(30.0, false)   // Non-SDT -> Include
		]);

		var excludingSdtValues = GetExcludingSdtValues(dataPoints);

		// Act
		var sumExcludingSdt = excludingSdtValues.Sum(d => d ?? 0);

		// Assert - Should only sum non-SDT values
		sumExcludingSdt.Should().Be(40.0); // 10 + 30 (excluding 20)
	}

	[Fact]
	public void SumSquared_AllValues_IncludesSdtPeriods()
	{
		// Arrange
		var dataPoints = CreateTestDataPoints(
		[
			(2.0, false),   // Non-SDT: 4
			(3.0, true),    // SDT: 9
			(4.0, false)    // Non-SDT: 16
		]);

		var allValues = GetAllValues(dataPoints);

		// Act
		var sumSquared = allValues.Sum(d => d.HasValue ? d.Value * d.Value : 0);

		// Assert - Should sum squares of ALL values
		sumSquared.Should().Be(29.0); // 4 + 9 + 16
	}

	[Fact]
	public void SumSquaredExcludingSdt_ExcludesSdtPeriods()
	{
		// Arrange
		var dataPoints = CreateTestDataPoints(
		[
			(2.0, false),   // Non-SDT: 4
			(3.0, true),    // SDT: excluded
			(4.0, false)    // Non-SDT: 16
		]);

		var excludingSdtValues = GetExcludingSdtValues(dataPoints);

		// Act
		var sumSquaredExcludingSdt = excludingSdtValues.Sum(d => d.HasValue ? d.Value * d.Value : 0);

		// Assert - Should only sum squares of non-SDT values
		sumSquaredExcludingSdt.Should().Be(20.0); // 4 + 16 (excluding 9)
	}

	#endregion

	#region Min/Max Tests

	[Fact]
	public void MinMax_AllValues_IncludesSdtPeriods()
	{
		// Arrange - SDT period has the min and max values
		var dataPoints = CreateTestDataPoints(
		[
			(50.0, false),  // Non-SDT
			(10.0, true),   // SDT - min
			(100.0, true),  // SDT - max
			(60.0, false)   // Non-SDT
		]);

		var allValues = GetAllValues(dataPoints);

		// Act
		var min = allValues.Where(d => d != null).DefaultIfEmpty(null).Min();
		var max = allValues.Where(d => d != null).DefaultIfEmpty(null).Max();

		// Assert - Should include SDT values in min/max
		min.Should().Be(10.0);
		max.Should().Be(100.0);
	}

	[Fact]
	public void MinMaxExcludingSdt_ExcludesSdtPeriods()
	{
		// Arrange - SDT period has the min and max values
		var dataPoints = CreateTestDataPoints(
		[
			(50.0, false),  // Non-SDT - will be min after excluding SDT
			(10.0, true),   // SDT - excluded
			(100.0, true),  // SDT - excluded
			(60.0, false)   // Non-SDT - will be max after excluding SDT
		]);

		var excludingSdtValues = GetExcludingSdtValues(dataPoints);

		// Act
		var minExcludingSdt = excludingSdtValues.Where(d => d != null).DefaultIfEmpty(null).Min();
		var maxExcludingSdt = excludingSdtValues.Where(d => d != null).DefaultIfEmpty(null).Max();

		// Assert - Should only consider non-SDT values
		minExcludingSdt.Should().Be(50.0);
		maxExcludingSdt.Should().Be(60.0);
	}

	#endregion

	#region First/Last Tests

	[Fact]
	public void FirstLast_AllValues_IncludesSdtPeriods()
	{
		// Arrange
		var dataPoints = CreateTestDataPoints(
		[
			(10.0, true),   // SDT - first
			(20.0, false),  // Non-SDT
			(30.0, true)    // SDT - last
		]);

		var allValues = GetAllValues(dataPoints);

		// Act
		var first = allValues.DefaultIfEmpty(null).First();
		var last = allValues.DefaultIfEmpty(null).Last();

		// Assert - Should include SDT values
		first.Should().Be(10.0);
		last.Should().Be(30.0);
	}

	[Fact]
	public void FirstLastExcludingSdt_ExcludesSdtPeriods()
	{
		// Arrange
		var dataPoints = CreateTestDataPoints(
		[
			(10.0, true),   // SDT - excluded
			(20.0, false),  // Non-SDT - will be first and last
			(30.0, true)    // SDT - excluded
		]);

		var excludingSdtValues = GetExcludingSdtValues(dataPoints);

		// Act
		var firstExcludingSdt = excludingSdtValues.DefaultIfEmpty(null).First();
		var lastExcludingSdt = excludingSdtValues.DefaultIfEmpty(null).Last();

		// Assert - Should only consider non-SDT values
		firstExcludingSdt.Should().Be(20.0);
		lastExcludingSdt.Should().Be(20.0);
	}

	[Fact]
	public void FirstLastWithData_AllValues_IncludesSdtPeriods()
	{
		// Arrange - First and last with actual values are in SDT
		var dataPoints = CreateTestDataPoints(
		[
			(null, false),  // Non-SDT, no data
			(10.0, true),   // SDT - first with data
			(null, false),  // Non-SDT, no data
			(30.0, true),   // SDT - last with data
			(null, false)   // Non-SDT, no data
		]);

		var allValues = GetAllValues(dataPoints);

		// Act
		var firstWithData = allValues.Where(d => d != null).DefaultIfEmpty(null).First();
		var lastWithData = allValues.Where(d => d != null).DefaultIfEmpty(null).Last();

		// Assert - Should include SDT values
		firstWithData.Should().Be(10.0);
		lastWithData.Should().Be(30.0);
	}

	[Fact]
	public void FirstLastWithDataExcludingSdt_ExcludesSdtPeriods()
	{
		// Arrange
		var dataPoints = CreateTestDataPoints(
		[
			(null, false),  // Non-SDT, no data
			(10.0, true),   // SDT - excluded
			(20.0, false),  // Non-SDT - first with data
			(30.0, true),   // SDT - excluded
			(40.0, false),  // Non-SDT - last with data
			(null, false)   // Non-SDT, no data
		]);

		var excludingSdtValues = GetExcludingSdtValues(dataPoints);

		// Act
		var firstWithDataExcludingSdt = excludingSdtValues.Where(d => d != null).DefaultIfEmpty(null).First();
		var lastWithDataExcludingSdt = excludingSdtValues.Where(d => d != null).DefaultIfEmpty(null).Last();

		// Assert
		firstWithDataExcludingSdt.Should().Be(20.0);
		lastWithDataExcludingSdt.Should().Be(40.0);
	}

	#endregion

	#region Percentile Tests

	[Fact]
	public void Percentiles_AllValues_IncludesSdtPeriods()
	{
		// Arrange - Values where SDT affects the percentiles
		var dataPoints = CreateTestDataPoints(
		[
			(10.0, false),  // Non-SDT
			(20.0, true),   // SDT
			(30.0, false),  // Non-SDT
			(40.0, true),   // SDT
			(50.0, false)   // Non-SDT
		]);

		var allValues = GetAllValues(dataPoints);
		var sortedNonNullValues = GetSortedNonNullValues(allValues);

		// Act
		var centile50 = LowResolutionDataSync.CalculatePercentile(sortedNonNullValues, 50);

		// Assert - Should include SDT values in percentile calculation
		centile50.Should().Be(30.0); // Median of [10, 20, 30, 40, 50]
	}

	[Fact]
	public void PercentilesExcludingSdt_ExcludesSdtPeriods()
	{
		// Arrange - Values where SDT affects the percentiles
		var dataPoints = CreateTestDataPoints(
		[
			(10.0, false),  // Non-SDT -> include
			(20.0, true),   // SDT -> exclude
			(30.0, false),  // Non-SDT -> include
			(40.0, true),   // SDT -> exclude
			(50.0, false)   // Non-SDT -> include
		]);

		var excludingSdtValues = GetExcludingSdtValues(dataPoints);
		var sortedNonNullValuesExcludingSdt = GetSortedNonNullValues(excludingSdtValues);

		// Act
		var centile50ExcludingSdt = LowResolutionDataSync.CalculatePercentile(sortedNonNullValuesExcludingSdt, 50);

		// Assert - Should only use non-SDT values [10, 30, 50]
		centile50ExcludingSdt.Should().Be(30.0); // Median of [10, 30, 50]
	}

	[Fact]
	public void PercentilesExcludingSdt_DifferentFromAllValues()
	{
		// Arrange - Create data where excluding SDT gives different percentiles
		var dataPoints = CreateTestDataPoints(
		[
			(10.0, false),  // Non-SDT
			(100.0, true),  // SDT - high value that affects percentiles
			(20.0, false),  // Non-SDT
			(200.0, true),  // SDT - high value that affects percentiles
			(30.0, false)   // Non-SDT
		]);

		var allValues = GetAllValues(dataPoints);
		var excludingSdtValues = GetExcludingSdtValues(dataPoints);
		var sortedAll = GetSortedNonNullValues(allValues);
		var sortedExcludingSdt = GetSortedNonNullValues(excludingSdtValues);

		// Act
		var centile50All = LowResolutionDataSync.CalculatePercentile(sortedAll, 50);
		var centile50ExcludingSdt = LowResolutionDataSync.CalculatePercentile(sortedExcludingSdt, 50);

		// Assert - Percentiles should be different
		// All values: [10, 20, 30, 100, 200] -> median = 30
		// Excluding SDT: [10, 20, 30] -> median = 20
		centile50All.Should().Be(30.0);
		centile50ExcludingSdt.Should().Be(20.0);
	}

	#endregion

	#region Edge Cases

	[Fact]
	public void ExcludingSdt_AllValuesInSdt_ReturnsEmptyOrNull()
	{
		// Arrange - All data points are in SDT
		var dataPoints = CreateTestDataPoints(
		[
			(10.0, true),
			(20.0, true),
			(30.0, true)
		]);

		var excludingSdtValues = GetExcludingSdtValues(dataPoints);

		// Assert
		excludingSdtValues.Should().BeEmpty();
	}

	[Fact]
	public void ExcludingSdt_NoValuesInSdt_SameAsAllValues()
	{
		// Arrange - No data points are in SDT
		var dataPoints = CreateTestDataPoints(
		[
			(10.0, false),
			(20.0, false),
			(30.0, false)
		]);

		var allValues = GetAllValues(dataPoints);
		var excludingSdtValues = GetExcludingSdtValues(dataPoints);

		// Assert - Should be identical
		excludingSdtValues.Should().BeEquivalentTo(allValues);
	}

	[Fact]
	public void ExcludingSdt_OnlyNullValuesOutsideSdt()
	{
		// Arrange - Only null values are outside SDT
		var dataPoints = CreateTestDataPoints(
		[
			(10.0, true),   // SDT
			(null, false),  // Non-SDT, no data
			(20.0, true),   // SDT
			(null, false)   // Non-SDT, no data
		]);

		var excludingSdtValues = GetExcludingSdtValues(dataPoints);

		// Act
		var dataCountExcludingSdt = excludingSdtValues.Count(d => d.HasValue);
		var noDataCountExcludingSdt = excludingSdtValues.Count(d => !d.HasValue);

		// Assert
		dataCountExcludingSdt.Should().Be(0);
		noDataCountExcludingSdt.Should().Be(2);
	}

	[Fact]
	public void ExcludingSdt_MixedNullAndValues()
	{
		// Arrange
		var dataPoints = CreateTestDataPoints(
		[
			(10.0, false),  // Non-SDT with value
			(null, false),  // Non-SDT without value
			(20.0, true),   // SDT with value (excluded)
			(null, true),   // SDT without value (excluded)
			(30.0, false)   // Non-SDT with value
		]);

		var excludingSdtValues = GetExcludingSdtValues(dataPoints);

		// Assert
		excludingSdtValues.Should().HaveCount(3);
		excludingSdtValues.Should().BeEquivalentTo(new double?[] { 10.0, null, 30.0 });
	}

	#endregion

	#region Availability Percent Tests

	[Fact]
	public void AvailabilityPercent_AllValues_IncludesSdtPeriods()
	{
		// Arrange - Mix of values and nulls
		var dataPoints = CreateTestDataPoints(
		[
			(5000.0, false),  // Non-SDT uptime value
			(5000.0, true),   // SDT uptime value
			(null, false),    // Non-SDT null
			(5000.0, false)   // Non-SDT uptime value
		]);

		var allValues = GetAllValues(dataPoints);

		// Act
		var availability = LowResolutionDataSync.CalculatePercentageAvailability(
			allValues,
			"PercentUpTime");

		// Assert - Should use ALL values including SDT
		availability.Should().NotBeNull();
		availability.Should().BeGreaterThan(0);
	}

	[Fact]
	public void AvailabilityPercentExcludingSdt_ExcludesSdtPeriods()
	{
		// Arrange - Nulls only in non-SDT periods
		var dataPoints = CreateTestDataPoints(
		[
			(5000.0, false),  // Non-SDT uptime
			(5000.0, true),   // SDT uptime (excluded)
			(null, false),    // Non-SDT null (included)
			(null, false),    // Non-SDT null (included)
			(5000.0, false)   // Non-SDT uptime
		]);

		var excludingSdtValues = GetExcludingSdtValues(dataPoints);

		// Act
		var availabilityExcludingSdt = LowResolutionDataSync.CalculatePercentageAvailability(
			excludingSdtValues,
			"PercentUpTime");

		// Assert - Should calculate using only non-SDT values
		availabilityExcludingSdt.Should().NotBeNull();
	}

	#endregion

	#region Count At Alert Level Tests

	[Fact]
	public void CountAtAlertLevel_AllValues_IncludesSdtPeriods()
	{
		// Arrange - Expression: "> 50" (Warning threshold)
		var dataPoints = CreateTestDataPoints(
		[
			(30.0, false),  // Normal, Non-SDT
			(60.0, true),   // Warning, SDT
			(70.0, false)   // Warning, Non-SDT
		]);

		var allValues = GetAllValues(dataPoints);

		// Act
		var normalCount = CallCountAtAlertLevel(allValues, "> 50", CountAlertLevel.Normal);
		var warningCount = CallCountAtAlertLevel(allValues, "> 50", CountAlertLevel.Warning);

		// Assert - Should count ALL values
		normalCount.Should().Be(1);  // Only 30.0 is normal
		warningCount.Should().Be(2); // 60.0 and 70.0 are warning
	}

	[Fact]
	public void CountAtAlertLevelExcludingSdt_ExcludesSdtPeriods()
	{
		// Arrange - Expression: "> 50" (Warning threshold)
		var dataPoints = CreateTestDataPoints(
		[
			(30.0, false),  // Normal, Non-SDT -> Count
			(60.0, true),   // Warning, SDT -> Exclude
			(70.0, false)   // Warning, Non-SDT -> Count
		]);

		var excludingSdtValues = GetExcludingSdtValues(dataPoints);

		// Act
		var normalCountExcludingSdt = CallCountAtAlertLevel(excludingSdtValues, "> 50", CountAlertLevel.Normal);
		var warningCountExcludingSdt = CallCountAtAlertLevel(excludingSdtValues, "> 50", CountAlertLevel.Warning);

		// Assert - Should only count non-SDT values
		normalCountExcludingSdt.Should().Be(1);  // 30.0 is normal (non-SDT)
		warningCountExcludingSdt.Should().Be(1); // Only 70.0 is warning (non-SDT)
	}

	#endregion

	#region Integration-like Tests

	[Fact]
	public void FullAggregation_ComparesAllVsExcludingSdt()
	{
		// Arrange - Realistic scenario with SDT during a maintenance window
		var dataPoints = CreateTestDataPoints(
		[
			(100.0, false), // Normal operation
			(95.0, false),  // Normal operation
			(5.0, true),    // During SDT - system down for maintenance
			(10.0, true),   // During SDT - system starting up
			(90.0, false),  // Normal operation resumed
			(92.0, false)   // Normal operation
		]);

		var allValues = GetAllValues(dataPoints);
		var excludingSdtValues = GetExcludingSdtValues(dataPoints);

		// Act - Calculate various aggregations
		var sumAll = allValues.Sum(d => d ?? 0);
		var sumExcluding = excludingSdtValues.Sum(d => d ?? 0);

		var avgAll = allValues.Where(d => d.HasValue).Average(d => d!.Value);
		var avgExcluding = excludingSdtValues.Where(d => d.HasValue).Average(d => d!.Value);

		var minAll = allValues.Where(d => d != null).Min();
		var minExcluding = excludingSdtValues.Where(d => d != null).Min();

		// Assert
		// All values include the low values during maintenance
		sumAll.Should().Be(392.0);  // 100+95+5+10+90+92
		sumExcluding.Should().Be(377.0);  // 100+95+90+92 (excluding 5+10)

		// Average is higher when excluding SDT maintenance period
		avgAll.Should().BeApproximately(65.33, 0.01);
		avgExcluding.Should().BeApproximately(94.25, 0.01);

		// Min is affected by SDT period values
		minAll.Should().Be(5.0);
		minExcluding.Should().Be(90.0);
	}

	#endregion

	#region Helper Methods

	/// <summary>
	/// Helper to call the private CountAtAlertLevel method via reflection
	/// </summary>
	private static int? CallCountAtAlertLevel(
		List<double?> data,
		string effectiveAlertExpression,
		CountAlertLevel countAlertLevel)
	{
		var method = typeof(LowResolutionDataSync).GetMethod(
			"CountAtAlertLevel",
			BindingFlags.NonPublic | BindingFlags.Static,
			null,
			[typeof(List<double?>), typeof(string), typeof(CountAlertLevel)],
			null);

		return method == null
			? throw new InvalidOperationException("Could not find CountAtAlertLevel method")
			: (int?)method.Invoke(null, [data, effectiveAlertExpression, countAlertLevel]);
	}

	#endregion
}
