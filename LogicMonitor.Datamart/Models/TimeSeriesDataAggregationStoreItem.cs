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
}
