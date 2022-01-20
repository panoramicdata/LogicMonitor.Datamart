namespace LogicMonitor.Datamart.Test;

public class AgingTests : TestWithOutput
{
	public AgingTests(ITestOutputHelper iTestOutputHelper) : base(iTestOutputHelper) { }

	// Age - retaining 5 days
	[Fact]
	public async void AgeData()
		=> await new DataAging(
				DatamartClient,
				5,
				LoggerFactory)
			.ExecuteAsync(default)
			.ConfigureAwait(false);

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
