using System.Runtime.CompilerServices;
using LogicMonitor.Api.Data;
using Microsoft.Extensions.Logging.Abstractions;

namespace LogicMonitor.Datamart.Test;

/// <summary>
/// Offline characterization tests pinning the current behaviour of the pure low resolution sync helpers.
/// </summary>
public class LowResolutionDataSyncCharacterizationTests
{
	private static readonly ILogger _logger = NullLogger.Instance;

	/// <summary>
	/// An empty array has no percentile.
	/// </summary>
	[Fact]
	public void CalculatePercentile_Empty_ReturnsNull()
		=> LowResolutionDataSync.CalculatePercentile([], 50).Should().BeNull();

	/// <summary>
	/// Whole and fractional ranks are read and interpolated from the sorted values.
	/// </summary>
	[Theory]
	[InlineData(0, 10)]
	[InlineData(25, 20)]
	[InlineData(50, 40)]
	[InlineData(90, 56)]
	[InlineData(100, 60)]
	public void CalculatePercentile_FiveValues_InterpolatesBetweenNeighbours(int n, double expected)
		=> LowResolutionDataSync.CalculatePercentile([10, 20, 40, 50, 60], n).Should().BeApproximately(expected, 1e-9);

	/// <summary>
	/// A single value is every percentile.
	/// </summary>
	[Fact]
	public void CalculatePercentile_SingleValue_ReturnsThatValue()
		=> LowResolutionDataSync.CalculatePercentile([7], 95).Should().Be(7);

	/// <summary>
	/// Availability is only calculated for the PercentUpTime calculation.
	/// </summary>
	[Theory]
	[InlineData("")]
	[InlineData("  ")]
	[InlineData("Other")]
	public void CalculatePercentageAvailability_UnsupportedCalculation_ReturnsNull(string calculation)
	{
		LowResolutionDataSync.CalculatePercentageAvailability([1, 2], calculation).Should().BeNull();
		LowResolutionDataSync.CalculatePercentageAvailabilityNew([1, 2], calculation).Should().BeNull();
		LowResolutionDataSync.CalculatePercentageAvailabilityOld([1, 2], calculation).Should().BeNull();
	}

	/// <summary>
	/// The current availability calculation counts gaps after low uptimes as downtime.
	/// </summary>
	[Theory]
	[InlineData(new double[] { }, null)]
	[InlineData(new[] { double.NaN, double.NaN, 100, 200 }, 50.0)]
	[InlineData(new[] { double.NaN, double.NaN, 10000, 10060 }, 100.0)]
	[InlineData(new[] { double.NaN, 5, 100 }, 66.66666666666667)]
	[InlineData(new[] { 100, double.NaN, double.NaN }, 100.0)]
	[InlineData(new[] { double.NaN, double.NaN }, null)]
	public void CalculatePercentageAvailability_PercentUpTime_PinsCounts(double[] values, double? expected)
		=> LowResolutionDataSync.CalculatePercentageAvailability(ToNullable(values), "PercentUpTime").Should().Be(expected);

	/// <summary>
	/// The legacy calculation reverses the caller's array in place.
	/// </summary>
	[Fact]
	public void CalculatePercentageAvailabilityOld_PercentUpTime_ReversesInputAndCounts()
	{
		double?[] values = [null, null, 100, 200];

		var result = LowResolutionDataSync.CalculatePercentageAvailabilityOld(values, "PercentUpTime");

		result.Should().Be(50);
		values.Should().Equal(200, 100, null, null);
	}

	/// <summary>
	/// The legacy calculation reports 100 percent when there are no values.
	/// </summary>
	[Fact]
	public void CalculatePercentageAvailabilityOld_NoValues_Returns100()
		=> LowResolutionDataSync.CalculatePercentageAvailabilityOld([], "PercentUpTime").Should().Be(100);

	/// <summary>
	/// The gradient based calculation detects gaps, nulls and counter resets.
	/// </summary>
	[Theory]
	[InlineData(new double[] { 10, 20, 30, 40, 50, 60, 70, 100, 110 }, 90.0)]
	[InlineData(new[] { 10, 20, double.NaN, 40 }, 75.0)]
	[InlineData(new double[] { 50, 10, 20, 30 }, 100.0)]
	[InlineData(new double[] { 5, 5, 5 }, null)]
	[InlineData(new[] { double.NaN, double.NaN }, null)]
	[InlineData(new double[] { }, null)]
	public void CalculatePercentageAvailabilityNew_PercentUpTime_PinsGradientBehaviour(double[] values, double? expected)
		=> LowResolutionDataSync.CalculatePercentageAvailabilityNew(ToNullable(values), "PercentUpTime").Should().Be(expected);

