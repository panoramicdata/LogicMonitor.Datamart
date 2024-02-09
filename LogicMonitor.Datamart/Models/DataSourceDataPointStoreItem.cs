namespace LogicMonitor.Datamart.Models;

public class DataSourceDataPointStoreItem : IdentifiedStoreItem
{
	public Guid DataSourceId { get; set; }

	public DataSourceStoreItem DataSource { get; set; } = null!;

	public string Name { get; set; } = string.Empty;

	public string Description { get; set; } = string.Empty;

	public string MeasurementUnit { get; set; } = string.Empty;

	public string GlobalAlertExpression { get; set; } = string.Empty;

	public string Calculation { get; set; } = string.Empty;

	public string PercentageAvailabilityCalculation { get; set; } = string.Empty;

	public string Tags { get; set; } = string.Empty;

	/// <summary>
	/// If true, the DataPoint will be resynced even if it is not out of sync
	/// This will remove all old data and replace it with new data
	/// </summary>
	public bool ResyncTimeSeriesData { get; set; }

	public ICollection<DeviceDataSourceInstanceDataPointStoreItem> DeviceDataSourceInstanceDataPoints { get; set; } = null!;
}