using Microsoft.Extensions.Logging.Abstractions;

namespace LogicMonitor.Datamart.Test;

/// <summary>
/// Unit tests for the GraphData merging logic used when combining chunks.
/// These tests verify:
/// - Duplicate timestamps are removed (first value kept)
/// - Out-of-order timestamps are sorted
/// - Line data stays synchronized with timestamps
/// - Edge cases are handled correctly
/// </summary>
public class GraphDataMergingTests
{
	private readonly ILogger _logger = NullLogger.Instance;

	#region MergeGraphDataChunks Tests

	[Fact]
	public void MergeGraphDataChunks_EmptyInput_ReturnsEmptyGraphData()
	{
		// Arrange
		var timestamps = new List<long>();
		var lineData = new Dictionary<string, List<double?>>();

		// Act
		var result = LowResolutionDataSync.MergeGraphDataChunks(
			timestamps,
			lineData,
			_logger,
			resourceDataSourceInstanceId: 12345);

		// Assert
		result.TimeStamps.Should().BeEmpty();
		result.Lines.Should().BeEmpty();
	}

	[Fact]
	public void MergeGraphDataChunks_SingleChunk_ReturnsUnmodified()
	{
		// Arrange
		var timestamps = new List<long> { 1000, 2000, 3000 };
		var lineData = new Dictionary<string, List<double?>>
		{
			["Metric1"] = [10.0, 20.0, 30.0]
		};

		// Act
		var result = LowResolutionDataSync.MergeGraphDataChunks(
			timestamps,
			lineData,
			_logger,
			resourceDataSourceInstanceId: 12345);

		// Assert
		result.TimeStamps.Should().BeEquivalentTo([1000, 2000, 3000]);
		result.Lines.Should().HaveCount(1);
		result.Lines[0].Legend.Should().Be("Metric1");
		result.Lines[0].Data.Should().BeEquivalentTo([10.0, 20.0, 30.0]);
	}

	[Fact]
	public void MergeGraphDataChunks_DuplicateTimestamps_KeepsFirstValue()
	{
		// Arrange: Simulate overlapping chunks where timestamp 3000 appears twice
		// First chunk: 1000, 2000, 3000 with values 10, 20, 30
		// Second chunk: 3000, 4000, 5000 with values 99, 40, 50 (3000 is duplicate with different value)
		var timestamps = new List<long> { 1000, 2000, 3000, 3000, 4000, 5000 };
		var lineData = new Dictionary<string, List<double?>>
		{
			["Metric1"] = [10.0, 20.0, 30.0, 99.0, 40.0, 50.0] // 99 should be discarded
		};

		// Act
		var result = LowResolutionDataSync.MergeGraphDataChunks(
			timestamps,
			lineData,
			_logger,
			resourceDataSourceInstanceId: 12345);

		// Assert: Duplicate removed, first value (30) kept
		result.TimeStamps.Should().BeEquivalentTo([1000, 2000, 3000, 4000, 5000]);
		result.TimeStamps.Should().HaveCount(5); // One duplicate removed
		result.Lines[0].Data.Should().BeEquivalentTo([10.0, 20.0, 30.0, 40.0, 50.0]);
		result.Lines[0].Data.Should().NotContain(99.0); // Duplicate value discarded
	}

	[Fact]
	public void MergeGraphDataChunks_OutOfOrderTimestamps_SortsChronologically()
	{
		// Arrange: Timestamps out of order (e.g., from parallel chunk processing)
		var timestamps = new List<long> { 3000, 1000, 4000, 2000 };
		var lineData = new Dictionary<string, List<double?>>
		{
			["Metric1"] = [30.0, 10.0, 40.0, 20.0]
		};

		// Act
		var result = LowResolutionDataSync.MergeGraphDataChunks(
			timestamps,
			lineData,
			_logger,
			resourceDataSourceInstanceId: 12345);

		// Assert: Sorted chronologically with values following
		result.TimeStamps.Should().BeEquivalentTo([1000, 2000, 3000, 4000]);
		result.TimeStamps.Should().BeInAscendingOrder();
		result.Lines[0].Data.Should().BeEquivalentTo([10.0, 20.0, 30.0, 40.0]);
	}

