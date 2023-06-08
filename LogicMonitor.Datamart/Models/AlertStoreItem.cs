namespace LogicMonitor.Datamart.Models;

public class AlertStoreItem : StoreItem
{
	// Navigation properties
	public AlertRuleStoreItem? AlertRule { get; set; }

	public Guid? AlertRuleId { get; set; }

	// Database properties
	[MaxLength(20)]
	public string LogicMonitorId { get; set; } = string.Empty;

	public AlertType AlertType { get; set; }

	[MaxLength(20)]
	public string InternalId { get; set; } = string.Empty;

	public int StartOnSeconds { get; set; }

	public int EndOnSeconds { get; set; }

	public bool Acked { get; set; }

	public int AckedOnSeconds { get; set; }

	[MaxLength(50)]
	public string AckedBy { get; set; } = string.Empty;

	[MaxLength(50)]
	public string AckComment { get; set; } = string.Empty;

	[MaxLength(50)]
	public string AlertRuleName { get; set; } = string.Empty;

	[MaxLength(50)]
	public string AlertEscalationChainName { get; set; } = string.Empty;

	public int? AlertEscalationChainId { get; set; }

	public int? AlertEscalationSubChainId { get; set; }

	public int NextRecipient { get; set; }

	[MaxLength(200)]
	public string AlertRecipients { get; set; } = string.Empty;

	public int Severity { get; set; }

	public bool IsCleared { get; set; }

	public bool InScheduledDownTime { get; set; }

	[MaxLength(50)]
	public string Value { get; set; } = string.Empty;

	[MaxLength(50)]
	public string Thresholds { get; set; } = string.Empty;

	[MaxLength(50)]
	public string ClearValue { get; set; } = string.Empty;

	public MonitoredObjectType MonitorObjectType { get; set; }

	public int? MonitorObjectId { get; set; }

	[MaxLength(50)]
	public string MonitorObjectName { get; set; } = string.Empty;

	public MonitorObjectGroupStoreItem? MonitorObjectGroup0 { get; set; }
	public Guid? MonitorObjectGroup0Id { get; set; }

	public MonitorObjectGroupStoreItem? MonitorObjectGroup1 { get; set; }
	public Guid? MonitorObjectGroup1Id { get; set; }

	public MonitorObjectGroupStoreItem? MonitorObjectGroup2 { get; set; }
	public Guid? MonitorObjectGroup2Id { get; set; }

	public MonitorObjectGroupStoreItem? MonitorObjectGroup3 { get; set; }
	public Guid? MonitorObjectGroup3Id { get; set; }

	public MonitorObjectGroupStoreItem? MonitorObjectGroup4 { get; set; }
	public Guid? MonitorObjectGroup4Id { get; set; }

	public MonitorObjectGroupStoreItem? MonitorObjectGroup5 { get; set; }
	public Guid? MonitorObjectGroup5Id { get; set; }

	public MonitorObjectGroupStoreItem? MonitorObjectGroup6 { get; set; }
	public Guid? MonitorObjectGroup6Id { get; set; }

	public MonitorObjectGroupStoreItem? MonitorObjectGroup7 { get; set; }
	public Guid? MonitorObjectGroup7Id { get; set; }

	public MonitorObjectGroupStoreItem? MonitorObjectGroup8 { get; set; }
	public Guid? MonitorObjectGroup8Id { get; set; }

	public MonitorObjectGroupStoreItem? MonitorObjectGroup9 { get; set; }
	public Guid? MonitorObjectGroup9Id { get; set; }

	public int? ResourceId { get; set; }

	public int? ResourceTemplateId { get; set; }

	[MaxLength(10)]
	public string? ResourceTemplateType { get; set; }

	[MaxLength(50)]
	public string? ResourceTemplateName { get; set; }

	public int InstanceId { get; set; }

	[MaxLength(50)]
	public string InstanceName { get; set; } = string.Empty;

	[MaxLength(1000)]
	public string InstanceDescription { get; set; } = string.Empty;

	[MaxLength(50)]
	public string DataPointName { get; set; } = string.Empty;

	public int DataPointId { get; set; }

	[MaxLength(200)]
	public string DetailMessageSubject { get; set; } = string.Empty;

	[MaxLength(1000)]
	public string DetailMessageBody { get; set; } = string.Empty;

	[MaxLength(50)]
	public string CustomColumn1 { get; set; } = string.Empty;

	[MaxLength(50)]
	public string CustomColumn2 { get; set; } = string.Empty;

	[MaxLength(50)]
	public string CustomColumn3 { get; set; } = string.Empty;

	[MaxLength(50)]
	public string CustomColumn4 { get; set; } = string.Empty;

	[MaxLength(50)]
	public string CustomColumn5 { get; set; } = string.Empty;

	[MaxLength(50)]
	public string EnableAnomalyAlertSuppression { get; set; } = string.Empty;

	[MaxLength(50)]
	public string EnableAnomalyAlertGeneration { get; set; } = string.Empty;

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
