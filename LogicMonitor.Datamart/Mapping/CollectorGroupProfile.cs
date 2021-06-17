using AutoMapper;
using LogicMonitor.Api.Collectors;
using LogicMonitor.Datamart.Models;

namespace LogicMonitor.Datamart.Mapping
{
	public class CollectorGroupProfile : Profile
	{
		public CollectorGroupProfile()
		{
			CreateMap<CollectorGroup, CollectorGroupStoreItem>()
				.ForMember(
					dest => dest.DatamartId,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.DatamartCreatedUtc,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.DatamartLastModifiedUtc,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.DatamartLastObservedUtc,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.Collectors,
					opts => opts.Ignore())
				;
			CreateMap<CollectorGroupStoreItem, CollectorGroup>()
				.ForMember(
					dest => dest.CustomProperties,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.UserPermission,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.AutoBalance,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.AutoBalanceStrategy,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.AutoBalanceDeviceCountThrehsold,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.AutoBalanceInstanceCountThrehsold,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.MismatchVerison,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.Platform,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.InstanceCount,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.DeviceCount,
					opts => opts.Ignore())
				;
		}
	}
}