	[Fact]
	public void MergeGraphDataChunks_MultipleLines_KeepsDataSynchronized()
	{
		// Arrange: Multiple data lines with duplicate timestamps
		var timestamps = new List<long> { 1000, 2000, 2000, 3000 }; // 2000 is duplicate
		var lineData = new Dictionary<string, List<double?>>
		{
			["CPU"] = [10.0, 20.0, 99.0, 30.0],   // 99 should be discarded
			["Memory"] = [100.0, 200.0, 999.0, 300.0] // 999 should be discarded
		};

		// Act
		var result = LowResolutionDataSync.MergeGraphDataChunks(
			timestamps,
			lineData,
			_logger,
			resourceDataSourceInstanceId: 12345);

		// Assert: Both lines have matching, deduplicated data
		result.TimeStamps.Should().HaveCount(3);
		result.Lines.Should().HaveCount(2);

		var cpuLine = result.Lines.First(l => l.Legend == "CPU");
		var memLine = result.Lines.First(l => l.Legend == "Memory");

		cpuLine.Data.Should().BeEquivalentTo([10.0, 20.0, 30.0]);
		memLine.Data.Should().BeEquivalentTo([100.0, 200.0, 300.0]);
	}

	[Fact]
	public void MergeGraphDataChunks_WithNullValues_PreservesNulls()
	{
		// Arrange: Some values are null (no data)
		var timestamps = new List<long> { 1000, 2000, 3000 };
		var lineData = new Dictionary<string, List<double?>>
		{
			["Metric1"] = [10.0, null, 30.0]
		};

		// Act
		var result = LowResolutionDataSync.MergeGraphDataChunks(
			timestamps,
			lineData,
			_logger,
			resourceDataSourceInstanceId: 12345);

		// Assert: Null values preserved
		result.Lines[0].Data.Should().HaveCount(3);
		result.Lines[0].Data[0].Should().Be(10.0);
		result.Lines[0].Data[1].Should().BeNull();
		result.Lines[0].Data[2].Should().Be(30.0);
	}

	[Fact]
	public void MergeGraphDataChunks_ComplexScenario_HandlesCorrectly()
	{
		// Arrange: Complex scenario simulating two chunks with:
		// - Chunk 1: Hourly resolution (3600s step) for days 1-14
		// - Chunk 2: 21-min resolution (1260s step) for days 14-28 (overlapping at boundary)
		// Timestamps: 0, 3600, 7200 (hourly) then 7200, 7200+1260, 7200+2520 (finer granularity)
		// The 7200 timestamp is duplicated

		var timestamps = new List<long>
		{
			0, 3600000, 7200000,           // Chunk 1 (hourly, in ms)
			7200000, 8460000, 9720000      // Chunk 2 (21-min, overlaps at 7200000)
		};
		var lineData = new Dictionary<string, List<double?>>
		{
			["Uptime"] = [0.0, 3600.0, 7200.0, 7200.5, 8460.0, 9720.0] // 7200.5 is the duplicate value
		};

		// Act
		var result = LowResolutionDataSync.MergeGraphDataChunks(
			timestamps,
			lineData,
			_logger,
			resourceDataSourceInstanceId: 12345);

		// Assert
		result.TimeStamps.Should().HaveCount(5); // One duplicate removed
		result.TimeStamps.Should().BeInAscendingOrder();
		result.TimeStamps.Should().BeEquivalentTo([0, 3600000, 7200000, 8460000, 9720000]);

		// First value (7200.0) kept, duplicate (7200.5) discarded
		result.Lines[0].Data.Should().Contain(7200.0);
		result.Lines[0].Data.Should().NotContain(7200.5);
	}

