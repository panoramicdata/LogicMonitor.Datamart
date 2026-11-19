using LogicMonitor.Api.Collectors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace LogicMonitor.Datamart.Test;

/// <summary>
/// Characterization tests of the generic dimension add-or-update against a stubbed portal and an in-memory database.
/// </summary>
public class DimensionAddOrUpdateCharacterizationTests
{
	private static async Task<DatamartClient> SyncCollectorGroupsAsync(string pageJson, bool haltOnError = true)
	{
		var client = InMemoryDatamart.CreateClient();
		_ = StubbedLogicMonitor.Install(client, _ => pageJson);

		await client.AddOrUpdate<CollectorGroup, CollectorGroupStoreItem>(
			context => context.CollectorGroups,
			haltOnError,
			NullLogger.Instance,
			new TestNotificationReceiver(NullLogger.Instance),
			TestContext.Current.CancellationToken);

		return client;
	}

	/// <summary>
	/// Items returned by the portal are stored with the portal identifier and an observed time.
	/// </summary>
	[Fact]
	public async Task AddOrUpdate_PortalItems_AreStored()
	{
		var client = await SyncCollectorGroupsAsync(StubbedLogicMonitor.Page(
			"""{"id":3,"name":"Group three"}""",
			"""{"id":4,"name":"Group four"}"""));

		using var context = client.GetContext();
		var stored = await context.CollectorGroups.AsNoTracking().OrderBy(g => g.LogicMonitorId).ToListAsync(TestContext.Current.CancellationToken);
		stored.Select(g => (g.LogicMonitorId, g.Name)).Should().Equal((3, "Group three"), (4, "Group four"));
		stored.Should().OnlyContain(g => g.DatamartLastObserved > DateTimeOffset.MinValue);
	}

	/// <summary>
	/// A second sync with a changed name updates the row instead of adding one.
	/// </summary>
	[Fact]
	public async Task AddOrUpdate_SecondSync_UpdatesExistingRows()
	{
		var client = InMemoryDatamart.CreateClient();
		var names = new Queue<string>(["First", "Second"]);
		_ = StubbedLogicMonitor.Install(client, _ => StubbedLogicMonitor.Page($$"""{"id":3,"name":"{{names.Peek()}}"}"""));

		for (var i = 0; i < 2; i++)
		{
			await client.AddOrUpdate<CollectorGroup, CollectorGroupStoreItem>(
				context => context.CollectorGroups,
				true,
				NullLogger.Instance,
				new TestNotificationReceiver(NullLogger.Instance),
				TestContext.Current.CancellationToken);
			_ = names.Dequeue();
		}

		using var verify = client.GetContext();
		(await verify.CollectorGroups.AsNoTracking().SingleAsync(TestContext.Current.CancellationToken)).Name.Should().Be("Second");
	}

	/// <summary>
	/// A portal failure is rethrown when halting on error and swallowed otherwise.
	/// </summary>
	[Fact]
	public async Task AddOrUpdate_PortalFailure_ThrowsOnlyWhenHaltingOnError()
	{
		var failing = async () => await SyncCollectorGroupsAsync("not json", haltOnError: true);
		await failing.Should().ThrowAsync<Exception>();

		var client = await SyncCollectorGroupsAsync("not json", haltOnError: false);

		using var context = client.GetContext();
		(await context.CollectorGroups.CountAsync(TestContext.Current.CancellationToken)).Should().Be(0);
	}
}
