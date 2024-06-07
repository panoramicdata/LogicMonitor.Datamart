namespace LogicMonitor.Datamart.Models;

public class ConfigSourceStoreItem : IdentifiedStoreItem
{
	public required string Name { get; set; }

	public required string Description { get; set; }
}
