using System.Collections.Generic;

namespace LogicMonitor.Datamart.Models
{
	public class DataSourceStoreItem : IdentifiedStoreItem
	{
#pragma warning disable CA2227 // Collection properties should be read only
		// Navigation properties
		public List<DeviceDataSourceStoreItem> DeviceDataSources { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

		// Database properties
		public string Name { get; set; }

		public string Description { get; set; }
	}
}
