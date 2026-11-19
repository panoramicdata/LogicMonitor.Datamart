using LogicMonitor.Api;
using LogicMonitor.Api.Filters;

namespace LogicMonitor.Datamart.Test;

/// <summary>
/// Offline characterization tests of the cached alert query against an in-memory database.
/// </summary>
public class CachedAlertsCharacterizationTests
{
	private const long PeriodStart = 1000;
	private const long PeriodEnd = 2000;

	private static async Task<DatamartClient> CreateSeededClientAsync()
	{
		var client = InMemoryDatamart.CreateClient();
		using var context = client.GetContext();

		var production = new MonitorObjectGroupStoreItem { Id = Guid.NewGuid(), FullPath = "/Prod/Linux", MonitoredObjectType = MonitoredObjectType.Resource };
		var development = new MonitorObjectGroupStoreItem { Id = Guid.NewGuid(), FullPath = "/Dev", MonitoredObjectType = MonitoredObjectType.Resource };
		var website = new MonitorObjectGroupStoreItem { Id = Guid.NewGuid(), FullPath = "/Prod/Web", MonitoredObjectType = MonitoredObjectType.Website };
		context.AddRange(production, development, website);

		context.Alerts.AddRange(
			new AlertStoreItem
			{
				Id = Guid.NewGuid(), LogicMonitorId = "a1", InternalId = "sig1", StartOnSeconds = 1100, EndOnSeconds = 1500, IsCleared = true,
				Severity = (int)AlertLevel.Error, AlertType = AlertType.DataSource, MonitorObjectType = MonitoredObjectType.Resource, MonitorObjectId = 10,
				MonitorObjectName = "srv1", ResourceTemplateId = 5, ResourceTemplateName = "WinCPU", InstanceName = "C:", DataPointName = "CPU",
				MonitorObjectGroup0 = production,
			},
			new AlertStoreItem
			{
				Id = Guid.NewGuid(), LogicMonitorId = "a2", StartOnSeconds = 900, EndOnSeconds = 0, IsCleared = false, Acked = true, InScheduledDownTime = true,
				Severity = (int)AlertLevel.Critical, AlertType = AlertType.EventSource, MonitorObjectType = MonitoredObjectType.Resource, MonitorObjectId = 11,
				MonitorObjectName = "srv2", ResourceTemplateId = 6, ResourceTemplateName = "WinEvents", InstanceName = "D:", DataPointName = "Disk",
				MonitorObjectGroup0 = production,
			},
			new AlertStoreItem
			{
				Id = Guid.NewGuid(), LogicMonitorId = "a3", MonitorObjectName = "srv3", StartOnSeconds = 2500, EndOnSeconds = 0, IsCleared = false,
				Severity = (int)AlertLevel.Warning, AlertType = AlertType.DataSource, MonitorObjectType = MonitoredObjectType.Resource,
			},
			new AlertStoreItem
			{
				Id = Guid.NewGuid(), LogicMonitorId = "a4", MonitorObjectName = "srv4", StartOnSeconds = 500, EndOnSeconds = 800, IsCleared = true,
				Severity = (int)AlertLevel.Warning, AlertType = AlertType.DataSource, MonitorObjectType = MonitoredObjectType.Resource,
			},
			new AlertStoreItem
			{
				Id = Guid.NewGuid(), LogicMonitorId = "a5", MonitorObjectName = "srv5", StartOnSeconds = 500, EndOnSeconds = 3000, IsCleared = true,
				Severity = (int)AlertLevel.Warning, AlertType = AlertType.Website, MonitorObjectType = MonitoredObjectType.Resource,
				MonitorObjectGroup1 = development, MonitorObjectGroup2 = website,
			});
		await context.SaveChangesAsync(TestContext.Current.CancellationToken);

		return client;
	}

