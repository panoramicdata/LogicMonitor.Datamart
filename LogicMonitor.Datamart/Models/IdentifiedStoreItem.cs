namespace LogicMonitor.Datamart.Models;

/// <summary>
/// Abstract base class for store items that correspond to an identified LogicMonitor entity.
/// </summary>
public abstract class IdentifiedStoreItem : StoreItem
{
	/// <summary>
	/// The LogicMonitor Id
	/// </summary>
	public int LogicMonitorId { get; set; }

	/// <summary>
	/// When the Item was last observed
	/// </summary>
	public DateTimeOffset DatamartLastObserved { get; set; } = DateTimeOffset.MinValue;
}
