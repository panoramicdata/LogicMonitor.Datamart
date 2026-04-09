namespace LogicMonitor.Datamart.Models;

/// <summary>
/// Represents a LogicMonitor resource group stored in the datamart.
/// </summary>
public class ResourceGroupStoreItem : IdentifiedStoreItem
{
	/// <summary>
	/// The resource group name.
	/// </summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// A description of the resource group.
	/// </summary>
	public string Description { get; set; } = string.Empty;

	/// <summary>
	/// The alert disable status for this group.
	/// </summary>
	public AlertDisableStatus AlertDisableStatus { get; set; }

	/// <summary>
	/// Whether alerting is enabled for this group.
	/// </summary>
	public bool AlertEnable { get; set; }

	/// <summary>
	/// The current alert status for the group.
	/// </summary>
	public AlertStatus AlertStatus { get; set; }

	/// <summary>
	/// The AppliesTo expression that dynamically assigns resources to this group.
	/// </summary>
	public string AppliesTo { get; set; } = string.Empty;

	/// <summary>
	/// The result of auto-visual processing for the group.
	/// </summary>
	public string? AutoVisualResult { get; set; }

	/// <summary>
	/// The cluster alert status as a string.
	/// </summary>
	public string ClusterAlertStatus { get; set; } = string.Empty;

	/// <summary>
	/// The priority of the cluster alert status for ordering.
	/// </summary>
	public int ClusterAlertStatusPriority { get; set; }

	/// <summary>
	/// The description of the default collector for the group.
	/// </summary>
	public string? DefaultCollectorDescription { get; set; } = string.Empty;

	/// <summary>
	/// The LogicMonitor identifier of the default collector.
	/// </summary>
	public int DefaultCollectorId { get; set; }

	/// <summary>
	/// The LogicMonitor identifier of the default agent.
	/// </summary>
	public int DefaultAgentId { get; set; }

	/// <summary>
	/// The number of AWS resources in this group.
	/// </summary>
	public int AwsDeviceCount { get; set; }

	/// <summary>
	/// JSON string containing AWS region information.
	/// </summary>
	public string AwsRegionsInfo { get; set; } = string.Empty;

	/// <summary>
	/// The result of the AWS connection test.
	/// </summary>
	public string? AwsTestResult { get; set; }

	/// <summary>
	/// The result code of the AWS connection test.
	/// </summary>
	public int AwsTestResultCode { get; set; }

	/// <summary>
	/// The number of Azure resources in this group.
	/// </summary>
	public int AzureDeviceCount { get; set; }

	/// <summary>
	/// JSON string containing Azure region information.
	/// </summary>
	public string AzureRegionsInfo { get; set; } = string.Empty;

	/// <summary>
	/// The result of the Azure connection test.
	/// </summary>
	public string? AzureTestResult { get; set; }

	/// <summary>
	/// The result code of the Azure connection test.
	/// </summary>
	public int AzureTestResultCode { get; set; }

	/// <summary>
	/// The number of GCP resources in this group.
	/// </summary>
	public int GcpDeviceCount { get; set; }

	/// <summary>
	/// JSON string containing GCP region information.
	/// </summary>
	public string GcpRegionsInfo { get; set; } = string.Empty;

	/// <summary>
	/// The result of the GCP connection test.
	/// </summary>
	public string? GcpTestResult { get; set; }

	/// <summary>
	/// The result code of the GCP connection test.
	/// </summary>
	public int GcpTestResultCode { get; set; }

	/// <summary>
	/// Whether NetFlow collection is enabled on this group.
	/// </summary>
	public bool IsNetflowEnabled { get; set; }

	/// <summary>
	/// Whether the group contains any resources with NetFlow enabled.
	/// </summary>
	public bool HasNetflowEnabledDevices { get; set; }

	/// <summary>
	/// The UTC epoch timestamp (seconds) when the group was created.
	/// </summary>
	public int? CreatedOnTimestampUtc { get; set; }

	/// <summary>
	/// The total number of resources in this group (including subgroups).
	/// </summary>
	public int DeviceCount { get; set; }

	/// <summary>
	/// The type of resource group (e.g. normal, dynamic).
	/// </summary>
	public ResourceGroupType DeviceGroupType { get; set; }

	/// <summary>
	/// The number of resources directly in this group (not in subgroups).
	/// </summary>
	public int DirectDeviceCount { get; set; }

	/// <summary>
	/// The number of direct child subgroups.
	/// </summary>
	public int DirectSubGroupCount { get; set; }

	/// <summary>
	/// The alert status priority for ordering.
	/// </summary>
	public int AlertStatusPriority { get; set; }

	/// <summary>
	/// Whether alerting is effectively enabled (considering inherited settings).
	/// </summary>
	public bool EffectiveAlertEnabled { get; set; }

	/// <summary>
	/// The full hierarchical path of the resource group.
	/// </summary>
	public string FullPath { get; set; } = string.Empty;

	/// <summary>
	/// The overall group status as a string.
	/// </summary>
	public string GroupStatus { get; set; } = string.Empty;

	/// <summary>
	/// Whether alerting is disabled for this group.
	/// </summary>
	public bool IsAlertingDisabled { get; set; }

	/// <summary>
	/// The LogicMonitor identifier of the parent group.
	/// </summary>
	public int ParentId { get; set; }

	/// <summary>
	/// The current Scheduled Down Time (SDT) status.
	/// </summary>
	public SdtStatus SdtStatus { get; set; }

	/// <summary>
	/// The user permission level for this resource group.
	/// </summary>
	public UserPermission UserPermission { get; set; }
}
