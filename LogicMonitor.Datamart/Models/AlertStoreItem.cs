namespace LogicMonitor.Datamart.Models;

public class AlertStoreItem : StoreItem
{
	// Navigation properties
	public AlertRuleStoreItem AlertRule { get; set; } = null!;

	// Database properties
	[MaxLength(20)]
	public string Id { get; set; } = string.Empty;

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

	public int? AlertRuleId { get; set; }

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

	public MonitorObjectGroupStoreItem MonitorObjectGroup0 { get; set; }
	public int? MonitorObjectGroup0Id { get; set; }

	public MonitorObjectGroupStoreItem MonitorObjectGroup1 { get; set; }
	public int? MonitorObjectGroup1Id { get; set; }

	public MonitorObjectGroupStoreItem MonitorObjectGroup2 { get; set; }
	public int? MonitorObjectGroup2Id { get; set; }

	public MonitorObjectGroupStoreItem MonitorObjectGroup3 { get; set; }
	public int? MonitorObjectGroup3Id { get; set; }

	public MonitorObjectGroupStoreItem MonitorObjectGroup4 { get; set; }
	public int? MonitorObjectGroup4Id { get; set; }

	public MonitorObjectGroupStoreItem MonitorObjectGroup5 { get; set; }
	public int? MonitorObjectGroup5Id { get; set; }

	public MonitorObjectGroupStoreItem MonitorObjectGroup6 { get; set; }
	public int? MonitorObjectGroup6Id { get; set; }

	public MonitorObjectGroupStoreItem MonitorObjectGroup7 { get; set; }
	public int? MonitorObjectGroup7Id { get; set; }

	public MonitorObjectGroupStoreItem MonitorObjectGroup8 { get; set; }
	public int? MonitorObjectGroup8Id { get; set; }

	public MonitorObjectGroupStoreItem MonitorObjectGroup9 { get; set; }
	public int? MonitorObjectGroup9Id { get; set; }

	public int? ResourceId { get; set; }

	public int? ResourceTemplateId { get; set; }

	[MaxLength(10)]
	public string ResourceTemplateType { get; set; } = string.Empty;

	[MaxLength(50)]
	public string ResourceTemplateName { get; set; } = string.Empty;

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
}
