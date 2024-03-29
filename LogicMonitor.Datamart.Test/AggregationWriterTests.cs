﻿namespace LogicMonitor.Datamart.Test;

public class AggregationWriterTests(ITestOutputHelper iTestOutputHelper) : TestWithOutput(iTestOutputHelper)
{
	[Fact]
	public async Task ListCreateAndRemoveAggregationTableForAPeriod_ValidRequest_TableExists()
	{
		var testAggregationPeriod = DateTimeOffset.Parse("2018-10-11 13:00", null, DateTimeStyles.AssumeUniversal);
		var tableName = AggregationWriter.GetTableName(testAggregationPeriod);

		// Try and remove the table if it already exists
		if ((await DatamartClient.GetAggregationTablesAsync().ConfigureAwait(true)).Contains(tableName))
		{
			await DatamartClient
				.DropAggregationTableAsync(testAggregationPeriod)
				.ConfigureAwait(true);
		}

		// Assert the table does not exist
		(await DatamartClient.GetAggregationTablesAsync().ConfigureAwait(true)).Should().NotContain(tableName);

		// Create the table
		await DatamartClient
			.EnsureTableExistsAsync(testAggregationPeriod)
			.ConfigureAwait(true);

		// Check the table is there
		(await DatamartClient.GetAggregationTablesAsync().ConfigureAwait(true)).Should().Contain(tableName);

		// Tidy up by removing the table
		await DatamartClient
		  .DropAggregationTableAsync(testAggregationPeriod)
		  .ConfigureAwait(true);

		// Check it's no longer there
		(await DatamartClient.GetAggregationTablesAsync().ConfigureAwait(true)).Should().NotContain(tableName);
	}
}
