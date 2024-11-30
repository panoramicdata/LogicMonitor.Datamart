namespace LogicMonitor.Datamart.Models;

public class AuditEventStoreItem : StoreItem
{
	/// <summary>
	/// The original LogItem ID
	/// </summary>
	[MaxLength(50)]
	public required string LogicMonitorId { get; set; }

	/// <summary>
	/// When the event occurred
	/// </summary>
	public DateTimeOffset DateTime { get; set; }

	/// <summary>
	/// The host that initiated the event
	/// </summary>
	[MaxLength(50)]
	public string? Host { get; set; }

	/// <summary>
	/// The original description from the LogItem
	/// </summary>
	[MaxLength(1000)]
	public string? OriginalDescription { get; set; }

	/// <summary>
	/// The event's session Id
	/// </summary>
	[MaxLength(50)]
	public string? SessionId { get; set; }

	/// <summary>
	/// The originator type
	/// </summary>
	[MaxLength(50)]
	public required string OriginatorType { get; set; }

	/// <summary>
	/// The entity type
	/// </summary>
	[MaxLength(50)]
	public required string EntityType { get; set; }

	/// <summary>
	/// The action type
	/// </summary>
	[MaxLength(50)]
	public required string ActionType { get; set; }

	/// <summary>
	/// The outcome type
	/// </summary>
	[MaxLength(50)]
	public required string OutcomeType { get; set; }

	/// <summary>
	/// The resource ids
	/// </summary>
	[MaxLength(50)]
	public string? ResourceIds { get; set; }

	/// <summary>
	/// The resource name
	/// </summary>
	[MaxLength(200)]
	public string? ResourceNames { get; set; }

	/// <summary>
	/// The LogicModule id
	/// </summary>
	public int? LogicModuleId { get; set; }

	/// <summary>
	/// The LogicModule name
	/// </summary>
	[MaxLength(50)]
	public string? LogicModuleName { get; set; }

	/// <summary>
	/// The LogicModule version
	/// </summary>
	public int? LogicModuleVersion { get; set; }

	/// <summary>
	/// The Instance id
	/// </summary>
	public int? InstanceId { get; set; }

	/// <summary>
	/// The Instance name
	/// </summary>
	[MaxLength(50)]
	public string? InstanceName { get; set; }

	/// <summary>
	/// The collector id
	/// </summary>
	public int? CollectorId { get; set; }

	/// <summary>
	/// The collector name
	/// </summary>
	[MaxLength(50)]
	public string? CollectorName { get; set; }

	/// <summary>
	/// The collector description
	/// </summary>
	[MaxLength(50)]
	public string? CollectorDescription { get; set; }

	/// <summary>
	/// Description
	/// </summary>
	[MaxLength(1000)]
	public string? Description { get; set; }

	/// <summary>
	/// The API Token Id
	/// </summary>
	[MaxLength(50)]
	public string? ApiTokenId { get; set; }

	/// <summary>
	/// The API Path
	/// </summary>
	[MaxLength(50)]
	public string? ApiPath { get; set; }

	/// <summary>
	/// The API Method
	/// </summary>
	[MaxLength(50)]
	public string? ApiMethod { get; set; }

	/// <summary>
	/// The DataSource new instance ids
	/// </summary>
	[MaxLength(50)]
	public string? DataSourceNewInstanceIds { get; set; }

	/// <summary>
	/// The DataSource new instance names
	/// </summary>
	[MaxLength(200)]
	public string? DataSourceNewInstanceNames { get; set; }

	/// <summary>
	/// The DataSource deleted instance ids
	/// </summary>
	[MaxLength(50)]
	public string? DataSourceDeletedInstanceIds { get; set; }

	/// <summary>
	/// The DataSource deleted instance names
	/// </summary>
	[MaxLength(200)]
	public string? DataSourceDeletedInstanceNames { get; set; }

	/// <summary>
	/// The ResourceGroup name
	/// </summary>
	[MaxLength(100)]
	public string? ResourceGroupName { get; set; }

	/// <summary>
	/// The ResourceGroup id
	/// </summary>
	public int? ResourceGroupId { get; set; }

	/// <summary>
	/// The Property Name
	/// </summary>
	[MaxLength(50)]
	public string? PropertyName { get; set; }

	/// <summary>
	/// The DeviceDataSource Id
	/// </summary>
	public int? DeviceDataSourceId { get; set; }

	/// <summary>
	/// The Id of the regex that resulted in the translation from the LogItem
	/// -1 means a regex match was not made
	/// </summary>
	public int MatchedRegExId { get; set; } = -1;

	/// <summary>
	/// The Property Value
	/// </summary>
	[MaxLength(100)]
	public string? PropertyValue { get; set; }

	/// <summary>
	/// Time
	/// </summary>
	[MaxLength(50)]
	public string? Time { get; set; }

	/// <summary>
	/// The WildValue - available when a DeviceDataSourceInstance was added without Ids
	/// </summary>
	[MaxLength(50)]
	public string? WildValue { get; set; }

	/// <summary>
	/// The login name for a login event
	/// </summary>
	[MaxLength(50)]
	public string? PerformedByUsername { get; set; }

	/// <summary>
	/// The user name for an event
	/// </summary>
	[MaxLength(50)]
	public string? UserName { get; set; }

	/// <summary>
	/// The email address
	/// </summary>
	[MaxLength(50)]
	public string? UserEmail { get; set; }

	/// <summary>
	/// The user id
	/// </summary>
	public int? UserId { get; set; }

	/// <summary>
	/// The role name for role events
	/// </summary>
	[MaxLength(50)]
	public string? UserRole { get; set; }

	/// <summary>
	/// The alert id for alert update events
	/// </summary>
	[MaxLength(50)]
	public string? AlertId { get; set; }

	/// <summary>
	/// The alert note for alert update events
	/// </summary>
	[MaxLength(50)]
	public string? AlertNote { get; set; }

	/// <summary>
	/// The regular device monthly metrics
	/// </summary>
	public long? MonthlyMetrics { get; set; }

	/// <summary>
	/// The scheduled down time start time
	/// </summary>
	[MaxLength(50)]
	public string? StartDownTime { get; set; }

	/// <summary>
	/// The scheduled down time end time
	/// </summary>
	[MaxLength(50)]
	public string? EndDownTime { get; set; }

	/// <summary>
	/// The Collector command that was run
	/// </summary>
	[MaxLength(50)]
	public string? Command { get; set; }

	/// <summary>
	/// Resource host name
	/// </summary>
	[MaxLength(50)]
	public string? ResourceHostname { get; set; }

	/// <summary>
	/// Remote Session ID
	/// </summary>
	[MaxLength(50)]
	public long? RemoteSessionId { get; set; }

	/// <summary>
	/// Remote Session Type
	/// </summary>
	[MaxLength(50)]
	public string? RemoteSessionType { get; set; }

	/// <summary>
	/// Restrict SSO
	/// </summary>
	[MaxLength(50)]
	public bool? RestrictSso { get; set; }

	/// <summary>
	/// Collector Group Name
	/// </summary>
	[MaxLength(50)]
	public string? CollectorGroupName { get; set; }

	/// <summary>
	/// Collector Group Id
	/// </summary>
	public int? CollectorGroupId { get; set; }

	/// <summary>
	/// RequestId
	/// </summary>
	public long? RequestId { get; set; }
}
