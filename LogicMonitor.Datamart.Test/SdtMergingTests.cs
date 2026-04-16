using Microsoft.Extensions.Logging.Abstractions;

namespace LogicMonitor.Datamart.Test;

/// <summary>
/// MS-21395: Unit tests for SDT period merging functionality which is part of the LowResolutionDataSync aggregation process if SDT is excluded
/// </summary>
/// <remarks>All tests use a null logger to avoid xUnit output helper complications for pure unit tests.</remarks>
public class SdtMergingTests
{
	private readonly ILogger<SdtMergingTests> _logger = NullLogger<SdtMergingTests>.Instance;

	/// <summary>
	/// Verifies that merging an empty list of SDT periods returns an empty list.
	/// </summary>
	[Fact]
	public void MergeSdtPeriods_EmptyList_ReturnsEmptyList()
	{
		// Arrange
		var sdtPeriods = new List<(long StartTimestampMs, long EndTimestampMs)>();

		// Act
		var result = LowResolutionDataSync.MergeSdtPeriods(sdtPeriods, _logger);

		// Assert
		result.Should().BeEmpty();
	}

	/// <summary>
	/// Verifies that a single SDT period is returned unchanged.
	/// </summary>
	[Fact]
	public void MergeSdtPeriods_SinglePeriod_ReturnsSamePeriod()
	{
		// Arrange
		var sdtPeriods = new List<(long StartTimestampMs, long EndTimestampMs)>
		{
			(1000L, 2000L)
		};

		// Act
		var result = LowResolutionDataSync.MergeSdtPeriods(sdtPeriods, _logger);

		// Assert
		result.Should().HaveCount(1);
		result[0].Should().Be((1000L, 2000L));
	}

	/// <summary>
	/// Verifies that two non-overlapping SDT periods are returned separately.
	/// </summary>
	[Fact]
	public void MergeSdtPeriods_TwoNonOverlappingPeriods_ReturnsTwoPeriods()
	{
		// Arrange
		var sdtPeriods = new List<(long StartTimestampMs, long EndTimestampMs)>
		{
			(1000L, 2000L),
			(3000L, 4000L)
		};

		// Act
		var result = LowResolutionDataSync.MergeSdtPeriods(sdtPeriods, _logger);

		// Assert
		result.Should().HaveCount(2);
		result[0].Should().Be((1000L, 2000L));
		result[1].Should().Be((3000L, 4000L));
	}

	/// <summary>
	/// Verifies that two SDT periods with various overlapping configurations are merged into a single period.
	/// </summary>
	/// <param name="start1Hours">Start offset in hours of the first period.</param>
	/// <param name="end1Hours">End offset in hours of the first period.</param>
	/// <param name="start2Hours">Start offset in hours of the second period.</param>
	/// <param name="end2Hours">End offset in hours of the second period.</param>
	/// <param name="expectedStartHours">Expected start offset in hours of the merged period.</param>
	/// <param name="expectedEndHours">Expected end offset in hours of the merged period.</param>
	[Theory]
	[InlineData(0, 8, 7, 9, 0, 9)]		// Partial overlap
	[InlineData(0, 10, 5, 15, 0, 15)]	// Partial overlap
	[InlineData(0, 10, 10, 20, 0, 20)]	// Adjacent periods (touching)
	public void MergeSdtPeriods_TwoPeriodsVariousOverlaps_MergesCorrectly(
		int start1Hours,
		int end1Hours,
		int start2Hours,
		int end2Hours,
		int expectedStartHours,
		int expectedEndHours)
	{
		// Arrange - Convert hours to milliseconds for timestamps
		var baseTime = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
		var sdtPeriods = new List<(long StartTimestampMs, long EndTimestampMs)>
		{
			(baseTime.AddHours(start1Hours).ToUnixTimeMilliseconds(), 
			 baseTime.AddHours(end1Hours).ToUnixTimeMilliseconds()),
			(baseTime.AddHours(start2Hours).ToUnixTimeMilliseconds(), 
			 baseTime.AddHours(end2Hours).ToUnixTimeMilliseconds())
		};

		var expectedStart = baseTime.AddHours(expectedStartHours).ToUnixTimeMilliseconds();
		var expectedEnd = baseTime.AddHours(expectedEndHours).ToUnixTimeMilliseconds();

		// Act
		var result = LowResolutionDataSync.MergeSdtPeriods(sdtPeriods, _logger);

		// Assert - These test cases should all merge into 1 period
		result.Should().HaveCount(1);
		result[0].StartTimestampMs.Should().Be(expectedStart);
		result[0].EndTimestampMs.Should().Be(expectedEnd);
	}

