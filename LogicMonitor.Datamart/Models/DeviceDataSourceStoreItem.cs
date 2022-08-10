namespace LogicMonitor.Datamart.Models;

public class DeviceDataSourceStoreItem : IdentifiedStoreItem
{
	// Navigation properties
	public List<DeviceDataSourceInstanceStoreItem> DeviceDataSourceInstances { get; set; } = null!;

	public DataSourceStoreItem DataSource { get; set; } = null!;

	public DeviceStoreItem Device { get; set; } = null!;

	// Database properties
	public long AssignedOnSeconds { get; set; }
	public long CreatedOnSeconds { get; set; }
	public long UpdatedOnSeconds { get; set; }
	public int DataSourceId { get; set; }
	public int DeviceId { get; set; }
}
