namespace LogicMonitor.Datamart.Test;

public class AggregationWriterTests : TestWithOutput
{
	public AggregationWriterTests(ITestOutputHelper iTestOutputHelper) : base(iTestOutputHelper)
	{
	}

	[Fact]
	public async Task ListCreateAndRemoveAggreationTableForAPeriod_ValidRequest_TableExists()
	{
		var testAggregationPeriod = DateTimeOffset.Parse("2018-10-11 13:00", null, DateTimeStyles.AssumeUniversal);
		var tableName = AggregationWriter.GetTableName(testAggregationPeriod);

		// Try and remove the table if it already exists
		if ((await DatamartClient.GetAggregationTablesAsync().ConfigureAwait(false)).Contains(tableName))
		{
			await DatamartClient
				.DropAggregationTableAsync(testAggregationPeriod)
				.ConfigureAwait(false);
		}

		// Assert the table does not exist
		(await DatamartClient.GetAggregationTablesAsync().ConfigureAwait(false)).Should().NotContain(tableName);

		// Create the table
		await DatamartClient
			.EnsureTableExistsAsync(testAggregationPeriod)
			.ConfigureAwait(false);

		// Check the table is there
		(await DatamartClient.GetAggregationTablesAsync().ConfigureAwait(false)).Should().Contain(tableName);

		// Tidy up by removing the table
		await DatamartClient
		  .DropAggregationTableAsync(testAggregationPeriod)
		  .ConfigureAwait(false);

		// Check it's no longer there
		(await DatamartClient.GetAggregationTablesAsync().ConfigureAwait(false)).Should().NotContain(tableName);
	}
}
