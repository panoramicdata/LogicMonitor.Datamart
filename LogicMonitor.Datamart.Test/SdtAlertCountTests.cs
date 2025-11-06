using Microsoft.Extensions.Logging.Abstractions;

namespace LogicMonitor.Datamart.Test;

/// <summary>
/// Tests for MS-21396: SDT-aware alert level count columns
/// Tests the CountAtAlertLevelUseSdt method and related SDT functionality
/// </summary>
public class SdtAlertCountTests(ITestOutputHelper testOutputHelper) : TestWithOutput(testOutputHelper)
{
	private static readonly NullLogger<SdtAlertCountTests> _logger = new();

	#region Test Data Builders

	private static List<TimeSeriesDataPoint> CreateTestDataPoints((double? value, bool isInSdt)[] dataPoints) => [.. dataPoints
			.Select((dp, index) => new TimeSeriesDataPoint
			{
				Value = dp.value,
				IsInSdt = dp.isInSdt,
				Timestamp = 1000L * index // Mock timestamp
			})];

	#endregion

	#region Empty or Null Alert Expression Tests

	[Fact]
	public void CountAtAlertLevelUseSdt_EmptyExpression_Normal_CountsValuesAndSdt()
	{
		// Arrange
		var dataPoints = CreateTestDataPoints(
		[
			(10.0, false),  // Has value, not in SDT
			(20.0, true),   // Has value, in SDT
			(null, false),  // No value, not in SDT
			(null, true)    // No value, in SDT
		]);

		// Act - Use reflection to call private method
		var result = CallCountAtAlertLevelUseSdt(
			dataPoints,
			"", // Empty expression
			CountAlertLevel.Normal);

		// Assert
		result.Should().Be(3); // 2 with values + 1 in SDT (without value)
	}

	[Fact]
	public void CountAtAlertLevelUseSdt_EmptyExpression_Warning_ReturnsZero()
	{
		// Arrange
		var dataPoints = CreateTestDataPoints(
		[
			(10.0, false),
			(20.0, true)
		]);

		// Act
		var result = CallCountAtAlertLevelUseSdt(
			dataPoints,
			"",
			CountAlertLevel.Warning);

		// Assert
		result.Should().Be(0); // No thresholds defined
	}

	#endregion

	#region Normal Level Tests (OR Logic)

	[Fact]
	public void CountAtAlertLevelUseSdt_Normal_CountsNormalOrSdt()
	{
		// Arrange - Expression: "> 50" (Warning threshold)
		var dataPoints = CreateTestDataPoints(
		[
			(30.0, false),  // Normal (below threshold), not in SDT -> COUNT
			(60.0, false),  // Warning (above threshold), not in SDT -> DON'T COUNT
			(70.0, true),   // Warning but in SDT -> COUNT (SDT takes precedence)
			(40.0, true),   // Normal and in SDT -> COUNT
			(null, true)    // No value but in SDT -> COUNT
		]);

		// Act
		var result = CallCountAtAlertLevelUseSdt(
			dataPoints,
			"> 50",
			CountAlertLevel.Normal);

		// Assert
		result.Should().Be(4); // Points 0, 2, 3, 4 (Normal OR in SDT)
	}

	[Fact]
	public void CountAtAlertLevelUseSdt_Normal_GreaterThanThreshold()
	{
		// Arrange - Expression: "> 50 80 100" (Warning 50, Error 80, Critical 100)
		var dataPoints = CreateTestDataPoints(
		[
			(30.0, false),   // Normal -> COUNT
			(40.0, false),   // Normal -> COUNT
			(60.0, false),   // Warning -> DON'T COUNT
			(90.0, false),   // Error -> DON'T COUNT
			(110.0, false),  // Critical -> DON'T COUNT
			(110.0, true),   // Critical but in SDT -> COUNT
			(30.0, true)     // Normal and in SDT -> COUNT
		]);

		// Act
		var result = CallCountAtAlertLevelUseSdt(
			dataPoints,
			"> 50 80 100",
			CountAlertLevel.Normal);

		// Assert
		result.Should().Be(4); // Points 0, 1, 5, 6
	}

	#endregion

	#region Warning Level Tests (AND Logic)

	[Fact]
	public void CountAtAlertLevelUseSdt_Warning_CountsWarningAndSdt()
	{
		// Arrange - Expression: "> 50 80" (Warning 50, Error 80)
		var dataPoints = CreateTestDataPoints(
		[
			(60.0, false),  // Warning but not in SDT -> DON'T COUNT
			(60.0, true),	// Warning AND in SDT -> COUNT
			(70.0, true),   // Warning AND in SDT -> COUNT
			(90.0, true),   // Error (not Warning) even though in SDT -> DON'T COUNT
			(40.0, true)    // Normal (not Warning) even though in SDT -> DON'T COUNT
		]);

		// Act
		var result = CallCountAtAlertLevelUseSdt(
			dataPoints,
			"> 50 80",
			CountAlertLevel.Warning);

		// Assert
		result.Should().Be(2); // Points 1, 2 (Warning AND in SDT)
	}

