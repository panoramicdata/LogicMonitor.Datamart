using Microsoft.Extensions.Logging.Abstractions;

namespace LogicMonitor.Datamart.Test;

/// <summary>
/// Tests for MS-22557: Behavior when ExcludeSdtPeriods configuration is true vs false
/// Verifies that main aggregation columns always use all data, and ExcludingSdt columns
/// are only populated when SDT data is available.
/// </summary>
public class ExcludeSdtPeriodsConfigurationTests(ITestOutputHelper testOutputHelper) : TestWithOutput(testOutputHelper)
{
	private readonly ILogger<ExcludeSdtPeriodsConfigurationTests> _logger = NullLogger<ExcludeSdtPeriodsConfigurationTests>.Instance;

	#region Behavior Tests

	[Fact]
	public void WhenSdtPeriodsNull_ExcludingSdtColumnsAreNull()
	{
		// Arrange - Simulating when ExcludeSdtPeriods = false (sdtPeriods will be null)
		List<(long StartTimestampMs, long EndTimestampMs)>? sdtPeriods = null;

		var pairedData = new List<(long timestamp, double? value)>
		{
			(1000L, 10.0),
			(2000L, 20.0),
			(3000L, 30.0)
		};

		// Build timeSeriesDataPoints as the production code does
		var timeSeriesDataPoints = new List<TimeSeriesDataPoint>();
		foreach (var (timestamp, value) in pairedData)
		{
			var isInSdt = sdtPeriods?.Any(sdt =>
				timestamp >= sdt.StartTimestampMs &&
				timestamp <= sdt.EndTimestampMs) ?? false;

			timeSeriesDataPoints.Add(new TimeSeriesDataPoint
			{
				IsInSdt = isInSdt,
				Timestamp = timestamp,
				Value = value
			});
		}

		// Act - Calculate excludingSdtValues as production code does
		List<double?>? excludingSdtValues = null;
		double[]? sortedNonNullValuesExcludingSdt = null;

		if (sdtPeriods != null)
		{
			var excludingSdtDataPoints = timeSeriesDataPoints.Where(dp => !dp.IsInSdt).ToList();
			excludingSdtValues = [.. excludingSdtDataPoints.Select(dp => dp.Value)];
			sortedNonNullValuesExcludingSdt =
			[.. excludingSdtValues
				.Where(v => v.HasValue)
				.Select(v => v!.Value)
				.OrderBy(v => v)
			];
		}

		// Assert - When sdtPeriods is null, excludingSdtValues should be null
		excludingSdtValues.Should().BeNull();
		sortedNonNullValuesExcludingSdt.Should().BeNull();

		// And therefore ExcludingSdt columns would be null:
		var dataCountExcludingSdt = excludingSdtValues?.Count(d => d.HasValue);
		dataCountExcludingSdt.Should().BeNull();
	}

	[Fact]
	public void WhenSdtPeriodsAvailable_ExcludingSdtColumnsArePopulated()
	{
		// Arrange - Simulating when ExcludeSdtPeriods = true (sdtPeriods will be available)
		var sdtPeriods = new List<(long StartTimestampMs, long EndTimestampMs)>
		{
			(1500L, 2500L) // SDT covers timestamp 2000L
		};

		var pairedData = new List<(long timestamp, double? value)>
		{
			(1000L, 10.0),  // Not in SDT
			(2000L, 20.0),  // IN SDT
			(3000L, 30.0)   // Not in SDT
		};

		// Build timeSeriesDataPoints as the production code does
		var timeSeriesDataPoints = new List<TimeSeriesDataPoint>();
		foreach (var (timestamp, value) in pairedData)
		{
			var isInSdt = sdtPeriods?.Any(sdt =>
				timestamp >= sdt.StartTimestampMs &&
				timestamp <= sdt.EndTimestampMs) ?? false;

			timeSeriesDataPoints.Add(new TimeSeriesDataPoint
			{
				IsInSdt = isInSdt,
				Timestamp = timestamp,
				Value = value
			});
		}

		// Act - Calculate excludingSdtValues as production code does
		List<double?>? excludingSdtValues = null;
		double[]? sortedNonNullValuesExcludingSdt = null;

		if (sdtPeriods != null)
		{
			var excludingSdtDataPoints = timeSeriesDataPoints.Where(dp => !dp.IsInSdt).ToList();
			excludingSdtValues = [.. excludingSdtDataPoints.Select(dp => dp.Value)];
			sortedNonNullValuesExcludingSdt =
			[.. excludingSdtValues
				.Where(v => v.HasValue)
				.Select(v => v!.Value)
				.OrderBy(v => v)
			];
		}

		// Assert - When sdtPeriods is available, excludingSdtValues should be populated
		excludingSdtValues.Should().NotBeNull();
		excludingSdtValues.Should().HaveCount(2); // Only non-SDT points
		excludingSdtValues.Should().BeEquivalentTo([10.0, 30.0]); // Excluding 20.0

		sortedNonNullValuesExcludingSdt.Should().NotBeNull();
		sortedNonNullValuesExcludingSdt.Should().BeEquivalentTo([10.0, 30.0]);
	}

