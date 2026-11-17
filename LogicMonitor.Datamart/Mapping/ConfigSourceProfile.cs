namespace LogicMonitor.Datamart.Mapping;

/// <summary>
/// AutoMapper profile for mapping LogicMonitor ConfigSource API objects to <see cref="ConfigSourceStoreItem"/>.
/// </summary>
public class ConfigSourceProfile : Profile
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ConfigSourceProfile"/> class.
	/// </summary>
	public ConfigSourceProfile()
	{
		CreateConfigSourceToConfigSourceStoreItemMap();
		CreateConfigSourceStoreItemToConfigSourceMap();
	}

	private void CreateConfigSourceToConfigSourceStoreItemMap()
	{
		CreateMap<ConfigSource, ConfigSourceStoreItem>()
			.IgnoreMembers(dest => dest.Id, dest => dest.DatamartCreated, dest => dest.DatamartLastModified, dest => dest.DatamartLastObserved)
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
				dest => dest.CollectionMethod,
				opts => opts.MapFrom(src => src.CollectionMethod.ToString()));
	}

	private void CreateConfigSourceStoreItemToConfigSourceMap()
	{
		CreateMap<ConfigSourceStoreItem, ConfigSource>()
			.IgnoreMembers(
				dest => dest.AuditVersion, dest => dest.Version, dest => dest.CollectionMethod, dest => dest.ConfigChecks,
				dest => dest.AutoDiscoveryConfig, dest => dest.CollectionIntervalSeconds, dest => dest.CollectorAttribute, dest => dest.EnableAutoDiscovery,
				dest => dest.FileFormat, dest => dest.HasMultiInstances, dest => dest.Tags, dest => dest.Technology,
				dest => dest.TimestampFormat, dest => dest.LineageId, dest => dest.InstallationMetadata, dest => dest.AccessGroupIds,
				dest => dest.AccessGroups)
			.ForMember(
				dest => dest.Id,
				opts => opts.MapFrom(src => src.LogicMonitorId))
			.ForMember(
				dest => dest.IsInUse,
				opts => opts.Ignore());
	}
}
