namespace LogicMonitor.Datamart.Models;

public class DeviceStoreItem : IdentifiedStoreItem
{
	// Navigation properties
	public virtual ICollection<DeviceDataSourceStoreItem>? DeviceDataSources { get; set; }

	/// <summary>
	/// Nullable to allow for cloud collectors
	/// </summary>
	public Guid? PreferredCollectorId { get; set; }

	public CollectorStoreItem? PreferredCollector { get; set; }

	// Database properties
	public string Name { get; set; } = string.Empty;

	public string Description { get; set; } = string.Empty;

	public AlertDisableStatus AlertDisableStatus { get; set; }

	public int AlertStatusPriority { get; set; }

	public AlertStatus AlertStatus { get; set; }

	public bool AncestorHasDisabledLogicModule { get; set; }

	public long? AutoPropertiesAssignedOnSeconds { get; set; }

	public long? AutoPropertiesUpdatedOnSeconds { get; set; }

	public AwsState AwsState { get; set; }

	public AzureState AzureState { get; set; }

	public AzureState GcpState { get; set; }

	public bool CanUseRemoteSession { get; set; }

	public string CollectorDescription { get; set; } = string.Empty;

	public long? CreatedOnSeconds { get; set; }

	public int CurrentCollectorId { get; set; }

	public long DeletedTimeInMs { get; set; }

	public DeviceType DeviceType { get; set; }

	public bool IsAlertingDisabled { get; set; }

	public string DisplayName { get; set; } = string.Empty;

	public bool EffectiveAlertEnabled { get; set; }

	public bool EnableNetflow { get; set; }

	public bool HasActiveInstance { get; set; }

	public bool HasDisabledSubResource { get; set; }

	public bool HasMore { get; set; }

	public string DeviceGroupIdsString { get; set; } = string.Empty;

	public Level DeviceStatus { get; set; }

	public long? LastDataTimeSeconds { get; set; }

	public long? LastRawDataTimeSeconds { get; set; }

	public string? Link { get; set; }

	public int NetflowCollectorId { get; set; }

	public string? NetflowCollectorDescription { get; set; }

	public int NetflowCollectorGroupId { get; set; }

	public string? NetflowCollectorGroupName { get; set; }

	public int RelatedDeviceId { get; set; }

	public int ScanConfigId { get; set; }

	public SdtStatus SdtStatus { get; set; }

	public long ToDeleteTimeInMs { get; set; }

	public int UptimeInSeconds { get; set; }

	public long? UpdatedOnSeconds { get; set; }

	public UserPermission UserPermission { get; set; }

	public long LastAlertClosedTimeSeconds { get; set; }

	// These properties are used for storing extra bits in

	public string? Property1 { get; set; } = string.Empty;

	public string? Property2 { get; set; } = string.Empty;

	public string? Property3 { get; set; } = string.Empty;

	public string? Property4 { get; set; } = string.Empty;

	public string? Property5 { get; set; } = string.Empty;

	public string? Property6 { get; set; } = string.Empty;

	public string? Property7 { get; set; } = string.Empty;

	public string? Property8 { get; set; } = string.Empty;

	public string? Property9 { get; set; } = string.Empty;

	public string? Property10 { get; set; } = string.Empty;

	public string? Property11 { get; set; } = string.Empty;

	public string? Property12 { get; set; } = string.Empty;

	public string? Property13 { get; set; } = string.Empty;

	public string? Property14 { get; set; } = string.Empty;

	public string? Property15 { get; set; } = string.Empty;

	public string? Property16 { get; set; } = string.Empty;

	public string? Property17 { get; set; } = string.Empty;

	public string? Property18 { get; set; } = string.Empty;

	public string? Property19 { get; set; } = string.Empty;

	public string? Property20 { get; set; } = string.Empty;

	public long? LastTimeSeriesDataSyncDurationMs { get; set; }
}
