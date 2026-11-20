using LogicMonitor.Api.ScheduledDownTimes;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;

namespace LogicMonitor.Datamart.Test;

/// <summary>
/// Characterization tests of historical SDT retrieval and merging against a stubbed portal.
/// </summary>
public class HistoricalSdtCharacterizationTests
{
	private static readonly DateTimeOffset _from = DateTimeOffset.FromUnixTimeSeconds(500);
	private static readonly DateTimeOffset _to = DateTimeOffset.FromUnixTimeSeconds(8000);

	private static string Sdt(string id, long start, long end)
		=> JsonConvert.SerializeObject(new ScheduledDownTimeHistory { Id = id, ApproximateStartEpoch = start, ApproximateEndEpoch = end });

	private static string? Respond(string path)
		=> path switch
		{
			var p when p.Contains("/instances/12/historysdts", StringComparison.Ordinal) => StubbedLogicMonitor.Page(Sdt("i", 1000, 2000)),
			var p when p.Contains("/devicedatasources/11/historysdts", StringComparison.Ordinal) => StubbedLogicMonitor.Page(Sdt("ds", 1500, 2500)),
			var p when p.Contains("/devices/3/historysdts", StringComparison.Ordinal) => StubbedLogicMonitor.Page(Sdt("d", 100, 200), Sdt("d2", 8100, 9000)),
			var p when p.Contains("/groups/7/historysdts", StringComparison.Ordinal) => StubbedLogicMonitor.Page(Sdt("g7", 4000, 5000)),
			var p when p.Contains("/groups/2/historysdts", StringComparison.Ordinal) => StubbedLogicMonitor.Page(),
			var p when p.Contains("/groups/1/historysdts", StringComparison.Ordinal) => StubbedLogicMonitor.Page(Sdt("g1", 6000, 7000)),
			var p when p.EndsWith("/groups/7", StringComparison.Ordinal) => """{"id":7,"parentId":2,"name":"g7"}""",
			var p when p.EndsWith("/groups/2", StringComparison.Ordinal) => """{"id":2,"parentId":1,"name":"g2"}""",
			_ => null,
		};

	private static async Task<(DatamartClient Client, ResourceStoreItem Device, ResourceDataSourceInstanceStoreItem Instance)> SeedAsync(Func<string, string?> responder)
	{
		var client = InMemoryDatamart.CreateClient();
		_ = StubbedLogicMonitor.Install(client, responder);
		using var context = client.GetContext();
		var dds = new ResourceDataSourceStoreItem { Id = Guid.NewGuid(), LogicMonitorId = 11 };
		var ddsi = new ResourceDataSourceInstanceStoreItem { Id = Guid.NewGuid(), LogicMonitorId = 12, DeviceDataSourceId = dds.Id };
		context.DeviceDataSources.Add(dds);
		context.DeviceDataSourceInstances.Add(ddsi);
		await context.SaveChangesAsync(TestContext.Current.CancellationToken);

		return (client, new ResourceStoreItem { LogicMonitorId = 3, DeviceGroupIdsString = "7,x,0" }, ddsi);
	}

	private static Task<List<(long StartTimestampMs, long EndTimestampMs)>?> PeriodsAsync(
		DatamartClient client,
		ResourceStoreItem device,
		ResourceDataSourceInstanceStoreItem? instance,
		Dictionary<string, List<ScheduledDownTimeHistory>> cache)
		=> LowResolutionDataSync.GetHistoricalSdtPeriodsAsync(client, device, instance, _from, _to, cache, NullLogger.Instance, TestContext.Current.CancellationToken);

	/// <summary>
	/// SDTs from every level that overlap the range are merged into non-overlapping millisecond periods.
	/// </summary>
	[Fact]
	public async Task GetHistoricalSdtPeriods_AllLevels_AreFilteredAndMerged()
	{
		var (client, device, instance) = await SeedAsync(Respond);

		var periods = await PeriodsAsync(client, device, instance, []);

		periods.Should().Equal((1_000_000L, 2_500_000L), (4_000_000L, 5_000_000L), (6_000_000L, 7_000_000L));
	}

	/// <summary>
	/// Device and group SDTs are cached, so a second call only refetches the instance and DataSource levels.
	/// </summary>
	[Fact]
	public async Task GetHistoricalSdtPeriods_SecondCallWithCache_SkipsCachedLevels()
	{
		var requests = new List<string>();
		var (client, device, instance) = await SeedAsync(path =>
		{
			requests.Add(path);
			return Respond(path);
		});
		var cache = new Dictionary<string, List<ScheduledDownTimeHistory>>();
		_ = await PeriodsAsync(client, device, instance, cache);
		requests.Clear();

		var periods = await PeriodsAsync(client, device, instance, cache);

		cache.Keys.Should().BeEquivalentTo("Device_3", "DeviceGroup_7", "DeviceGroup_2", "DeviceGroup_1");
		requests.Should().HaveCount(4);
		requests.Should().NotContain(r => r.Contains("/historysdts", StringComparison.Ordinal) && r.Contains("/groups/", StringComparison.Ordinal));
		periods.Should().HaveCount(3);
	}

	/// <summary>
	/// Without an instance, or without its DataSource in the database, no SDT periods are returned and nothing is requested.
	/// </summary>
	[Fact]
	public async Task GetHistoricalSdtPeriods_MissingInstanceOrDataSource_ReturnsEmpty()
	{
		var (client, device, instance) = await SeedAsync(Respond);

		(await PeriodsAsync(client, device, null, [])).Should().BeEmpty();

		instance.DeviceDataSourceId = Guid.NewGuid();
		(await PeriodsAsync(client, device, instance, [])).Should().BeEmpty();
	}

	/// <summary>
	/// A portal failure while reading SDTs returns null.
	/// </summary>
	[Fact]
	public async Task GetHistoricalSdtPeriods_PortalFailure_ReturnsNull()
	{
		var (client, device, instance) = await SeedAsync(_ => "not json");

		(await PeriodsAsync(client, device, instance, [])).Should().BeNull();
	}
}