	private sealed class AlertQuery
	{
		public bool IncludeInactive { get; init; } = true;
		public int? Skip { get; init; }
		public int? Take { get; init; }
		public string? Id { get; init; }
		public AckFilter AckFilter { get; init; } = AckFilter.All;
		public ICollection<string>? Groups { get; init; }
		public string? MonitorObjectName { get; init; }
		public int? MonitorObjectId { get; init; }
		public ICollection<AlertType>? AlertTypes { get; init; }
		public string? TemplateName { get; init; }
		public int? TemplateId { get; init; }
		public string? InstanceName { get; init; }
		public string? DataPointName { get; init; }
		public ICollection<AlertLevel>? AlertLevels { get; init; }
		public string? OrderBy { get; init; }
		public OrderDirection? OrderDirection { get; init; }
		public SdtFilter SdtFilter { get; init; } = SdtFilter.All;
		public string? ProblemSignature { get; init; }
	}

	private static async Task<List<string>> QueryAsync(AlertQuery? q = null)
	{
		q ??= new AlertQuery();
		var client = await CreateSeededClientAsync();

		var alerts = await client.GetCachedAlertsAsync(
			PeriodStart, PeriodEnd, q.IncludeInactive, q.Skip, q.Take, q.Id, q.AckFilter, q.Groups, q.MonitorObjectName, q.MonitorObjectId,
			q.AlertTypes, q.TemplateName, q.TemplateId, q.InstanceName, q.DataPointName, q.AlertLevels, q.OrderBy, q.OrderDirection,
			q.SdtFilter, q.ProblemSignature);

		return [.. alerts.Select(a => "a" + a.MonitorObjectName[3..])];
	}
	/// <summary>
	/// Active, ending-inside and covering alerts are in the period; later and earlier ones are not.
	/// </summary>
	[Fact]
	public async Task GetCachedAlerts_NoFilters_ReturnsAlertsOverlappingThePeriod()
		=> (await QueryAsync()).Should().BeEquivalentTo("a1", "a2", "a5");

	/// <summary>
	/// Excluding inactive alerts leaves only the uncleared one.
	/// </summary>
	[Fact]
	public async Task GetCachedAlerts_ExcludeInactive_ReturnsOnlyUncleared()
		=> (await QueryAsync(new AlertQuery { IncludeInactive = false })).Should().BeEquivalentTo("a2");

	/// <summary>
	/// Each simple equality filter narrows to the matching alert.
	/// </summary>
	[Fact]
	public async Task GetCachedAlerts_EqualityFilters_NarrowResults()
	{
		(await QueryAsync(new AlertQuery { Id = "a1" })).Should().Equal("a1");
		(await QueryAsync(new AlertQuery { ProblemSignature = "sig1" })).Should().Equal("a1");
		(await QueryAsync(new AlertQuery { MonitorObjectId = 11 })).Should().Equal("a2");
		(await QueryAsync(new AlertQuery { MonitorObjectName = "srv1" })).Should().Equal("a1");
		(await QueryAsync(new AlertQuery { TemplateId = 6 })).Should().Equal("a2");
		(await QueryAsync(new AlertQuery { TemplateName = "WinCPU" })).Should().Equal("a1");
		(await QueryAsync(new AlertQuery { InstanceName = "D:" })).Should().Equal("a2");
		(await QueryAsync(new AlertQuery { DataPointName = "CPU" })).Should().Equal("a1");
	}

	/// <summary>
	/// Alert levels and alert types are matched as sets.
	/// </summary>
	[Fact]
	public async Task GetCachedAlerts_LevelAndTypeFilters_MatchSets()
	{
		(await QueryAsync(new AlertQuery { AlertLevels = [AlertLevel.Critical, AlertLevel.Error] })).Should().BeEquivalentTo("a1", "a2");
		(await QueryAsync(new AlertQuery { AlertLevels = [AlertLevel.Warning] })).Should().BeEquivalentTo("a5");
		(await QueryAsync(new AlertQuery { AlertTypes = [AlertType.DataSource] })).Should().Equal("a1");
		(await QueryAsync(new AlertQuery { AlertTypes = [AlertType.EventSource, AlertType.Website] })).Should().BeEquivalentTo("a2", "a5");
	}

