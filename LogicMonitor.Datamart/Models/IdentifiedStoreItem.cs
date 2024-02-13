namespace LogicMonitor.Datamart.Models;

public abstract class IdentifiedStoreItem : StoreItem
{
	/// <summary>
	/// The LogicMonitor Id
	/// </summary>
	public int LogicMonitorId { get; set; }

	/// <summary>
	/// When the Item was last observed
	/// </summary>
	public DateTimeOffset DatamartLastObserved { get; set; } = DateTimeOffset.Now.AddYears(-100);
}
