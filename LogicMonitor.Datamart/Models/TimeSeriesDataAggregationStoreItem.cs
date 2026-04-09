namespace LogicMonitor.Datamart.Models;

/// <summary>
/// Represents an aggregated time series data record for a DataPoint stored in the datamart.
/// </summary>
public class TimeSeriesDataAggregationStoreItem
{
	/// <summary>
	/// The unique identifier for this aggregation record.
	/// </summary>
	public Guid Id { get; set; }

	/// <summary>
	/// The datamart identifier of the parent DataPoint store item.
	/// </summary>
	public Guid DeviceDataSourceInstanceDataPointId { get; set; }

	/// <summary>
	/// Navigation property to the parent DataPoint store item.
	/// </summary>
	public ResourceDataSourceInstanceDataPointStoreItem DeviceDataSourceInstanceDataPoint { get; set; } = null!;

	/// <summary>
	/// The start of the aggregation period.
	/// </summary>
	public DateTimeOffset PeriodStart { get; set; }

	/// <summary>
	/// The end of the aggregation period.
	/// </summary>
	public DateTimeOffset PeriodEnd { get; set; }

	/// <summary>
	/// The 5th percentile value for the aggregation period.
	/// </summary>
	public double? Centile05 { get; set; }

	/// <summary>
	/// The 10th percentile value for the aggregation period.
	/// </summary>
	public double? Centile10 { get; set; }

	/// <summary>
	/// The 25th percentile value for the aggregation period.
	/// </summary>
	public double? Centile25 { get; set; }

	/// <summary>
	/// The 50th percentile (median) value for the aggregation period.
	/// </summary>
	public double? Centile50 { get; set; }

	/// <summary>
	/// The 75th percentile value for the aggregation period.
	/// </summary>
	public double? Centile75 { get; set; }

	/// <summary>
	/// The 90th percentile value for the aggregation period.
	/// </summary>
	public double? Centile90 { get; set; }

	/// <summary>
	/// The 95th percentile value for the aggregation period.
	/// </summary>
	public double? Centile95 { get; set; }

	/// <summary>
	/// The availability percentage for the aggregation period.
	/// </summary>
	public double? AvailabilityPercent { get; set; }

	/// <summary>
	/// The availability percentage (alternate calculation) for the aggregation period.
	/// </summary>
	public double? AvailabilityPercent2 { get; set; }

	/// <summary>
	/// The first value recorded in the aggregation period.
	/// </summary>
	public double? First { get; set; }

	/// <summary>
	/// The last value recorded in the aggregation period.
	/// </summary>
	public double? Last { get; set; }

	/// <summary>
	/// The first non-null value recorded in the aggregation period.
	/// </summary>
	public double? FirstWithData { get; set; }

	/// <summary>
	/// The last non-null value recorded in the aggregation period.
	/// </summary>
	public double? LastWithData { get; set; }

	/// <summary>
	/// The minimum value recorded in the aggregation period.
	/// </summary>
	public double? Min { get; set; }

	/// <summary>
	/// The maximum value recorded in the aggregation period.
	/// </summary>
	public double? Max { get; set; }

	/// <summary>
	/// The sum of all values in the aggregation period.
	/// </summary>
	public double Sum { get; set; }

	/// <summary>
	/// The sum of squared values in the aggregation period (for standard deviation calculation).
	/// </summary>
	public double SumSquared { get; set; }

	/// <summary>
	/// The count of data points with values in the aggregation period.
	/// </summary>
	public int DataCount { get; set; }

	/// <summary>
	/// The count of data points without values (no data) in the aggregation period.
	/// </summary>
	public int NoDataCount { get; set; }

	/// <summary>
	/// The count of polls in a normal (non-alerting) state.
	/// </summary>
	public int? NormalCount { get; set; }

	/// <summary>
	/// The count of polls in a warning alert state.
	/// </summary>
	public int? WarningCount { get; set; }

	/// <summary>
	/// The count of polls in an error alert state.
	/// </summary>
	public int? ErrorCount { get; set; }

	/// <summary>
	/// The count of polls in a critical alert state.
	/// </summary>
	public int? CriticalCount { get; set; }