	[Fact]
	public void MergeGraphDataChunks_AllDuplicates_KeepsOnlyFirst()
	{
		// Arrange: All timestamps are the same (edge case)
		var timestamps = new List<long> { 1000, 1000, 1000 };
		var lineData = new Dictionary<string, List<double?>>
		{
			["Metric1"] = [10.0, 20.0, 30.0]
		};

		// Act
		var result = LowResolutionDataSync.MergeGraphDataChunks(
			timestamps,
			lineData,
			_logger,
			resourceDataSourceInstanceId: 12345);

		// Assert: Only one timestamp, first value kept
		result.TimeStamps.Should().HaveCount(1);
		result.TimeStamps.Should().BeEquivalentTo([1000]);
		result.Lines[0].Data.Should().BeEquivalentTo([10.0]);
	}

	[Fact]
	public void MergeGraphDataChunks_LargeDataset_PerformsCorrectly()
	{
		// Arrange: 1000 timestamps with 50 duplicates interspersed
		var timestamps = new List<long>();
		var values = new List<double?>();

		for (var i = 0; i < 1000; i++)
		{
			timestamps.Add(i * 1000);
			values.Add(i * 1.0);
		}

		// Add 50 duplicates with clearly different values (-1.0 series)
		for (var i = 0; i < 50; i++)
		{
			timestamps.Add(i * 1000); // Duplicate timestamp
			values.Add(-1.0 - i); // Different value: -1.0, -2.0, ..., -50.0
		}

		var lineData = new Dictionary<string, List<double?>>
		{
			["Metric1"] = values
		};

		// Act
		var result = LowResolutionDataSync.MergeGraphDataChunks(
			timestamps,
			lineData,
			_logger,
			resourceDataSourceInstanceId: 12345);

		// Assert
		result.TimeStamps.Should().HaveCount(1000); // 50 duplicates removed
		result.TimeStamps.Should().BeInAscendingOrder();

		// Verify first values kept (0.0 to 49.0), not duplicate values (-1.0 to -50.0)
		result.Lines[0].Data[0].Should().Be(0.0);   // Not -1.0
		result.Lines[0].Data[1].Should().Be(1.0);   // Not -2.0
		result.Lines[0].Data[49].Should().Be(49.0); // Not -50.0

		// Verify no negative values (all duplicates should be discarded)
		result.Lines[0].Data.Should().OnlyContain(v => v >= 0);
	}

	#endregion

	#region Edge Cases

	[Fact]
	public void MergeGraphDataChunks_SingleTimestamp_Works()
	{
		// Arrange
		var timestamps = new List<long> { 1000 };
		var lineData = new Dictionary<string, List<double?>>
		{
			["Metric1"] = [42.0]
		};

		// Act
		var result = LowResolutionDataSync.MergeGraphDataChunks(
			timestamps,
			lineData,
			_logger,
			resourceDataSourceInstanceId: 12345);

		// Assert
		result.TimeStamps.Should().BeEquivalentTo([1000]);
		result.Lines[0].Data.Should().BeEquivalentTo([42.0]);
	}

	[Fact]
	public void MergeGraphDataChunks_ReverseOrder_SortsCorrectly()
	{
		// Arrange: Completely reversed order
		var timestamps = new List<long> { 5000, 4000, 3000, 2000, 1000 };
		var lineData = new Dictionary<string, List<double?>>
		{
			["Metric1"] = [50.0, 40.0, 30.0, 20.0, 10.0]
		};

		// Act
		var result = LowResolutionDataSync.MergeGraphDataChunks(
			timestamps,
			lineData,
			_logger,
			resourceDataSourceInstanceId: 12345);

		// Assert
		result.TimeStamps.Should().BeEquivalentTo([1000, 2000, 3000, 4000, 5000]);
		result.Lines[0].Data.Should().BeEquivalentTo([10.0, 20.0, 30.0, 40.0, 50.0]);
	}

	#endregion
}