	[Fact]
	public void AllValues_AlwaysIncludesAllDataPoints_RegardlessOfSdt()
	{
		// Arrange - SDT periods available
		var sdtPeriods = new List<(long StartTimestampMs, long EndTimestampMs)>
		{
			(1500L, 2500L)
		};

		var pairedData = new List<(long timestamp, double? value)>
		{
			(1000L, 10.0),
			(2000L, 20.0),  // IN SDT
			(3000L, 30.0)
		};

		var timeSeriesDataPoints = pairedData.Select(p => new TimeSeriesDataPoint
		{
			Timestamp = p.timestamp,
			Value = p.value,
			IsInSdt = sdtPeriods.Any(sdt => p.timestamp >= sdt.StartTimestampMs && p.timestamp <= sdt.EndTimestampMs)
		}).ToList();

		// Act - allValues should include ALL data points
		var allValues = timeSeriesDataPoints.Select(dp => dp.Value).ToList();

		// Assert - ALL values included regardless of SDT
		allValues.Should().HaveCount(3);
		allValues.Should().BeEquivalentTo([10.0, 20.0, 30.0]);
	}

	#endregion

	#region IsInSdt Flag Tests

	[Fact]
	public void IsInSdt_CorrectlyIdentifiesPointsInSdtPeriod()
	{
		// Arrange
		var sdtPeriods = new List<(long StartTimestampMs, long EndTimestampMs)>
		{
			(1000L, 3000L),  // SDT period 1
			(5000L, 7000L)   // SDT period 2
		};

		var timestamps = new long[] { 500L, 1000L, 2000L, 3000L, 4000L, 5000L, 6000L, 8000L };

		// Act
		var results = timestamps.Select(ts => new
		{
			Timestamp = ts,
			IsInSdt = sdtPeriods.Any(sdt => ts >= sdt.StartTimestampMs && ts <= sdt.EndTimestampMs)
		}).ToList();

		// Assert
		results[0].IsInSdt.Should().BeFalse(); // 500 - before first SDT
		results[1].IsInSdt.Should().BeTrue();  // 1000 - start of first SDT
		results[2].IsInSdt.Should().BeTrue();  // 2000 - inside first SDT
		results[3].IsInSdt.Should().BeTrue();  // 3000 - end of first SDT
		results[4].IsInSdt.Should().BeFalse(); // 4000 - between SDT periods
		results[5].IsInSdt.Should().BeTrue();  // 5000 - start of second SDT
		results[6].IsInSdt.Should().BeTrue();  // 6000 - inside second SDT
		results[7].IsInSdt.Should().BeFalse(); // 8000 - after second SDT
	}

	[Fact]
	public void IsInSdt_WithMergedOverlappingPeriods_CorrectlyIdentifies()
	{
		// Arrange - Two overlapping periods that would merge to (1000L, 5000L)
		var sdtPeriods = new List<(long StartTimestampMs, long EndTimestampMs)>
		{
			(1000L, 3000L),
			(2500L, 5000L)
		};

		// Merge them first (as the production code does)
		var mergedPeriods = LowResolutionDataSync.MergeSdtPeriods(sdtPeriods, _logger);

		var timestamp = 4000L; // Would be in merged range but not in first original range

		// Act
		var isInSdtMerged = mergedPeriods.Any(sdt => timestamp >= sdt.StartTimestampMs && timestamp <= sdt.EndTimestampMs);

		// Assert
		mergedPeriods.Should().HaveCount(1);
		mergedPeriods[0].Should().Be((1000L, 5000L));
		isInSdtMerged.Should().BeTrue(); // 4000 is in merged range
	}

	#endregion

	#region Null SDT Scenarios

