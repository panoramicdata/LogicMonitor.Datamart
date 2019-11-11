using System;

namespace LogicMonitor.Datamart.Models
{
	public class DeviceDataSourceInstanceAggregatedDataBulkWriteModel
	{
		public int Id { get; set; }

		public DateTime PeriodStart { get; set; }

		public DateTime PeriodEnd { get; set; }

		public int DeviceDataSourceInstanceId { get; set; }

		public DataSourceDataPointStoreItem DataPoint { get; set; }

		public int DataPointId { get; set; }

		public double? Min { get; set; }

		public double? Max { get; set; }

		public double Sum { get; set; }

		public double SumSquared { get; set; }

		public int DataCount { get; set; }

		public int NoDataCount { get; set; }
	}
}
