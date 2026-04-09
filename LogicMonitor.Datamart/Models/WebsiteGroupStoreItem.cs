namespace LogicMonitor.Datamart.Models;

/// <summary>
/// Represents a LogicMonitor website group stored in the datamart.
/// </summary>
public class WebsiteGroupStoreItem : IdentifiedStoreItem
{
	/// <summary>
	/// Navigation property to the websites in this group.
	/// </summary>
	public List<WebsiteStoreItem> Websites { get; set; } = null!;

	/// <summary>
	/// The website group name.
	/// </summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// A description of the website group.
	/// </summary>
	public string Description { get; set; } = string.Empty;

	/// <summary>
	/// The current alert status for the group.
	/// </summary>
	public AlertStatus AlertStatus { get; set; }

	/// <summary>
	/// The alert status priority for ordering.
	/// </summary>
	public int AlertStatusPriority { get; set; }

	/// <summary>
	/// The alert disable status for this group.
	/// </summary>
	public AlertDisableStatus AlertDisableStatus { get; set; }

	/// <summary>
	/// The number of websites directly in this group.
	/// </summary>
	public int DirectWebsiteCount { get; set; }

	/// <summary>
	/// The number of direct child subgroups.
	/// </summary>
	public int DirectWebsiteGroupCount { get; set; }

	/// <summary>
	/// Whether alerting is disabled for this group.
	/// </summary>
	public bool DisableAlerting { get; set; }

	/// <summary>
	/// The full hierarchical path of the website group.
	/// </summary>
	public string FullPath { get; set; } = string.Empty;

	/// <summary>
	/// The overall group alert status.
	/// </summary>
	public AlertStatus GroupStatus { get; set; }

	/// <summary>
	/// Whether the group contains any disabled websites.
	/// </summary>
	public bool HasWebsitesDisabled { get; set; }

	/// <summary>
	/// The LogicMonitor identifier of the parent group.
	/// </summary>
	public int ParentId { get; set; }

	/// <summary>
	/// The current Scheduled Down Time (SDT) status.
	/// </summary>
	public SdtStatus SdtStatus { get; set; }

	/// <summary>
	/// The total number of websites in this group (including subgroups).
	/// </summary>
	public int WebsiteCount { get; set; }

	/// <summary>
	/// Whether monitoring is stopped for all websites in this group.
	/// </summary>
	public bool? StopMonitoring { get; set; }

	/// <summary>
	/// The user permission level for this website group.
	/// </summary>
	public UserPermission UserPermissionString { get; set; }
}
