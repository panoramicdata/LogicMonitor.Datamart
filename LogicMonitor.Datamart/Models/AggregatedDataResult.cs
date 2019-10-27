using System.Collections.Generic;

namespace LogicMonitor.Datamart.Models
{
	public class AggregatedDataResult
	{
		public List<DeviceDataSourceInstanceAggregatedDataStoreItem> Values { get; set; }
		public int LastId { get; set; }
	}
}
