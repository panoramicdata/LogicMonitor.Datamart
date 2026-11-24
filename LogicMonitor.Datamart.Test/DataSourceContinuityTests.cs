using LogicMonitor.Api.LogicModules;
using LogicMonitor.Datamart.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;

namespace LogicMonitor.Datamart.Test;

/// <summary>
/// MS-24125 A DataSource that is cloned, deleted and recreated in LogicMonitor under the same name keeps its data in its own rows,
/// whether the configuration names it or gives its LogicMonitor ID.
/// </summary>
public class DataSourceContinuityTests
{
	private static readonly DateTimeOffset _earlier = new(2026, 5, 1, 0, 0, 0, TimeSpan.Zero);

	private static readonly DateTimeOffset _later = new(2026, 11, 1, 0, 0, 0, TimeSpan.Zero);

	/// <summary>
	/// Dag's reproducer with a name-only configuration: after the recreation, the recreated DataSource gets its own DataPoint and the
	/// original's is left alone.
	/// </summary>
	[Fact]
	public async Task UpdateDataPoints_ADataSourceRecreatedUnderTheSameName_GetsItsOwnDataPoints()
	{
		var client = ClientFor(new DataSourceConfigurationItem { Name = "WinCPU", DataPoints = [CpuBusy()] });
		var original = Api(1, "WinCPU", 7);
		var originalId = await SeedAsync(client, original, _earlier);
		await UpdateDataPointsAsync(client, original);

		var recreated = Api(2, "WinCPU", 17);
		var recreatedId = await SeedAsync(client, recreated, _later);
		await UpdateDataPointsAsync(client, recreated);

		StoredDataPoints(client).Should().BeEquivalentTo([(originalId, 7), (recreatedId, 17)]);
	}

	/// <summary>
	/// Dag's reproducer with the recreated DataSource's LogicMonitor ID configured: its DataPoint is added to it, not merged into the
	/// original's.
	/// </summary>
	[Fact]
	public async Task UpdateDataPoints_ConfiguredWithTheRecreatedDataSourcesId_GetsItsOwnDataPoints()
	{
		var client = ClientFor(new DataSourceConfigurationItem { LogicMonitorId = 2, Name = "WinCPU", DataPoints = [CpuBusy()] });
		var originalId = await SeedAsync(client, Api(1, "WinCPU", 7), _earlier);
		await SeedDataPointAsync(client, originalId, 7);
		var recreated = Api(2, "WinCPU", 17);
		var recreatedId = await SeedAsync(client, recreated, _later);

		await UpdateDataPointsAsync(client, recreated);

		StoredDataPoints(client).Should().BeEquivalentTo([(originalId, 7), (recreatedId, 17)]);
	}

	/// <summary>
	/// The instance sync's DataPoint step adds the configured DataPoint to the DataSource it is given, even when an older DataSource of
	/// the same name already has one.
	/// </summary>
	[Fact]
	public async Task SyncDataSourceDataPoints_ForTheRecreatedDataSource_AddsItsOwnDataPoint()
	{
		var client = ClientFor();
		var originalId = await SeedAsync(client, Api(1, "WinCPU", 7), _earlier);
		await SeedDataPointAsync(client, originalId, 7);
		var recreatedId = await SeedAsync(client, Api(2, "WinCPU", 17), _later);

		using (var context = client.GetContext())
		{
			var recreated = await context.DataSources.SingleAsync(ds => ds.Id == recreatedId, TestContext.Current.CancellationToken);
			var synced = await DatamartClient.SyncDataSourceDataPointsAsync(recreated, context, [CpuBusy()], TestContext.Current.CancellationToken);
			synced.Should().ContainSingle().Which.DataSourceId.Should().Be(recreatedId);
		}

		StoredDataPoints(client).Select(dataPoint => dataPoint.DataSourceId).Should().BeEquivalentTo([originalId, recreatedId]);
	}

