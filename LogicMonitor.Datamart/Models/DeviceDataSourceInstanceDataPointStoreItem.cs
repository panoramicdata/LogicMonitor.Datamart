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
	/// Property 1
	/// </summary>
	public string Property1 { get; set; } = string.Empty;

	/// <summary>
	/// Property 1
	/// </summary>
	public string Property2 { get; set; } = string.Empty;

	/// <summary>
	/// Property 3
	/// </summary>
	public string Property3 { get; set; } = string.Empty;

	/// <summary>
	/// Property 4
	/// </summary>
	public string Property4 { get; set; } = string.Empty;

	/// <summary>
	/// Property 5
	/// </summary>
	public string Property5 { get; set; } = string.Empty;

	/// <summary>
	/// Property 6
	/// </summary>
	public string Property6 { get; set; } = string.Empty;

	/// <summary>
	/// Property 7
	/// </summary>
	public string Property7 { get; set; } = string.Empty;

	/// <summary>
	/// Property 8
	/// </summary>
	public string Property8 { get; set; } = string.Empty;

	/// <summary>
	/// Property 9
	/// </summary>
	public string Property9 { get; set; } = string.Empty;

	/// <summary>
	/// Property 10
	/// </summary>
	public string Property10 { get; set; } = string.Empty;

	/// <summary>
	/// A condition which must be fulfilled in order to do the sync
	/// </summary>
	public string Condition { get; set; } = "true";
	/// <summary>
	/// The last hour for which we have written complete aggregations for this DataPoint store item
	/// </summary>
	public DateTimeOffset? DataCompleteTo { get; set; }

	public ICollection<TimeSeriesDataAggregationStoreItem> TimeSeriesDataAggregations { get; set; } = null!;
}
