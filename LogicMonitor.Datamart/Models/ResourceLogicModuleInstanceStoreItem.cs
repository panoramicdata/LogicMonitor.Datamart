namespace LogicMonitor.Datamart.Models;

public abstract class ResourceLogicModuleInstanceStoreItem : IdentifiedStoreItem
{
	// Database properties
	public AlertDisableStatus AlertDisableStatus { get; set; }

	public AlertStatus AlertStatus { get; set; }

	public int AlertStatusPriority { get; set; }

	public string Description { get; set; } = string.Empty;

	public bool DisableAlerting { get; set; }

	public string DisplayName { get; set; } = string.Empty;

	public int GroupId { get; set; }

	public string GroupName { get; set; } = string.Empty;

	public long LastCollectedTimeSeconds { get; set; }

	public long LastUpdatedTimeSeconds { get; set; }

	public bool LockDescription { get; set; }

	public string Name { get; set; } = string.Empty;

	public bool StopMonitoring { get; set; }

	public SdtStatus SdtStatus { get; set; }

	public string? SdtAt { get; set; }

	public string WildValue { get; set; } = string.Empty;

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
