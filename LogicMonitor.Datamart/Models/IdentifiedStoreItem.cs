namespace LogicMonitor.Datamart.Models;

public abstract class IdentifiedStoreItem : StoreItem
{
	/// <summary>
	/// The LogicMonitor Id
	/// </summary>
	public int Id { get; set; }

	/// <summary>
	/// When the Item was last observed
	/// </summary>
	public DateTime DatamartLastObservedUtc { get; set; } = DateTime.MinValue;
}
