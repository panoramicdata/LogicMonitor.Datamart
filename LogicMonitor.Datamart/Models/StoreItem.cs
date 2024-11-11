namespace LogicMonitor.Datamart.Models;

public abstract class StoreItem
{
	/// <summary>
	/// Database unique Id
	/// </summary>
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public Guid Id { get; set; }

	public DateTimeOffset DatamartCreated { get; set; }

	public DateTimeOffset DatamartLastModified { get; set; }
}