namespace LogicMonitor.Datamart.Models;

public class DataSourceStoreItem : IdentifiedStoreItem
{
	// Navigation properties
	public List<DeviceDataSourceStoreItem> DeviceDataSources { get; set; } = null!;

	public List<DataSourceDataPointStoreItem> DataPoints { get; set; } = null!;

	// Database properties
	public string Name { get; set; } = string.Empty;

	public string Description { get; set; } = string.Empty;
}
