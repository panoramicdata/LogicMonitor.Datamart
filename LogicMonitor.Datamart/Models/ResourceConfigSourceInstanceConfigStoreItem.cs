namespace LogicMonitor.Datamart.Models;

/// <summary>
/// Represents a configuration snapshot collected from a ConfigSource instance stored in the datamart.
/// </summary>
public class ResourceConfigSourceInstanceConfigStoreItem : IdentifiedStoreItem
{
	/// <summary>
	/// Navigation property to the parent ConfigSource instance.
	/// </summary>
	public ResourceConfigSourceInstanceStoreItem? DeviceConfigSourceInstance { get; set; }

	/// <summary>
	/// The datamart identifier of the parent ConfigSource instance.
	/// </summary>
	public Guid DeviceConfigSourceInstanceId { get; set; }

	/// <summary>
	/// The configuration content collected in this snapshot.
	/// </summary>
	public string Config { get; set; } = string.Empty;

	/// <summary>
	/// The LogicMonitor string identifier for this configuration snapshot.
	/// </summary>
	public string LogicMonitorStringId { get; set; } = string.Empty;

	/// <summary>
	/// The UTC timestamp when the configuration was polled.
	/// </summary>
	public DateTimeOffset PollUtc { get; set; }

	/// <summary>
	/// The change status indicating whether the configuration changed since the last poll.
	/// </summary>
	public string ChangeStatus { get; set; } = string.Empty;

	/// <summary>
	/// The numeric status code of the configuration collection.
	/// </summary>
	public int ConfigStatus { get; set; }

	/// <summary>
	/// The error message if configuration collection failed.
	/// </summary>
	public string ConfigErrorMessage { get; set; } = string.Empty;

	/// <summary>
	/// The version identifier for the configuration snapshot.
	/// </summary>
	public string Version { get; set; } = string.Empty;
}