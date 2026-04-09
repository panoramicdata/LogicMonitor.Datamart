namespace LogicMonitor.Datamart.Models;

/// <summary>
/// Represents a LogicMonitor alert stored in the datamart.
/// </summary>
public class AlertStoreItem : StoreItem
{
	/// <summary>
	/// Navigation property to the associated alert rule.
	/// </summary>
	public AlertRuleStoreItem? AlertRule { get; set; }

	/// <summary>
	/// Foreign key to the associated alert rule.
	/// </summary>
	public Guid? AlertRuleId { get; set; }

	/// <summary>
	/// The LogicMonitor alert identifier.
	/// </summary>
	[MaxLength(20)]
	public string LogicMonitorId { get; set; } = string.Empty;

	/// <summary>
	/// The type of alert.
	/// </summary>
	public AlertType AlertType { get; set; }

	/// <summary>
	/// The LogicMonitor internal identifier for the alert.
	/// </summary>
	[MaxLength(20)]
	public string InternalId { get; set; } = string.Empty;

	/// <summary>
	/// The epoch timestamp (seconds) when the alert started.
	/// </summary>
	public int StartOnSeconds { get; set; }

	/// <summary>
	/// The epoch timestamp (seconds) when the alert ended, or 0 if still active.
	/// </summary>
	public int EndOnSeconds { get; set; }

	/// <summary>
	/// Whether the alert has been acknowledged.
	/// </summary>
	public bool Acked { get; set; }

	/// <summary>
	/// The epoch timestamp (seconds) when the alert was acknowledged.
	/// </summary>
	public int AckedOnSeconds { get; set; }

	/// <summary>
	/// The user who acknowledged the alert.
	/// </summary>
	[MaxLength(50)]
	public string AckedBy { get; set; } = string.Empty;

	/// <summary>
	/// The comment left when the alert was acknowledged.
	/// </summary>
	[MaxLength(50)]
	public string AckComment { get; set; } = string.Empty;

	/// <summary>
	/// The name of the alert rule that matched this alert.
	/// </summary>
	[MaxLength(50)]
	public string AlertRuleName { get; set; } = string.Empty;

	/// <summary>
	/// The name of the escalation chain for this alert.
	/// </summary>
	[MaxLength(50)]
	public string AlertEscalationChainName { get; set; } = string.Empty;

	/// <summary>
	/// The LogicMonitor identifier for the alert escalation chain, or null if unassigned.
	/// </summary>
	public int? AlertEscalationChainId { get; set; }

	/// <summary>
	/// The LogicMonitor identifier for the alert escalation sub-chain, or null if unassigned.
	/// </summary>
	public int? AlertEscalationSubChainId { get; set; }

	/// <summary>
	/// The index of the next recipient in the escalation chain.
	/// </summary>
	public int NextRecipient { get; set; }

	/// <summary>
	/// The comma-separated list of alert recipients.
	/// </summary>
	[MaxLength(200)]
	public string AlertRecipients { get; set; } = string.Empty;

	/// <summary>
	/// The alert severity level (1=Warning, 2=Error, 3=Critical, 4=Fatal).
	/// </summary>
	public int Severity { get; set; }

	/// <summary>
	/// Whether the alert has been cleared.
	/// </summary>
	public bool IsCleared { get; set; }

	/// <summary>
	/// Whether the alert occurred during a scheduled downtime window.
	/// </summary>
	public bool InScheduledDownTime { get; set; }

	/// <summary>
	/// The value that triggered the alert.
	/// </summary>
	[MaxLength(50)]
	public string Value { get; set; } = string.Empty;

	/// <summary>
	/// The threshold expression that triggered the alert.
	/// </summary>
	[MaxLength(50)]
	public string Thresholds { get; set; } = string.Empty;

	/// <summary>
	/// The value at the time the alert was cleared.
	/// </summary>
	[MaxLength(50)]
	public string ClearValue { get; set; } = string.Empty;

	/// <summary>
	/// The type of monitored object that generated the alert.
	/// </summary>
	public MonitoredObjectType MonitorObjectType { get; set; }

