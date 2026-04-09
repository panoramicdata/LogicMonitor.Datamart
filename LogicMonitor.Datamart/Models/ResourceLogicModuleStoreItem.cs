namespace LogicMonitor.Datamart.Models;

/// <summary>
/// Abstract base class for a LogicModule assignment to a specific resource (device).
/// </summary>
public abstract class ResourceLogicModuleStoreItem : IdentifiedStoreItem
{
	/// <summary>
	/// Navigation property to the parent resource.
	/// </summary>
	public ResourceStoreItem? Device { get; set; } = null!;

	/// <summary>
	/// Foreign key to the parent resource.
	/// </summary>
	public Guid DeviceId { get; set; }

	/// <summary>
	/// The epoch timestamp (seconds) when the LogicModule was assigned to the resource.
	/// </summary>
	public long AssignedOnSeconds { get; set; }

	/// <summary>
	/// The epoch timestamp (seconds) when the LogicModule assignment was created.
	/// </summary>
	public long CreatedOnSeconds { get; set; }

	/// <summary>
	/// The epoch timestamp (seconds) when the LogicModule assignment was last updated.
	/// </summary>
	public long UpdatedOnSeconds { get; set; }
}
