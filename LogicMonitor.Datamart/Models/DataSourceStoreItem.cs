namespace LogicMonitor.Datamart.Models;

public class DataSourceStoreItem : IdentifiedStoreItem
{
	// Navigation properties
	public ICollection<DeviceDataSourceStoreItem>? DeviceDataSources { get; set; } = null!;

	public ICollection<DataSourceGraphStoreItem>? Graphs { get; set; } = null!;

	public ICollection<DeviceDataSourceInstanceDataPointStoreItem>? DataPoints { get; set; } = null!;

	// Database properties
	public required string Description { get; set; }

	public required string Group { get; set; } = string.Empty;

	public required string AppliesTo { get; set; }

	public required string Technology { get; set; }

	public required string Tags { get; set; }

	public required string Checksum { get; set; }

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

	public required string Name { get; set; }

	public required string DisplayName { get; set; }

	public required string Version { get; set; }

	public required string AuditVersion { get; set; }

	public required string PayloadVersion { get; set; }

	public required bool HasMultiInstances { get; set; }

	public required bool UseWildValueAsUuid { get; set; }

	public required int PollingIntervalSeconds { get; set; }

	public required string CollectionMethod { get; set; }

	public required string? CollectionAttributeName { get; set; }

	public required string? CollectionAttributeIp { get; set; }

	public required long? LastTimeSeriesDataSyncDurationMs { get; set; }
}