	/// <summary>
	/// The LogicMonitor identifier of the monitored object.
	/// </summary>
	public int? MonitorObjectId { get; set; }

	/// <summary>
	/// The name of the monitored object.
	/// </summary>
	[MaxLength(50)]
	public string MonitorObjectName { get; set; } = string.Empty;

	/// <summary>
	/// Navigation property to monitor object group at index 0.
	/// </summary>
	public MonitorObjectGroupStoreItem? MonitorObjectGroup0 { get; set; }

	/// <summary>
	/// Foreign key to monitor object group at index 0.
	/// </summary>
	public Guid? MonitorObjectGroup0Id { get; set; }

	/// <summary>
	/// Navigation property to monitor object group at index 1.
	/// </summary>
	public MonitorObjectGroupStoreItem? MonitorObjectGroup1 { get; set; }

	/// <summary>
	/// Foreign key to monitor object group at index 1.
	/// </summary>
	public Guid? MonitorObjectGroup1Id { get; set; }

	/// <summary>
	/// Navigation property to monitor object group at index 2.
	/// </summary>
	public MonitorObjectGroupStoreItem? MonitorObjectGroup2 { get; set; }

	/// <summary>
	/// Foreign key to monitor object group at index 2.
	/// </summary>
	public Guid? MonitorObjectGroup2Id { get; set; }

	/// <summary>
	/// Navigation property to monitor object group at index 3.
	/// </summary>
	public MonitorObjectGroupStoreItem? MonitorObjectGroup3 { get; set; }

	/// <summary>
	/// Foreign key to monitor object group at index 3.
	/// </summary>
	public Guid? MonitorObjectGroup3Id { get; set; }

	/// <summary>
	/// Navigation property to monitor object group at index 4.
	/// </summary>
	public MonitorObjectGroupStoreItem? MonitorObjectGroup4 { get; set; }

	/// <summary>
	/// Foreign key to monitor object group at index 4.
	/// </summary>
	public Guid? MonitorObjectGroup4Id { get; set; }

	/// <summary>
	/// Navigation property to monitor object group at index 5.
	/// </summary>
	public MonitorObjectGroupStoreItem? MonitorObjectGroup5 { get; set; }

	/// <summary>
	/// Foreign key to monitor object group at index 5.
	/// </summary>
	public Guid? MonitorObjectGroup5Id { get; set; }

	/// <summary>
	/// Navigation property to monitor object group at index 6.
	/// </summary>
	public MonitorObjectGroupStoreItem? MonitorObjectGroup6 { get; set; }

	/// <summary>
	/// Foreign key to monitor object group at index 6.
	/// </summary>
	public Guid? MonitorObjectGroup6Id { get; set; }

	/// <summary>
	/// Navigation property to monitor object group at index 7.
	/// </summary>
	public MonitorObjectGroupStoreItem? MonitorObjectGroup7 { get; set; }

	/// <summary>
	/// Foreign key to monitor object group at index 7.
	/// </summary>
	public Guid? MonitorObjectGroup7Id { get; set; }

	/// <summary>
	/// Navigation property to monitor object group at index 8.
	/// </summary>
	public MonitorObjectGroupStoreItem? MonitorObjectGroup8 { get; set; }

	/// <summary>
	/// Foreign key to monitor object group at index 8.
	/// </summary>
	public Guid? MonitorObjectGroup8Id { get; set; }

	/// <summary>
	/// Navigation property to monitor object group at index 9.
	/// </summary>
	public MonitorObjectGroupStoreItem? MonitorObjectGroup9 { get; set; }

	/// <summary>
	/// Foreign key to monitor object group at index 9.
	/// </summary>
	public Guid? MonitorObjectGroup9Id { get; set; }

	/// <summary>
	/// The LogicMonitor resource identifier associated with the alert.
	/// </summary>
	public int? ResourceId { get; set; }

	/// <summary>
	/// The LogicMonitor resource template (DataSource/EventSource) identifier.
	/// </summary>
	public int? ResourceTemplateId { get; set; }

	/// <summary>
	/// The type of resource template (e.g. DataSource).
	/// </summary>
	[MaxLength(10)]
	public string? ResourceTemplateType { get; set; }

