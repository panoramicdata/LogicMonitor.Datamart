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

	public string Tags { get; set; } = string.Empty;

	public ICollection<DeviceDataSourceInstanceDataPointStoreItem> DeviceDataSourceInstanceDataPoints { get; set; } = null!;
}