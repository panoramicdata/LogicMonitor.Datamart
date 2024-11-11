namespace LogicMonitor.Datamart.Models;

public class ResourceDataSourceStoreItem : IdentifiedStoreItem
{
	// Navigation properties
	public virtual ICollection<ResourceDataSourceInstanceStoreItem>? DeviceDataSourceInstances { get; set; }

	public DataSourceStoreItem? DataSource { get; set; }

	public Guid DataSourceId { get; set; }

	public ResourceStoreItem? Device { get; set; } = null!;

	public Guid DeviceId { get; set; }

	// Database properties
	public long AssignedOnSeconds { get; set; }

	public long CreatedOnSeconds { get; set; }

	public long UpdatedOnSeconds { get; set; }
}
