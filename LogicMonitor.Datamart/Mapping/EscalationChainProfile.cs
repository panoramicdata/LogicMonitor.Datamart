using System.Text.Json;

namespace LogicMonitor.Datamart.Mapping;

public class EscalationChainProfile : Profile
{
	private static readonly JsonSerializerOptions _jsonSerializerOptions = new();

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
				dest => dest.DatamartCreated,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DatamartLastModified,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DatamartLastObserved,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Destination,
				opts => opts.MapFrom(src => JsonSerializer.Serialize(src.Destination, _jsonSerializerOptions)))
			.ForMember(
				dest => dest.Destinations,
				opts => opts.MapFrom(src => JsonSerializer.Serialize(src.Destinations, _jsonSerializerOptions)))
			.ForMember(
				dest => dest.CcDestination,
				opts => opts.MapFrom(src => JsonSerializer.Serialize(src.CcDestination, _jsonSerializerOptions)))
			.ForMember(
				dest => dest.CcDestinations,
				opts => opts.MapFrom(src => JsonSerializer.Serialize(src.CcDestinations, _jsonSerializerOptions)))
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
