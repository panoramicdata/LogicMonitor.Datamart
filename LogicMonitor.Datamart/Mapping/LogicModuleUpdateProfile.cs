namespace LogicMonitor.Datamart.Mapping;

/// <summary>
/// AutoMapper profile for mapping LogicMonitor ExchangeLogicModule API objects to <see cref="LogicModuleUpdateStoreItem"/>.
/// </summary>
public class LogicModuleUpdateProfile : Profile
{
	/// <summary>
	/// Initializes a new instance of the <see cref="LogicModuleUpdateProfile"/> class.
	/// </summary>
	public LogicModuleUpdateProfile()
	{
		CreateMap<ExchangeLogicModule, LogicModuleUpdateStoreItem>()
			.ForMember(
				dest => dest.Id,
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
				dest => dest.ExchangeId,
				opts => opts.MapFrom(src => src.Id))
			.ForMember(
				dest => dest.Type,
				opts => opts.MapFrom(src => src.Type.ToString()))
			.AfterMap<TruncateMappingAction<ExchangeLogicModule, LogicModuleUpdateStoreItem>>();
	}
}
