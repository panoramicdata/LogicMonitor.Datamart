namespace LogicMonitor.Datamart.Models;

public class DataSourceStoreItem : IdentifiedStoreItem
{
	// Navigation properties
	public List<DeviceDataSourceStoreItem> DeviceDataSources { get; set; }

	public List<DataSourceDataPointStoreItem> DataPoints { get; set; }

	// Database properties
	public string Name { get; set; }

	public string Description { get; set; }
}