	/// <summary>
	/// The name of the resource template.
	/// </summary>
	[MaxLength(50)]
	public string? ResourceTemplateName { get; set; }

	/// <summary>
	/// The LogicMonitor instance identifier that generated the alert.
	/// </summary>
	public int InstanceId { get; set; }

	/// <summary>
	/// The name of the instance that generated the alert.
	/// </summary>
	[MaxLength(50)]
	public string InstanceName { get; set; } = string.Empty;

	/// <summary>
	/// The description of the instance that generated the alert.
	/// </summary>
	[MaxLength(1000)]
	public string InstanceDescription { get; set; } = string.Empty;

	/// <summary>
	/// The name of the DataPoint that triggered the alert.
	/// </summary>
	[MaxLength(50)]
	public string DataPointName { get; set; } = string.Empty;

	/// <summary>
	/// The LogicMonitor DataPoint identifier that triggered the alert.
	/// </summary>
	public int DataPointId { get; set; }

	/// <summary>
	/// The subject of the detailed alert message.
	/// </summary>
	[MaxLength(200)]
	public string DetailMessageSubject { get; set; } = string.Empty;

	/// <summary>
	/// The body of the detailed alert message.
	/// </summary>
	[MaxLength(1000)]
	public string DetailMessageBody { get; set; } = string.Empty;

	/// <summary>
	/// Custom column 1 for user-defined alert metadata.
	/// </summary>
	[MaxLength(50)]
	public string CustomColumn1 { get; set; } = string.Empty;

	/// <summary>
	/// Custom column 2 for user-defined alert metadata.
	/// </summary>
	[MaxLength(50)]
	public string CustomColumn2 { get; set; } = string.Empty;

	/// <summary>
	/// Custom column 3 for user-defined alert metadata.
	/// </summary>
	[MaxLength(50)]
	public string CustomColumn3 { get; set; } = string.Empty;

	/// <summary>
	/// Custom column 4 for user-defined alert metadata.
	/// </summary>
	[MaxLength(50)]
	public string CustomColumn4 { get; set; } = string.Empty;

	/// <summary>
	/// Custom column 5 for user-defined alert metadata.
	/// </summary>
	[MaxLength(50)]
	public string CustomColumn5 { get; set; } = string.Empty;

	/// <summary>
	/// Whether anomaly-based alert suppression is enabled.
	/// </summary>
	[MaxLength(50)]
	public string EnableAnomalyAlertSuppression { get; set; } = string.Empty;

	/// <summary>
	/// Whether anomaly-based alert generation is enabled.
	/// </summary>
	[MaxLength(50)]
	public string EnableAnomalyAlertGeneration { get; set; } = string.Empty;

	/// <summary>
	/// The LogicMonitor tenant (account) name that originated the alert.
	/// </summary>
	[MaxLength(50)]
	public string Tenant { get; set; } = string.Empty;

	/// <summary>
	/// The dependency role
	/// </summary>
	[MaxLength(50)]
	public string? DependencyRole { get; set; }

	/// <summary>
	/// The dependency routing state
	/// </summary>
	[MaxLength(50)]
	public string? DependencyRoutingState { get; set; }

	/// <summary>
	/// Whether or not the alert is dynamic threshold based
	/// </summary>
	[MaxLength(50)]
	public bool IsActiveDiscoveryAlert { get; set; }

	/// <summary>
	/// The description for dynamic threshold based alert
	/// </summary>
	[MaxLength(50)]
	public string? ActiveDiscoveryAlertDescription { get; set; }

	/// <summary>
	/// Indicates the anomaly alert, value can be true/false/null. If alert value lies within confidence band then false, otherwise true. If confidence band is not available then value will be null.
	/// </summary>
	public bool IsAnomaly { get; set; }

	/// <summary>
	/// The component (Ex SDT, HostClusterAlert) which suppressed the alert
	/// </summary>
	[MaxLength(50)]
	public string? Suppressor { get; set; }

	/// <summary>
	/// The description for suppressed alert
	/// </summary>
	[MaxLength(50)]
	public string? SuppressedDescending { get; set; }
}
