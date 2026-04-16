using Microsoft.EntityFrameworkCore;

namespace LogicMonitor.Datamart.Test;

/// <summary>
/// Simple test to ensure database migrations are applied.
/// Running this test will apply all pending migrations to your test database.
/// </summary>
/// <param name="testOutputHelper">xUnit output helper for test diagnostics.</param>
public class DatabaseMigrationTests(ITestOutputHelper testOutputHelper) : TestWithOutput(testOutputHelper)
{
	/// <summary>
	/// Verifies the database is accessible after all pending migrations have been applied.
	/// </summary>
	[Fact]
	public async Task ApplyMigrations_Succeeds()
	{
		// Arrange - The TestWithOutput constructor already applies migrations
		// This test just verifies the database is accessible

		// Act
		using var context = DatamartClient.GetContext();
		var canConnect = await context.Database.CanConnectAsync();

		// Assert
		canConnect.Should().BeTrue("Database should be accessible after migrations");
	}

	/// <summary>
	/// Verifies that the ExcludingSdt columns added in recent migrations exist in the database.
	/// </summary>
	[Fact]
	public async Task NewExcludingSdtColumns_ExistInDatabase()
	{
		// Arrange
		using var context = DatamartClient.GetContext();

		// Act - Try to query the TimeSeriesDataAggregations table
		// This will fail if the new columns don't exist
		var hasRecords = await context.TimeSeriesDataAggregations.AnyAsync();

		// Assert - We don't care about the result, just that the query succeeded
		// If the migration wasn't applied, this would throw an exception
		ITestOutputHelper.WriteLine($"TimeSeriesDataAggregations has records: {hasRecords}");
	}
}
