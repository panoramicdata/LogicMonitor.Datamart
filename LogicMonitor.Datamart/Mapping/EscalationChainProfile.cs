using System.Text.Json;

namespace LogicMonitor.Datamart.Mapping;

/// <summary>
/// AutoMapper profile for mapping LogicMonitor EscalationChain API objects to <see cref="EscalationChainStoreItem"/>.
/// </summary>
public class EscalationChainProfile : Profile
{
	private static readonly JsonSerializerOptions _jsonSerializerOptions = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="EscalationChainProfile"/> class.
	/// </summary>
	public EscalationChainProfile()
	{
		CreateEscalationChainToEscalationChainStoreItemMap();
		CreateEscalationChainStoreItemToEscalationChainMap();
	}

	private void CreateEscalationChainToEscalationChainStoreItemMap()
	{
		CreateMap<EscalationChain, EscalationChainStoreItem>()
			.IgnoreMembers(
				dest => dest.Id, dest => dest.AlertRules, dest => dest.DatamartCreated, dest => dest.DatamartLastModified,
				dest => dest.DatamartLastObserved)
			.ForMember(
				dest => dest.LogicMonitorId,
				opts => opts.MapFrom(src => src.Id))
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
				opts => opts.MapFrom(src => JsonSerializer.Serialize(src.CcDestinations, _jsonSerializerOptions)));
	}

	private void CreateEscalationChainStoreItemToEscalationChainMap()
	{
		CreateMap<EscalationChainStoreItem, EscalationChain>()
			.IgnoreMembers(dest => dest.CcDestination, dest => dest.CcDestinations, dest => dest.Destination, dest => dest.Destinations)
			.ForMember(
				dest => dest.Id,
				opts => opts.MapFrom(src => src.LogicMonitorId));
	}
}
