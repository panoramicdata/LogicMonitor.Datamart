using AutoMapper;
using LogicMonitor.Api.Alerts;
using LogicMonitor.Datamart.Models;

namespace LogicMonitor.Datamart.Mapping
{
	public class EscalationChainProfile : Profile
	{
		public EscalationChainProfile()
		{
			CreateMap<EscalationChain, EscalationChainStoreItem>()
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
			CreateMap<EscalationChainStoreItem, EscalationChain>()
				.ForMember(
					dest => dest.CcDestination,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.CcDestinations,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.Destination,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.Destinations,
					opts => opts.Ignore())
				;
		}
	}
}
