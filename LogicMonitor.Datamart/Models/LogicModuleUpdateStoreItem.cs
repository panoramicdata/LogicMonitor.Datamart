
namespace LogicMonitor.Datamart.Models;

/// <summary>
/// Represents an Exchange LogicModule record stored in the datamart.
/// </summary>
public class LogicModuleUpdateStoreItem : StoreItem
{
	/// <summary>
	/// The Exchange module ID (from ExchangeLogicModule.Id).
	/// </summary>
	[MaxLength(200)]
	public string ExchangeId { get; set; } = string.Empty;

	/// <summary>
	/// The name.
	/// </summary>
	[MaxLength(200)]
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// The display name.
	/// </summary>
	[MaxLength(200)]
	public string DisplayName { get; set; } = string.Empty;

	/// <summary>
	/// The module type.
	/// </summary>
	[MaxLength(50)]
	public string Type { get; set; } = string.Empty;

	/// <summary>
	/// The model.
	/// </summary>
	[MaxLength(200)]
	public string Model { get; set; } = string.Empty;

	/// <summary>
	/// The description.
	/// </summary>
	[MaxLength(1000)]
	public string Description { get; set; } = string.Empty;

	/// <summary>
	/// The collection method.
	/// </summary>
	[MaxLength(50)]
	public string CollectionMethod { get; set; } = string.Empty;

	/// <summary>
	/// The group.
	/// </summary>
	[MaxLength(200)]
	public string? Group { get; set; }

	/// <summary>
	/// The version string.
	/// </summary>
	[MaxLength(200)]
	public string Version { get; set; } = string.Empty;

	/// <summary>
	/// The status.
	/// </summary>
	[MaxLength(200)]
	public string Status { get; set; } = string.Empty;

	/// <summary>
	/// The source.
	/// </summary>
	[MaxLength(200)]
	public string Source { get; set; } = string.Empty;

	/// <summary>
	/// The locator.
	/// </summary>
	[MaxLength(200)]
	public string Locator { get; set; } = string.Empty;

	/// <summary>
	/// The author portal name.
	/// </summary>
	[MaxLength(200)]
	public string AuthorPortalName { get; set; } = string.Empty;

	/// <summary>
	/// The origin version.
	/// </summary>
	[MaxLength(200)]
	public string OriginVersion { get; set; } = string.Empty;

	/// <summary>
	/// The origin name.
	/// </summary>
	[MaxLength(200)]
	public string OriginName { get; set; } = string.Empty;

	/// <summary>
	/// The origin status.
	/// </summary>
	[MaxLength(200)]
	public string OriginStatus { get; set; } = string.Empty;

	/// <summary>
	/// The origin locator.
	/// </summary>
	[MaxLength(200)]
	public string OriginLocator { get; set; } = string.Empty;

	/// <summary>
	/// The origin author namespace.
	/// </summary>
	[MaxLength(200)]
	public string OriginAuthorNamespace { get; set; } = string.Empty;

	/// <summary>
	/// The origin registry ID.
	/// </summary>
	[MaxLength(200)]
	public string OriginRegistryId { get; set; } = string.Empty;

	/// <summary>
	/// The upgradeable registry ID.
	/// </summary>
	[MaxLength(200)]
	public string UpgradeableRegistryId { get; set; } = string.Empty;

	/// <summary>
	/// The origin published-at timestamp (milliseconds).
	/// </summary>
	public long OriginPublishedAtMs { get; set; }

	/// <summary>
	/// The updated-at timestamp (milliseconds).
	/// </summary>
	public long UpdatedAtMs { get; set; }

	/// <summary>
	/// Whether an update is available for this module.
	/// </summary>
	public bool HasUpdateAvailable { get; set; }

	/// <summary>
	/// Whether the module is installed.
	/// </summary>
	public bool IsInstalled { get; set; }

	/// <summary>
	/// Whether the module has been customized.
	/// </summary>
	public bool IsCustomized { get; set; }

	/// <summary>
	/// Whether the module is deprecated.
	/// </summary>
	public bool IsDeprecated { get; set; }

	/// <summary>
	/// Whether the module is in use.
	/// </summary>
	public bool IsInUse { get; set; }

	/// <summary>
	/// Whether the module has been changed from the last published target.
	/// </summary>
	public bool IsChangedFromTargetLastPublished { get; set; }

	/// <summary>
	/// The last time this was observed in the API.
	/// </summary>
	public DateTime DatamartLastObserved { get; internal set; }
}
