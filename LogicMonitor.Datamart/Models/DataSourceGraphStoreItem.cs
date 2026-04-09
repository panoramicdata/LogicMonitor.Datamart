namespace LogicMonitor.Datamart.Models;

/// <summary>
/// Represents a DataSource graph definition stored in the datamart.
/// </summary>
public class DataSourceGraphStoreItem : IdentifiedStoreItem
{
	/// <summary>
	/// Navigation property to the parent DataSource.
	/// </summary>
	public DataSourceStoreItem? DataSource { get; set; } = null!;

	/// <summary>
	/// Foreign key to the parent DataSource.
	/// </summary>
	public Guid DataSourceId { get; set; }

	/// <summary>
	/// Whether this is an overview graph.
	/// </summary>
	public bool IsOverview { get; set; }

	/// <summary>
	/// The name of the graph.
	/// </summary>
	public required string Name { get; set; }

	/// <summary>
	/// The display title of the graph.
	/// </summary>
	public required string Title { get; set; }

	/// <summary>
	/// The label for the vertical (Y) axis.
	/// </summary>
	public required string VerticalLabel { get; set; }

	/// <summary>
	/// Whether the graph Y-axis scale is rigid (fixed min/max).
	/// </summary>
	public required bool IsRigid { get; set; }

	/// <summary>
	/// The width of the graph in pixels.
	/// </summary>
	public required int Width { get; set; }

	/// <summary>
	/// The height of the graph in pixels.
	/// </summary>
	public required int Height { get; set; }

	/// <summary>
	/// The maximum Y-axis value, or null for auto-scale.
	/// </summary>
	public required double? MaxValue { get; set; }

	/// <summary>
	/// The minimum Y-axis value, or null for auto-scale.
	/// </summary>
	public required double? MinValue { get; set; }

	/// <summary>
	/// The display priority for ordering graphs.
	/// </summary>
	public required int DisplayPriority { get; set; }

	/// <summary>
	/// The default timescale for the graph display.
	/// </summary>
	public required string Timescale { get; set; }

	/// <summary>
	/// Whether to use base-1024 (binary) units instead of base-1000 (SI).
	/// </summary>
	public required bool IsBase1024 { get; set; }

	/// <summary>
	/// Navigation property to the DataPoints displayed on this graph.
	/// </summary>
	public virtual ICollection<DataSourceDataPointStoreItem>? DataPoints { get; set; } = null!;
}
