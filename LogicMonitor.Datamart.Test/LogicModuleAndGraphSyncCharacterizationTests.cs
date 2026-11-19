using LogicMonitor.Api.LogicModules;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;

namespace LogicMonitor.Datamart.Test;

/// <summary>
/// Characterization tests of the Exchange LogicModule and DataSource graph syncs against a stubbed portal.
/// </summary>
public class LogicModuleAndGraphSyncCharacterizationTests
{
	private static string Json(object value) => JsonConvert.SerializeObject(value);

	private static TestNotificationReceiver Receiver() => new(NullLogger.Instance);

	private static async Task SyncExchangeAsync(DatamartClient client, bool haltOnError = true)
		=> await client.AddOrUpdateLogicModuleUpdates(
			context => context.LogicModuleUpdates,
			haltOnError,
			NullLogger.Instance,
			Receiver(),
			TestContext.Current.CancellationToken);

	private static ExchangeLogicModule Module(string id, string version)
		=> new()
		{
			Id = id,
			Name = "WinCPU",
			DisplayName = "Windows CPU",
			Version = version,
		};

	/// <summary>
	/// Exchange LogicModules are stored once, and a later sync updates them in place.
	/// </summary>
	[Fact]
	public async Task AddOrUpdateLogicModuleUpdates_StoresThenUpdatesInPlace()
	{
		var client = InMemoryDatamart.CreateClient();
		var payload = $"[{Json(Module("x1", "1.0"))},{Json(Module("x2", "2.0"))}]";
		_ = StubbedLogicMonitor.Install(client, _ => payload);

		await SyncExchangeAsync(client);
		payload = $"[{Json(Module("x1", "1.1"))},{Json(Module("x2", "2.0"))}]";
		await SyncExchangeAsync(client);

		using var context = client.GetContext();
		var stored = await context.LogicModuleUpdates.AsNoTracking().OrderBy(m => m.ExchangeId).ToListAsync(TestContext.Current.CancellationToken);
		stored.Select(m => (m.ExchangeId, m.Version)).Should().Equal(("x1", "1.1"), ("x2", "2.0"));
		stored.Should().OnlyContain(m => m.DisplayName == "Windows CPU");
	}

	/// <summary>
	/// A portal failure is rethrown only when halting on error.
	/// </summary>
	[Fact]
	public async Task AddOrUpdateLogicModuleUpdates_PortalFailure_ThrowsOnlyWhenHaltingOnError()
	{
		var client = InMemoryDatamart.CreateClient();
		_ = StubbedLogicMonitor.Install(client, _ => "not json");

		var halting = async () => await SyncExchangeAsync(client, haltOnError: true);
		await halting.Should().ThrowAsync<Exception>();

		await SyncExchangeAsync(client, haltOnError: false);
	}

	private static async Task<DatamartClient> SeededClientAsync(params DataSource[] dataSources)
	{
		var client = InMemoryDatamart.CreateClient();
		using var context = client.GetContext();
		foreach (var dataSource in dataSources)
		{
			var item = DatamartClient.MapperInstance.Map<DataSource, DataSourceStoreItem>(dataSource);
			item.Id = Guid.NewGuid();
			context.DataSources.Add(item);
		}

		await context.SaveChangesAsync(TestContext.Current.CancellationToken);
		return client;
	}

	private static string Graph(string name, string title)
		=> Json(new DataSourceGraph { Name = name, Title = title, VerticalLabel = "v", DataPoints = [], VirtualDataPoints = [], Lines = [] });

	private static async Task UpdateGraphsAsync(DatamartClient client, params DataSource[] apiDataSources)
	{
		using var context = client.GetContext();
		var method = typeof(DatamartClient).GetMethod("UpdateGraphsAsync", BindingFlags.NonPublic | BindingFlags.Instance)!;
		await (Task)method.Invoke(client, [context, new List<DataSource>(apiDataSources), Receiver(), TestContext.Current.CancellationToken])!;
	}

	private static async Task<List<(string Name, string Title, bool IsOverview)>> StoredGraphsAsync(DatamartClient client)
	{
		using var context = client.GetContext();
		var graphs = await context.DataSourceGraphs.AsNoTracking().ToListAsync(TestContext.Current.CancellationToken);
		return [.. graphs.OrderBy(g => g.IsOverview).ThenBy(g => g.Name).Select(g => (g.Name, g.Title, g.IsOverview))];
	}

	/// <summary>
	/// Graphs and overview graphs are added for a DataSource that has none.
	/// </summary>
	[Fact]
	public async Task UpdateGraphs_NewDataSource_AddsGraphsAndOverviewGraphs()
	{
		var dataSource = new DataSource { Id = 1, Name = "WinCPU" };
		var client = await SeededClientAsync(dataSource);
		var graphs = StubbedLogicMonitor.Page(Graph("A", "Title A"), Graph("B", "Title B"));
		var overview = StubbedLogicMonitor.Page(Graph("O", "Overview"));
		_ = StubbedLogicMonitor.Install(client, path => path.Contains("ographs", StringComparison.Ordinal) ? overview : graphs);

		await UpdateGraphsAsync(client, dataSource);

		(await StoredGraphsAsync(client)).Should().Equal(("A", "Title A", false), ("B", "Title B", false), ("O", "Overview", true));
	}

	/// <summary>
	/// Known defect, pinned as it behaves today: a second sync compares each graph kind against all stored graphs, so it deletes every graph.
	/// </summary>
	[Fact]
	public async Task UpdateGraphs_RepeatSyncOfUnchangedData_CurrentlyDeletesAllGraphs()
	{
		var dataSource = new DataSource { Id = 1, Name = "WinCPU" };
		var client = await SeededClientAsync(dataSource);
		var graphs = StubbedLogicMonitor.Page(Graph("A", "Title A"));
		var overview = StubbedLogicMonitor.Page(Graph("O", "Overview"));
		_ = StubbedLogicMonitor.Install(client, path => path.Contains("ographs", StringComparison.Ordinal) ? overview : graphs);
		await UpdateGraphsAsync(client, dataSource);

		await UpdateGraphsAsync(client, dataSource);

		(await StoredGraphsAsync(client)).Should().BeEmpty();
	}
	/// <summary>
	/// A DataSource missing from the database, or a failing portal call, adds nothing and does not throw.
	/// </summary>
	[Fact]
	public async Task UpdateGraphs_MissingDataSourceOrPortalFailure_AddsNothing()
	{
		var known = new DataSource { Id = 1, Name = "WinCPU" };
		var unknown = new DataSource { Id = 2, Name = "Unknown" };
		var client = await SeededClientAsync(known);
		_ = StubbedLogicMonitor.Install(client, path => path.Contains("/datasources/2/", StringComparison.Ordinal)
			? StubbedLogicMonitor.Page(Graph("X", "Title X"))
			: null);

		await UpdateGraphsAsync(client, known, unknown);

		(await StoredGraphsAsync(client)).Should().BeEmpty();
	}
}
