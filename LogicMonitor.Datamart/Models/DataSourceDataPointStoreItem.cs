namespace LogicMonitor.Datamart.Models;

/// <summary>
/// Represents a DataSource DataPoint definition stored in the datamart.
/// </summary>
public class DataSourceDataPointStoreItem : IdentifiedStoreItem
{
	/// <summary>
	/// Foreign key to the parent DataSource.
	/// </summary>
	public Guid DataSourceId { get; set; }

	/// <summary>
	/// An optional reference to a DataSourceGraph.
	/// If null, the DataPoint is a raw DataSource DataPoint.
	/// </summary>
	public Guid? DataSourceGraphId { get; set; }

	/// <summary>
	/// Navigation property to the parent DataSource.
	/// </summary>
	public DataSourceStoreItem DataSource { get; set; } = null!;

	/// <summary>
	/// The name of the DataPoint.
	/// </summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// A description of the DataPoint.
	/// </summary>
	public string Description { get; set; } = string.Empty;

	/// <summary>
	/// The measurement unit for the DataPoint values.
	/// </summary>
	public string MeasurementUnit { get; set; } = string.Empty;

	/// <summary>
	/// The global alert expression for this DataPoint.
	/// </summary>
	public string GlobalAlertExpression { get; set; } = string.Empty;

	/// <summary>
	/// The calculation expression applied to the DataPoint values.
	/// </summary>
	public string Calculation { get; set; } = string.Empty;

	/// <summary>
	/// The NCalc expression for computing percentage availability.
	/// </summary>
	public string PercentageAvailabilityCalculation { get; set; } = string.Empty;

	/// <summary>
	/// Tags associated with this DataPoint.
	/// </summary>
	public string Tags { get; set; } = string.Empty;

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
	/// If true, the DataPoint will be resynced even if it is not out of sync
	/// This will remove all old data and replace it with new data
	/// </summary>
	public bool ResyncTimeSeriesData { get; set; }

	/// <summary>
	/// Navigation property to the resource-DataSource instance DataPoint associations.
	/// </summary>
	public virtual ICollection<ResourceDataSourceInstanceDataPointStoreItem> DeviceDataSourceInstanceDataPoints { get; set; } = null!;
}