namespace LogicMonitor.Datamart.Models;

public class LogStoreItem : StoreItem
{
	// Database properties
	[MaxLength(50)]
	public string Id { get; set; }

	/// <summary>
	/// The user that performed the action
	/// </summary>
	[MaxLength(100)]
	public string UserName { get; set; }

	/// <summary>
	/// The DateTime the event happened in seconds since the Epoch
	/// </summary>
	public long HappenedOnTimeStampUtc { get; set; }

	/// <summary>
	/// The session ID
	/// </summary>
	[MaxLength(50)]
	public string SessionId { get; set; }

	/// <summary>
	/// Event description
	/// </summary>
	public string Description { get; set; }

	/// <summary>
	/// IP Address
	/// </summary>
	[MaxLength(200)]
	public string IpAddress { get; set; }
}
