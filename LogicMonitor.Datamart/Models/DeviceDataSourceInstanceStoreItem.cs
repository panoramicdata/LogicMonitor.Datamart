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

	public bool DisableAlerting { get; set; }

	public string DisplayName { get; set; } = string.Empty;

	public int GroupId { get; set; }

	public string GroupName { get; set; } = string.Empty;

	public long LastCollectedTimeSeconds { get; set; }

	public long LastUpdatedTimeSeconds { get; set; }

	public bool LockDescription { get; set; }

	public bool StopMonitoring { get; set; }

	public SdtStatus SdtStatus { get; set; }

	public string? SdtAt { get; set; }

	public string WildValue { get; set; } = string.Empty;

	public string WildValue2 { get; set; } = string.Empty;

	/// <summary>
	/// The last hour for which we have written complete aggregations for this
	/// </summary>
	public DateTime? DataCompleteToUtc { get; set; }

	/// <summary>
	/// If present, this is the UTC timestamp when a dimension update query was made to LogicMonitor and this instance was not returned
	/// </summary>
	public DateTime? LastWentMissingUtc { get; set; }

	/// <summary>
	/// The effective alert expression, which is the alert expression from the DataSource, or the parent DeviceGroup or the Instance, in that order
	/// Note that timezones and time-dependent expressions are not supported
	/// </summary>
	public string EffectiveAlertExpression { get; set; } = string.Empty;
}
