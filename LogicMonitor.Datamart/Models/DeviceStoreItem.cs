using LogicMonitor.Api;
using LogicMonitor.Api.Alerts;
using LogicMonitor.Api.Devices;
using System.Collections.Generic;

namespace LogicMonitor.Datamart.Models
{
	public class DeviceStoreItem : IdentifiedStoreItem
	{
		// Navigation properties
		public List<DeviceDataSourceInstanceStoreItem> DeviceDataSourceInstances { get; set; }

		public List<DeviceDataSourceStoreItem> DeviceDataSources { get; set; }

		// Database properties
		public string Name { get; set; }
		public string Description { get; set; }
		public AlertDisableStatus AlertDisableStatus { get; set; }
		public int AlertStatusPriority { get; set; }
		public AlertStatus AlertStatus { get; set; }
		public bool AncestorHasDisabledLogicModule { get; set; }
		public long? AutoPropertiesAssignedOnSeconds { get; set; }
		public long? AutoPropertiesUpdatedOnSeconds { get; set; }
		public AwsState AwsState { get; set; }
		public AzureState AzureState { get; set; }
		public AzureState GcpState { get; set; }
		public bool CanUseRemoteSession { get; set; }
		public string CollectorDescription { get; set; }
		public long? CreatedOnSeconds { get; set; }
		public int CurrentCollectorId { get; set; }
		public long DeletedTimeinMs { get; set; }
		public DeviceType DeviceType { get; set; }
		public bool IsAlertingDisabled { get; set; }
		public string DisplayName { get; set; }
		public bool EffectiveAlertEnabled { get; set; }
		public bool EnableNetflow { get; set; }
		public bool HasActiveInstance { get; set; }
		public bool HasDisabledSubResource { get; set; }
		public bool HasMore { get; set; }
		public string DeviceGroupIdsString { get; set; }
		public Level DeviceStatus { get; set; }
		public long? LastDataTimeSeconds { get; set; }
		public long? LastRawDataTimeSeconds { get; set; }
		public string Link { get; set; }
		public int NetflowCollectorId { get; set; }
		public string NetflowCollectorDescription { get; set; }
		public int NetflowCollectorGroupId { get; set; }
		public string NetflowCollectorGroupName { get; set; }
		public int PreferredCollectorId { get; set; }
		public int PreferredCollectorGroupId { get; set; }
		public string PreferredCollectorGroupName { get; set; }
		public int RelatedDeviceId { get; set; }
		public int ScanConfigId { get; set; }
		public SdtStatus SdtStatus { get; set; }
		public long ToDeleteTimeinMs { get; set; }
		public int UptimeInSeconds { get; set; }
		public long? UpdatedOnSeconds { get; set; }
		public UserPermission UserPermission { get; set; }
		public long LastAlertClosedTimeSeconds { get; set; }
		// These properties are used for storing extra bits in
		public string Property1 { get; set; }
		public string Property2 { get; set; }
		public string Property3 { get; set; }
		public string Property4 { get; set; }
		public string Property5 { get; set; }
		//public List<DevicePropertyStoreItem> SystemProperties { get; set; }
		//public List<DevicePropertyStoreItem> CustomProperties { get; set; }

	}
}