	/// <summary>
	/// A wildcard group matches any of the ten group slots of the right object type; an empty collection is no filter.
	/// </summary>
	[Fact]
	public async Task GetCachedAlerts_GroupFilter_UsesPrefixLikeOnMatchingObjectType()
	{
		(await QueryAsync(new AlertQuery { Groups = ["/Prod*"] })).Should().BeEquivalentTo("a1", "a2");
		(await QueryAsync(new AlertQuery { Groups = ["/Prod/Linux"] })).Should().BeEquivalentTo("a1", "a2");
		(await QueryAsync(new AlertQuery { Groups = ["/Dev"] })).Should().Equal("a5");
		(await QueryAsync(new AlertQuery { Groups = ["/Nothing"] })).Should().BeEmpty();
		(await QueryAsync(new AlertQuery { Groups = [] })).Should().BeEquivalentTo("a1", "a2", "a5");
	}

	/// <summary>
	/// A group of a different object type than the alert's does not match.
	/// </summary>
	[Fact]
	public async Task GetCachedAlerts_GroupOfOtherObjectType_DoesNotMatch()
		=> (await QueryAsync(new AlertQuery { Groups = ["/Prod/Web"] })).Should().BeEmpty();

	/// <summary>
	/// SDT and acknowledgement filters select by flag.
	/// </summary>
	[Fact]
	public async Task GetCachedAlerts_SdtAndAckFilters_SelectByFlag()
	{
		(await QueryAsync(new AlertQuery { SdtFilter = SdtFilter.Sdt })).Should().Equal("a2");
		(await QueryAsync(new AlertQuery { SdtFilter = SdtFilter.NonSdt })).Should().BeEquivalentTo("a1", "a5");
		(await QueryAsync(new AlertQuery { AckFilter = AckFilter.Acked })).Should().Equal("a2");
		(await QueryAsync(new AlertQuery { AckFilter = AckFilter.Nonacked })).Should().BeEquivalentTo("a1", "a5");
	}

	/// <summary>
	/// Ordering by start time supports both directions, and skip and take page the ordered result.
	/// </summary>
	[Fact]
	public async Task GetCachedAlerts_OrderAndPage_AppliesInOrder()
	{
		(await QueryAsync(new AlertQuery { OrderBy = "StartOnSeconds", OrderDirection = OrderDirection.Asc })).Should().Equal("a5", "a2", "a1");
		(await QueryAsync(new AlertQuery { OrderBy = "StartOnSeconds", OrderDirection = OrderDirection.Desc })).Should().Equal("a1", "a2", "a5");
		(await QueryAsync(new AlertQuery { OrderBy = "StartOnSeconds", OrderDirection = OrderDirection.Asc, Skip = 1, Take = 1 })).Should().Equal("a2");
	}

	/// <summary>
	/// Only ordering by StartOnSeconds is supported.
	/// </summary>
	[Fact]
	public async Task GetCachedAlerts_UnsupportedOrder_Throws()
	{
		var act = () => QueryAsync(new AlertQuery { OrderBy = "Severity", OrderDirection = OrderDirection.Asc });

		await act.Should().ThrowAsync<NotSupportedException>();
	}

	/// <summary>
	/// The percentage availability projection returns the narrow set of fields.
	/// </summary>
	[Fact]
	public async Task GetCachedAlerts_PercentageAvailability_ReturnsProjectedAlerts()
	{
		var client = await CreateSeededClientAsync();

		var alerts = await client.GetCachedAlertsAsync(
			PeriodStart, PeriodEnd, true, null, null, null, AckFilter.All, null, null, null, null, null, null, null, null, null, null, null,
			SdtFilter.All, null, true);

		alerts.Should().HaveCount(3);
		alerts.Select(a => a.InstanceName).Should().BeEquivalentTo("C:", "D:", string.Empty);
	}
}
