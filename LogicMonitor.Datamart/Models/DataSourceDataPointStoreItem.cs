namespace LogicMonitor.Datamart.Models;

public class DataSourceDataPointStoreItem : IdentifiedStoreItem
{
	// Navigation properties
	public DataSourceStoreItem? DataSource { get; set; }

	// Database properties
	public Guid DataSourceId { get; set; }

	public string Name { get; set; } = string.Empty;

	public string Description { get; set; } = string.Empty;

	public string MeasurementUnit { get; set; } = string.Empty;

	public string GlobalAlertExpression { get; set; } = string.Empty;
}
