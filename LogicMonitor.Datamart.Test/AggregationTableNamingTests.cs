namespace LogicMonitor.Datamart.Test;

/// <summary>
/// Offline tests of the aggregation table naming and aging rules.
/// </summary>
public class AggregationTableNamingTests
{
	/// <summary>
	/// GetTableName - Period - UsesUtcDate.
	/// </summary>
	[Theory]
	[InlineData("2018-10-11T13:00:00+00:00", "DeviceDataSourceInstanceAggregatedData_20181011")]
	[InlineData("2018-10-11T23:30:00-05:00", "DeviceDataSourceInstanceAggregatedData_20181012")]
	[InlineData("2024-02-29T00:00:00+00:00", "DeviceDataSourceInstanceAggregatedData_20240229")]
	public void GetTableName_Period_UsesUtcDate(string start, string expected)
		=> AggregationWriter
			.GetTableName(DateTimeOffset.Parse(start, CultureInfo.InvariantCulture))
			.Should()
			.Be(expected);

	/// <summary>
	/// DetermineTablesToAge - ReturnsOnlyTablesOlderThanRetention.
	/// </summary>
	[Fact]
	public void DetermineTablesToAge_ReturnsOnlyTablesOlderThanRetention()
	{
		var today = DateTimeOffset.UtcNow.Date;
		var tables = new List<string>
		{
			AggregationWriter.GetTableName(today.AddDays(-10)),
			AggregationWriter.GetTableName(today.AddDays(-3)),
			AggregationWriter.GetTableName(today.AddDays(-2)),
			AggregationWriter.GetTableName(today),
		};

		var toAge = AggregationWriter.DetermineTablesToAge(tables, 2);

		toAge.Should().BeEquivalentTo([tables[0], tables[1]]);
	}

	/// <summary>
	/// DetermineTablesToAge - NothingOldEnough - ReturnsEmpty.
	/// </summary>
	[Fact]
	public void DetermineTablesToAge_NothingOldEnough_ReturnsEmpty()
		=> AggregationWriter
			.DetermineTablesToAge([AggregationWriter.GetTableName(DateTimeOffset.UtcNow)], 5)
			.Should()
			.BeEmpty();
}
