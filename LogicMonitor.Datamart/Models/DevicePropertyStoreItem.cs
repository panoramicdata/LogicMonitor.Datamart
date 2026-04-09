namespace LogicMonitor.Datamart.Models;

/// <summary>
/// Represents a resource (device) property stored in the datamart.
/// </summary>
public class DevicePropertyStoreItem : StoreItem
{
	/// <summary>
	/// Navigation property to the parent resource.
	/// </summary>
	public virtual ResourceStoreItem? Device { get; set; }

	/// <summary>
	/// Foreign key to the parent resource.
	/// </summary>
	public Guid DeviceId { get; set; }

	/// <summary>
	/// The property name.
	/// </summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// The property value.
	/// </summary>
	public string Value { get; set; } = string.Empty;

	/// <summary>
	/// The type of property (e.g. system, custom, auto, inherited).
	/// </summary>
	public PropertyType Type { get; set; }
}
