namespace LogicMonitor.Datamart.Models;

public class CollectorGroupStoreItem : IdentifiedStoreItem
{
	// Navigation properties
	public virtual ICollection<CollectorStoreItem>? Collectors { get; set; }

	// Database properties
	public string Name { get; set; } = string.Empty;

	public string Description { get; set; } = string.Empty;

	public long CreatedOnTimeStampSeconds { get; set; }

	public int CollectorCount { get; set; }
}