	/// <summary>
	/// Step size alone decides throttling.
	/// </summary>
	[Theory]
	[InlineData(3600000L, false, 3600)]
	[InlineData(4500000L, false, 4500)]
	[InlineData(4501000L, true, 4501)]
	[InlineData(7200000L, true, 7200)]
	public void IsThrottled_StepSize_DecidesThrottling(long stepMs, bool expected, long expectedStepSeconds)
	{
		var graph = new GraphData { TimeStamps = [0, stepMs, stepMs * 2, stepMs * 3], Lines = [] };

		LowResolutionDataSync.IsThrottled(graph, 2, out var stepSeconds, out var pointsPerDay).Should().Be(expected);

		stepSeconds.Should().Be(expectedStepSeconds);
		pointsPerDay.Should().Be(2);
	}

	/// <summary>
	/// Fewer than two timestamps is never throttling and reports zeros.
	/// </summary>
	[Fact]
	public void IsThrottled_SingleTimestamp_IsNotThrottled()
	{
		var graph = new GraphData { TimeStamps = [5], Lines = [] };

		LowResolutionDataSync.IsThrottled(graph, 1, out var stepSeconds, out var pointsPerDay).Should().BeFalse();

		stepSeconds.Should().Be(0);
		pointsPerDay.Should().Be(0);
	}

	/// <summary>
	/// Zero or one period is returned unchanged.
	/// </summary>
	[Fact]
	public void MergeSdtPeriods_ZeroOrOne_ReturnsInput()
	{
		LowResolutionDataSync.MergeSdtPeriods([], _logger).Should().BeEmpty();
		LowResolutionDataSync.MergeSdtPeriods([(5, 9)], _logger).Should().Equal((5L, 9L));
	}

	/// <summary>
	/// Overlapping, touching and contained periods merge after sorting; disjoint ones stay apart.
	/// </summary>
	[Fact]
	public void MergeSdtPeriods_UnsortedOverlappingAndTouching_AreMerged()
	{
		var result = LowResolutionDataSync.MergeSdtPeriods(
			[(50, 60), (0, 10), (10, 20), (15, 18), (30, 40), (35, 45)],
			_logger);

		result.Should().Equal((0L, 20L), (30L, 45L), (50L, 60L));
	}

	/// <summary>
	/// No chunks produces an empty graph.
	/// </summary>
	[Fact]
	public void MergeGraphDataChunks_NoTimestamps_ReturnsEmptyGraph()
	{
		var graph = LowResolutionDataSync.MergeGraphDataChunks([], [], _logger, 1);

		graph.TimeStamps.Should().BeEmpty();
		graph.Lines.Should().BeEmpty();
	}

	/// <summary>
	/// Timestamps are sorted and de-duplicated, keeping the first value for a duplicate, in line order.
	/// </summary>
	[Fact]
	public void MergeGraphDataChunks_DuplicateAndUnsorted_SortsAndKeepsFirstValue()
	{
		var graph = LowResolutionDataSync.MergeGraphDataChunks(
			[300, 100, 200, 100],
			new Dictionary<string, List<double?>>
			{
				["A"] = [3, 1, 2, 99],
				["B"] = [null, 10, 20, 98],
			},
			_logger,
			1);

		graph.TimeStamps.Should().Equal(100, 200, 300);
		graph.Lines.Select(l => l.Legend).Should().Equal("A", "B");
		graph.Lines[0].Data.Should().Equal(1, 2, 3);
		graph.Lines[1].Data.Should().Equal(10, 20, null);
	}

