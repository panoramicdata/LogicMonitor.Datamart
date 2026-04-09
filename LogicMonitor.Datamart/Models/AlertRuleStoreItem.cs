namespace LogicMonitor.Datamart.Models;

/// <summary>
/// Represents a LogicMonitor alert rule stored in the datamart.
/// </summary>
public class AlertRuleStoreItem : IdentifiedStoreItem
{
	/// <summary>
	/// Navigation property to the alerts matched by this rule.
	/// </summary>
	public virtual ICollection<AlertStoreItem>? AlertStoreItems { get; set; }

	/// <summary>
	/// Navigation property to the associated escalation chain.
	/// </summary>
	public EscalationChainStoreItem? EscalationChain { get; set; }

	/// <summary>
	/// Foreign key to the associated escalation chain.
	/// </summary>
	public Guid EscalationChainId { get; set; }

	/// <summary>
	/// The name of the alert rule.
	/// </summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// A description of the alert rule.
	/// </summary>
	public string? Description { get; set; } = string.Empty;

	/// <summary>
	/// The priority of the alert rule for evaluation ordering.
	/// </summary>
	public int Priority { get; set; }

	/// <summary>
	/// The alert severity level filter as a string representation.
	/// </summary>
	public string LevelString { get; set; } = string.Empty;

	/// <summary>
	/// The DataSource name filter for the alert rule.
	/// </summary>
	public string DataSourceName { get; set; } = string.Empty;

	/// <summary>
	/// The DataSource instance name filter for the alert rule.
	/// </summary>
	public string DataSourceInstanceName { get; set; } = string.Empty;

	/// <summary>
	/// The DataPoint filter for the alert rule.
	/// </summary>
	public string DataPoint { get; set; } = string.Empty;

	/// <summary>
	/// The interval in minutes between escalation chain notifications.
	/// </summary>
	public int EscalationChainIntervalMinutes { get; set; }

	/// <summary>
	/// Whether to suppress clear notifications for this alert rule.
	/// </summary>
	public bool SuppressAlertClear { get; set; }

	/// <summary>
	/// Whether to suppress acknowledgement and SDT notifications for this alert rule.
	/// </summary>
	public bool SuppressAlertAckSdt { get; set; }
}
