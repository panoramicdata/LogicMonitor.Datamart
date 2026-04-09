namespace LogicMonitor.Datamart.Models;

/// <summary>
/// Represents a LogicMonitor resource (device) stored in the datamart.
/// </summary>
public class ResourceStoreItem : IdentifiedStoreItem
{
	/// <summary>
	/// Navigation property to the resource-DataSource assignments.
	/// </summary>
	public virtual ICollection<ResourceDataSourceStoreItem>? DeviceDataSources { get; set; }

	/// <summary>
	/// Nullable to allow for cloud collectors.
	/// </summary>
	public Guid? PreferredCollectorId { get; set; }

	/// <summary>
	/// Navigation property to the preferred collector for this resource.
	/// </summary>
	public CollectorStoreItem? PreferredCollector { get; set; }

	/// <summary>
	/// The resource name.
	/// </summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// A description of the resource.
	/// </summary>
	public string Description { get; set; } = string.Empty;

	/// <summary>
	/// The alert disable status for this resource.
	/// </summary>
	public AlertDisableStatus AlertDisableStatus { get; set; }

	/// <summary>
	/// The alert status priority for ordering.
	/// </summary>
	public int AlertStatusPriority { get; set; }

	/// <summary>
	/// The current alert status for the resource.
	/// </summary>
	public AlertStatus AlertStatus { get; set; }

	/// <summary>
	/// Whether an ancestor resource group has a disabled LogicModule.
	/// </summary>
	public bool AncestorHasDisabledLogicModule { get; set; }

	/// <summary>
	/// The epoch timestamp (seconds) when auto-properties were assigned.
	/// </summary>
	public long? AutoPropertiesAssignedOnSeconds { get; set; }

	/// <summary>
	/// The epoch timestamp (seconds) when auto-properties were last updated.
	/// </summary>
	public long? AutoPropertiesUpdatedOnSeconds { get; set; }

	/// <summary>
	/// The AWS state of the resource.
	/// </summary>
	public AwsState AwsState { get; set; }

	/// <summary>
	/// The Azure state of the resource.
	/// </summary>
	public AzureState AzureState { get; set; }

	/// <summary>
	/// The GCP state of the resource.
	/// </summary>
	public AzureState GcpState { get; set; }

	/// <summary>
	/// Whether a remote session can be used with this resource.
	/// </summary>
	public bool CanUseRemoteSession { get; set; }

	/// <summary>
	/// The description of the collector assigned to this resource.
	/// </summary>
	public string CollectorDescription { get; set; } = string.Empty;

	/// <summary>
	/// The epoch timestamp (seconds) when the resource was created.
	/// </summary>
	public long? CreatedOnSeconds { get; set; }

	/// <summary>
	/// The LogicMonitor identifier of the currently active collector.
	/// </summary>
	public int CurrentCollectorId { get; set; }

	/// <summary>
	/// The epoch timestamp (milliseconds) when the resource was deleted, or 0 if active.
	/// </summary>
	public long DeletedTimeInMs { get; set; }

	/// <summary>
	/// The type of the resource (e.g. regular, AWS, Azure).
	/// </summary>
	public ResourceType DeviceType { get; set; }

	/// <summary>
	/// Whether alerting is disabled for this resource.
	/// </summary>
	public bool IsAlertingDisabled { get; set; }

	/// <summary>
	/// The display name of the resource.
	/// </summary>
	public string DisplayName { get; set; } = string.Empty;

	/// <summary>
	/// Whether alerting is effectively enabled (considering inherited settings).
	/// </summary>
	public bool EffectiveAlertEnabled { get; set; }

	/// <summary>
	/// Whether NetFlow collection is enabled.
	/// </summary>
	public bool EnableNetflow { get; set; }

	/// <summary>
	/// Whether the resource has an active monitoring instance.
	/// </summary>
	public bool HasActiveInstance { get; set; }

	/// <summary>
	/// Whether the resource has a disabled sub-resource.
	/// </summary>
	public bool HasDisabledSubResource { get; set; }

	/// <summary>
	/// Whether there are more items available in the API response.
	/// </summary>
	public bool HasMore { get; set; }

	/// <summary>
	/// Comma-separated list of resource group identifiers this resource belongs to.
	/// </summary>
	public string DeviceGroupIdsString { get; set; } = string.Empty;

	/// <summary>
	/// The current operational status level of the resource.
	/// </summary>
	public Level DeviceStatus { get; set; }

	/// <summary>
	/// The epoch timestamp (seconds) of the last data received.
	/// </summary>
	public long? LastDataTimeSeconds { get; set; }

	/// <summary>
	/// The epoch timestamp (seconds) of the last raw data received.
	/// </summary>
	public long? LastRawDataTimeSeconds { get; set; }

	/// <summary>
	/// An optional link associated with the resource.
	/// </summary>
	public string? Link { get; set; }

	/// <summary>
	/// The LogicMonitor identifier of the NetFlow collector.
	/// </summary>
	public int NetflowCollectorId { get; set; }

