namespace LogicMonitor.Datamart.Models;

public abstract class ResourceLogicModuleStoreItem : IdentifiedStoreItem
{
	// Navigation properties
	public ResourceStoreItem? Device { get; set; } = null!;

	public Guid DeviceId { get; set; }

	// Database properties
	public long AssignedOnSeconds { get; set; }

	public long CreatedOnSeconds { get; set; }

	public long UpdatedOnSeconds { get; set; }
}
