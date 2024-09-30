namespace LogicMonitor.Datamart.Models;

public class DataSourceDataPointStoreItem : IdentifiedStoreItem
{
	public Guid DataSourceId { get; set; }

	/// <summary>
	/// An optional reference to a DataSourceGraph
	/// If null, the DataPoint is a raw DataSource DataPoint
	/// </summary>
	public Guid? DataSourceGraphId { get; set; }

	public DataSourceStoreItem DataSource { get; set; } = null!;

	public string Name { get; set; } = string.Empty;

	public string Description { get; set; } = string.Empty;

	public string MeasurementUnit { get; set; } = string.Empty;

	public string GlobalAlertExpression { get; set; } = string.Empty;

	public string Calculation { get; set; } = string.Empty;

	public string PercentageAvailabilityCalculation { get; set; } = string.Empty;

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

	public virtual ICollection<DeviceDataSourceInstanceDataPointStoreItem> DeviceDataSourceInstanceDataPoints { get; set; } = null!;
}