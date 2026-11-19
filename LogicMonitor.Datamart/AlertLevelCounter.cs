using LogicMonitor.Datamart.Models;

namespace LogicMonitor.Datamart;

/// <summary>
/// Counts data points at a given alert level for an expression of the form "&gt; 1 2 3".
/// </summary>
internal static class AlertLevelCounter
{
	private sealed record Thresholds(string Symbol, int LevelCount, double? Warning, double? Error, double? Critical);

	private static readonly Dictionary<string, Func<double, double?, bool>> _beyond = new()
	{
		[">"] = (v, l) => v > l,
		[">="] = (v, l) => v >= l,
		["<"] = (v, l) => v < l,
		["<="] = (v, l) => v <= l,
		["="] = (v, l) => v == l,
		["!="] = (v, l) => v != l,
	};

	private static readonly Dictionary<string, Func<double, double?, bool>> _notBeyond = new()
	{
		[">"] = (v, l) => v <= l,
		[">="] = (v, l) => v < l,
		["<"] = (v, l) => v >= l,
		["<="] = (v, l) => v > l,
		["="] = (v, l) => v != l,
		["!="] = (v, l) => v == l,
	};

	internal static int? Count(List<double?> data, string expression, CountAlertLevel level)
	{
		if (string.IsNullOrEmpty(expression))
		{
			return level == CountAlertLevel.Normal ? data.Count(d => d.HasValue) : 0;
		}

		var thresholds = Parse(expression);
		if (level == CountAlertLevel.Normal && thresholds.LevelCount == 0)
		{
			return data.Count(d => d.HasValue);
		}

		var predicate = BuildPredicate(thresholds, level, guardMissingError: false, warningOnlyTerm: false, out var fixedResult);
		return predicate is null ? fixedResult : data.Count(d => d.HasValue && predicate(d.Value));
	}

	internal static int? CountUseSdt(List<TimeSeriesDataPoint> points, string expression, CountAlertLevel level)
	{
		var thresholds = string.IsNullOrEmpty(expression) ? null : Parse(expression);
		if (thresholds is null || (level == CountAlertLevel.Normal && thresholds.LevelCount == 0))
		{
			return level == CountAlertLevel.Normal || thresholds is not null
				? points.Count(dp => dp.Value.HasValue || dp.IsInSdt)
				: 0;
		}

		var predicate = BuildPredicate(thresholds, level, guardMissingError: true, warningOnlyTerm: true, out var fixedResult);
		return predicate is null ? fixedResult : CountMatchingSdt(points, level, predicate);
	}

	private static int CountMatchingSdt(List<TimeSeriesDataPoint> points, CountAlertLevel level, Func<double, bool> predicate)
		=> level == CountAlertLevel.Normal
			? points.Count(dp => dp.IsInSdt || (dp.Value.HasValue && predicate(dp.Value.Value)))
			: points.Count(dp => dp.Value.HasValue && dp.IsInSdt && predicate(dp.Value.Value));

	private static Thresholds Parse(string expression)
	{
		var parts = expression.Split(' ');
		var levels = parts
			.Skip(1)
			.Where(x => double.TryParse(x, out var _))
			.Select(double.Parse)
			.ToArray();

		return new Thresholds(
			parts[0],
			levels.Length,
			levels.Length >= 1 ? levels[0] : null,
			levels.Length >= 2 ? levels[1] : null,
			levels.Length == 3 ? levels[2] : null);
	}

	private static bool IsGuardedOut(Thresholds t, CountAlertLevel level, bool guardMissingError)
		=> level switch
		{
			CountAlertLevel.Critical => t.Critical is null,
			CountAlertLevel.Error => guardMissingError && t.Error is null,
			CountAlertLevel.Warning => t.LevelCount == 0,
			CountAlertLevel.Normal => false,
			_ => throw new ArgumentException($"Unexpected {nameof(CountAlertLevel)} '{level}'"),
		};

	private static Func<double, bool>? BuildPredicate(
		Thresholds t,
		CountAlertLevel level,
		bool guardMissingError,
		bool warningOnlyTerm,
		out int? fixedResult)
	{
		fixedResult = 0;
		if (IsGuardedOut(t, level, guardMissingError))
		{
			return null;
		}

		if (!_beyond.TryGetValue(t.Symbol, out var beyond))
		{
			fixedResult = null;
			return null;
		}

		return LevelPredicate(t, level, beyond, _notBeyond[t.Symbol], warningOnlyTerm);
	}

	private static Func<double, bool> LevelPredicate(
		Thresholds t,
		CountAlertLevel level,
		Func<double, double?, bool> beyond,
		Func<double, double?, bool> notBeyond,
		bool warningOnlyTerm)
	{
		bool Below(double v, double? limit) => limit is null || notBeyond(v, limit);

		return level switch
		{
			CountAlertLevel.Critical => v => beyond(v, t.Critical),
			CountAlertLevel.Error => v => Below(v, t.Critical) && beyond(v, t.Error),
			CountAlertLevel.Warning => v => Below(v, t.Critical) && Below(v, t.Error) && beyond(v, t.Warning),
			_ => v => Below(v, t.Critical) && Below(v, t.Error) && NormalWarningTerm(t, v, warningOnlyTerm, Below, notBeyond),
		};
	}

	// Without warningOnlyTerm, every symbol except ">" tests the warning level only when the error level is null.
	private static bool NormalWarningTerm(
		Thresholds t,
		double v,
		bool warningOnlyTerm,
		Func<double, double?, bool> below,
		Func<double, double?, bool> notBeyond)
		=> warningOnlyTerm || t.Symbol == ">" ? below(v, t.Warning) : t.Error is null || notBeyond(v, t.Warning);
}