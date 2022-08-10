namespace LogicMonitor.Datamart.Models;

public class WebsiteStoreItem : IdentifiedStoreItem
{
	// Navigation properties
	public WebsiteGroupStoreItem WebsiteGroup { get; set; } = null!;

	// Database properties
	public string Name { get; set; } = string.Empty;

	public string Description { get; set; } = string.Empty;

	public int Count { get; set; }

	public string Domain { get; set; } = string.Empty;

	public string HostName { get; set; } = string.Empty;

	public AlertDisableStatus AlertDisableStatus { get; set; }

	public string AlertExpression { get; set; } = string.Empty;

	public AlertStatus AlertStatus { get; set; }

	public int AlertStatusPriority { get; set; }

	public bool IsAlertingDisabled { get; set; }

	public int GlobalSmAlertCond { get; set; }

	public bool IndividualSmAlertEnable { get; set; }

	public Level IndividualAlertLevel { get; set; }

	public bool IgnoreSsl { get; set; }

	public bool IsInternal { get; set; }

	public WebsiteMethod WebsiteMethod { get; set; }

	public Level OverallAlertLevel { get; set; }

	public int PageLoadAlertTimeInMs { get; set; }

	public int PercentPacketsNotReceiveInTime { get; set; }

	public int PacketsNotReceivedTimeoutMs { get; set; }

	public int PollingIntervalMinutes { get; set; }

	public string Schema { get; set; } = string.Empty;

	public string Script { get; set; } = string.Empty;

	public SdtStatus SdtStatus { get; set; }

	public int WebsiteGroupId { get; set; }

	public bool StopMonitoring { get; set; }

	public bool StopMonitoringByWebsiteGroup { get; set; }

	public bool TriggerSslStatusAlerts { get; set; }

	public bool TriggerSslExpirationAlerts { get; set; }

	public int Transition { get; set; }

	public WebsiteType Type { get; set; }

	public bool UseDefaultAlertSetting { get; set; }

	public bool UseDefaultLocationSetting { get; set; }

	public UserPermission UserPermissionString { get; set; }

	public WebsiteStatus Status { get; set; }
}
