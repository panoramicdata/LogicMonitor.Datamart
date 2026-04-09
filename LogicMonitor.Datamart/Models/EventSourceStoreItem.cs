namespace LogicMonitor.Datamart.Models;

/// <summary>
/// Represents a LogicMonitor EventSource stored in the datamart.
/// </summary>
public class EventSourceStoreItem : IdentifiedStoreItem
{
	/// <summary>
	/// The name of the EventSource.
	/// </summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// A description of the EventSource.
	/// </summary>
	public string Description { get; set; } = string.Empty;
}
