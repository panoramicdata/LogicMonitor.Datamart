namespace LogicMonitor.Datamart.Models;

/// <summary>
/// Represents a LogicMonitor website (service) check stored in the datamart.
/// </summary>
public class WebsiteStoreItem : IdentifiedStoreItem
{
	/// <summary>
	/// Navigation property to the parent website group.
	/// </summary>
	public WebsiteGroupStoreItem? WebsiteGroup { get; set; }

	/// <summary>
	/// The datamart identifier of the parent website group.
	/// </summary>
	public Guid WebsiteGroupId { get; set; }

	/// <summary>
	/// The website check name.
	/// </summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// A description of the website check.
	/// </summary>
	public string Description { get; set; } = string.Empty;

	/// <summary>
	/// The number of monitoring checkpoints.
	/// </summary>
	public int Count { get; set; }

	/// <summary>
	/// The domain to check.
	/// </summary>
	public string? Domain { get; set; }

	/// <summary>
	/// The hostname to check.
	/// </summary>
	public string? HostName { get; set; }

	/// <summary>
	/// The alert disable status for this website.
	/// </summary>
	public AlertDisableStatus AlertDisableStatus { get; set; }

	/// <summary>
	/// The alert expression used to trigger alerts.
	/// </summary>
	public string? AlertExpression { get; set; }

	/// <summary>
	/// The current alert status for the website.
	/// </summary>
	public AlertStatus AlertStatus { get; set; }

	/// <summary>
	/// The alert status priority for ordering.
	/// </summary>
	public int AlertStatusPriority { get; set; }

	/// <summary>
	/// Whether alerting is disabled for this website.
	/// </summary>
	public bool IsAlertingDisabled { get; set; }

	/// <summary>
	/// The global service monitor alert condition.
	/// </summary>
	public int GlobalSmAlertCond { get; set; }

	/// <summary>
	/// Whether individual service monitor alerts are enabled.
	/// </summary>
	public bool IndividualSmAlertEnable { get; set; }

	/// <summary>
	/// The individual alert level for this website.
	/// </summary>
	public Level IndividualAlertLevel { get; set; }

	/// <summary>
	/// Whether SSL certificate errors should be ignored.
	/// </summary>
	public bool IgnoreSsl { get; set; }

	/// <summary>
	/// Whether this is an internal website check.
	/// </summary>
	public bool IsInternal { get; set; }

	/// <summary>
	/// The HTTP method used for the website check.
	/// </summary>
	public WebsiteMethod WebsiteMethod { get; set; }

	/// <summary>
	/// The overall alert level across all checkpoints.
	/// </summary>
	public Level OverallAlertLevel { get; set; }

	/// <summary>
	/// The threshold in milliseconds for page load time alerts.
	/// </summary>
	public int PageLoadAlertTimeInMs { get; set; }

	/// <summary>
	/// The percentage of packets not received in time (for ping checks).
	/// </summary>
	public int PercentPacketsNotReceiveInTime { get; set; }

	/// <summary>
	/// The timeout in milliseconds for packets not received (for ping checks).
	/// </summary>
	public int PacketsNotReceivedTimeoutMs { get; set; }

	/// <summary>
	/// The polling interval in minutes for the website check.
	/// </summary>
	public int PollingIntervalMinutes { get; set; }

	/// <summary>
	/// The URL schema (e.g. http, https).
	/// </summary>
	public string Schema { get; set; } = string.Empty;

	/// <summary>
	/// The script content for scripted website checks.
	/// </summary>
	public string? Script { get; set; }

	/// <summary>
	/// The current Scheduled Down Time (SDT) status.
	/// </summary>
	public SdtStatus SdtStatus { get; set; }

	/// <summary>
	/// Whether monitoring is stopped for this website.
	/// </summary>
	public bool StopMonitoring { get; set; }

	/// <summary>
	/// Whether monitoring is stopped due to the parent website group setting.
	/// </summary>
	public bool StopMonitoringByWebsiteGroup { get; set; }

	/// <summary>
	/// Whether to trigger alerts on SSL status changes.
	/// </summary>
	public bool TriggerSslStatusAlerts { get; set; }

	/// <summary>
	/// Whether to trigger alerts on SSL certificate expiration.
	/// </summary>
	public bool TriggerSslExpirationAlerts { get; set; }

	/// <summary>
	/// The number of consecutive failures before triggering an alert.
	/// </summary>
	public int Transition { get; set; }

	/// <summary>
	/// The type of website check (e.g. web check, ping check).
	/// </summary>
	public WebsiteType Type { get; set; }

	/// <summary>
	/// Whether the default alert setting is used.
	/// </summary>
	public bool UseDefaultAlertSetting { get; set; }

	/// <summary>
	/// Whether the default location (checkpoint) setting is used.
	/// </summary>
	public bool UseDefaultLocationSetting { get; set; }

	/// <summary>
	/// The user permission level for this website.
	/// </summary>
	public UserPermission UserPermissionString { get; set; }

	/// <summary>
	/// The current monitoring status of the website.
	/// </summary>
	public WebsiteStatus Status { get; set; }
}
