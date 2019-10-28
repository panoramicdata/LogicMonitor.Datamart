using System;

namespace LogicMonitor.Datamart.Models
{

	public class DeviceDataSourceInstanceAggregatedData
	{
		public DateTime Hour { get; set; }

		public string DeviceDisplayName { get; set; }

		public string DataSourceName { get; set; }

		public string DataPointName { get; set; }

		public string DataPointMeasurementUnit { get; set; }

		public double? Min { get; set; }

		public double? Max { get; set; }

		public double? Sum { get; set; }

		public double? SumSquared { get; set; }

		public int DataCount { get; set; }

		public int NoDataCount { get; set; }
		public int Id { get; internal set; }
	}
}
