namespace LogicMonitor.Datamart.Models;

public class EscalationChainStoreItem : IdentifiedStoreItem
{
	// Navigation properties
	public virtual ICollection<AlertRuleStoreItem>? AlertRules { get; set; }

	// Database properties
	public string Name { get; set; } = string.Empty;

	public string Description { get; set; } = string.Empty;

	public bool EnableThrottling { get; set; }

	public int ThrottlingPeriodMinutes { get; set; }

	public int ThrottlingAlertCount { get; set; }

	public bool InAlerting { get; set; }

	public string CcDestination { get; set; } = string.Empty;

	public string CcDestinations { get; set; } = string.Empty;

	public string Destination { get; set; } = string.Empty;

	public string Destinations { get; set; } = string.Empty;
}
