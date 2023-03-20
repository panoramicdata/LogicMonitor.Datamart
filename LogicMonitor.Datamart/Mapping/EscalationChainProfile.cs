namespace LogicMonitor.Datamart.Mapping;

public class EscalationChainProfile : Profile
{
	public EscalationChainProfile()
	{
		CreateMap<EscalationChain, EscalationChainStoreItem>()
			.ForMember(
				dest => dest.Id,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.LogicMonitorId,
				opts => opts.MapFrom(src => src.Id))
			.ForMember(
				dest => dest.AlertRules,
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
			;
		CreateMap<EscalationChainStoreItem, EscalationChain>()
			.ForMember(
				dest => dest.Id,
				opts => opts.MapFrom(src => src.LogicMonitorId))
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