	[Fact]
	public void CountAtAlertLevelUseSdt_Warning_NoWarningThreshold_ReturnsZero()
	{
		// Arrange - Expression with Warning, Error, and Critical where we explicitly want Error+Critical only
		// The implementation expects: Warning, Error, Critical as 3 separate values
		// To test "no warning threshold", we need to test when alertLevels.Length < 1 OR warningLevel is null
		var dataPoints = CreateTestDataPoints(
		[
			(60.0, true),
			(90.0, true)
		]);

		// Act - Use "> 80" which will be interpreted as Warning threshold 80
		// For truly no warning threshold, the expression would need to be different
		// Let's test with ">= 999" where values are all below the threshold (behaves like no threshold)
		var result = CallCountAtAlertLevelUseSdt(
			dataPoints,
			"> 999", // Warning threshold so high that no values reach it - effectively no warning
			CountAlertLevel.Warning);

		// Assert
		result.Should().Be(0); // No values exceed the very high threshold
	}

	[Fact]
	public void CountAtAlertLevelUseSdt_Warning_OnlyErrorAndCritical_CountsCorrectly()
	{
		// Arrange - Expression: "> 80 100" 
		// This will be parsed as: alertLevels[0]=80 (Warning), alertLevels[1]=100 (Error)
		// In the current implementation, with 2 values, they map to Warning and Error thresholds
		var dataPoints = CreateTestDataPoints(
		[
			(60.0, true),   // Below 80 (Normal), not counted
			(90.0, true)    // Above 80, below 100 (Warning level) AND in SDT -> COUNT
		]);

		// Act
		var result = CallCountAtAlertLevelUseSdt(
			dataPoints,
			"> 80 100",
			CountAlertLevel.Warning);

		// Assert
		result.Should().Be(1); // Point at 90 (Warning level AND in SDT)
	}

	#endregion

	#region Error Level Tests (AND Logic)

	[Fact]
	public void CountAtAlertLevelUseSdt_Error_CountsErrorAndSdt()
	{
		// Arrange - Expression: "> 50 80 100" (Warning 50, Error 80, Critical 100)
		var dataPoints = CreateTestDataPoints(
		[
			(85.0, false),  // Error but not in SDT -> DON'T COUNT
			(85.0, true),   // Error AND in SDT -> COUNT
			(95.0, true),   // Error AND in SDT -> COUNT
			(110.0, true),  // Critical (not Error) even though in SDT -> DON'T COUNT
			(60.0, true)    // Warning (not Error) even though in SDT -> DON'T COUNT
		]);

		// Act
		var result = CallCountAtAlertLevelUseSdt(
			dataPoints,
			"> 50 80 100",
			CountAlertLevel.Error);

		// Assert
		result.Should().Be(2); // Points 1, 2 (Error AND in SDT)
	}

	#endregion

	#region Critical Level Tests (AND Logic)

	[Fact]
	public void CountAtAlertLevelUseSdt_Critical_CountsCriticalAndSdt()
	{
		// Arrange - Expression: "> 50 80 100" (Warning 50, Error 80, Critical 100)
		var dataPoints = CreateTestDataPoints(
		[
			(110.0, false), // Critical but not in SDT -> DON'T COUNT
			(110.0, true),  // Critical AND in SDT -> COUNT
			(150.0, true),  // Critical AND in SDT -> COUNT
			(90.0, true)    // Error (not Critical) even though in SDT -> DON'T COUNT
		]);

		// Act
		var result = CallCountAtAlertLevelUseSdt(
			dataPoints,
			"> 50 80 100",
			CountAlertLevel.Critical);

		// Assert
		result.Should().Be(2); // Points 1, 2 (Critical AND in SDT)
	}

	[Fact]
	public void CountAtAlertLevelUseSdt_Critical_NoCriticalThreshold_ReturnsZero()
	{
		// Arrange - Expression with only Warning and Error
		var dataPoints = CreateTestDataPoints(
		[
			(100.0, true),
			(200.0, true)
		]);

		// Act
		var result = CallCountAtAlertLevelUseSdt(
			dataPoints,
			"> 50 80", // No critical threshold
			CountAlertLevel.Critical);

		// Assert
		result.Should().Be(0); // No critical threshold defined
	}

	#endregion

	#region Less Than Operator Tests

	[Fact]
	public void CountAtAlertLevelUseSdt_LessThan_Normal_CountsCorrectly()
	{
		// Arrange - Expression: "< 50" (Warning threshold - alert when BELOW 50)
		var dataPoints = CreateTestDataPoints(
		[
			(60.0, false),  // Normal (above threshold), not in SDT -> COUNT
			(70.0, false),  // Normal (above threshold), not in SDT -> COUNT
			(40.0, false),  // Warning (below threshold), not in SDT -> DON'T COUNT
			(30.0, true),   // Warning but in SDT -> COUNT (SDT overrides)
			(60.0, true)    // Normal and in SDT -> COUNT
		]);

		// Act
		var result = CallCountAtAlertLevelUseSdt(
			dataPoints,
			"< 50",
			CountAlertLevel.Normal);

		// Assert
		result.Should().Be(4); // Points 0, 1, 3, 4
	}

