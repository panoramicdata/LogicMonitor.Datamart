namespace LogicMonitor.Datamart.Models;

public class AlertRuleStoreItem : IdentifiedStoreItem
{
	// Navigation properties
	public List<AlertStoreItem> Alerts { get; set; }

	// DataStore properties
	public string Name { get; set; }

	public string Description { get; set; }

	public int Priority { get; set; }

	public string LevelString { get; set; }

	public string DataSourceName { get; set; }

	public string DataSourceInstanceName { get; set; }

	public string DataPoint { get; set; }

	public int EscalationChainIntervalMinutes { get; set; }

	public int EscalationChainId { get; set; }

	public bool SuppressAlertClear { get; set; }

	public bool SuppressAlertAckSdt { get; set; }
}
