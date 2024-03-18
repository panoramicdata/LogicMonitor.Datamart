
namespace LogicMonitor.Datamart.Models;

public class LogicModuleUpdateStoreItem : StoreItem
{
	/// <summary>
	/// The local ID
	/// </summary>
	public int LocalId { get; set; }

	/// <summary>
	/// The name
	/// </summary>
	[MaxLength(200)]
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// The appliesTo
	/// </summary>
	[MaxLength(1000)]
	public string AppliesTo { get; set; } = string.Empty;

	/// <summary>
	/// The permission
	/// </summary>
	[MaxLength(20)]
	public string Category { get; set; } = string.Empty;

	/// <summary>
	/// The type
	/// </summary>
	[MaxLength(20)]
	public string Type { get; set; } = string.Empty;

	/// <summary>
	/// The collection method
	/// </summary>
	[MaxLength(20)]
	public string CollectionMethod { get; set; } = string.Empty;

	/// <summary>
	/// The description
	/// </summary>
	[MaxLength(1000)]
	public string Description { get; set; } = string.Empty;

	/// <summary>
	/// The group
	/// </summary>
	[MaxLength(50)]
	public string? Group { get; set; }

	/// <summary>
	/// The version (installed). This is an epoch timestamp
	/// </summary>
	public long Version { get; set; }

	/// <summary>
	/// The local version. This is an epoch timestamp
	/// </summary>
	public long LocalVersion { get; set; }

	/// <summary>
	/// The audit version. This is an epoch timestamp
	/// </summary>
	public long AuditVersion { get; set; }

	/// <summary>
	/// The rest LM (?)
	/// </summary>
	[MaxLength(200)]
	public string? RestLm { get; set; } = string.Empty;

	/// <summary>
	/// The registryVersion
	/// </summary>
	[MaxLength(20)]
	public string RegistryVersion { get; set; } = string.Empty;

	/// <summary>
	/// The publish time
	/// </summary>
	public long PublishedAtMilliseconds { get; set; }

	/// <summary>
	/// The quality
	/// </summary>
	[MaxLength(20)]
	public string Quality { get; set; } = string.Empty;

	/// <summary>
	/// The locator
	/// </summary>
	[MaxLength(20)]
	public string Locator { get; set; } = string.Empty;

	/// <summary>
	/// The currentUuid
	/// </summary>
	[MaxLength(30)]
	public string CurrentUuid { get; set; } = string.Empty;

	/// <summary>
	/// The namespace
	/// </summary>
	[MaxLength(50)]
	public string Namespace { get; set; } = string.Empty;

	/// <summary>
	/// The local version
	/// </summary>
	[MaxLength(50)]
	public string Local { get; set; } = string.Empty;

	/// <summary>
	/// The remote version
	/// </summary>
	[MaxLength(1000)]
	public string Remote { get; set; } = string.Empty;

	/// <summary>
	/// The last time this was observed in the API
	/// </summary>
	public DateTime DatamartLastObserved { get; internal set; }
}
