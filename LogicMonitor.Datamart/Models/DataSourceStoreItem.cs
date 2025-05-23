namespace LogicMonitor.Datamart.Models;

public class DataSourceStoreItem : LogicModuleStoreItem
{
	// Navigation properties
	public virtual ICollection<ResourceDataSourceStoreItem>? DeviceDataSources { get; set; } = null!;

	public virtual ICollection<DataSourceGraphStoreItem>? Graphs { get; set; } = null!;

	public virtual ICollection<ResourceDataSourceInstanceDataPointStoreItem>? DataPoints { get; set; } = null!;

	// Database properties
	public required string Technology { get; set; }

	public required string Tags { get; set; }

	public required string LineageId { get; set; }

	public required string? InstallationMetadataOriginRegistryId { get; set; }

	public required string? InstallationMetadataOriginLineageId { get; set; }

	public required string? InstallationMetadataOriginAuthorCompanyUuid { get; set; }

	public required string? InstallationMetadataOriginAuthorNamespace { get; set; }

	public required string? InstallationMetadataOriginVersion { get; set; }

	public required string? InstallationMetadataOriginChecksum { get; set; }

	public required string? InstallationMetadataAuditedRegistryId { get; set; }

	public required string? InstallationMetadataAuditedVersion { get; set; }

	public required string? InstallationMetadataTargetLineageId { get; set; }

	public required string? InstallationMetadataTargetLastPublishedId { get; set; }

	public required string? InstallationMetadataTargetLastPublishedVersion { get; set; }

	public required string? InstallationMetadataTargetLastPublishedChecksum { get; set; }

	public required string? InstallationMetadataLogicModuleType { get; set; }

	public required int? InstallationMetadataLogicModuleId { get; set; }

	public required bool? InstallationMetadataIsChangedFromOrigin { get; set; }

	public required bool? InstallationMetadataIsChangedFromTargetLastPublished { get; set; }

	public required string PayloadVersion { get; set; }

	public required bool HasMultiInstances { get; set; }

	public required bool UseWildValueAsUuid { get; set; }

	public required int PollingIntervalSeconds { get; set; }

	public required string? CollectionAttributeName { get; set; }

	public required string? CollectionAttributeIp { get; set; }

	public required long? LastTimeSeriesDataSyncDurationMs { get; set; }
}