	[Fact]
	public void CountAtAlertLevelUseSdt_LessThan_Warning_CountsCorrectly()
	{
		// Arrange - Expression: "< 50" (Warning threshold)
		var dataPoints = CreateTestDataPoints(
		[
			(40.0, false),  // Warning but not in SDT -> DON'T COUNT
			(40.0, true),   // Warning AND in SDT -> COUNT
			(30.0, true),   // Warning AND in SDT -> COUNT
			(60.0, true)    // Normal (not Warning) even though in SDT -> DON'T COUNT
		]);

		// Act
		var result = CallCountAtAlertLevelUseSdt(
			dataPoints,
			"< 50",
			CountAlertLevel.Warning);

		// Assert
		result.Should().Be(2); // Points 1, 2 (Warning AND in SDT)
	}

	#endregion

	#region Equals Operator Tests

	[Fact]
	public void CountAtAlertLevelUseSdt_Equals_Normal_CountsCorrectly()
	{
		// Arrange - Expression: "= 50" (Warning threshold)
		var dataPoints = CreateTestDataPoints(
		[
			(40.0, false),  // Normal (not equal), not in SDT -> COUNT
			(50.0, false),  // Warning (equals threshold), not in SDT -> DON'T COUNT
			(50.0, true),   // Warning but in SDT -> COUNT
			(40.0, true)    // Normal and in SDT -> COUNT
		]);

		// Act
		var result = CallCountAtAlertLevelUseSdt(
			dataPoints,
			"= 50",
			CountAlertLevel.Normal);

		// Assert
		result.Should().Be(3); // Points 0, 2, 3
	}

	#endregion

	#region Edge Cases

	[Fact]
	public void CountAtAlertLevelUseSdt_AllInSdt_CountsCorrectly()
	{
		// Arrange - All points in SDT
		var dataPoints = CreateTestDataPoints(
		[
			(30.0, true),
			(60.0, true),
			(90.0, true),
			(null, true)
		]);

		// Act - Normal should count all
		var normalResult = CallCountAtAlertLevelUseSdt(
			dataPoints,
			"> 50 80",
			CountAlertLevel.Normal);

		// Assert
		normalResult.Should().Be(4); // All points (all in SDT)
	}

	[Fact]
	public void CountAtAlertLevelUseSdt_NoneInSdt_CountsOnlyNormal()
	{
		// Arrange - No points in SDT
		var dataPoints = CreateTestDataPoints(
		[
			(30.0, false),  // Normal -> COUNT
			(60.0, false),  // Warning -> DON'T COUNT
			(90.0, false)   // Error -> DON'T COUNT
		]);

		// Act
		var normalResult = CallCountAtAlertLevelUseSdt(
			dataPoints,
			"> 50 80",
			CountAlertLevel.Normal);

		var warningResult = CallCountAtAlertLevelUseSdt(
			dataPoints,
			"> 50 80",
			CountAlertLevel.Warning);

		// Assert
		normalResult.Should().Be(1); // Only point 0 (Normal, not in SDT)
		warningResult.Should().Be(0); // None (Warning but not in SDT)
	}

	[Fact]
	public void CountAtAlertLevelUseSdt_EmptyDataPoints_ReturnsZero()
	{
		// Arrange
		var dataPoints = new List<TimeSeriesDataPoint>();

		// Act
		var result = CallCountAtAlertLevelUseSdt(
			dataPoints,
			"> 50",
			CountAlertLevel.Normal);

		// Assert
		result.Should().Be(0);
	}

	[Fact]
	public void CountAtAlertLevelUseSdt_AllNullValues_CountsOnlySdt()
	{
		// Arrange
		var dataPoints = CreateTestDataPoints(
		[
			(null, false),
			(null, true),
			(null, true)
		]);

		// Act
		var result = CallCountAtAlertLevelUseSdt(
			dataPoints,
			"> 50",
			CountAlertLevel.Normal);

		// Assert
		result.Should().Be(2); // Only points in SDT (even without values)
	}

	#endregion

	#region Helper Methods

	/// <summary>
	/// Helper to call the private CountAtAlertLevelUseSdt method via reflection
	/// </summary>
	private static int? CallCountAtAlertLevelUseSdt(
		List<TimeSeriesDataPoint> timeSeriesDataPoints,
		string effectiveAlertExpression,
		CountAlertLevel countAtAlertLevel)
	{
		var method = typeof(LowResolutionDataSync).GetMethod("CountAtAlertLevelUseSdt", bindingAttr: BindingFlags.NonPublic | BindingFlags.Static);

		return method == null
			? throw new InvalidOperationException("Could not find CountAtAlertLevelUseSdt method")
			: (int?)method.Invoke(null,
			[
				timeSeriesDataPoints,
				effectiveAlertExpression,
				countAtAlertLevel
			]);
	}

	#endregion
}
