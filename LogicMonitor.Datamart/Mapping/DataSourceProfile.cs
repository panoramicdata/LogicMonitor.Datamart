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
		CreateDataSourceToDataSourceStoreItemMap();
		CreateDataSourceStoreItemToDataSourceMap();
	}

	private void CreateDataSourceToDataSourceStoreItemMap()
	{
		CreateMap<DataSource, DataSourceStoreItem>()
			.IgnoreMembers(
				dest => dest.Id, dest => dest.Graphs, dest => dest.DatamartCreated, dest => dest.DatamartLastModified,
				dest => dest.DatamartLastObserved, dest => dest.DeviceDataSources, dest => dest.DataPoints, dest => dest.LastTimeSeriesDataSyncDurationMs,
				dest => dest.CollectionAttributeName, dest => dest.CollectionAttributeIp)
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
			.ForMember(dest => dest.InstallationMetadataTargetLastPublishedId, opts => opts.MapFrom(src => Installation(src, m => m.TargetLastPublishedId)))
			.ForMember(dest => dest.InstallationMetadataTargetLastPublishedVersion, opts => opts.MapFrom(src => Installation(src, m => m.TargetLastPublishedVersion)))
			.ForMember(dest => dest.InstallationMetadataTargetLastPublishedChecksum, opts => opts.MapFrom(src => Installation(src, m => m.TargetLastPublishedChecksum)))
			.ForMember(dest => dest.InstallationMetadataLogicModuleId, opts => opts.MapFrom(src => Installation(src, m => (int?)m.LocalModuleId)))
			.ForMember(dest => dest.InstallationMetadataOriginRegistryId, opts => opts.MapFrom(src => Installation(src, m => m.OriginRegistryId)))
			.ForMember(dest => dest.InstallationMetadataOriginLineageId, opts => opts.MapFrom(src => Installation(src, m => m.OriginLineageId)))
			.ForMember(dest => dest.InstallationMetadataOriginAuthorCompanyUuid, opts => opts.MapFrom(src => Installation(src, m => m.OriginAuthorCompanyUUID)))
			.ForMember(dest => dest.InstallationMetadataOriginAuthorNamespace, opts => opts.MapFrom(src => Installation(src, m => m.OriginAuthorNamespace)))
			.ForMember(dest => dest.InstallationMetadataOriginVersion, opts => opts.MapFrom(src => Installation(src, m => m.OriginVersion)))
			.ForMember(dest => dest.InstallationMetadataOriginChecksum, opts => opts.MapFrom(src => Installation(src, m => m.OriginChecksum)))
			.ForMember(dest => dest.InstallationMetadataAuditedRegistryId, opts => opts.MapFrom(src => Installation(src, m => m.AuditedRegistryId)))
			.ForMember(dest => dest.InstallationMetadataAuditedVersion, opts => opts.MapFrom(src => Installation(src, m => m.AuditedVersion)))
			.ForMember(dest => dest.InstallationMetadataTargetLineageId, opts => opts.MapFrom(src => Installation(src, m => m.TargetLineageId)))
			.ForMember(dest => dest.InstallationMetadataLogicModuleType, opts => opts.MapFrom(src => Installation(src, m => m.LogicModuleType.ToString())))
			.ForMember(dest => dest.InstallationMetadataIsChangedFromOrigin, opts => opts.MapFrom(src => Installation(src, m => (bool?)m.IsChangedFromOrigin)))
			.ForMember(dest => dest.InstallationMetadataIsChangedFromTargetLastPublished, opts => opts.MapFrom(src => Installation(src, m => (bool?)m.IsChangedFromTargetLastPublished)));
	}

	private void CreateDataSourceStoreItemToDataSourceMap()
	{
		CreateMap<DataSourceStoreItem, DataSource>()
			.IgnoreMembers(
				dest => dest.AgdMethod, dest => dest.AgdParams, dest => dest.AppliesTo, dest => dest.AuditVersion,
				dest => dest.AutoDiscoveryDeleteInactive, dest => dest.AutoDiscoveryGroovyscript, dest => dest.AutoDiscoveryIntervalMinutes, dest => dest.AutoDiscoveryLinuxCmdLine,
				dest => dest.AutoDiscoveryLinuxScript, dest => dest.AutoDiscoveryWindowsCommandLine, dest => dest.AutoDiscoveryWindowsScript, dest => dest.CollectionMethod,
				dest => dest.CollectionMethod, dest => dest.CollectorAttribute, dest => dest.DisplayName, dest => dest.EnableEriDiscovery,
				dest => dest.EriDiscoveryInterval, dest => dest.EriDiscoveryConfig, dest => dest.Group, dest => dest.HasMultiInstances,
				dest => dest.HasUnacknowledgedAlert, dest => dest.IsAutoDiscoveryDisabled, dest => dest.IsAutoDiscoveryEnabled, dest => dest.AutoDiscoveryConfiguration,
				dest => dest.PollingIntervalSeconds, dest => dest.Published, dest => dest.Tags, dest => dest.Technology,
				dest => dest.UseWildValueAsUuid, dest => dest.Version, dest => dest.DataSourceDataPoints, dest => dest.PayloadVersion,
				dest => dest.Checksum, dest => dest.LineageId, dest => dest.InstallationMetadata, dest => dest.AccessGroupIds,
				dest => dest.AccessGroups)
			.ForMember(
				dest => dest.Id,
				opts => opts.MapFrom(src => src.LogicMonitorId))
			.ForMember(
				dest => dest.IsInUse,
				opts => opts.Ignore());
	}

	private static T Installation<T>(DataSource source, Func<IntegrationMetadata, T> select)
		=> source.InstallationMetadata is null ? default! : select(source.InstallationMetadata);
}
