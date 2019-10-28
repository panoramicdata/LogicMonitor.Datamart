using System.Collections.Generic;

namespace LogicMonitor.Datamart.Models
{
	public class AggregatedDataResult
	{
		public List<DeviceDataSourceInstanceAggregatedData> Values { get; set; }
		public int LastId { get; set; }
	}
}
