namespace LogicMonitor.Datamart.Models;
public class DataSourceGraphStoreItem : IdentifiedStoreItem
{
	public DataSourceStoreItem? DataSource { get; set; } = null!;

	public Guid DataSourceId { get; set; }

	public bool IsOverview { get; set; }

	public required string Name { get; set; }

	public required string Title { get; set; }

	public required string VerticalLabel { get; set; }

	public required bool IsRigid { get; set; }

	public required int Width { get; set; }

	public required int Height { get; set; }

	public required double? MaxValue { get; set; }

	public required double? MinValue { get; set; }

	public required int DisplayPriority { get; set; }

	public required string Timescale { get; set; }

	public required bool IsBase1024 { get; set; }

	public ICollection<DataSourceDataPointStoreItem>? DataPoints { get; set; } = null!;
}
