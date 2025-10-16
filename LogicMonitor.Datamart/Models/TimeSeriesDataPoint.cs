namespace LogicMonitor.Datamart.Models;

/// <summary>
/// Represents a single time series data point with SDT (Scheduled Down Time) information.
/// Used for MS-21395 to support excluding SDT periods from aggregation calculations.
/// </summary>
internal class TimeSeriesDataPoint
{
	/// <summary>
	/// Whether this data point falls within a Scheduled Down Time period.
	/// When ExcludeSdtPeriods is true, data points with IsInSdt=true will be excluded from aggregations.
	/// </summary>
	public required bool IsInSdt { get; set; }

	/// <summary>
	/// The timestamp of this data point in milliseconds since epoch
	/// </summary>
	public required long Timestamp { get; set; }

	/// <summary>
	/// The data point value (null if no data available)
	/// </summary>
	public required double? Value { get; set; }

}