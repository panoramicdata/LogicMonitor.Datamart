using AutoMapper;
using LogicMonitor.Api.LogicModules;
using LogicMonitor.Datamart.Models;

namespace LogicMonitor.Datamart.Mapping
{
	public class ConfigSourceProfile : Profile
	{
		public ConfigSourceProfile()
		{
			CreateMap<ConfigSource, ConfigSourceStoreItem>()
				.ForMember(
					dest => dest.DatamartId,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.DatamartCreatedUtc,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.DatamartLastModifiedUtc,
					opts => opts.Ignore())
				;
			CreateMap<ConfigSourceStoreItem, ConfigSource>()
				.ForMember(
					dest => dest.ConfigChecks,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.Published,
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
					dest => dest.Retention,
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
				;
		}
	}
}
