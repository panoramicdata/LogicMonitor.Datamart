namespace LogicMonitor.Datamart.Models;

public class DeviceDataSourceStoreItem : IdentifiedStoreItem
{
	// Navigation properties
	public virtual ICollection<DeviceDataSourceInstanceStoreItem>? DeviceDataSourceInstances { get; set; }

	public DataSourceStoreItem? DataSource { get; set; }

	public Guid DataSourceId { get; set; }

	public DeviceStoreItem? Device { get; set; } = null!;

	public Guid DeviceId { get; set; }

	// Database properties
	public long AssignedOnSeconds { get; set; }

	public long CreatedOnSeconds { get; set; }

	public long UpdatedOnSeconds { get; set; }
}
