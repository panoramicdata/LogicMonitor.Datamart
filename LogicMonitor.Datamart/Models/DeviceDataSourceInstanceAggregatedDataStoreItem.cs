using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogicMonitor.Datamart.Models
{
	public class DeviceDataSourceInstanceAggregatedDataStoreItem
	{
		// Navigation properties
		public DeviceDataSourceInstanceStoreItem DeviceDataSourceInstance { get; set; }

		// Database properties
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public DateTime Hour { get; set; }

		public int DeviceDataSourceInstanceId { get; set; }

		public DataSourceDataPointStoreItem DataPoint { get; set; }

		public int DataPointId { get; set; }

		public double? Min { get; set; }

		public double? Max { get; set; }

		public double? Sum { get; set; }

		public double? SumSquared { get; set; }

		public int DataCount { get; set; }

		public int NoDataCount { get; set; }
	}
}
