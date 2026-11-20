namespace LogicMonitor.Datamart;

/// <summary>
/// Calculates PercentUpTime availability from the values of an uptime counter.
/// </summary>
internal static class AvailabilityCalculator
{
	private const int DeviceDownTimeWindowSeconds = 3000;

	private const string PercentUpTime = "PercentUpTime";

	private sealed class UptimeCounter
	{
		private double _previous = double.NaN;
		private int _ambiguous;
		private bool _isAmbiguous = true;

		public int UpTime { get; private set; }

		public int DownTime { get; private set; }

		public void Add(double? value)
		{
			if (value is double current)
			{
				_previous = current;
				UpTime += _ambiguous + 1;
				_ambiguous = 0;
				_isAmbiguous = false;
			}
			else
			{
				AddMissing();
			}
		}

		private void AddMissing()
		{
			if (!double.IsNaN(_previous) && _previous >= DeviceDownTimeWindowSeconds)
			{
				UpTime++;
			}
			else if (_isAmbiguous)
			{
				_ambiguous++;
			}
			else
			{
				DownTime++;
			}
		}
	}

	internal static double? Calculate(List<double?> values, string calculation)
	{
		if (!IsPercentUpTime(calculation) || values.Count == 0)
		{
			return null;
		}

		var counter = new UptimeCounter();
		foreach (var value in Enumerable.Reverse(values))
		{
			counter.Add(value);
		}

		return counter.UpTime + counter.DownTime == 0
			? null
			: 100d * counter.UpTime / (counter.UpTime + counter.DownTime);
	}

	internal static double? CalculateGradientBased(List<double?> values, string calculation)
	{
		if (!IsPercentUpTime(calculation) || values.Count == 0 || values.All(v => v is null))
		{
			return null;
		}

		var gradient = CalculateGradient(values);
		if (Math.Abs(gradient) < 1e-6)
		{
			return null;
		}

		var (totalDowntime, totalTime) = CalculateDowntime(values, gradient);

		return totalTime == 0 ? 0d : 100d * (totalTime - totalDowntime) / totalTime;
	}

	internal static double? CalculateLegacy(double?[] values, string calculation)
	{
		if (!IsPercentUpTime(calculation))
		{
			return null;
		}

		// Reverse the array to process from oldest to newest
		Array.Reverse(values);

		var previous = double.NaN;
		var upTime = 0;
		var downTime = 0;
		foreach (var value in values)
		{
			if (value is double current)
			{
				previous = current;
				upTime++;
			}
			else if (double.IsNaN(previous) || previous < DeviceDownTimeWindowSeconds)
			{
				downTime++;
			}
			else
			{
				upTime++;
			}
		}

		return upTime + downTime == 0 ? 100 : 100d * upTime / (upTime + downTime);
	}

	private static bool IsPercentUpTime(string calculation)
		=> !string.IsNullOrWhiteSpace(calculation) && calculation == PercentUpTime;

	private static double CalculateGradient(List<double?> values)
	{
		double gradientSum = 0;
		var gradientCount = 0;
		double? previous = null;
		foreach (var value in values.OfType<double>())
		{
			if (previous.HasValue && value > previous.Value)
			{
				gradientSum += value - previous.Value;
				gradientCount++;
			}

			previous = value;
		}

		return gradientCount > 0 ? gradientSum / gradientCount : 0;
	}

	private static (int TotalDowntime, int TotalExpected) CalculateDowntime(List<double?> values, double gradient)
	{
		var totalDowntime = 0;
		var totalExpected = 0;
		double? previous = null;

		foreach (var value in values)
		{
			if (value is not double current)
			{
				totalDowntime++;
				totalExpected++;
				continue;
			}

			if (previous.HasValue)
			{
				var (downtime, expected) = GapBetween(previous.Value, current, gradient);
				totalDowntime += downtime;
				totalExpected += expected;
			}

			previous = current;
		}

		return (totalDowntime, totalExpected + values.Count(v => v is double));
	}

	// A smaller value is a counter reset, which contributes no gap.
	private static (int Downtime, int Expected) GapBetween(double previous, double current, double gradient)
	{
		if (current <= previous || Math.Abs(current - (previous + gradient)) <= gradient * 1.1)
		{
			return (0, 0);
		}

		var gap = (int)Math.Round((current - previous) / gradient) - 1;
		if (gap <= 0)
		{
			return (0, 0);
		}

		var projectedStart = current - gap * gradient - gradient;
		return (gap + (int)Math.Round((previous - projectedStart) / gradient), gap);
	}
}
