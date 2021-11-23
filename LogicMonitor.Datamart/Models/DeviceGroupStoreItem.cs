namespace LogicMonitor.Datamart.Models;

public class DeviceGroupStoreItem : IdentifiedStoreItem
{
	public string Name { get; set; }

	public string Description { get; set; }

	public AlertDisableStatus AlertDisableStatus { get; set; }

	public bool AlertEnable { get; set; }

	public AlertStatus AlertStatus { get; set; }

	public string AppliesTo { get; set; }

	public string AutoVisualResult { get; set; }

	public string ClusterAlertStatus { get; set; }

	public int ClusterAlertStatusPriority { get; set; }

	public string DefaultCollectorDescription { get; set; }

	public int DefaultCollectorId { get; set; }

	public int DefaultAgentId { get; set; }

	public int AwsDeviceCount { get; set; }

	public string AwsRegionsInfo { get; set; }

	public string AwsTestResult { get; set; }

	public int AwsTestResultCode { get; set; }

	public int AzureDeviceCount { get; set; }

	public string AzureRegionsInfo { get; set; }

	public string AzureTestResult { get; set; }

	public int AzureTestResultCode { get; set; }

	public int GcpDeviceCount { get; set; }

	public string GcpRegionsInfo { get; set; }

	public string GcpTestResult { get; set; }

	public int GcpTestResultCode { get; set; }

	public bool IsNetflowEnabled { get; set; }

	public bool HasNetflowEnabledDevices { get; set; }

	public int? CreatedOnTimestampUtc { get; set; }

	public int DeviceCount { get; set; }

	public DeviceGroupType DeviceGroupType { get; set; }

	public int DirectDeviceCount { get; set; }

	public int DirectSubGroupCount { get; set; }

	public int AlertStatusPriority { get; set; }

	public bool EffectiveAlertEnabled { get; set; }

	public string FullPath { get; set; }

	public string GroupStatus { get; set; }

	public bool IsAlertingDisabled { get; set; }

	public int ParentId { get; set; }

	public SdtStatus SdtStatus { get; set; }

	public UserPermission UserPermission { get; set; }
}
