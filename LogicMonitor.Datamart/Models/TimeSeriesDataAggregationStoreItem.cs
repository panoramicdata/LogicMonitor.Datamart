namespace LogicMonitor.Datamart.Models;

public class TimeSeriesDataAggregationStoreItem
{
	public Guid Id { get; set; }

	public Guid DeviceDataSourceInstanceDataPointId { get; set; }

	public ResourceDataSourceInstanceDataPointStoreItem DeviceDataSourceInstanceDataPoint { get; set; } = null!;

	public DateTimeOffset PeriodStart { get; set; }

	public DateTimeOffset PeriodEnd { get; set; }

	public double? Centile05 { get; set; }

	public double? Centile10 { get; set; }

	public double? Centile25 { get; set; }

	public double? Centile50 { get; set; }

	public double? Centile75 { get; set; }

	public double? Centile90 { get; set; }

	public double? Centile95 { get; set; }

	public double? AvailabilityPercent { get; set; }

	public double? AvailabilityPercent2 { get; set; }

	public double? First { get; set; }

	public double? Last { get; set; }

	public double? FirstWithData { get; set; }

	public double? LastWithData { get; set; }

	public double? Min { get; set; }

	public double? Max { get; set; }

	public double Sum { get; set; }

	public double SumSquared { get; set; }

	public int DataCount { get; set; }

	public int NoDataCount { get; set; }

	public int? NormalCount { get; set; }

	public int? WarningCount { get; set; }

	public int? ErrorCount { get; set; }

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
