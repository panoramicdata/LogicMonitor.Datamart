namespace LogicMonitor.Datamart.Models;

/// <summary>
/// Represents a LogicMonitor collector stored in the datamart.
/// </summary>
public class CollectorStoreItem : IdentifiedStoreItem
{
	/// <summary>
	/// Navigation property to the resources monitored by this collector.
	/// </summary>
	public virtual ICollection<ResourceStoreItem>? Devices { get; set; }

	/// <summary>
	/// Navigation property to the collector group.
	/// </summary>
	public CollectorGroupStoreItem? CollectorGroup { get; set; }

	/// <summary>
	/// Foreign key to the collector group.
	/// </summary>
	public Guid CollectorGroupId { get; set; }

	/// <summary>
	/// The collector name.
	/// </summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// A description of the collector.
	/// </summary>
	public string Description { get; set; } = string.Empty;

	/// <summary>
	/// The comment left when the collector was acknowledged.
	/// </summary>
	public string AckComment { get; set; } = string.Empty;

	/// <summary>
	/// Whether the collector has been acknowledged.
	/// </summary>
	public bool Acked { get; set; }

	/// <summary>
	/// The user who acknowledged the collector.
	/// </summary>
	public string AckedBy { get; set; } = string.Empty;

	/// <summary>
	/// The local time string when the collector was acknowledged.
	/// </summary>
	public string AckedOnLocalString { get; set; } = string.Empty;

	/// <summary>
	/// The UTC epoch timestamp when the collector was acknowledged.
	/// </summary>
	public long? AckedOnUtcTimestampUtc { get; set; }

	/// <summary>
	/// The system architecture of the collector (e.g. 64-bit).
	/// </summary>
	public string Architecture { get; set; } = string.Empty;

	/// <summary>
	/// The LogicMonitor identifier of the backup collector.
	/// </summary>
	public int BackupCollectorId { get; set; }

	/// <summary>
	/// The build number of the collector software.
	/// </summary>
	public int Build { get; set; }

	/// <summary>
	/// Whether the collector can be downgraded.
	/// </summary>
	public bool CanDowngrade { get; set; }

	/// <summary>
	/// The reason the collector cannot be downgraded, if applicable.
	/// </summary>
	public string CanDowngradeReason { get; set; } = string.Empty;

	/// <summary>
	/// Whether the clear notification has been sent.
	/// </summary>
	public bool ClearSent { get; set; }

	/// <summary>
	/// The collector configuration data.
	/// </summary>
	public string CollectorConfiguration { get; set; } = string.Empty;

	/// <summary>
	/// Additional configuration data for the collector.
	/// </summary>
	public string? Configuration { get; set; }

	/// <summary>
	/// The version number of the collector configuration.
	/// </summary>
	public int ConfigurationVersion { get; set; }

	/// <summary>
	/// The local time string when the collector was created.
	/// </summary>
	public string CreatedOnLocalString { get; set; } = string.Empty;

	/// <summary>
	/// The UTC epoch timestamp when the collector was created.
	/// </summary>
	public long CreatedOnTimeStampUtc { get; set; }

	/// <summary>
	/// The primary credential for the collector.
	/// </summary>
	public string? Credential { get; set; }

	/// <summary>
	/// The secondary credential for the collector.
	/// </summary>
	public string? Credential2 { get; set; }

	/// <summary>
	/// The number of devices (resources) assigned to this collector.
	/// </summary>
	public int DeviceCount { get; set; }

	/// <summary>
	/// Whether this is an Early Access (EA) collector.
	/// </summary>
	public bool Ea { get; set; }

	/// <summary>
	/// Whether fail-back is enabled for this collector.
	/// </summary>
	public bool EnableFailBack { get; set; }

	/// <summary>
	/// Whether fail-over on the collector device is enabled.
	/// </summary>
	public bool EnableFailOverOnCollectorDevice { get; set; }

	/// <summary>
	/// The LogicMonitor escalation chain identifier for collector alerts.
	/// </summary>
	public int EscalationChainId { get; set; }

	/// <summary>
	/// Whether a fail-over device is configured.
	/// </summary>
	public bool HasFailOverDevice { get; set; }

	/// <summary>
	/// The hostname of the collector machine.
	/// </summary>
	public string HostName { get; set; } = string.Empty;

	/// <summary>
	/// Whether the collector is currently in a Scheduled Down Time (SDT) window.
	/// </summary>
	public bool InSdt { get; set; }

	/// <summary>
	/// Whether the collector is currently down.
	/// </summary>
	public bool IsDown { get; set; }

	/// <summary>
	/// Whether the collector configuration is encoded.
	/// </summary>
	public bool IsEncoded { get; set; }

	/// <summary>
	/// Whether LM Logs is enabled on this collector.
	/// </summary>
	public bool IsLmLogsEnabled { get; set; }

