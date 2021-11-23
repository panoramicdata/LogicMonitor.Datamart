namespace LogicMonitor.Datamart.Models;

public class CollectorStoreItem : IdentifiedStoreItem
{
	// Navigation properties
	public CollectorGroupStoreItem CollectorGroup { get; set; }

	// Database properties
	public string Name { get; set; }

	public string Description { get; set; }

	public string AckComment { get; set; }

	public bool Acked { get; set; }

	public string AckedBy { get; set; }

	public string AckedOnLocalString { get; set; }

	public long? AckedOnUtcTimestampUtc { get; set; }

	public string Architecture { get; set; }

	public int BackupCollectorId { get; set; }

	public int Build { get; set; }

	public bool CanDowngrade { get; set; }

	public string CanDowngradeReason { get; set; }

	public bool ClearSent { get; set; }

	public string CollectorConfiguration { get; set; }

	public int DeviceId { get; set; }

	public int GroupId { get; set; }

	public string GroupName { get; set; }

	public string Configuration { get; set; }

	public int ConfigurationVersion { get; set; }

	public string CreatedOnLocalString { get; set; }

	public long CreatedOnTimeStampUtc { get; set; }

	public string Credential { get; set; }

	public string Credential2 { get; set; }

	public int DeviceCount { get; set; }

	public bool Ea { get; set; }

	public bool EnableFailBack { get; set; }

	public bool EnableFailOverOnCollectorDevice { get; set; }

	public int EscalationChainId { get; set; }

	public bool HasFailOverDevice { get; set; }

	public string HostName { get; set; }

	public bool InSdt { get; set; }

	public bool IsDown { get; set; }

	public string LastSentNotificationOnLocal { get; set; }

	public int LastSentNotificationOnTimeStampUtc { get; set; }

	public bool NeedAutoCreateCollectorDevice { get; set; }

	public int NetscanVersion { get; set; }

	public int NextRecipient { get; set; }

	public string OnetimeDowngradeInfo { get; set; }

	public string Platform { get; set; }

	public int PreviousVersion { get; set; }

	public string ProxyConfiguration { get; set; }

	public int ResendIntervalSeconds { get; set; }

	public string WebsiteConfiguration { get; set; }

	public int WebsiteCount { get; set; }

	public string Size { get; set; }

	public int SpecifiedCollectorDeviceGroupId { get; set; }

	public int Status { get; set; }

	public bool SuppressAlertClear { get; set; }

	public string UpdatedOnLocalString { get; set; }

	public long UpgradeTimeUtcSeconds { get; set; }

	public long? UpdatedOnTimeStampUtc { get; set; }

	public int UptimeSeconds { get; set; }

	public string UserChangeOnLocal { get; set; }

	public long UserChangeOnUtcSeconds { get; set; }

	public UserPermission UserPermission { get; set; }

	public int UserVisibleDeviceCount { get; set; }

	public int UserVisibleWebsiteCount { get; set; }

	public string WatchdogConfiguration { get; set; }

	public string WatchdogUpdatedOnLocal { get; set; }

	public long? WatchdogUpdatedOnSeconds { get; set; }

	public string WrapperConfiguration { get; set; }
}
