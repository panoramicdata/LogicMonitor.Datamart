namespace LogicMonitor.Datamart.Models;

public class WebsiteGroupStoreItem : IdentifiedStoreItem
{
	// Navigation properties
	public List<WebsiteStoreItem> Websites { get; set; } = null!;

	// Database properties
	public string Name { get; set; } = string.Empty;

	public string Description { get; set; } = string.Empty;

	public AlertStatus AlertStatus { get; set; }

	public int AlertStatusPriority { get; set; }

	public AlertDisableStatus AlertDisableStatus { get; set; }

	public int DirectWebsiteCount { get; set; }

	public int DirectWebsiteGroupCount { get; set; }

	public bool DisableAlerting { get; set; }

	public string FullPath { get; set; } = string.Empty;

	public AlertStatus GroupStatus { get; set; }

	public bool HasWebsitesDisabled { get; set; }

	public int ParentId { get; set; }

	public SdtStatus SdtStatus { get; set; }

	public int WebsiteCount { get; set; }

	public bool? StopMonitoring { get; set; }

	public UserPermission UserPermissionString { get; set; }
}
