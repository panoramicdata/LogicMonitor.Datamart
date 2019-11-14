using LogicMonitor.Api.Alerts;
using System;

namespace LogicMonitor.Datamart.Models
{
	public class DeviceDataSourceInstanceStoreItem : IdentifiedStoreItem
	{
		// Navigation properties
		public DeviceDataSourceStoreItem DeviceDataSource { get; set; }
		public DeviceStoreItem Device { get; set; }

		// Database properties
		public AlertDisableStatus AlertDisableStatus { get; set; }
		public AlertStatus AlertStatus { get; set; }
		public int AlertStatusPriority { get; set; }
		public int? DataSourceId { get; set; }
		public int DeviceDataSourceId { get; set; }
		public int? DeviceId { get; set; }
		public bool DisableAlerting { get; set; }
		public string DisplayName { get; set; }
		public int GroupId { get; set; }
		public string GroupName { get; set; }
		public long LastCollectedTimeSeconds { get; set; }
		public long LastUpdatedTimeSeconds { get; set; }
		public bool LockDescription { get; set; }
		public bool StopMonitoring { get; set; }
		public SdtStatus SdtStatus { get; set; }
		public string SdtAt { get; set; }
		public string WildValue { get; set; }
		public string WildValue2 { get; set; }

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