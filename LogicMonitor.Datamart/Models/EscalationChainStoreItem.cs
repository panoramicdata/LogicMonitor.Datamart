namespace LogicMonitor.Datamart.Models;

/// <summary>
/// Represents a LogicMonitor escalation chain stored in the datamart.
/// </summary>
public class EscalationChainStoreItem : IdentifiedStoreItem
{
	/// <summary>
	/// Navigation property to the alert rules using this escalation chain.
	/// </summary>
	public virtual ICollection<AlertRuleStoreItem>? AlertRules { get; set; }

	/// <summary>
	/// The name of the escalation chain.
	/// </summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// A description of the escalation chain.
	/// </summary>
	public string Description { get; set; } = string.Empty;

	/// <summary>
	/// Whether notification throttling is enabled.
	/// </summary>
	public bool EnableThrottling { get; set; }

	/// <summary>
	/// The throttling period in minutes.
	/// </summary>
	public int ThrottlingPeriodMinutes { get; set; }

	/// <summary>
	/// The maximum number of alerts within the throttling period before throttling activates.
	/// </summary>
	public int ThrottlingAlertCount { get; set; }

	/// <summary>
	/// Whether the escalation chain is currently in an alerting state.
	/// </summary>
	public bool InAlerting { get; set; }

	/// <summary>
	/// The CC destination for notifications.
	/// </summary>
	public string CcDestination { get; set; } = string.Empty;

	/// <summary>
	/// A serialized list of CC destinations for notifications.
	/// </summary>
	public string CcDestinations { get; set; } = string.Empty;

	/// <summary>
	/// The primary destination for notifications.
	/// </summary>
	public string Destination { get; set; } = string.Empty;

	/// <summary>
	/// A serialized list of notification destinations.
	/// </summary>
	public string Destinations { get; set; } = string.Empty;
}
