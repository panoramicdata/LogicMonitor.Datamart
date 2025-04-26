namespace LogicMonitor.Datamart.Models;

public class ResourceDataSourceStoreItem : ResourceLogicModuleStoreItem
{
	public virtual ICollection<ResourceDataSourceInstanceStoreItem>? DeviceDataSourceInstances { get; set; }

	public DataSourceStoreItem? DataSource { get; set; }

	public Guid DataSourceId { get; set; }
}