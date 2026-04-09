namespace LogicMonitor.Datamart.Models;

/// <summary>
/// Represents a LogicMonitor collector group stored in the datamart.
/// </summary>
public class CollectorGroupStoreItem : IdentifiedStoreItem
{
	/// <summary>
	/// Navigation property to the collectors in this group.
	/// </summary>
	public virtual ICollection<CollectorStoreItem>? Collectors { get; set; }

	/// <summary>
	/// The name of the collector group.
	/// </summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// A description of the collector group.
	/// </summary>
	public string Description { get; set; } = string.Empty;

	/// <summary>
	/// The epoch timestamp (seconds) when the collector group was created.
	/// </summary>
	public long CreatedOnTimeStampSeconds { get; set; }

	/// <summary>
	/// The number of collectors in this group.
	/// </summary>
	public int CollectorCount { get; set; }
}