	/// <summary>
	/// Whether LM Logs Syslog collection is enabled on this collector.
	/// </summary>
	public bool IsLmLogsSyslogEnabled { get; set; }

	/// <summary>
	/// The local time string of the last sent notification.
	/// </summary>
	public string LastSentNotificationOnLocal { get; set; } = string.Empty;

	/// <summary>
	/// The UTC epoch timestamp of the last sent notification.
	/// </summary>
	public int LastSentNotificationOnTimeStampUtc { get; set; }

	/// <summary>
	/// The LogicMonitor device identifier representing this collector.
	/// </summary>
	public int LogicMonitorDeviceId { get; set; }

	/// <summary>
	/// Whether a collector device needs to be auto-created.
	/// </summary>
	public bool NeedAutoCreateCollectorDevice { get; set; }

	/// <summary>
	/// The netscan version for the collector.
	/// </summary>
	public int NetscanVersion { get; set; }

	/// <summary>
	/// The index of the next notification recipient.
	/// </summary>
	public int NextRecipient { get; set; }

	/// <summary>
	/// One-time downgrade information, if any.
	/// </summary>
	public string? OnetimeDowngradeInfo { get; set; }

	/// <summary>
	/// The OpenTelemetry version running on this collector.
	/// </summary>
	public string OtelVersion { get; set; } = string.Empty;

	/// <summary>
	/// The operating system platform of the collector.
	/// </summary>
	public string Platform { get; set; } = string.Empty;

	/// <summary>
	/// The previous software version of the collector.
	/// </summary>
	public int PreviousVersion { get; set; }

	/// <summary>
	/// The proxy configuration for the collector.
	/// </summary>
	public string ProxyConfiguration { get; set; } = string.Empty;

	/// <summary>
	/// The interval in seconds between resending notifications.
	/// </summary>
	public int ResendIntervalSeconds { get; set; }

	/// <summary>
	/// The website monitoring configuration for the collector.
	/// </summary>
	public string WebsiteConfiguration { get; set; } = string.Empty;

	/// <summary>
	/// The number of websites monitored by this collector.
	/// </summary>
	public int WebsiteCount { get; set; }

	/// <summary>
	/// The size classification of the collector (e.g. small, medium, large).
	/// </summary>
	public string Size { get; set; } = string.Empty;

	/// <summary>
	/// The LogicMonitor identifier of the specified collector device group.
	/// </summary>
	public int SpecifiedCollectorDeviceGroupId { get; set; }

	/// <summary>
	/// The current status code of the collector.
	/// </summary>
	public int Status { get; set; }

	/// <summary>
	/// Whether alert clear notifications are suppressed.
	/// </summary>
	public bool SuppressAlertClear { get; set; }

	/// <summary>
	/// The local time string when the collector was last updated.
	/// </summary>
	public string UpdatedOnLocalString { get; set; } = string.Empty;

	/// <summary>
	/// The UTC epoch timestamp (seconds) of the scheduled upgrade time.
	/// </summary>
	public long UpgradeTimeUtcSeconds { get; set; }

	/// <summary>
	/// The UTC epoch timestamp when the collector was last updated.
	/// </summary>
	public long? UpdatedOnTimeStampUtc { get; set; }

	/// <summary>
	/// The collector uptime in seconds.
	/// </summary>
	public int UptimeSeconds { get; set; }

	/// <summary>
	/// The local time string of the last user-initiated change.
	/// </summary>
	public string UserChangeOnLocal { get; set; } = string.Empty;

	/// <summary>
	/// The UTC epoch timestamp (seconds) of the last user-initiated change.
	/// </summary>
	public long UserChangeOnUtcSeconds { get; set; }

	/// <summary>
	/// The user permission level for this collector.
	/// </summary>
	public UserPermission UserPermission { get; set; }

	/// <summary>
	/// The number of devices visible to the current user on this collector.
	/// </summary>
	public int UserVisibleDeviceCount { get; set; }

	/// <summary>
	/// The number of websites visible to the current user on this collector.
	/// </summary>
	public int UserVisibleWebsiteCount { get; set; }

	/// <summary>
	/// The watchdog configuration for the collector.
	/// </summary>
	public string WatchdogConfiguration { get; set; } = string.Empty;

	/// <summary>
	/// The local time string when the watchdog was last updated.
	/// </summary>
	public string WatchdogUpdatedOnLocal { get; set; } = string.Empty;

	/// <summary>
	/// The epoch timestamp (seconds) when the watchdog was last updated.
	/// </summary>
	public long? WatchdogUpdatedOnSeconds { get; set; }

	/// <summary>
	/// The wrapper configuration for the collector.
	/// </summary>
	public string WrapperConfiguration { get; set; } = string.Empty;
}
