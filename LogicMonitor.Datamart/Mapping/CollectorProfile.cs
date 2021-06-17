using AutoMapper;
using LogicMonitor.Api.Collectors;
using LogicMonitor.Datamart.Models;

namespace LogicMonitor.Datamart.Mapping
{
	public class CollectorProfile : Profile
	{
		public CollectorProfile()
		{
			CreateMap<Collector, CollectorStoreItem>()
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
					dest => dest.Name,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.CollectorGroup,
					opts => opts.Ignore())
				;

			CreateMap<CollectorStoreItem, Collector>()
				.ForMember(
					dest => dest.AutomaticUpgradeInfo,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.CustomProperties,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.EscalationChain,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.NextUpgradeInfo,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.OneTimeUpgradeInfo,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.PredefinedConfiguration,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.InstanceCount,
					opts => opts.Ignore())
				;
		}
	}
}
