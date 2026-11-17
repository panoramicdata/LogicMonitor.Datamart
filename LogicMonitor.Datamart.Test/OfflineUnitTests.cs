using LogicMonitor.Api.Collectors;
using LogicMonitor.Datamart.Extensions;
using LogicMonitor.Datamart.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace LogicMonitor.Datamart.Test;

/// <summary>
/// Offline tests of small datamart building blocks that need no database server or LogicMonitor portal.
/// </summary>
public class OfflineUnitTests
{
	private static InMemoryContext CreateContext()
		=> new(new DbContextOptionsBuilder<Context>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options);

	/// <summary>
	/// A new identified API item is added to the set with the observation time stamped on it.
	/// </summary>
	[Fact]
	public async Task AddOrUpdateIdentifiedItem_NewItem_IsAddedAndObserved()
	{
		using var context = CreateContext();
		var observed = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero);

		await context.CollectorGroups.AddOrUpdateIdentifiedItemAsync(
			context,
			new CollectorGroup { Id = 5, Name = "Group A" },
			observed,
			NullLogger.Instance,
			TestContext.Current.CancellationToken);
		await context.SaveChangesAsync(TestContext.Current.CancellationToken);

		var stored = await context.CollectorGroups.SingleAsync(TestContext.Current.CancellationToken);
		stored.LogicMonitorId.Should().Be(5);
		stored.Name.Should().Be("Group A");
		stored.DatamartLastObserved.Should().Be(observed);
		stored.DatamartCreated.Should().NotBe(default);
	}

	/// <summary>
	/// An existing item is updated in place rather than duplicated.
	/// </summary>
	[Fact]
	public async Task AddOrUpdateIdentifiedItem_ExistingItem_IsUpdatedInPlace()
	{
		using var context = CreateContext();
		var first = new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.Zero);
		var second = first.AddHours(1);
		await context.CollectorGroups.AddOrUpdateIdentifiedItemAsync(context, new CollectorGroup { Id = 5, Name = "Old" }, first, NullLogger.Instance, TestContext.Current.CancellationToken);
		await context.SaveChangesAsync(TestContext.Current.CancellationToken);

		await context.CollectorGroups.AddOrUpdateIdentifiedItemAsync(context, new CollectorGroup { Id = 5, Name = "New" }, second, NullLogger.Instance, TestContext.Current.CancellationToken);
		await context.SaveChangesAsync(TestContext.Current.CancellationToken);

		var stored = await context.CollectorGroups.SingleAsync(TestContext.Current.CancellationToken);
		stored.Name.Should().Be("New");
		stored.DatamartLastObserved.Should().Be(second);
	}

	/// <summary>
	/// Saving stamps the creation time once and moves the modified time on every later change.
	/// </summary>
	[Fact]
	public async Task SaveChanges_StampsCreatedOnceAndModifiedEachChange()
	{
		using var context = CreateContext();
		var item = new CollectorGroupStoreItem { LogicMonitorId = 1, Name = "a" };
		context.CollectorGroups.Add(item);
		await context.SaveChangesAsync(TestContext.Current.CancellationToken);
		var created = item.DatamartCreated;
		var modified = item.DatamartLastModified;
		created.Should().NotBe(default);
		modified.Should().Be(created);

		await Task.Delay(20, TestContext.Current.CancellationToken);
		item.Name = "b";
		await context.SaveChangesAsync(TestContext.Current.CancellationToken);

		item.DatamartCreated.Should().Be(created);
		item.DatamartLastModified.Should().BeAfter(modified);
	}

	/// <summary>
	/// Hits and misses accumulate, merge and reset.
	/// </summary>
	[Fact]
	public void CacheStats_CountsMergesAndResets()
	{
		var stats = new CacheStats("test");
		stats.AddHit();
		stats.AddHit();
		stats.AddMiss();
		var other = new CacheStats("other");
		other.AddMiss();

		stats.Add(other);

		stats.Hits.Should().Be(2);
		stats.Misses.Should().Be(2);
		stats.Reset();
		stats.Hits.Should().Be(0);
		stats.Misses.Should().Be(0);
	}

	/// <summary>
	/// A fixed time is returned until it is cleared, and a non-UTC time is refused.
	/// </summary>
	[Fact]
	public void TimeProviderService_FixedTimeIsReturnedUntilCleared()
	{
		var service = new TimeProviderService();
		var fixedTime = new DateTime(2026, 5, 6, 7, 8, 9, DateTimeKind.Utc);

		service.SetDateTimeNow(fixedTime);
		service.UtcNow.Should().Be(fixedTime);
		service.UtcOffsetNow.Should().Be(new DateTimeOffset(fixedTime));

		service.SetDateTimeNow(null);
		service.UtcNow.Should().BeAfter(fixedTime);

		var local = () => service.SetDateTimeNow(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Local));
		local.Should().Throw<ArgumentException>();
	}

	/// <summary>
	/// ExecuteOnce runs the body exactly once.
	/// </summary>
	[Fact]
	public async Task LoopInterval_ExecuteOnce_RunsOnce()
	{
		var loop = new CountingLoop();

		await loop.LoopAsync(LoopIntervals.ExecuteOnce, TestContext.Current.CancellationToken);

		loop.Runs.Should().Be(1);
	}

	/// <summary>
	/// A failing body is logged and does not escape the loop.
	/// </summary>
	[Fact]
	public async Task LoopInterval_FailingBody_IsSwallowed()
	{
		var loop = new CountingLoop { Failure = new InvalidOperationException("boom") };

		await loop.LoopAsync(LoopIntervals.ExecuteOnce, TestContext.Current.CancellationToken);

		loop.Runs.Should().Be(1);
	}

	/// <summary>
	/// Immediate looping repeats until the token is cancelled.
	/// </summary>
	[Fact]
	public async Task LoopInterval_Immediately_RepeatsUntilCancelled()
	{
		using var source = new CancellationTokenSource();
		var loop = new CountingLoop { StopAfter = 3, Source = source };

		await loop.LoopAsync(LoopIntervals.Immediately, source.Token);

		loop.Runs.Should().Be(3);
	}

	private sealed class CountingLoop() : LoopInterval("test", NullLoggerFactory.Instance)
	{
		public int Runs { get; private set; }

		public Exception? Failure { get; init; }

		public int StopAfter { get; init; }

		public CancellationTokenSource? Source { get; init; }

		public override Task ExecuteAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Runs++;
			if (Source is not null && Runs >= StopAfter)
			{
				Source.Cancel();
			}

			return Failure is null ? Task.CompletedTask : Task.FromException(Failure);
		}
	}
}
