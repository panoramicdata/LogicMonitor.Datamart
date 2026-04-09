namespace LogicMonitor.Datamart.Models;

/// <summary>
/// Represents a DataPoint value collected from a DataSource instance on a resource stored in the datamart.
/// </summary>
public class ResourceDataSourceInstanceDataPointStoreItem : IdentifiedStoreItem
{
	/// <summary>
	/// Navigation property to the parent DataSource instance.
	/// </summary>
	public ResourceDataSourceInstanceStoreItem? DeviceDataSourceInstance { get; set; }

	/// <summary>
	/// The datamart identifier of the parent DataSource instance.
	/// </summary>
	public Guid DeviceDataSourceInstanceId { get; set; }

	/// <summary>
	/// Navigation property to the DataPoint definition.
	/// </summary>
	public DataSourceDataPointStoreItem? DataSourceDataPoint { get; set; }

	/// <summary>
	/// The datamart identifier of the DataPoint definition.
	/// </summary>
	public Guid DataSourceDataPointId { get; set; }

	/// <summary>
	/// The last hour for which we have written complete aggregations for this DataPoint store item
	/// </summary>
	public DateTimeOffset? DataCompleteTo { get; set; }

	/// <summary>
	/// InstanceDatapointProperty 1
	/// </summary>
	public string InstanceDatapointProperty1 { get; set; } = string.Empty;

	/// <summary>
	/// InstanceDatapointProperty 2
	/// </summary>
	public string InstanceDatapointProperty2 { get; set; } = string.Empty;

	/// <summary>
	/// InstanceDatapointProperty 3
	/// </summary>
	public string InstanceDatapointProperty3 { get; set; } = string.Empty;

	/// <summary>
	/// InstanceDatapointProperty 4
	/// </summary>
	public string InstanceDatapointProperty4 { get; set; } = string.Empty;

	/// <summary>
	/// InstanceDatapointProperty 5
	/// </summary>
	public string InstanceDatapointProperty5 { get; set; } = string.Empty;

	/// <summary>
	/// InstanceDatapointProperty 6
	/// </summary>
	public string InstanceDatapointProperty6 { get; set; } = string.Empty;

	/// <summary>
	/// InstanceDatapointProperty 7
	/// </summary>
	public string InstanceDatapointProperty7 { get; set; } = string.Empty;

	/// <summary>
	/// InstanceDatapointProperty 8
	/// </summary>
	public string InstanceDatapointProperty8 { get; set; } = string.Empty;

	/// <summary>
	/// InstanceDatapointProperty 9
	/// </summary>
	public string InstanceDatapointProperty9 { get; set; } = string.Empty;

	/// <summary>
	/// InstanceDatapointProperty 10
	/// </summary>
	public string InstanceDatapointProperty10 { get; set; } = string.Empty;

	/// <summary>
	/// Navigation property to the time series data aggregations for this DataPoint.
	/// </summary>
	public virtual ICollection<TimeSeriesDataAggregationStoreItem> TimeSeriesDataAggregations { get; set; } = null!;
}
