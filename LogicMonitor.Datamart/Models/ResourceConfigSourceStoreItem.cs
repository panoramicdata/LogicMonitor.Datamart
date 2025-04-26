namespace LogicMonitor.Datamart.Models;

public class ResourceConfigSourceStoreItem : ResourceLogicModuleStoreItem
{
	public virtual ICollection<ResourceConfigSourceInstanceStoreItem>? DeviceConfigSourceInstances { get; set; }

	public ConfigSourceStoreItem? ConfigSource { get; set; }

	public Guid ConfigSourceId { get; set; }
}