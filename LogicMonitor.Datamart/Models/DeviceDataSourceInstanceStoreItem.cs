using LogicMonitor.Api.Alerts;
using System;
using System.Collections.Generic;

namespace LogicMonitor.Datamart.Models
{
	public class DeviceDataSourceInstanceStoreItem : IdentifiedStoreItem
	{
#pragma warning disable CA2227 // Collection properties should be read only
		// Navigation properties
		public List<DeviceDataSourceInstanceDataStoreItem> DataMeasures { get; set; }

		public DeviceDataSourceStoreItem DeviceDataSource { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

		// Database properties
		public DeviceStoreItem Device { get; set; }

		//public AlertingDisabledOn AlertingDisabledOn { get; set; }
		public AlertDisableStatus AlertDisableStatus { get; set; }
		public AlertStatus AlertStatus { get; set; }
		public int AlertStatusPriority { get; set; }
		//public List<Property> CustomProperties { get; set; }
		//public List<Property> SystemProperties { get; set; }
		//public List<Property> AutoProperties { get; set; }
		public int? DataSourceId { get; set; }
		public string DeviceDisplayName { get; set; }
		public int DeviceDataSourceId { get; set; }
		public int? DeviceId { get; set; }
		public bool DisableAlerting { get; set; }
		public string DisplayName { get; set; }
		public int GroupId { get; set; }
		public string GroupName { get; set; }
		//public List<DisabledGroup> GroupsDisabledThisSource { get; set; }
		public long LastCollectedTimeSeconds { get; set; }
		public long LastUpdatedTimeSeconds { get; set; }
		public bool LockDescription { get; set; }
		public bool StopMonitoring { get; set; }
		public SdtStatus SdtStatus { get; set; }
		public string SdtAt { get; set; }
		public string WildValue { get; set; }
		public string WildValue2 { get; set; }

		/// <summary>
		/// The last observed measurement
		/// </summary>
		public long LastMeasurementUpdatedTimeSeconds { get; set; }

		/// <summary>
		/// The last hour for which we have written complete aggregations for this
		/// </summary>
		public DateTime? LastAggregationHourWrittenUtc { get; set; }

		/// <summary>
		/// If present, this is the UTC timestamp when a dimension update query was made to LogicMonitor and this instance was not returned
		/// </summary>
		public DateTime? LastWentMissingUtc { get; set; }
	}
}