	/// <summary>
	/// The recreated DataSource's graphs are stored against it, not against the original of the same name.
	/// </summary>
	[Fact]
	public async Task UpdateGraphs_ForTheRecreatedDataSource_StoresThemAgainstIt()
	{
		var client = ClientFor();
		_ = await SeedAsync(client, Api(1, "WinCPU", 7), _earlier);
		var recreated = Api(2, "WinCPU", 17);
		var recreatedId = await SeedAsync(client, recreated, _later);
		var graphs = StubbedLogicMonitor.Page(Graph("A"));
		_ = StubbedLogicMonitor.Install(client, path => path.Contains("ographs", StringComparison.Ordinal) ? StubbedLogicMonitor.Page() : graphs);

		await UpdateGraphsAsync(client, recreated);

		using var context = client.GetContext();
		var stored = await context.DataSourceGraphs.AsNoTracking().ToListAsync(TestContext.Current.CancellationToken);
		stored.Should().ContainSingle().Which.DataSourceId.Should().Be(recreatedId);
	}

	/// <summary>
	/// A name-only configuration that matches several DataSources uses the one LogicMonitor returned most recently.
	/// </summary>
	[Fact]
	public async Task ResolveDataSource_ByANameSeveralDataSourcesShare_UsesTheMostRecentlyObserved()
	{
		var client = ClientFor();
		_ = await SeedAsync(client, Api(1, "WinCPU", 7), _earlier);
		_ = await SeedAsync(client, Api(2, "WinCPU", 17), _later);

		using var context = client.GetContext();
		var resolved = await client.ResolveDataSourceStoreItemAsync(
			context, new DataSourceConfigurationItem { Name = "WinCPU" }, NullLogger.Instance, TestContext.Current.CancellationToken);

		resolved!.LogicMonitorId.Should().Be(2);
	}

	/// <summary>
	/// A name-only configuration whose DataSource has moved to a new LogicMonitor ID is reported, naming both IDs.
	/// </summary>
	[Fact]
	public async Task ResolveDataSource_ByANameThatHasMovedToANewId_ReportsIt()
	{
		var client = ClientFor();
		_ = await SeedAsync(client, Api(1, "WinCPU", 7), _earlier);
		_ = await SeedAsync(client, Api(2, "WinCPU", 17), _later);
		var receiver = new RecordingNotificationReceiver();

		using var context = client.GetContext();
		var resolved = await client.ResolveDataSourceStoreItemAsync(
			context, new DataSourceConfigurationItem { Name = "WinCPU" }, NullLogger.Instance, receiver, TestContext.Current.CancellationToken);

		resolved!.LogicMonitorId.Should().Be(2);
		receiver.Warnings.Should().ContainSingle()
			.Which.Should().Contain("'WinCPU' is now LogicMonitor ID 2").And.Contain("Earlier data stays under LogicMonitor ID 1");
	}

	/// <summary>
	/// A configuration whose name and LogicMonitor ID disagree is skipped, and the disagreement is reported.
	/// </summary>
	[Fact]
	public async Task UpdateDataPoints_ANameAndIdThatDisagree_AreSkippedAndReported()
	{
		var client = ClientFor(new DataSourceConfigurationItem { LogicMonitorId = 2, Name = "Wrong", DataPoints = [CpuBusy()] });
		var dataSource = Api(2, "WinCPU", 17);
		_ = await SeedAsync(client, dataSource, _later);
		var receiver = new RecordingNotificationReceiver();

		using (var context = client.GetContext())
		{
			await client.UpdateDataPointsAsync(context, [dataSource], receiver, TestContext.Current.CancellationToken);
		}

		StoredDataPoints(client).Should().BeEmpty();
		receiver.Warnings.Should().ContainSingle()
			.Which.Should().Contain("LogicMonitor ID 2 is named 'WinCPU'").And.Contain("names it 'Wrong'");
	}

	/// <summary>
	/// The time series sync's DataSources include one configured by its LogicMonitor ID alone, and each DataSource once, not every
	/// DataSource that shares a configured name.
	/// </summary>
	[Fact]
	public async Task ResolveConfiguredDataSources_AnIdOnlyConfiguration_IsIncluded_AndEachDataSourceOnce()
	{
		var client = ClientFor(
			new DataSourceConfigurationItem { LogicMonitorId = 2, DataPoints = [CpuBusy()] },
			new DataSourceConfigurationItem { Name = "WinCPU", DataPoints = [CpuBusy()] });
		_ = await SeedAsync(client, Api(1, "WinCPU", 7), _earlier);
		_ = await SeedAsync(client, Api(2, "WinCPU", 17), _later);

		using var context = client.GetContext();
		var resolved = await client.ResolveConfiguredDataSourcesAsync(context, NullLogger.Instance, null, TestContext.Current.CancellationToken);

		resolved.Select(dataSource => dataSource.LogicMonitorId).Should().Equal(2);
	}