	/// <summary>
	/// MS-21396: this is the total of the number of polls in the aggregation period that are in a normal state (not alerting) or during a period of SDT (service down time)
	/// </summary>
	public int? NormalOrSdtCount { get; set; }

	/// <summary>
	/// MS-21396: this is the total of the number of polls in the aggregation period that are in a warning state only and are in SDT (service down time)
	/// </summary>
	public int? WarningSdtCount { get; set; }


	/// <summary>
	/// MS-21396: this is the total of the number of polls in the aggregation period that are in an error state only and are in SDT (service down time)
	/// </summary>
	public int? ErrorSdtCount { get; set; }

	/// <summary>
	/// MS-21396 : this is the total of the number of polls in the aggregation period that are in a critical state only and are in SDT (service down time)
	/// </summary>
	public int? CriticalSdtCount { get; set; }

	#region MS-22557: Aggregation columns excluding SDT periods (only populated when ExcludeSdtPeriods = true)

	/// <summary>
	/// MS-22557: Count of data points with values, excluding those during SDT periods
	/// </summary>
	public int? DataCountExcludingSdt { get; set; }

	/// <summary>
	/// MS-22557: Count of data points without values, excluding those during SDT periods
	/// </summary>
	public int? NoDataCountExcludingSdt { get; set; }

	/// <summary>
	/// MS-22557: Sum of values, excluding those during SDT periods
	/// </summary>
	public double? SumExcludingSdt { get; set; }

	/// <summary>
	/// MS-22557: Sum of squared values, excluding those during SDT periods
	/// </summary>
	public double? SumSquaredExcludingSdt { get; set; }

	/// <summary>
	/// MS-22557: Maximum value, excluding those during SDT periods
	/// </summary>
	public double? MaxExcludingSdt { get; set; }

	/// <summary>
	/// MS-22557: Minimum value, excluding those during SDT periods
	/// </summary>
	public double? MinExcludingSdt { get; set; }

	/// <summary>
	/// MS-22557: First value in the period, excluding those during SDT periods
	/// </summary>
	public double? FirstExcludingSdt { get; set; }

	/// <summary>
	/// MS-22557: Last value in the period, excluding those during SDT periods
	/// </summary>
	public double? LastExcludingSdt { get; set; }

	/// <summary>
	/// MS-22557: First non-null value in the period, excluding those during SDT periods
	/// </summary>
	public double? FirstWithDataExcludingSdt { get; set; }

	/// <summary>
	/// MS-22557: Last non-null value in the period, excluding those during SDT periods
	/// </summary>
	public double? LastWithDataExcludingSdt { get; set; }

	/// <summary>
	/// MS-22557: 5th percentile, excluding those during SDT periods
	/// </summary>
	public double? Centile05ExcludingSdt { get; set; }

	/// <summary>
	/// MS-22557: 10th percentile, excluding those during SDT periods
	/// </summary>
	public double? Centile10ExcludingSdt { get; set; }

	/// <summary>
	/// MS-22557: 25th percentile, excluding those during SDT periods
	/// </summary>
	public double? Centile25ExcludingSdt { get; set; }

	/// <summary>
	/// MS-22557: 50th percentile (median), excluding those during SDT periods
	/// </summary>
	public double? Centile50ExcludingSdt { get; set; }

	/// <summary>
	/// MS-22557: 75th percentile, excluding those during SDT periods
	/// </summary>
	public double? Centile75ExcludingSdt { get; set; }

	/// <summary>
	/// MS-22557: 90th percentile, excluding those during SDT periods
	/// </summary>
	public double? Centile90ExcludingSdt { get; set; }

	/// <summary>
	/// MS-22557: 95th percentile, excluding those during SDT periods
	/// </summary>
	public double? Centile95ExcludingSdt { get; set; }

	/// <summary>
	/// MS-22557: Availability percentage, excluding those during SDT periods
	/// </summary>
	public double? AvailabilityPercentExcludingSdt { get; set; }

	/// <summary>
	/// MS-22557: Availability percentage (new calculation), excluding those during SDT periods
	/// </summary>
	public double? AvailabilityPercent2ExcludingSdt { get; set; }

	#endregion
}