	/// <summary>
	/// Verifies that two SDT periods with a gap between them are not merged.
	/// </summary>
	[Fact]
	public void MergeSdtPeriods_TwoPeriodsWithGap_DoesNotMerge()
	{
		// Arrange - Gap between periods (should NOT merge)
		// Period 1: 0-10 hours, Period 2: 11-20 hours (1 hour gap)
		var baseTime = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
		var sdtPeriods = new List<(long StartTimestampMs, long EndTimestampMs)>
		{
			(baseTime.AddHours(0).ToUnixTimeMilliseconds(), 
			 baseTime.AddHours(10).ToUnixTimeMilliseconds()),
			(baseTime.AddHours(11).ToUnixTimeMilliseconds(), 
			 baseTime.AddHours(20).ToUnixTimeMilliseconds())
		};

		// Act
		var result = LowResolutionDataSync.MergeSdtPeriods(sdtPeriods, _logger);

		// Assert - Should have 2 separate periods
		result.Should().HaveCount(2);
		result[0].StartTimestampMs.Should().Be(baseTime.AddHours(0).ToUnixTimeMilliseconds());
		result[0].EndTimestampMs.Should().Be(baseTime.AddHours(10).ToUnixTimeMilliseconds());
		result[1].StartTimestampMs.Should().Be(baseTime.AddHours(11).ToUnixTimeMilliseconds());
		result[1].EndTimestampMs.Should().Be(baseTime.AddHours(20).ToUnixTimeMilliseconds());
	}

	/// <summary>
	/// Verifies that when one SDT period is completely inside another they merge into the outer period.
	/// </summary>
	[Fact]
	public void MergeSdtPeriods_CompleteOverlap_MergesIntoOnePeriod()
	{
		// Arrange - One period completely inside another
		var sdtPeriods = new List<(long StartTimestampMs, long EndTimestampMs)>
		{
			(1000L, 10000L),	// Outer period
			(3000L, 5000L)		// Inner period
		};

		// Act
		var result = LowResolutionDataSync.MergeSdtPeriods(sdtPeriods, _logger);

		// Assert
		result.Should().HaveCount(1);
		result[0].Should().Be((1000L, 10000L));
	}

	/// <summary>
	/// Verifies that a chain of overlapping SDT periods is merged into a single continuous period.
	/// </summary>
	[Fact]
	public void MergeSdtPeriods_MultipleOverlappingPeriods_MergesAll()
	{
		// Arrange - Chain of overlapping periods
		var sdtPeriods = new List<(long StartTimestampMs, long EndTimestampMs)>
		{
			(1000L, 3000L),
			(2500L, 5000L),
			(4500L, 7000L),
			(6500L, 9000L)
		};

		// Act
		var result = LowResolutionDataSync.MergeSdtPeriods(sdtPeriods, _logger);

		// Assert - Should merge into one continuous period
		result.Should().HaveCount(1);
		result[0].Should().Be((1000L, 9000L));
	}

	/// <summary>
	/// Verifies that unsorted SDT periods are sorted before merging and overlapping ones are combined correctly.
	/// </summary>
	[Fact]
	public void MergeSdtPeriods_UnsortedPeriods_SortsAndMergesCorrectly()
	{
		// Arrange - Periods in random order
		var sdtPeriods = new List<(long StartTimestampMs, long EndTimestampMs)>
		{
			(6000L, 8000L),
			(1000L, 3000L),
			(2500L, 5000L)
		};

		// Act
		var result = LowResolutionDataSync.MergeSdtPeriods(sdtPeriods, _logger);

		// Assert - Should sort and merge overlapping periods
		result.Should().HaveCount(2);
		result[0].Should().Be((1000L, 5000L)); // First two merged
		result[1].Should().Be((6000L, 8000L)); // Last one separate
	}

