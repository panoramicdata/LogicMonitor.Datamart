namespace LogicMonitor.Datamart.Models;

public class ResourceConfigSourceInstanceConfigStoreItem : IdentifiedStoreItem
{
	// Navigation properties
	public ResourceConfigSourceInstanceStoreItem? DeviceConfigSourceInstance { get; set; }

	public Guid DeviceConfigSourceInstanceId { get; set; }

	// Database properties
	public string Config { get; set; } = string.Empty;

	public string LogicMonitorStringId { get; set; } = string.Empty;

	public DateTimeOffset PollUtc { get; set; }

	public string ChangeStatus { get; set; } = string.Empty;

	public int ConfigStatus { get; set; }

	public string ConfigErrorMessage { get; set; } = string.Empty;

	public string Version { get; set; } = string.Empty;
}