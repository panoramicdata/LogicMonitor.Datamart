namespace LogicMonitor.Datamart.Models;

public class ResourceDataSourceInstanceStoreItem : ResourceLogicModuleInstanceStoreItem
{
	// Navigation properties
	public ResourceDataSourceStoreItem? DeviceDataSource { get; set; }

	public Guid DeviceDataSourceId { get; set; }

	public virtual ICollection<ResourceDataSourceInstanceDataPointStoreItem> DeviceDataSourceInstanceDataPoints { get; set; } = null!;
}