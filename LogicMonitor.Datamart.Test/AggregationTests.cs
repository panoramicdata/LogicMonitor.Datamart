namespace LogicMonitor.Datamart.Test;
public class AggregationTests
{
	[Theory]
	[InlineData(50d, null, null, 0d, 60d)]
	[InlineData(null, null, null, null, null)]
	[InlineData(100d, 5000d, 5060d, null, null)]
	[InlineData(100d, null, null, 5060d, 5120d)]

	[InlineData(100d, null, 5000d, null, 5120d, null)]
	[InlineData(75d, null, 0d, 60d, 120d)]
	[InlineData(75d, 5000d, null, 60d, 120d)]
	[InlineData(100d, null, 5000d, 5060d, null)]

	[InlineData(50d, null, 0d, null, 120d)]
	[InlineData(100d, 5000d, null, 5120d, null)]
	[InlineData(100d, 0d, 60d, 120d, 180d)]
	[InlineData(100d, 5000d, 5060d, 5120d, 5180d)]
	public void PercentageUptimeTest(double? expectedUptimePercent, params double?[] values)
	{
		LowResolutionDataSync
			.CalculatePercentageAvailability(values, "PercentUpTime")
			.Should()
			.Be(expectedUptimePercent);

		LowResolutionDataSync
			.CalculatePercentageAvailability(values, "xxx")
			.Should()
			.BeNull();
	}
}
