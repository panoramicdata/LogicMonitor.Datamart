namespace LogicMonitor.Datamart.Models;

public abstract class LogicModuleStoreItem : IdentifiedStoreItem
{
	public required string AppliesTo { get; set; }

	public required string AuditVersion { get; set; }

	public required string Checksum { get; set; }

	public required string CollectionMethod { get; set; }

	public required string Description { get; set; }

	public required string DisplayName { get; set; }

	public required string Group { get; set; }

	public required string Name { get; set; }

	public required string Version { get; set; }
}