	/// <summary>
	/// Without a calculation the named line is aggregated, nulls are counted as no data and SDT columns stay null.
	/// </summary>
	[Fact]
	public async Task GetTimeSeriesDataAggregationStoreItem_PlainLine_AggregatesAllPoints()
	{
		var item = await AggregateAsync(
			Graph(("CPU", [10, 20, null, 40, 50, 60])),
			createdOnSeconds: 0,
			alertExpression: "> 30 50 70");

		item.DataCount.Should().Be(5);
		item.NoDataCount.Should().Be(1);
		item.Sum.Should().Be(180);
		item.SumSquared.Should().Be(8200);
		item.Max.Should().Be(60);
		item.Min.Should().Be(10);
		item.First.Should().Be(10);
		item.Last.Should().Be(60);
		item.FirstWithData.Should().Be(10);
		item.LastWithData.Should().Be(60);
		item.Centile05.Should().BeApproximately(12, 1e-9);
		item.Centile10.Should().BeApproximately(14, 1e-9);
		item.Centile25.Should().Be(20);
		item.Centile50.Should().Be(40);
		item.Centile75.Should().Be(50);
		item.Centile90.Should().BeApproximately(56, 1e-9);
		item.Centile95.Should().BeApproximately(58, 1e-9);
		item.NormalCount.Should().Be(2);
		item.WarningCount.Should().Be(2);
		item.ErrorCount.Should().Be(1);
		item.CriticalCount.Should().Be(0);
		item.AvailabilityPercent.Should().BeNull();
		item.AvailabilityPercent2.Should().BeNull();
		item.NormalOrSdtCount.Should().BeNull();
		item.DataCountExcludingSdt.Should().BeNull();
		item.Centile50ExcludingSdt.Should().BeNull();
		item.PeriodStart.Should().Be(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));
		item.PeriodEnd.Should().Be(new DateTimeOffset(2026, 2, 1, 0, 0, 0, TimeSpan.Zero));
		item.Id.Should().NotBe(Guid.Empty);
	}

	/// <summary>
	/// Points before the device was created in LogicMonitor are dropped.
	/// </summary>
	[Fact]
	public async Task GetTimeSeriesDataAggregationStoreItem_DeviceCreatedMidPeriod_DropsEarlierPoints()
	{
		var item = await AggregateAsync(
			Graph(("CPU", [10, 20, null, 40, 50, 60])),
			createdOnSeconds: 3,
			alertExpression: string.Empty);

		item.DataCount.Should().Be(3);
		item.NoDataCount.Should().Be(1);
		item.Sum.Should().Be(150);
		item.First.Should().BeNull();
		item.FirstWithData.Should().Be(40);
		item.NormalCount.Should().Be(3);
		item.WarningCount.Should().Be(0);
	}

	/// <summary>
	/// A graph with no points yields zero counts and null statistics.
	/// </summary>
	[Fact]
	public async Task GetTimeSeriesDataAggregationStoreItem_NoPoints_ReturnsEmptyStatistics()
	{
		var item = await AggregateAsync(
			Graph(("CPU", [])),
			createdOnSeconds: 0,
			alertExpression: "> 1");

		item.DataCount.Should().Be(0);
		item.NoDataCount.Should().Be(0);
		item.Sum.Should().Be(0);
		item.Max.Should().BeNull();
		item.First.Should().BeNull();
		item.Centile50.Should().BeNull();
		item.NormalCount.Should().Be(0);
	}

	/// <summary>
	/// A calculation is evaluated per point and infinite results are treated as no data.
	/// </summary>
	[Fact]
	public async Task GetTimeSeriesDataAggregationStoreItem_Calculation_EvaluatesAndDropsInfinity()
	{
		var item = await AggregateAsync(
			Graph(("CPU", [1, 2, 3])),
			createdOnSeconds: 0,
			alertExpression: string.Empty,
			calculation: "1 / (CPU - 2)");

		item.DataCount.Should().Be(2);
		item.NoDataCount.Should().Be(1);
		item.Sum.Should().Be(0);
		item.Min.Should().Be(-1);
		item.Max.Should().Be(1);
	}

	/// <summary>
	/// A missing line throws a FormatException naming the data point and data source.
	/// </summary>
	[Fact]
	public async Task GetTimeSeriesDataAggregationStoreItem_MissingLine_ThrowsFormatException()
	{
		var act = () => AggregateAsync(
			Graph(("Other", [1, 2, 3])),
			createdOnSeconds: 0,
			alertExpression: string.Empty);

		(await act.Should().ThrowAsync<FormatException>())
			.WithMessage("Could not find DataPoint 'CPU' for DataSource 'WinCPU'");
	}

	/// <summary>
	/// PercentUpTime availability is calculated on the aggregated values, both old and new style.
	/// </summary>
	[Fact]
	public async Task GetTimeSeriesDataAggregationStoreItem_PercentUpTime_CalculatesAvailability()
	{
		var item = await AggregateAsync(
			Graph(("CPU", [10, 20, null, 40])),
			createdOnSeconds: 0,
			alertExpression: string.Empty,
			availability: "PercentUpTime");

		item.AvailabilityPercent2.Should().Be(75);
		item.AvailabilityPercent.Should().Be(100d * 3 / 4);
	}

	/// <summary>
	/// With SDT periods available, the SDT counts and the excluding-SDT columns ignore points inside the SDT.
	/// </summary>
	[Fact]
	public void Build_WithSdtPeriod_PopulatesSdtAndExcludingSdtColumns()
	{
		var dataPoint = new DataSourceDataPointStoreItem { Id = Guid.NewGuid(), GlobalAlertExpression = "> 30 50 70" };
		var request = new TimeSeriesAggregationRequest
		{
			DatamartClient = null!,
			Device = new ResourceStoreItem(),
			InstanceDataPoint = new ResourceDataSourceInstanceDataPointStoreItem { Id = Guid.NewGuid() },
			DataPointName = "CPU",
			DataPoint = dataPoint,
			StartDateTimeUtc = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
			EndDateTimeUtc = new DateTimeOffset(2026, 2, 1, 0, 0, 0, TimeSpan.Zero),
			ExcludeSdtPeriods = true,
			SdtCache = [],
			Logger = _logger,
		};
		List<(long timestamp, double? value)> pairs =
			[(1000, 10), (2000, 20), (3000, null), (4000, 40), (5000, 50), (6000, 60)];

		var item = TimeSeriesAggregationBuilder.Build(request, pairs, [(2000, 3000)]);

		item.DataCount.Should().Be(5);
		item.DataCountExcludingSdt.Should().Be(4);
		item.NoDataCountExcludingSdt.Should().Be(0);
		item.SumExcludingSdt.Should().Be(160);
		item.MaxExcludingSdt.Should().Be(60);
		item.MinExcludingSdt.Should().Be(10);
		item.FirstExcludingSdt.Should().Be(10);
		item.LastExcludingSdt.Should().Be(60);
		item.Centile50ExcludingSdt.Should().Be(45);
		item.NormalOrSdtCount.Should().Be(3);
		item.WarningSdtCount.Should().Be(0);
		item.ErrorSdtCount.Should().Be(0);
		item.CriticalSdtCount.Should().Be(0);
	}
	private static List<double?> ToNullable(double[] values)
		=> [.. values.Select(v => double.IsNaN(v) ? (double?)null : v)];

	private static GraphData Graph(params (string Legend, double?[] Data)[] lines)
		=> new()
		{
			TimeStamps = [.. Enumerable.Range(1, lines[0].Data.Length).Select(i => i * 1000L)],
			Lines = [.. lines.Select(l => new Line { Legend = l.Legend, Data = [.. l.Data] })],
		};

	private static async Task<TimeSeriesDataAggregationStoreItem> AggregateAsync(
		GraphData graphData,
		long createdOnSeconds,
		string alertExpression,
		string calculation = "",
		string availability = "")
	{
		var dataSource = (DataSourceStoreItem)RuntimeHelpers.GetUninitializedObject(typeof(DataSourceStoreItem));
		dataSource.Name = "WinCPU";
		var dataPoint = new DataSourceDataPointStoreItem
		{
			Id = Guid.NewGuid(),
			DataSource = dataSource,
			Name = "CPU",
			GlobalAlertExpression = alertExpression,
			Calculation = calculation,
			PercentageAvailabilityCalculation = availability,
		};
		var instanceDataPoint = new ResourceDataSourceInstanceDataPointStoreItem
		{
			Id = Guid.NewGuid(),
			DataSourceDataPoint = dataPoint,
		};

		var result = await LowResolutionDataSync.GetTimeSeriesDataAggregationStoreItem(
			new TimeSeriesAggregationRequest
			{
				DatamartClient = null!,
				Device = new ResourceStoreItem { CreatedOnSeconds = createdOnSeconds },
				InstanceDataPoint = instanceDataPoint,
				DataPointName = "CPU",
				DataPoint = dataPoint,
				StartDateTimeUtc = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
				EndDateTimeUtc = new DateTimeOffset(2026, 2, 1, 0, 0, 0, TimeSpan.Zero),
				ExcludeSdtPeriods = false,
				SdtCache = [],
				Logger = _logger,
			},
			graphData,
			CancellationToken.None);

		return result!;
	}
}
