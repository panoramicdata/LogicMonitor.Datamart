using System.Linq;

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
			.CalculatePercentageAvailability(values.ToList(), "PercentUpTime")
			.Should()
			.Be(expectedUptimePercent);

		LowResolutionDataSync
			.CalculatePercentageAvailability(values.ToList(), "xxx")
			.Should()
			.BeNull();
	}

	/// <summary>
	/// This test aims to have a realistic number of data points
	/// The shape of the input data is as follows:
	/// |/__/|
	/// So a ramp from 0 to 5000, then a flat set of nulls representing 50 of the time, then a ramp for 0 back up to 5000
	/// </summary>
	[Fact]
	public void PercentageUptimeTest_LargeDatapointCount()
	{
		var values = new List<double?>();
		for (var i = 0; i < 5000; i++)
		{
			values.Add(i);
		}

		for (var i = 0; i < 10000; i++)
		{
			values.Add(null);
		}

		for (var i = 0; i < 5000; i++)
		{
			values.Add(i);
		}

		LowResolutionDataSync
			.CalculatePercentageAvailability(values, "PercentUpTime")
			.Should()
			.Be(50);
	}
}