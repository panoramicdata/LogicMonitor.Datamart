namespace LogicMonitor.Datamart.Mapping;

/// <summary>
/// AutoMapper profile for mapping LogicMonitor DataSource API objects to <see cref="DataSourceStoreItem"/>.
/// </summary>
public class DataSourceProfile : Profile
{
	/// <summary>
	/// Initializes a new instance of the <see cref="DataSourceProfile"/> class.
	/// </summary>
	public DataSourceProfile()
	{
		CreateMap<DataSource, DataSourceStoreItem>()
			.ForMember(
				dest => dest.Id,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.LogicMonitorId,
				opts => opts.MapFrom(src => src.Id))
			.ForMember(
				dest => dest.AuditVersion,
				opts => opts.MapFrom(src => src.AuditVersion.ToString(CultureInfo.InvariantCulture)))
			.ForMember(
				dest => dest.Version,
				opts => opts.MapFrom(src => src.Version.ToString(CultureInfo.InvariantCulture)))
			.ForMember(
				dest => dest.PayloadVersion,
				opts => opts.MapFrom(src => src.PayloadVersion.ToString(CultureInfo.InvariantCulture)))
			.ForMember(
				dest => dest.Graphs,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DatamartCreated,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DatamartLastModified,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DatamartLastObserved,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DeviceDataSources,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DataPoints,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.LastTimeSeriesDataSyncDurationMs,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.InstallationMetadataTargetLastPublishedId,
				opts => opts.MapFrom(src => src.InstallationMetadata == null ? null : src.InstallationMetadata.TargetLastPublishedId))
			.ForMember(
				dest => dest.InstallationMetadataTargetLastPublishedVersion,
				opts => opts.MapFrom(src => src.InstallationMetadata == null ? null : src.InstallationMetadata.TargetLastPublishedVersion))
			.ForMember(
				dest => dest.InstallationMetadataTargetLastPublishedChecksum,
				opts => opts.MapFrom(src => src.InstallationMetadata == null ? null : src.InstallationMetadata.TargetLastPublishedChecksum))
			.ForMember(
				dest => dest.InstallationMetadataLogicModuleId,
				opts => opts.MapFrom(src => src.InstallationMetadata == null ? null : (int?)src.InstallationMetadata.LocalModuleId))
			.ForMember(
				dest => dest.CollectionAttributeName,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.CollectionAttributeIp,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.InstallationMetadataOriginRegistryId,
				opts => opts.MapFrom(src => src.InstallationMetadata == null ? null : src.InstallationMetadata.OriginRegistryId))
			.ForMember(
				dest => dest.InstallationMetadataOriginLineageId,
				opts => opts.MapFrom(src => src.InstallationMetadata == null ? null : src.InstallationMetadata.OriginLineageId))
			.ForMember(
				dest => dest.InstallationMetadataOriginAuthorCompanyUuid,
				opts => opts.MapFrom(src => src.InstallationMetadata == null ? null : src.InstallationMetadata.OriginAuthorCompanyUUID))
			.ForMember(
				dest => dest.InstallationMetadataOriginAuthorNamespace,
				opts => opts.MapFrom(src => src.InstallationMetadata == null ? null : src.InstallationMetadata.OriginAuthorNamespace))
			.ForMember(
				dest => dest.InstallationMetadataOriginVersion,
				opts => opts.MapFrom(src => src.InstallationMetadata == null ? null : src.InstallationMetadata.OriginVersion))
			.ForMember(
				dest => dest.InstallationMetadataOriginChecksum,
				opts => opts.MapFrom(src => src.InstallationMetadata == null ? null : src.InstallationMetadata.OriginChecksum))
			.ForMember(
				dest => dest.InstallationMetadataAuditedRegistryId,
				opts => opts.MapFrom(src => src.InstallationMetadata == null ? null : src.InstallationMetadata.AuditedRegistryId))
			.ForMember(
				dest => dest.InstallationMetadataAuditedVersion,
				opts => opts.MapFrom(src => src.InstallationMetadata == null ? null : src.InstallationMetadata.AuditedVersion))
			.ForMember(
				dest => dest.InstallationMetadataTargetLineageId,
				opts => opts.MapFrom(src => src.InstallationMetadata == null ? null : src.InstallationMetadata.TargetLineageId))
			.ForMember(
				dest => dest.InstallationMetadataLogicModuleType,
				opts => opts.MapFrom(src => src.InstallationMetadata == null ? null : src.InstallationMetadata.LogicModuleType.ToString()))
			.ForMember(
				dest => dest.InstallationMetadataIsChangedFromOrigin,
				opts => opts.MapFrom(src => src.InstallationMetadata == null ? null : (bool?)src.InstallationMetadata.IsChangedFromOrigin))
			.ForMember(
				dest => dest.InstallationMetadataIsChangedFromTargetLastPublished,
				opts => opts.MapFrom(src => src.InstallationMetadata == null ? null : (bool?)src.InstallationMetadata.IsChangedFromTargetLastPublished))
			;

		CreateMap<DataSourceStoreItem, DataSource>()
			.ForMember(
				dest => dest.Id,
				opts => opts.MapFrom(src => src.LogicMonitorId))
			.ForMember(
				dest => dest.AgdMethod,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AgdParams,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AppliesTo,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AuditVersion,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AutoDiscoveryDeleteInactive,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AutoDiscoveryGroovyscript,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AutoDiscoveryIntervalMinutes,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AutoDiscoveryLinuxCmdLine,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AutoDiscoveryLinuxScript,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AutoDiscoveryWindowsCommandLine,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AutoDiscoveryWindowsScript,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.CollectionMethod,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.CollectionMethod,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.CollectorAttribute,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DisplayName,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.EnableEriDiscovery,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.EriDiscoveryInterval,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.EriDiscoveryConfig,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Group,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.HasMultiInstances,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.HasUnacknowledgedAlert,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.IsAutoDiscoveryDisabled,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.IsAutoDiscoveryEnabled,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AutoDiscoveryConfiguration,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.PollingIntervalSeconds,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Published,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Tags,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Technology,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.UseWildValueAsUuid,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Version,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DataSourceDataPoints,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.PayloadVersion,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Checksum,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.LineageId,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.InstallationMetadata,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AccessGroupIds,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AccessGroups,
				opts => opts.Ignore());
	}
}