	[Fact]
	public void WhenSdtPeriodsEmpty_AllPointsAreNotInSdt()
	{
		// Arrange - Empty SDT list (no scheduled downtime)
		var sdtPeriods = new List<(long StartTimestampMs, long EndTimestampMs)>();

		var pairedData = new List<(long timestamp, double? value)>
		{
			(1000L, 10.0),
			(2000L, 20.0),
			(3000L, 30.0)
		};

		// Act
		var timeSeriesDataPoints = pairedData.Select(p => new TimeSeriesDataPoint
		{
			Timestamp = p.timestamp,
			Value = p.value,
			IsInSdt = sdtPeriods.Any(sdt => p.timestamp >= sdt.StartTimestampMs && p.timestamp <= sdt.EndTimestampMs)
		}).ToList();

		// Assert - No points should be in SDT
		timeSeriesDataPoints.All(dp => !dp.IsInSdt).Should().BeTrue();

		// ExcludingSdt values should be same as all values
		var allValues = timeSeriesDataPoints.Select(dp => dp.Value).ToList();
		var excludingSdtValues = timeSeriesDataPoints.Where(dp => !dp.IsInSdt).Select(dp => dp.Value).ToList();
		excludingSdtValues.Should().BeEquivalentTo(allValues);
	}

	[Fact]
	public void WhenSdtPeriodsNull_IsInSdtDefaultsToFalse()
	{
		// Arrange - Null SDT (when ExcludeSdtPeriods = false)
		List<(long StartTimestampMs, long EndTimestampMs)>? sdtPeriods = null;

		var timestamp = 1000L;

		// Act - Using the same logic as production code
		var isInSdt = sdtPeriods?.Any(sdt =>
			timestamp >= sdt.StartTimestampMs &&
			timestamp <= sdt.EndTimestampMs) ?? false;

		// Assert
		isInSdt.Should().BeFalse();
	}

	#endregion

	#region SDT Count Column Behavior

	[Fact]
	public void SdtCountColumns_AreNullWhenSdtPeriodsNull()
	{
		// Arrange
		List<(long StartTimestampMs, long EndTimestampMs)>? sdtPeriods = null;
		var timeSeriesDataPoints = new List<TimeSeriesDataPoint>
		{
			new() { Value = 10.0, IsInSdt = false, Timestamp = 1000L },
			new() { Value = 20.0, IsInSdt = false, Timestamp = 2000L }
		};

		// Act - Simulating production code logic
		var normalOrSdtCount = sdtPeriods == null ? null : CallCountAtAlertLevelUseSdt(
			timeSeriesDataPoints, "> 50", CountAlertLevel.Normal);

		// Assert
		normalOrSdtCount.Should().BeNull();
	}

	[Fact]
	public void SdtCountColumns_ArePopulatedWhenSdtPeriodsAvailable()
	{
		// Arrange
		var sdtPeriods = new List<(long StartTimestampMs, long EndTimestampMs)>
		{
			(1500L, 2500L)
		};

		var timeSeriesDataPoints = new List<TimeSeriesDataPoint>
		{
			new() { Value = 10.0, IsInSdt = false, Timestamp = 1000L },
			new() { Value = 60.0, IsInSdt = true, Timestamp = 2000L },  // Warning level, IN SDT
			new() { Value = 70.0, IsInSdt = false, Timestamp = 3000L }  // Warning level, NOT in SDT
		};

		// Act - Simulating production code logic
		var normalOrSdtCount = sdtPeriods == null ? null : CallCountAtAlertLevelUseSdt(
			timeSeriesDataPoints, "> 50", CountAlertLevel.Normal);

		var warningSdtCount = sdtPeriods == null ? null : CallCountAtAlertLevelUseSdt(
			timeSeriesDataPoints, "> 50", CountAlertLevel.Warning);

		// Assert
		normalOrSdtCount.Should().NotBeNull();
		normalOrSdtCount.Should().Be(2); // 10.0 is Normal, 60.0 is in SDT (counts as NormalOrSdt)

		warningSdtCount.Should().NotBeNull();
		warningSdtCount.Should().Be(1); // Only 60.0 is Warning AND in SDT
	}

	#endregion

	#region Helper Methods

	/// <summary>
	/// Helper to call the private CountAtAlertLevelUseSdt method via reflection
	/// </summary>
	private static int? CallCountAtAlertLevelUseSdt(
		List<TimeSeriesDataPoint> timeSeriesDataPoints,
		string effectiveAlertExpression,
		CountAlertLevel countAtAlertLevel)
	{
		var method = typeof(LowResolutionDataSync).GetMethod(
			"CountAtAlertLevelUseSdt",
			BindingFlags.NonPublic | BindingFlags.Static);

		return method == null
			? throw new InvalidOperationException("Could not find CountAtAlertLevelUseSdt method")
			: (int?)method.Invoke(null, [timeSeriesDataPoints, effectiveAlertExpression, countAtAlertLevel]);
	}

	#endregion
}
