namespace LogicMonitor.Datamart.Models;

public class LogStoreItem : StoreItem
{
	// Database properties
	[MaxLength(50)]
	public string Id { get; set; } = string.Empty;

	/// <summary>
	/// The user that performed the action
	/// </summary>
	[MaxLength(100)]
	public string UserName { get; set; } = string.Empty;

	/// <summary>
	/// The DateTime the event happened in seconds since the Epoch
	/// </summary>
	public long HappenedOnTimeStampUtc { get; set; }

	/// <summary>
	/// The session ID
	/// </summary>
	[MaxLength(50)]
	public string? SessionId { get; set; } = string.Empty;

	/// <summary>
	/// Event description
	/// </summary>
	public string Description { get; set; } = string.Empty;

	/// <summary>
	/// IP Address
	/// </summary>
	[MaxLength(200)]
	public string IpAddress { get; set; } = string.Empty;
}
