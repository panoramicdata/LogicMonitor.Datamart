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
		CreateMap<ConfigSource, ConfigSourceStoreItem>()
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
				dest => dest.CollectionMethod,
				opts => opts.MapFrom(src => src.CollectionMethod.ToString()))
			.ForMember(
				dest => dest.DatamartCreated,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DatamartLastModified,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DatamartLastObserved,
				opts => opts.Ignore())
			;
		CreateMap<ConfigSourceStoreItem, ConfigSource>()
			.ForMember(
				dest => dest.Id,
				opts => opts.MapFrom(src => src.LogicMonitorId))
			.ForMember(
				dest => dest.AuditVersion,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Version,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.CollectionMethod,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.ConfigChecks,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AutoDiscoveryConfig,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.CollectionIntervalSeconds,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.CollectorAttribute,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.EnableAutoDiscovery,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.FileFormat,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.HasMultiInstances,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Tags,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Technology,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.TimestampFormat,
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
