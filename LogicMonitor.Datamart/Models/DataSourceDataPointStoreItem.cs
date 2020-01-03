namespace LogicMonitor.Datamart.Models
{
	public class DataSourceDataPointStoreItem : IdentifiedStoreItem
	{
		// Navigation properties
		public DataSourceStoreItem DataSource { get; set; }

		// Database properties
		public int DataSourceId { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public string MeasurementUnit { get; set; }
	}
}
