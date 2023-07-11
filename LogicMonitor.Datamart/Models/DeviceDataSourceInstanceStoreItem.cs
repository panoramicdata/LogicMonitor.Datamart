namespace LogicMonitor.Datamart.Models;

public class DeviceDataSourceInstanceStoreItem : IdentifiedStoreItem
{
	// Navigation properties
	public DeviceDataSourceStoreItem? DeviceDataSource { get; set; }

	public Guid DeviceDataSourceId { get; set; }

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

	public bool Name { get; set; }

	public bool StopMonitoring { get; set; }

	public SdtStatus SdtStatus { get; set; }

	public string? SdtAt { get; set; }

	public string WildValue { get; set; } = string.Empty;

	public string WildValue2 { get; set; } = string.Empty;

	/// <summary>
	/// If present, this is the UTC timestamp when a dimension update query was made to LogicMonitor and this instance was not returned
	/// </summary>
	public DateTimeOffset? LastWentMissing { get; set; }

	public ICollection<DeviceDataSourceInstanceDataPointStoreItem> DeviceDataSourceInstanceDataPoints { get; set; } = null!;
}
