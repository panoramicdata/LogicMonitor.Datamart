using LogicMonitor.Api;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace LogicMonitor.Datamart.Test;

/// <summary>
/// Offline checks that the EF model still matches the committed migration snapshots. No connection is opened.
/// </summary>
public class ModelSnapshotTests
{
	/// <summary>
	/// The current model must produce no schema difference against the latest migration snapshot.
	/// </summary>
	/// <param name="databaseType">The provider whose snapshot is checked.</param>
	[Theory]
	[InlineData(DatabaseType.SqlServer)]
	[InlineData(DatabaseType.Postgres)]
	public void Model_MatchesLatestMigrationSnapshot(DatabaseType databaseType)
	{
		using var client = new DatamartClient(
			new Configuration
			{
				Name = "Contoso",
				DatabaseType = databaseType,
				DatabaseServerName = "db.invalid",
				DatabaseServerPort = 1,
				DatabaseName = "Datamart",
				DatabaseUsername = "user",
				DatabasePassword = "password",
				LogicMonitorClientOptions = new LogicMonitorClientOptions
				{
					Account = "contoso",
					AccessId = "id",
					AccessKey = "key",
				},
			},
			NullLoggerFactory.Instance);
		using var context = client.GetContext();

		context.Database.HasPendingModelChanges().Should().BeFalse();
	}
}