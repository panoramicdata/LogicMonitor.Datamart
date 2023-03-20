namespace LogicMonitor.Datamart.Models;

public class DataSourceStoreItem : IdentifiedStoreItem
{
	// Navigation properties
	public ICollection<DeviceDataSourceStoreItem>? DeviceDataSources { get; set; } = null!;

	public ICollection<DataSourceDataPointStoreItem>? DataPoints { get; set; } = null!;

	// Database properties
	public string Name { get; set; } = string.Empty;

	public string Description { get; set; } = string.Empty;
}
