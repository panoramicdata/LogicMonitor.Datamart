using Microsoft.EntityFrameworkCore;

namespace LogicMonitor.Datamart.Test;

/// <summary>
/// Verifies that the database connection, schema creation, and migration state are healthy.
/// </summary>
/// <param name="iTestOutputHelper">xUnit output helper for test diagnostics.</param>
public class DatabaseTests(ITestOutputHelper iTestOutputHelper) : TestWithOutput(iTestOutputHelper)
{
	/// <summary>
	/// Ensures the database can be opened and reports no pending migrations.
	/// </summary>
	[Fact]
	public async Task GetDatabase_RunsSuccessFully()
	{
		// Ensure the database is created
		var database = DatamartClient.GetContext().Database;
		database.Should().NotBeNull();

		await database.EnsureCreatedAsync()
			.ConfigureAwait(true);

		// Ensure the database is created
		var pendingMigrations = await database
			.GetPendingMigrationsAsync()
			.ConfigureAwait(true);

		pendingMigrations.Should().BeEmpty();
	}
}
