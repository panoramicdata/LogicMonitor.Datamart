namespace LogicMonitor.Datamart.Models;

public abstract class StoreItem
{
	/// <summary>
	/// Database unique Id
	/// </summary>
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public Guid Id { get; set; }

	public DateTime DatamartCreatedUtc { get; set; }

	public DateTime DatamartLastModifiedUtc { get; set; }
}
