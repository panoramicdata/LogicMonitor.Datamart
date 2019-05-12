using System.Collections.Generic;

namespace LogicMonitor.Datamart.Models
{
	public class DeviceDataSourceStoreItem : IdentifiedStoreItem
	{
#pragma warning disable CA2227 // Collection properties should be read only
		// Navigation properties
		public List<DeviceDataSourceInstanceStoreItem> DeviceDataSourceInstances { get; set; }

		public DataSourceStoreItem DataSource { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

		// Database properties
		public string Name { get; set; }
		public string Description { get; set; }
		public int AlertStatusPriority { get; set; }
		public long AssignedOnSeconds { get; set; }
		public long CreatedOnSeconds { get; set; }
		public string DataSourceType { get; set; }
		public long UpdatedOnSeconds { get; set; }
		public int DataSourceId { get; set; }
		public string DataSourceName { get; set; }
		public string DataSourceDescription { get; set; }
		public string DataSourceDisplayName { get; set; }
		public int DeviceId { get; set; }
		public string DeviceName { get; set; }
		public string DeviceDisplayName { get; set; }
		public string GroupName { get; set; }
		public bool InstanceAutoGroupEnabled { get; set; }
		public int InstanceCount { get; set; }
		public int MonitoringInstanceCount { get; set; }
		public long NextAutoDiscoveryOnSeconds { get; set; }
		public bool IsAutoDiscoveryEnabled { get; set; }
		public bool IsMonitoringDisabled { get; set; }
		public bool IsMultiple { get; set; }
	}
}