	private static DataPointConfigurationItem CpuBusy() => new() { Name = "CPUBusyPercent", MeasurementUnit = "%" };

	private static DataSource Api(int id, string name, int dataPointId)
		=> new()
		{
			Id = id,
			Name = name,
			DataSourceDataPoints = [new DataPoint { Id = dataPointId, Name = "CPUBusyPercent", AlertExpression = string.Empty, Description = string.Empty }],
		};

	private static DatamartClient ClientFor(params DataSourceConfigurationItem[] dataSources)
		=> InMemoryDatamart.CreateClient(configuration => configuration.DataSources = [.. dataSources]);

	private static async Task<Guid> SeedAsync(DatamartClient client, DataSource dataSource, DateTimeOffset lastObserved)
	{
		using var context = client.GetContext();
		var storeItem = DatamartClient.MapperInstance.Map<DataSource, DataSourceStoreItem>(dataSource);
		storeItem.Id = Guid.NewGuid();
		storeItem.DatamartLastObserved = lastObserved;
		context.DataSources.Add(storeItem);
		await context.SaveChangesAsync(TestContext.Current.CancellationToken);
		return storeItem.Id;
	}

	private static async Task SeedDataPointAsync(DatamartClient client, Guid dataSourceId, int logicMonitorId)
	{
		using var context = client.GetContext();
		context.DataSourceDataPoints.Add(new DataSourceDataPointStoreItem
		{
			Id = Guid.NewGuid(),
			DataSourceId = dataSourceId,
			Name = "CPUBusyPercent",
			LogicMonitorId = logicMonitorId,
		});
		await context.SaveChangesAsync(TestContext.Current.CancellationToken);
	}

	private static async Task UpdateDataPointsAsync(DatamartClient client, params DataSource[] apiDataSources)
	{
		using var context = client.GetContext();
		await client.UpdateDataPointsAsync(context, [.. apiDataSources], TestContext.Current.CancellationToken);
	}

	private static async Task UpdateGraphsAsync(DatamartClient client, params DataSource[] apiDataSources)
	{
		using var context = client.GetContext();
		var method = typeof(DatamartClient).GetMethod("UpdateGraphsAsync", BindingFlags.NonPublic | BindingFlags.Instance)!;
		await (Task)method.Invoke(client, [context, new List<DataSource>(apiDataSources), new TestNotificationReceiver(NullLogger.Instance), TestContext.Current.CancellationToken])!;
	}

	private static string Graph(string name)
		=> JsonConvert.SerializeObject(new DataSourceGraph { Name = name, Title = name, VerticalLabel = "v", DataPoints = [], VirtualDataPoints = [], Lines = [] });

	private static List<(Guid DataSourceId, int LogicMonitorId)> StoredDataPoints(DatamartClient client)
	{
		using var context = client.GetContext();
		return [.. context.DataSourceDataPoints.AsNoTracking().Select(dataPoint => new { dataPoint.DataSourceId, dataPoint.LogicMonitorId })
			.AsEnumerable()
			.Select(dataPoint => (dataPoint.DataSourceId, dataPoint.LogicMonitorId))];
	}

	private sealed class RecordingNotificationReceiver : INotificationReceiver
	{
		public List<string> Warnings { get; } = [];

		public Task SetStageNameAsync(string stageName, CancellationToken cancellationToken) => Task.CompletedTask;

		public Task SetItemCountAsync(int itemCount, CancellationToken cancellationToken) => Task.CompletedTask;

		public Task SetItemIndexAsync(int itemIndex, CancellationToken cancellationToken) => Task.CompletedTask;

		public Task ReportWarningAsync(string message, CancellationToken cancellationToken)
		{
			Warnings.Add(message);
			return Task.CompletedTask;
		}
	}
}
