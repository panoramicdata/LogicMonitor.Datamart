namespace LogicMonitor.Datamart.Models;

public class CollectorStoreItem : IdentifiedStoreItem
{
	// Navigation properties
	public ICollection<DeviceStoreItem> Devices { get; set; }

	public CollectorGroupStoreItem? CollectorGroup { get; set; }

	public Guid CollectorGroupId { get; set; }

	// Database properties
	public string Name { get; set; } = string.Empty;

	public string Description { get; set; } = string.Empty;

	public string AckComment { get; set; } = string.Empty;

	public bool Acked { get; set; }

	public string AckedBy { get; set; } = string.Empty;

	public string AckedOnLocalString { get; set; } = string.Empty;

	public long? AckedOnUtcTimestampUtc { get; set; }

	public string Architecture { get; set; } = string.Empty;

	public int BackupCollectorId { get; set; }

	public int Build { get; set; }

	public bool CanDowngrade { get; set; }

	public string CanDowngradeReason { get; set; } = string.Empty;

	public bool ClearSent { get; set; }

	public string CollectorConfiguration { get; set; } = string.Empty;

	public string? Configuration { get; set; }

	public int ConfigurationVersion { get; set; }

	public string CreatedOnLocalString { get; set; } = string.Empty;

	public long CreatedOnTimeStampUtc { get; set; }

	public string? Credential { get; set; }

	public string? Credential2 { get; set; }

	public int DeviceCount { get; set; }

	public bool Ea { get; set; }

	public bool EnableFailBack { get; set; }

	public bool EnableFailOverOnCollectorDevice { get; set; }

	public int EscalationChainId { get; set; }

	public bool HasFailOverDevice { get; set; }

	public string HostName { get; set; } = string.Empty;

	public bool InSdt { get; set; }

	public bool IsDown { get; set; }

	public bool IsEncoded { get; set; }

	public bool IsLmLogsEnabled { get; set; }

	public bool IsLmLogsSyslogEnabled { get; set; }

	public string LastSentNotificationOnLocal { get; set; } = string.Empty;

	public int LastSentNotificationOnTimeStampUtc { get; set; }

	public int LogicMonitorDeviceId { get; set; }

	public bool NeedAutoCreateCollectorDevice { get; set; }

	public int NetscanVersion { get; set; }

	public int NextRecipient { get; set; }

	public string? OnetimeDowngradeInfo { get; set; }

	public string OtelVerison { get; set; } = string.Empty;

	public string Platform { get; set; } = string.Empty;

	public int PreviousVersion { get; set; }

	public string ProxyConfiguration { get; set; } = string.Empty;

	public int ResendIntervalSeconds { get; set; }

	public string WebsiteConfiguration { get; set; } = string.Empty;

	public int WebsiteCount { get; set; }

	public string Size { get; set; } = string.Empty;

	public int SpecifiedCollectorDeviceGroupId { get; set; }

	public int Status { get; set; }

	public bool SuppressAlertClear { get; set; }

	public string UpdatedOnLocalString { get; set; } = string.Empty;

	public long UpgradeTimeUtcSeconds { get; set; }

	public long? UpdatedOnTimeStampUtc { get; set; }

	public int UptimeSeconds { get; set; }

	public string UserChangeOnLocal { get; set; } = string.Empty;

	public long UserChangeOnUtcSeconds { get; set; }

	public UserPermission UserPermission { get; set; }

	public int UserVisibleDeviceCount { get; set; }

	public int UserVisibleWebsiteCount { get; set; }

	public string WatchdogConfiguration { get; set; } = string.Empty;

	public string WatchdogUpdatedOnLocal { get; set; } = string.Empty;

	public long? WatchdogUpdatedOnSeconds { get; set; }

	public string WrapperConfiguration { get; set; } = string.Empty;
}
