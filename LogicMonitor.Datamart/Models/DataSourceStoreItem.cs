namespace LogicMonitor.Datamart.Models;

/// <summary>
/// Represents a LogicMonitor DataSource stored in the datamart.
/// </summary>
public class DataSourceStoreItem : LogicModuleStoreItem
{
	/// <summary>
	/// Navigation property to the resource-DataSource assignments.
	/// </summary>
	public virtual ICollection<ResourceDataSourceStoreItem>? DeviceDataSources { get; set; } = null!;

	/// <summary>
	/// Navigation property to the graphs defined for this DataSource.
	/// </summary>
	public virtual ICollection<DataSourceGraphStoreItem>? Graphs { get; set; } = null!;

	/// <summary>
	/// Navigation property to the DataPoints for this DataSource.
	/// </summary>
	public virtual ICollection<ResourceDataSourceInstanceDataPointStoreItem>? DataPoints { get; set; } = null!;

	/// <summary>
	/// The technology tag for the DataSource (e.g. WMI, SNMP).
	/// </summary>
	public required string Technology { get; set; }

	/// <summary>
	/// Comma-separated tags associated with the DataSource.
	/// </summary>
	public required string Tags { get; set; }

	/// <summary>
	/// The lineage identifier used for tracking DataSource provenance.
	/// </summary>
	public required string LineageId { get; set; }

	/// <summary>
	/// The registry identifier of the origin DataSource.
	/// </summary>
	public required string? InstallationMetadataOriginRegistryId { get; set; }

	/// <summary>
	/// The lineage identifier of the origin DataSource.
	/// </summary>
	public required string? InstallationMetadataOriginLineageId { get; set; }

	/// <summary>
	/// The UUID of the company that authored the origin DataSource.
	/// </summary>
	public required string? InstallationMetadataOriginAuthorCompanyUuid { get; set; }

	/// <summary>
	/// The namespace of the origin DataSource author.
	/// </summary>
	public required string? InstallationMetadataOriginAuthorNamespace { get; set; }

	/// <summary>
	/// The version of the origin DataSource.
	/// </summary>
	public required string? InstallationMetadataOriginVersion { get; set; }

	/// <summary>
	/// The checksum of the origin DataSource.
	/// </summary>
	public required string? InstallationMetadataOriginChecksum { get; set; }

	/// <summary>
	/// The registry identifier of the audited DataSource version.
	/// </summary>
	public required string? InstallationMetadataAuditedRegistryId { get; set; }

	/// <summary>
	/// The version of the audited DataSource.
	/// </summary>
	public required string? InstallationMetadataAuditedVersion { get; set; }

	/// <summary>
	/// The lineage identifier of the target DataSource.
	/// </summary>
	public required string? InstallationMetadataTargetLineageId { get; set; }

	/// <summary>
	/// The last published identifier of the target DataSource.
	/// </summary>
	public required string? InstallationMetadataTargetLastPublishedId { get; set; }

	/// <summary>
	/// The last published version of the target DataSource.
	/// </summary>
	public required string? InstallationMetadataTargetLastPublishedVersion { get; set; }

	/// <summary>
	/// The last published checksum of the target DataSource.
	/// </summary>
	public required string? InstallationMetadataTargetLastPublishedChecksum { get; set; }

	/// <summary>
	/// The LogicModule type from installation metadata.
	/// </summary>
	public required string? InstallationMetadataLogicModuleType { get; set; }

	/// <summary>
	/// The LogicModule identifier from installation metadata.
	/// </summary>
	public required int? InstallationMetadataLogicModuleId { get; set; }

	/// <summary>
	/// Whether the DataSource has been changed from its origin version.
	/// </summary>
	public required bool? InstallationMetadataIsChangedFromOrigin { get; set; }

	/// <summary>
	/// Whether the DataSource has been changed from the last published target version.
	/// </summary>
	public required bool? InstallationMetadataIsChangedFromTargetLastPublished { get; set; }

	/// <summary>
	/// The payload version of the DataSource definition.
	/// </summary>
	public required string PayloadVersion { get; set; }

	/// <summary>
	/// Whether the DataSource supports multiple instances.
	/// </summary>
	public required bool HasMultiInstances { get; set; }

	/// <summary>
	/// Whether the wild value is used as the instance UUID.
	/// </summary>
	public required bool UseWildValueAsUuid { get; set; }

	/// <summary>
	/// The polling interval in seconds for data collection.
	/// </summary>
	public required int PollingIntervalSeconds { get; set; }

	/// <summary>
	/// The collection attribute name used for data gathering.
	/// </summary>
	public required string? CollectionAttributeName { get; set; }

	/// <summary>
	/// The collection attribute IP address used for data gathering.
	/// </summary>
	public required string? CollectionAttributeIp { get; set; }

	/// <summary>
	/// The duration in milliseconds of the last time-series data sync for this DataSource.
	/// </summary>
	public required long? LastTimeSeriesDataSyncDurationMs { get; set; }
}
