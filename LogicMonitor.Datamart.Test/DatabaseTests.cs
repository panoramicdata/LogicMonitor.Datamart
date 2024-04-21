using Microsoft.EntityFrameworkCore;

namespace LogicMonitor.Datamart.Test;

public class DatabaseTests(ITestOutputHelper iTestOutputHelper) : TestWithOutput(iTestOutputHelper)
{
	[Fact]
	public async Task GetDatabase_RunsSuccessFully()
	{
		// Ensure the database is created
		var database = DatamartClient.GetContext().Database;
		database.Should().NotBeNull();

		await database.EnsureCreatedAsync()
			.ConfigureAwait(true);

		// Ensure the database is created
		var databaseName = Configuration.DatabaseName;
		var pendingMigrations = await database
			.GetPendingMigrationsAsync()
			.ConfigureAwait(true);

		pendingMigrations.Should().BeEmpty();
	}
}
