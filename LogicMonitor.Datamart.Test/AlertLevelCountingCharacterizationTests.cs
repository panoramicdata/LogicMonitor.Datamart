namespace LogicMonitor.Datamart.Test;

/// <summary>
/// Offline characterization tests pinning the current alert level counting behaviour, quirks included.
/// </summary>
public class AlertLevelCountingCharacterizationTests
{
	private static readonly List<double?> _values =
	[
		1, 2, 3, 4, 5, 6, 7, 8, 9, 10, null
	];

	private static readonly List<TimeSeriesDataPoint> _points =
	[
		Point(1, false), Point(2, false), Point(3, true), Point(4, false), Point(5, false), Point(6, false),
		Point(7, false), Point(8, true), Point(9, false), Point(10, false), Point(null, true), Point(null, false)
	];

	private static TimeSeriesDataPoint Point(double? value, bool isInSdt)
		=> new() { Value = value, IsInSdt = isInSdt, Timestamp = 0 };

	private static CountAlertLevel Level(string level) => Enum.Parse<CountAlertLevel>(level);

	private static int? Count(string expression, string level)
		=> (int?)InvokePrivate("CountAtAlertLevel", _values, expression, Level(level));

	private static int? CountSdt(string expression, string level)
		=> (int?)InvokePrivate("CountAtAlertLevelUseSdt", _points, expression, Level(level));

