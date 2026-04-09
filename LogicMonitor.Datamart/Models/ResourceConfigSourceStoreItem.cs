namespace LogicMonitor.Datamart.Models;

/// <summary>
/// Represents a ConfigSource assignment to a resource stored in the datamart.
/// </summary>
public class ResourceConfigSourceStoreItem : ResourceLogicModuleStoreItem
{
	/// <summary>
	/// Navigation property to the ConfigSource instances on this resource.
	/// </summary>
	public virtual ICollection<ResourceConfigSourceInstanceStoreItem>? DeviceConfigSourceInstances { get; set; }

	/// <summary>
	/// Navigation property to the parent ConfigSource definition.
	/// </summary>
	public ConfigSourceStoreItem? ConfigSource { get; set; }

	/// <summary>
	/// The datamart identifier of the ConfigSource.
	/// </summary>
	public Guid ConfigSourceId { get; set; }
}