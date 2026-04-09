namespace LogicMonitor.Datamart.Models;

/// <summary>
/// Abstract base class for a specific instance of a LogicModule on a resource.
/// </summary>
public abstract class ResourceLogicModuleInstanceStoreItem : IdentifiedStoreItem
{
	/// <summary>
	/// The alert disable status for this instance.
	/// </summary>
	public AlertDisableStatus AlertDisableStatus { get; set; }

	/// <summary>
	/// The current alert status for this instance.
	/// </summary>
	public AlertStatus AlertStatus { get; set; }

	/// <summary>
	/// The alert status priority for ordering.
	/// </summary>
	public int AlertStatusPriority { get; set; }

	/// <summary>
	/// A description of the instance.
	/// </summary>
	public string Description { get; set; } = string.Empty;

	/// <summary>
	/// Whether alerting is disabled for this instance.
	/// </summary>
	public bool DisableAlerting { get; set; }

	/// <summary>
	/// The display name of the instance.
	/// </summary>
	public string DisplayName { get; set; } = string.Empty;

	/// <summary>
	/// The LogicMonitor group identifier for this instance.
	/// </summary>
	public int GroupId { get; set; }

	/// <summary>
	/// The name of the group this instance belongs to.
	/// </summary>
	public string GroupName { get; set; } = string.Empty;

	/// <summary>
	/// The epoch timestamp (seconds) of the last successful data collection.
	/// </summary>
	public long LastCollectedTimeSeconds { get; set; }

	/// <summary>
	/// The epoch timestamp (seconds) when the instance was last updated.
	/// </summary>
	public long LastUpdatedTimeSeconds { get; set; }

	/// <summary>
	/// Whether the description is locked and cannot be overwritten by active discovery.
	/// </summary>
	public bool LockDescription { get; set; }

	/// <summary>
	/// The unique name of this instance.
	/// </summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// Whether monitoring is stopped for this instance.
	/// </summary>
	public bool StopMonitoring { get; set; }

	/// <summary>
	/// The current Scheduled Down Time (SDT) status.
	/// </summary>
	public SdtStatus SdtStatus { get; set; }

	/// <summary>
	/// The SDT schedule details, if any.
	/// </summary>
	public string? SdtAt { get; set; }

	/// <summary>
	/// The primary wild value used for active discovery instance identification.
	/// </summary>
	public string WildValue { get; set; } = string.Empty;

	/// <summary>
	/// The secondary wild value used for active discovery instance identification.
	/// </summary>
	public string WildValue2 { get; set; } = string.Empty;

	/// <summary>
	/// If present, this is the UTC timestamp when a dimension update query was made to LogicMonitor and this instance was not returned
	/// </summary>
	public DateTimeOffset? LastWentMissing { get; set; }

	/// <summary>
	/// InstanceProperty 1
	/// </summary>
	public string InstanceProperty1 { get; set; } = string.Empty;

	/// <summary>
	/// InstanceProperty 2
	/// </summary>
	public string InstanceProperty2 { get; set; } = string.Empty;

	/// <summary>
	/// InstanceProperty 3
	/// </summary>
	public string InstanceProperty3 { get; set; } = string.Empty;

	/// <summary>
	/// InstanceProperty 4
	/// </summary>
	public string InstanceProperty4 { get; set; } = string.Empty;

	/// <summary>
	/// InstanceProperty 5
	/// </summary>
	public string InstanceProperty5 { get; set; } = string.Empty;

	/// <summary>
	/// InstanceProperty 6
	/// </summary>
	public string InstanceProperty6 { get; set; } = string.Empty;

	/// <summary>
	/// InstanceProperty 7
	/// </summary>
	public string InstanceProperty7 { get; set; } = string.Empty;

	/// <summary>
	/// InstanceProperty 8
	/// </summary>
	public string InstanceProperty8 { get; set; } = string.Empty;

	/// <summary>
	/// InstanceProperty 9
	/// </summary>
	public string InstanceProperty9 { get; set; } = string.Empty;

	/// <summary>
	/// InstanceProperty 10
	/// </summary>
	public string InstanceProperty10 { get; set; } = string.Empty;
}
