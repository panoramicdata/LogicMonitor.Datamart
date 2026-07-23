namespace LogicMonitor.Datamart.Models;

/// <summary>
/// Abstract base class for LogicMonitor LogicModule entities (DataSources, ConfigSources, EventSources).
/// </summary>
public abstract class LogicModuleStoreItem : IdentifiedStoreItem
{
	/// <summary>
	/// The AppliesTo expression that determines which resources this LogicModule targets.
	/// </summary>
	public required string AppliesTo { get; set; }

	/// <summary>
	/// The audit version identifier for change tracking.
	/// </summary>
	public required string AuditVersion { get; set; }

	/// <summary>
	/// The checksum used to detect modifications to the LogicModule definition.
	/// </summary>
	public required string Checksum { get; set; }

	/// <summary>
	/// A human-readable description of the LogicModule.
	/// </summary>
	public required string Description { get; set; }

	/// <summary>
	/// The group classification for the LogicModule.
	/// </summary>
	public required string Group { get; set; }

	/// <summary>
	/// The unique name of the LogicModule.
	/// </summary>
	public required string Name { get; set; }

	/// <summary>
	/// The version string of the LogicModule definition.
	/// </summary>
	public required string Version { get; set; }

	/// <summary>
	/// Whether the LogicModule is currently in use by any resources.
	/// </summary>
	public required bool? IsInUse { get; set; }
}