using LogicMonitor.Api;
using LogicMonitor.Api.Alerts;
using LogicMonitor.Api.Filters;
using Microsoft.EntityFrameworkCore;

namespace LogicMonitor.Datamart.Test;

/// <summary>
/// Confirms that the cached alert query translates to SQL for the real providers, without connecting.
/// </summary>
public class CachedAlertQueryTranslationTests
{
	private static CachedAlertQuery FullQuery() => new()
	{
		StartSeconds = 10,
		EndSeconds = 20,
		IncludeInactive = false,
		Skip = 1,
		Take = 5,
		Id = "a1",
		AckFilter = AckFilter.Nonacked,
		MonitorObjectGroups = ["/Prod*"],
		MonitorObjectName = "srv",
		MonitorObjectId = 3,
		AlertTypes = [AlertType.DataSource],
		ResourceTemplateDisplayName = "WinCPU",
		ResourceTemplateId = 4,
		InstanceName = "C:",
		DataPointName = "CPU",
		AlertLevels = [AlertLevel.Error],
		OrderBy = "StartOnSeconds",
		OrderDirection = OrderDirection.Asc,
		SdtFilter = SdtFilter.NonSdt,
		ProblemSignature = "sig",
		IsPercentageAvailability = false,
	};

	/// <summary>
	/// The query with every filter translates for SQL Server and Postgres and contains the group LIKE clauses.
	/// </summary>
	[Theory]
	[InlineData(DatabaseType.SqlServer)]
	[InlineData(DatabaseType.Postgres)]
	public void Apply_AllFilters_TranslatesToSql(DatabaseType type)
	{
		var client = InMemoryDatamart.CreateClient(configuration =>
		{
			configuration.DatabaseType = type;
			configuration.DatabaseUsername = "u";
			configuration.DatabasePassword = "p";
		});
		using var context = client.GetContext();

		var sql = FullQuery().Apply(context.Alerts.AsQueryable()).ToQueryString();

		sql.Should().Contain("LIKE");
		sql.Should().Contain("/Prod%");
		sql.Split("LIKE", StringSplitOptions.None).Length.Should().Be(11);
	}
}