	private static object? InvokePrivate(string name, params object?[] args)
	{
		var method = typeof(LowResolutionDataSync).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static)
			?? throw new InvalidOperationException($"Method {name} not found");
		return method.Invoke(null, args);
	}

	/// <summary>
	/// Pins current behaviour: .
	/// </summary>
	[Theory]
	[InlineData("> 5 7 9", "Normal", 5)]
	[InlineData("> 5 7 9", "Warning", 2)]
	[InlineData("> 5 7 9", "Error", 2)]
	[InlineData("> 5 7 9", "Critical", 1)]
	[InlineData(">= 5 7 9", "Normal", 4)]
	[InlineData(">= 5 7 9", "Warning", 2)]
	[InlineData(">= 5 7 9", "Error", 2)]
	[InlineData(">= 5 7 9", "Critical", 2)]
	[InlineData("< 5 3 1", "Normal", 6)]
	[InlineData("< 5 3 1", "Warning", 2)]
	[InlineData("< 5 3 1", "Error", 2)]
	[InlineData("< 5 3 1", "Critical", 0)]
	[InlineData("<= 5 3 1", "Normal", 5)]
	[InlineData("<= 5 3 1", "Warning", 2)]
	[InlineData("<= 5 3 1", "Error", 2)]
	[InlineData("<= 5 3 1", "Critical", 1)]
	[InlineData("= 5 7 9", "Normal", 7)]
	[InlineData("= 5 7 9", "Warning", 1)]
	[InlineData("= 5 7 9", "Error", 1)]
	[InlineData("= 5 7 9", "Critical", 1)]
	[InlineData("!= 5 7 9", "Normal", 0)]
	[InlineData("!= 5 7 9", "Warning", 0)]
	[InlineData("!= 5 7 9", "Error", 1)]
	[InlineData("!= 5 7 9", "Critical", 9)]
	[InlineData("> 5 7", "Normal", 5)]
	[InlineData("> 5 7", "Warning", 2)]
	[InlineData("> 5 7", "Error", 3)]
	[InlineData("> 5 7", "Critical", 0)]
	[InlineData("> 5", "Normal", 5)]
	[InlineData("> 5", "Warning", 5)]
	[InlineData("> 5", "Error", 0)]
	[InlineData("> 5", "Critical", 0)]
	[InlineData("> 5 7 9 11", "Normal", 5)]
	[InlineData("> 5 7 9 11", "Warning", 2)]
	[InlineData("> 5 7 9 11", "Error", 3)]
	[InlineData("> 5 7 9 11", "Critical", 0)]
	public void CountAtAlertLevel_WithThresholds_CountsEachLevel(string expression, string level, int expected)
		=> Count(expression, level).Should().Be(expected);

	// The two-sided Normal branches compare the warning level only when the error level is null.
	/// <summary>
	/// Pins current behaviour: .
	/// </summary>
	[Theory]
	[InlineData(">= 5", "Normal", 10)]
	[InlineData(">= 5", "Warning", 6)]
	[InlineData(">= 5", "Error", 0)]
	[InlineData("< 5", "Normal", 10)]
	[InlineData("<= 5", "Normal", 10)]
	[InlineData("= 5", "Normal", 10)]
	[InlineData("!= 5", "Normal", 10)]
	[InlineData("!= 5", "Error", 10)]
	public void CountAtAlertLevel_SingleThreshold_PinsErrorLevelNullQuirks(string expression, string level, int expected)
		=> Count(expression, level).Should().Be(expected);

	/// <summary>
	/// Pins current behaviour: .
	/// </summary>
	[Theory]
	[InlineData("~ 1 2 3", "Normal")]
	[InlineData("~ 1 2 3", "Warning")]
	[InlineData("~ 1 2 3", "Error")]
	[InlineData("~ 1 2 3", "Critical")]
	public void CountAtAlertLevel_UnknownOperator_ReturnsNull(string expression, string level)
		=> Count(expression, level).Should().BeNull();

	/// <summary>
	/// Pins current behaviour: .
	/// </summary>
	[Theory]
	[InlineData("", "Normal", 10)]
	[InlineData("", "Warning", 0)]
	[InlineData("", "Error", 0)]
	[InlineData("", "Critical", 0)]
	[InlineData("> abc", "Normal", 10)]
	[InlineData("> abc", "Warning", 0)]
	[InlineData("> abc", "Error", 0)]
	[InlineData("> abc", "Critical", 0)]
	public void CountAtAlertLevel_NoThresholds_CountsOnlyNormal(string expression, string level, int expected)
		=> Count(expression, level).Should().Be(expected);

	/// <summary>
	/// Pins current behaviour: .
	/// </summary>
	[Fact]
	public void CountAtAlertLevel_UndefinedLevel_Throws()
	{
		var act = () => InvokePrivate("CountAtAlertLevel", _values, "> 1", (CountAlertLevel)99);
		act.Should().Throw<TargetInvocationException>().WithInnerException<ArgumentException>();
	}

	/// <summary>
	/// Pins current behaviour: .
	/// </summary>
	[Fact]
	public void CountAtAlertLevel_EmptyData_ReturnsZero()
		=> ((int?)InvokePrivate("CountAtAlertLevel", new List<double?>(), "> 1 2 3", CountAlertLevel.Normal)).Should().Be(0);

	/// <summary>
	/// Pins current behaviour: .
	/// </summary>
	[Theory]
	[InlineData("", "Normal", 11)]
	[InlineData("", "Warning", 0)]
	[InlineData("", "Error", 0)]
	[InlineData("", "Critical", 0)]
	[InlineData("> abc", "Normal", 11)]
	[InlineData("> abc", "Warning", 0)]
	[InlineData("> 5 7 9", "Normal", 7)]
	[InlineData("> 5 7 9", "Warning", 0)]
	[InlineData("> 5 7 9", "Error", 1)]
	[InlineData("> 5 7 9", "Critical", 0)]
	[InlineData(">= 5 7 9", "Normal", 6)]
	[InlineData(">= 5 7 9", "Warning", 0)]
	[InlineData(">= 5 7 9", "Error", 1)]
	[InlineData(">= 5 7 9", "Critical", 0)]
	[InlineData("< 5 3 1", "Normal", 8)]
	[InlineData("< 5 3 1", "Warning", 1)]
	[InlineData("< 5 3 1", "Error", 0)]
	[InlineData("< 5 3 1", "Critical", 0)]
	[InlineData("= 5 7 9", "Normal", 8)]
	[InlineData("= 5 7 9", "Warning", 0)]
	[InlineData("= 5 7 9", "Error", 0)]
	[InlineData("= 5 7 9", "Critical", 0)]
	[InlineData("> 5 7", "Normal", 7)]
	[InlineData("> 5 7", "Warning", 0)]
	[InlineData("> 5 7", "Error", 1)]
	[InlineData("> 5 7", "Critical", 0)]
	[InlineData("> 5", "Normal", 7)]
	[InlineData("> 5", "Warning", 1)]
	[InlineData("> 5", "Error", 0)]
	[InlineData("> 5", "Critical", 0)]
	[InlineData(">= 5", "Normal", 6)]
	[InlineData(">= 5", "Warning", 1)]
	[InlineData(">= 5", "Error", 0)]
	public void CountAtAlertLevelUseSdt_CountsOnlyPointsInSdtForAlertLevels(string expression, string level, int expected)
		=> CountSdt(expression, level).Should().Be(expected);

	/// <summary>
	/// Pins current behaviour: .
	/// </summary>
	[Theory]
	[InlineData("~ 1 2 3", "Normal")]
	[InlineData("~ 1 2 3", "Warning")]
	[InlineData("~ 1 2 3", "Error")]
	[InlineData("~ 1 2 3", "Critical")]
	public void CountAtAlertLevelUseSdt_UnknownOperator_ReturnsNull(string expression, string level)
		=> CountSdt(expression, level).Should().BeNull();

	/// <summary>
	/// Pins current behaviour: .
	/// </summary>
	[Fact]
	public void CountAtAlertLevelUseSdt_UndefinedLevel_Throws()
	{
		var act = () => InvokePrivate("CountAtAlertLevelUseSdt", _points, "> 1", (CountAlertLevel)99);
		act.Should().Throw<TargetInvocationException>().WithInnerException<ArgumentException>();
	}
}
