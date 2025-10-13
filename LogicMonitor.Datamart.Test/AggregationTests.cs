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
			.CalculatePercentageAvailability([.. values], "PercentUpTime")
			.Should()
			.Be(expectedUptimePercent);

		LowResolutionDataSync
			.CalculatePercentageAvailability([.. values], "xxx")
			.Should()
			.BeNull();
	}

	// See unpublished up-time algorithm documentation at: https://www.blogger.com/blog/page/edit/preview/289149861252142016/4567219761703676543
	[Theory]
	// Simple sequence with a gap
	[InlineData(83.33d, 1d, 2d, 3d, null, 4d, 5d)]
	// Sequence with counter reset
	[InlineData(100d, 1d, 2d, 3d, 4d, 1d, 2d, 3d)]
	// Sequence with humps / gaps
	[InlineData(71.43d, 1d, 2d, 3d, 10d, 11d)]
	// Multiple gaps and counter reset
	[InlineData(57.14d, 1d, 2d, null, 3d, 10d, null, 11d, 1d, 2d, null, 3d)]
	// Long sequence with multiple jumps and resets
	[InlineData(59.09d, 5d, 6d, 7d, 20d, null, 21d, 22d, 5d, 6d, 7d, 8d, 30d, 31d, null, 32d)]
	public void PercentageUptime2Test(double? expectedUptimePercent, params double?[] values)
	{
		// MS-21394 DataMagic: PercentUpTime availability calculation should calculate downtime based on up-time after no data
		LowResolutionDataSync
			.CalculatePercentageAvailabilityNew([.. values], "PercentUpTime")
			.Should()
			.BeApproximately(expectedUptimePercent, 2);
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