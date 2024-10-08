namespace LogicMonitor.Datamart.Models;

public class AlertRuleStoreItem : IdentifiedStoreItem
{
	// Navigation properties
	public virtual ICollection<AlertStoreItem>? AlertStoreItems { get; set; }

	public EscalationChainStoreItem? EscalationChain { get; set; }

	public Guid EscalationChainId { get; set; }

	// DataStore properties
	public string Name { get; set; } = string.Empty;

	public string? Description { get; set; } = string.Empty;

	public int Priority { get; set; }

	public string LevelString { get; set; } = string.Empty;

	public string DataSourceName { get; set; } = string.Empty;

	public string DataSourceInstanceName { get; set; } = string.Empty;

	public string DataPoint { get; set; } = string.Empty;

	public int EscalationChainIntervalMinutes { get; set; }

	public bool SuppressAlertClear { get; set; }

	public bool SuppressAlertAckSdt { get; set; }
}
