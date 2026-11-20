using System.Text.RegularExpressions;
using LogicMonitor.Api.Data;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;

namespace LogicMonitor.Datamart.Test;

/// <summary>
/// Characterization tests of graph data fetching, throttling detection and chunking against a stubbed portal.
/// </summary>
public partial class GraphDataFetchCharacterizationTests
{
	private static readonly DateTimeOffset _start = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

	[GeneratedRegex(@"start=(\d+)&end=(\d+)")]
	private static partial Regex RangeRegex();

	private static string GraphJson(string path, Func<double, long> stepSecondsForDays)
	{
		var match = RangeRegex().Match(path);
		var from = long.Parse(match.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture);
		var to = long.Parse(match.Groups[2].Value, System.Globalization.CultureInfo.InvariantCulture);
		var step = stepSecondsForDays((to - from) / 86400d);
		var seconds = Enumerable.Range(0, (int)((to - from) / step)).Select(i => from + (i * step)).ToList();
		return JsonConvert.SerializeObject(new GraphData
		{
			Status = (LogicMonitor.Api.Status)200,
			TimeStamps = [.. seconds.Select(s => s * 1000)],
			Lines = [new Line { Legend = "CPU", Data = [.. seconds.Select(s => (double?)s)] }],
		});
	}

	private static (DatamartClient Client, List<string> Requests) Client(Func<double, long> stepSecondsForDays)
	{
		var client = InMemoryDatamart.CreateClient();
		var requests = StubbedLogicMonitor.Install(client, path => GraphJson(path, stepSecondsForDays));
		return (client, requests);
	}

	/// <summary>
	/// A single fetch asks for zoomed graph data of the instance and returns the portal's points.
	/// </summary>
	[Fact]
	public async Task GetGraphData_ReturnsPortalData()
	{
		var (client, requests) = Client(_ => 3600);

		var graph = await LowResolutionDataSync.GetGraphDataAsync(client, 5, _start, _start.AddDays(1), NullLogger.Instance, TestContext.Current.CancellationToken);

		requests.Should().ContainSingle().Which.Should().Contain("/devicedatasourceinstances/5/graphs/-1/data?time=zoom");
		graph.TimeStamps.Should().HaveCount(24);
		graph.Lines.Should().ContainSingle().Which.Legend.Should().Be("CPU");
	}

	/// <summary>
	/// A portal failure is rethrown.
	/// </summary>
	[Fact]
	public async Task GetGraphData_PortalFailure_Throws()
	{
		var client = InMemoryDatamart.CreateClient();
		_ = StubbedLogicMonitor.Install(client, _ => "not json");

		var act = () => LowResolutionDataSync.GetGraphDataAsync(client, 5, _start, _start.AddDays(1), NullLogger.Instance, TestContext.Current.CancellationToken);

		await act.Should().ThrowAsync<Exception>();
	}

	/// <summary>
	/// Hourly data is returned after one request, with or without auto-chunking.
	/// </summary>
	[Theory]
	[InlineData(false)]
	[InlineData(true)]
	public async Task AutoChunking_HourlyData_IsReturnedAfterOneRequest(bool enableAutoChunking)
	{
		var (client, requests) = Client(_ => 3600);

		var graph = await LowResolutionDataSync.GetGraphDataWithAutoChunkingAsync(
			client, 5, _start, _start.AddDays(10), enableAutoChunking, 31, NullLogger.Instance, TestContext.Current.CancellationToken);

		requests.Should().HaveCount(1);
		graph.TimeStamps.Should().HaveCount(240);
	}

	/// <summary>
	/// Throttled data is returned as is when auto-chunking is off.
	/// </summary>
	[Fact]
	public async Task AutoChunking_ThrottledAndDisabled_ReturnsThrottledData()
	{
		var (client, requests) = Client(_ => 7200);

		var graph = await LowResolutionDataSync.GetGraphDataWithAutoChunkingAsync(
			client, 5, _start, _start.AddDays(10), false, 31, NullLogger.Instance, TestContext.Current.CancellationToken);

		requests.Should().HaveCount(1);
		graph.TimeStamps.Should().HaveCount(120);
	}

	/// <summary>
	/// Throttled data with auto-chunking on is refetched in shrinking chunks until hourly resolution is restored.
	/// </summary>
	[Fact]
	public async Task AutoChunking_ThrottledAndEnabled_ShrinksChunksAndMerges()
	{
		var (client, requests) = Client(days => days > 20 ? 7200 : 3600);

		var graph = await LowResolutionDataSync.GetGraphDataWithAutoChunkingAsync(
			client, 5, _start, _start.AddDays(62), true, 31, NullLogger.Instance, TestContext.Current.CancellationToken);

		requests.Should().HaveCount(7);
		graph.TimeStamps.Should().HaveCount(62 * 24);
		graph.TimeStamps.Should().BeInAscendingOrder();
		graph.Lines.Should().ContainSingle().Which.Data.Should().HaveCount(62 * 24);
	}

	/// <summary>
	/// Data still throttled at the smallest chunk size is used as it is.
	/// </summary>
	[Fact]
	public async Task AdaptiveChunked_AlwaysThrottled_UsesAvailableData()
	{
		var (client, requests) = Client(_ => 7200);

		var graph = await LowResolutionDataSync.GetGraphDataAdaptiveChunkedAsync(
			client, 5, _start, _start.AddDays(14), 14, NullLogger.Instance, TestContext.Current.CancellationToken);

		requests.Should().HaveCount(3);
		graph.TimeStamps.Should().HaveCount(14 * 12);
	}

	/// <summary>
	/// Fixed-size chunking fetches each chunk and concatenates the lines.
	/// </summary>
	[Fact]
	public async Task Chunked_FetchesEachChunkAndConcatenates()
	{
		var (client, requests) = Client(_ => 3600);

		var graph = await LowResolutionDataSync.GetGraphDataChunkedAsync(
			client, 5, _start, _start.AddDays(70), 31, NullLogger.Instance, TestContext.Current.CancellationToken);

		requests.Should().HaveCount(3);
		graph.TimeStamps.Should().HaveCount(70 * 24);
		graph.Lines.Should().ContainSingle().Which.Data.Should().HaveCount(70 * 24);
	}
}
