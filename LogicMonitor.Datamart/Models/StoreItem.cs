namespace LogicMonitor.Datamart.Models;

/// <summary>
/// Abstract base class for all datamart store items with common audit fields.
/// </summary>
public abstract class StoreItem
{
	/// <summary>
	/// Database unique Id
	/// </summary>
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public Guid Id { get; set; }

	/// <summary>
	/// The UTC timestamp when this record was first created in the datamart.
	/// </summary>
	public DateTimeOffset DatamartCreated { get; set; }

	/// <summary>
	/// The UTC timestamp when this record was last modified in the datamart.
	/// </summary>
	public DateTimeOffset DatamartLastModified { get; set; }
}