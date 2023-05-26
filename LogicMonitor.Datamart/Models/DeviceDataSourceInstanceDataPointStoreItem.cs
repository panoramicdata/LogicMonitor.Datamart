namespace LogicMonitor.Datamart.Models;

public class DeviceDataSourceInstanceDataPointStoreItem : IdentifiedStoreItem
{
	// Navigation properties
	public DeviceDataSourceInstanceStoreItem? DeviceDataSourceInstance { get; set; }

	// Database properties
	public Guid DeviceDataSourceInstanceId { get; set; }

	// Navigation properties
	public DataSourceDataPointStoreItem? DataSourceDataPoint { get; set; }

	// Database properties
	public Guid DataSourceDataPointId { get; set; }

	/// <summary>
	/// The last hour for which we have written complete aggregations for this DataPoint store item
	/// </summary>
	public DateTimeOffset? DataCompleteTo { get; set; }

	public ICollection<TimeSeriesDataAggregationStoreItem> TimeSeriesDataAggregations { get; set; } = null!;
}
