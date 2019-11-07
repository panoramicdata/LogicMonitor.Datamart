namespace LogicMonitor.Datamart.Models
{
	public class DataSourceDataPointStoreItem : IdentifiedStoreItem
	{
#pragma warning disable CA2227 // Collection properties should be read only
		// Navigation properties
		public DataSourceStoreItem DataSource { get; set; }
		public int DataSourceId { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

		// Database properties
		public string Name { get; set; }

		public string Description { get; set; }

		public string MeasurementUnit { get; set; }
	}
}