	/// <summary>
	/// Verifies that only overlapping SDT periods are merged while non-overlapping ones remain separate.
	/// </summary>
	[Fact]
	public void MergeSdtPeriods_MixOfOverlappingAndNonOverlapping_MergesOnlyOverlapping()
	{
		// Arrange
		var sdtPeriods = new List<(long StartTimestampMs, long EndTimestampMs)>
		{
			(1000L, 2000L),
			(1500L, 2500L),		// Overlaps with first
			(5000L, 6000L),		// Separate
			(10000L, 11000L),	// Separate
			(10500L, 12000L)	// Overlaps with previous
		};

		// Act
		var result = LowResolutionDataSync.MergeSdtPeriods(sdtPeriods, _logger);

		// Assert
		result.Should().HaveCount(3);
		result[0].Should().Be((1000L, 2500L));		// First two merged
		result[1].Should().Be((5000L, 6000L));		// Separate
		result[2].Should().Be((10000L, 12000L));	// Last two merged
	}

	/// <summary>
	/// Verifies merging of a device-level and DataSource-level SDT that partially overlap, simulating a real monitoring scenario.
	/// </summary>
	[Fact]
	public void MergeSdtPeriods_RealWorldScenario_DeviceAndDataSourceLevels()
	{
		// Arrange - Simulating the example from the requirement
		// Device level: midnight (00:00) to 08:00
		// DataSource level: 07:55 to 09:00
		var midnight = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
		var sdtPeriods = new List<(long StartTimestampMs, long EndTimestampMs)>
		{
			(midnight.ToUnixTimeMilliseconds(), 
			 midnight.AddHours(8).ToUnixTimeMilliseconds()),
			(midnight.AddHours(7).AddMinutes(55).ToUnixTimeMilliseconds(), 
			 midnight.AddHours(9).ToUnixTimeMilliseconds())
		};

		// Act
		var result = LowResolutionDataSync.MergeSdtPeriods(sdtPeriods, _logger);

		// Assert - Should merge into one period from 00:00 to 09:00
		result.Should().HaveCount(1);
		result[0].StartTimestampMs.Should().Be(midnight.ToUnixTimeMilliseconds());
		result[0].EndTimestampMs.Should().Be(midnight.AddHours(9).ToUnixTimeMilliseconds());
	}

	/// <summary>
	/// Verifies that two adjacent SDT periods with a one-millisecond gap are not merged.
	/// </summary>
	[Fact]
	public void MergeSdtPeriods_AdjacentPeriodsWithOneMillisecondGap_DoesNotMerge()
	{
		// Arrange - Periods with a tiny gap (not touching)
		var sdtPeriods = new List<(long StartTimestampMs, long EndTimestampMs)>
		{
			(1000L, 2000L),
			(2001L, 3000L) // 1ms gap
		};

		// Act
		var result = LowResolutionDataSync.MergeSdtPeriods(sdtPeriods, _logger);

		// Assert - Should NOT merge (they don't overlap or touch)
		result.Should().HaveCount(2);
		result[0].Should().Be((1000L, 2000L));
		result[1].Should().Be((2001L, 3000L));
	}

	/// <summary>
	/// Verifies that identical duplicate SDT periods are merged into a single period.
	/// </summary>
	[Fact]
	public void MergeSdtPeriods_IdenticalPeriods_MergesIntoOne()
	{
		// Arrange - Duplicate periods
		var sdtPeriods = new List<(long StartTimestampMs, long EndTimestampMs)>
		{
			(1000L, 2000L),
			(1000L, 2000L),
			(1000L, 2000L)
		};

		// Act
		var result = LowResolutionDataSync.MergeSdtPeriods(sdtPeriods, _logger);

		// Assert - Should merge into one period
		result.Should().HaveCount(1);
		result[0].Should().Be((1000L, 2000L));
	}

	/// <summary>
	/// Verifies that nested SDT periods (inner fully inside outer) are merged into the outermost period.
	/// </summary>
	[Fact]
	public void MergeSdtPeriods_NestedPeriods_MergesIntoOuterPeriod()
	{
		// Arrange - Multiple nested periods
		var sdtPeriods = new List<(long StartTimestampMs, long EndTimestampMs)>
		{
			(1000L, 10000L),  // Outer
			(2000L, 8000L),   // Middle
			(4000L, 6000L)    // Inner
		};

		// Act
		var result = LowResolutionDataSync.MergeSdtPeriods(sdtPeriods, _logger);

		// Assert - Should merge into the outer period
		result.Should().HaveCount(1);
		result[0].Should().Be((1000L, 10000L));
	}
}
