using System.Text.Json;

namespace LogicMonitor.Datamart.Mapping;

public class IntegrationProfile : Profile
{
	public IntegrationProfile()
	{
		var serializationOptions = new JsonSerializerOptions
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase
		};

		CreateMap<HttpIntegration, IntegrationStoreItem>()
			.ForMember(
				dest => dest.Headers,
				opts => opts.MapFrom(src => JsonSerializer.Serialize(src.Headers, serializationOptions))
			)
			.ForMember(
				dest => dest.UpdateHeaders,
				opts => opts.MapFrom(src => JsonSerializer.Serialize(src.UpdateHeaders, serializationOptions))
			)
			.ForMember(
				dest => dest.UpdateDataHeaders,
				opts => opts.MapFrom(src => JsonSerializer.Serialize(src.UpdateDataHeaders, serializationOptions))
			)
			.ForMember(
				dest => dest.AckHeaders,
				opts => opts.MapFrom(src => JsonSerializer.Serialize(src.AckHeaders, serializationOptions))
			)
			.ForMember(
				dest => dest.ClearHeaders,
				opts => opts.MapFrom(src => JsonSerializer.Serialize(src.ClearHeaders, serializationOptions))
			)
			;
	}
}