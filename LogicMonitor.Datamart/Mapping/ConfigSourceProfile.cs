namespace LogicMonitor.Datamart.Mapping;

public class ConfigSourceProfile : Profile
{
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
				dest => dest.ConfigChecks,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AppliesTo,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AuditVersion,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AutoDiscoveryConfig,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.CollectionIntervalSeconds,
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
				dest => dest.EnableAutoDiscovery,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.FileFormat,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Group,
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
				dest => dest.Version,
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
