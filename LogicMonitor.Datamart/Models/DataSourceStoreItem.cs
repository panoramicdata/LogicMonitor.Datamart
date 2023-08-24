namespace LogicMonitor.Datamart.Models;

public class DataSourceStoreItem : IdentifiedStoreItem
{
	// Navigation properties
	public ICollection<DeviceDataSourceStoreItem>? DeviceDataSources { get; set; } = null!;

	public ICollection<DeviceDataSourceInstanceDataPointStoreItem>? DataPoints { get; set; } = null!;

	// Database properties
	public string Name { get; set; } = string.Empty;

	public string Description { get; set; } = string.Empty;

	public string AppliesTo { get; set; } = string.Empty;

	public string CollectionMethod { get; set; } = string.Empty;

	public string AuditVersion { get; set; } = string.Empty;

	public string Version { get; set; } = string.Empty;

	public string DisplayName { get; set; } = string.Empty;

	public bool HasMultiInstances { get; set; }

	public int PollingIntervalSeconds { get; set; }

	public string Technology { get; set; } = string.Empty;

	public long? LastTimeSeriesDataSyncDurationMs { get; set; }
}