	/// <summary>
	/// The description of the NetFlow collector.
	/// </summary>
	public string? NetflowCollectorDescription { get; set; }

	/// <summary>
	/// The LogicMonitor identifier of the NetFlow collector group.
	/// </summary>
	public int NetflowCollectorGroupId { get; set; }

	/// <summary>
	/// The name of the NetFlow collector group.
	/// </summary>
	public string? NetflowCollectorGroupName { get; set; }

	/// <summary>
	/// The LogicMonitor identifier of a related device.
	/// </summary>
	public int RelatedDeviceId { get; set; }

	/// <summary>
	/// The scan configuration identifier.
	/// </summary>
	public int ScanConfigId { get; set; }

	/// <summary>
	/// The current Scheduled Down Time (SDT) status.
	/// </summary>
	public SdtStatus SdtStatus { get; set; }

	/// <summary>
	/// The epoch timestamp (milliseconds) when the resource is scheduled for deletion.
	/// </summary>
	public long ToDeleteTimeInMs { get; set; }

	/// <summary>
	/// The resource uptime in seconds.
	/// </summary>
	public int UptimeInSeconds { get; set; }

	/// <summary>
	/// The epoch timestamp (seconds) when the resource was last updated.
	/// </summary>
	public long? UpdatedOnSeconds { get; set; }

	/// <summary>
	/// The user permission level for this resource.
	/// </summary>
	public UserPermission UserPermission { get; set; }

	/// <summary>
	/// The epoch timestamp (seconds) when the last alert was closed on this resource.
	/// </summary>
	public long LastAlertClosedTimeSeconds { get; set; }

	/// <summary>
	/// Custom resource property 1 (mapped from configured DeviceProperties).
	/// </summary>
	public string? Property1 { get; set; } = string.Empty;

	/// <summary>
	/// Custom resource property 2 (mapped from configured DeviceProperties).
	/// </summary>
	public string? Property2 { get; set; } = string.Empty;

	/// <summary>
	/// Custom resource property 3 (mapped from configured DeviceProperties).
	/// </summary>
	public string? Property3 { get; set; } = string.Empty;

	/// <summary>
	/// Custom resource property 4 (mapped from configured DeviceProperties).
	/// </summary>
	public string? Property4 { get; set; } = string.Empty;

	/// <summary>
	/// Custom resource property 5 (mapped from configured DeviceProperties).
	/// </summary>
	public string? Property5 { get; set; } = string.Empty;

	/// <summary>
	/// Custom resource property 6 (mapped from configured DeviceProperties).
	/// </summary>
	public string? Property6 { get; set; } = string.Empty;

	/// <summary>
	/// Custom resource property 7 (mapped from configured DeviceProperties).
	/// </summary>
	public string? Property7 { get; set; } = string.Empty;

	/// <summary>
	/// Custom resource property 8 (mapped from configured DeviceProperties).
	/// </summary>
	public string? Property8 { get; set; } = string.Empty;

	/// <summary>
	/// Custom resource property 9 (mapped from configured DeviceProperties).
	/// </summary>
	public string? Property9 { get; set; } = string.Empty;

	/// <summary>
	/// Custom resource property 10 (mapped from configured DeviceProperties).
	/// </summary>
	public string? Property10 { get; set; } = string.Empty;

	/// <summary>
	/// Custom resource property 11 (mapped from configured DeviceProperties).
	/// </summary>
	public string? Property11 { get; set; } = string.Empty;

	/// <summary>
	/// Custom resource property 12 (mapped from configured DeviceProperties).
	/// </summary>
	public string? Property12 { get; set; } = string.Empty;

	/// <summary>
	/// Custom resource property 13 (mapped from configured DeviceProperties).
	/// </summary>
	public string? Property13 { get; set; } = string.Empty;

	/// <summary>
	/// Custom resource property 14 (mapped from configured DeviceProperties).
	/// </summary>
	public string? Property14 { get; set; } = string.Empty;

	/// <summary>
	/// Custom resource property 15 (mapped from configured DeviceProperties).
	/// </summary>
	public string? Property15 { get; set; } = string.Empty;

	/// <summary>
	/// Custom resource property 16 (mapped from configured DeviceProperties).
	/// </summary>
	public string? Property16 { get; set; } = string.Empty;

	/// <summary>
	/// Custom resource property 17 (mapped from configured DeviceProperties).
	/// </summary>
	public string? Property17 { get; set; } = string.Empty;

	/// <summary>
	/// Custom resource property 18 (mapped from configured DeviceProperties).
	/// </summary>
	public string? Property18 { get; set; } = string.Empty;

	/// <summary>
	/// Custom resource property 19 (mapped from configured DeviceProperties).
	/// </summary>
	public string? Property19 { get; set; } = string.Empty;

	/// <summary>
	/// Custom resource property 20 (mapped from configured DeviceProperties).
	/// </summary>
	public string? Property20 { get; set; } = string.Empty;

	/// <summary>
	/// The duration in milliseconds of the last time-series data sync for this resource.
	/// </summary>
	public long? LastTimeSeriesDataSyncDurationMs { get; set; }
}
