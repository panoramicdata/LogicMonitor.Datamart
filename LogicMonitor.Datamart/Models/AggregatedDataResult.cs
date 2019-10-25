using System;
using System.Collections.Generic;
using System.Text;

namespace LogicMonitor.Datamart.Models
{
	public class AggregatedDataResult
	{
		public List<DeviceDataSourceInstanceAggregatedDataStoreItem> Values { get; internal set; }
		public int LastId { get; internal set; }
	}
}
