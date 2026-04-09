namespace LogicMonitor.Datamart.Models;

/// <summary>
/// Represents a ConfigSource instance on a resource stored in the datamart.
/// </summary>
public class ResourceConfigSourceInstanceStoreItem : ResourceLogicModuleInstanceStoreItem
{
	/// <summary>
	/// Navigation property to the parent resource-ConfigSource assignment.
	/// </summary>
	public ResourceConfigSourceStoreItem? DeviceConfigSource { get; set; }

	/// <summary>
	/// The datamart identifier of the parent resource-ConfigSource assignment.
	/// </summary>
	public Guid DeviceConfigSourceId { get; set; }

	/// <summary>
	/// Navigation property to the configuration snapshots collected by this instance.
	/// </summary>
	public virtual ICollection<ResourceConfigSourceInstanceConfigStoreItem>? DeviceConfigSourceInstanceConfigs { get; set; }
}
