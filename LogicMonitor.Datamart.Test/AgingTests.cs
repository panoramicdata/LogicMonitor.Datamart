namespace LogicMonitor.Datamart.Test;

/// <summary>
/// Verifies retention aging behavior for historical aggregation tables.
/// </summary>
/// <param name="iTestOutputHelper">xUnit output helper used for diagnostics.</param>
public class AgingTests(ITestOutputHelper iTestOutputHelper) : TestWithOutput(iTestOutputHelper)
{

	// Age - retaining 5 days
	/// <summary>
	/// Executes table aging with a five-day retention window.
	/// </summary>
	[Fact]
	public async Task AgeData()
		=> await new DataAgeing(
				DatamartClient,
				5,
				LoggerFactory)
			.ExecuteAsync(default)
			.ConfigureAwait(true);

	/// <summary>
	/// Verifies only tables older than the retention boundary are selected for removal.
	/// </summary>
	[Fact]
	public void DetermineTablesToAge_GivenList_CorrectResult()
	{
		var today = DateTimeOffset.UtcNow.Date;
		var existingTables = new List<string>
			{
				AggregationWriter.GetTableName(today),
				AggregationWriter.GetTableName(today.AddDays(-1)),
				AggregationWriter.GetTableName(today.AddDays(-2)),
				AggregationWriter.GetTableName(today.AddDays(-3)),
				AggregationWriter.GetTableName(today.AddDays(-4)),
				AggregationWriter.GetTableName(today.AddDays(-5)),
				AggregationWriter.GetTableName(today.AddDays(-6)),
			};

		var tablesToRemove = AggregationWriter.DetermineTablesToAge(existingTables, 3);

		var expectedResult = new List<string>
			{
				AggregationWriter.GetTableName(today.AddDays(-4)),
				AggregationWriter.GetTableName(today.AddDays(-5)),
				AggregationWriter.GetTableName(today.AddDays(-6))
			};

		tablesToRemove.Should().BeEquivalentTo(expectedResult);
	}
}
