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
		CreateExchangeLogicModuleToLogicModuleUpdateStoreItemMap();
	}

	private void CreateExchangeLogicModuleToLogicModuleUpdateStoreItemMap()
	{
		CreateMap<ExchangeLogicModule, LogicModuleUpdateStoreItem>()
			.IgnoreMembers(dest => dest.Id, dest => dest.DatamartCreated, dest => dest.DatamartLastModified, dest => dest.DatamartLastObserved)
			.ForMember(
				dest => dest.ExchangeId,
				opts => opts.MapFrom(src => src.Id))
			.ForMember(
				dest => dest.Type,
				opts => opts.MapFrom(src => src.Type.ToString()))
			.AfterMap<TruncateMappingAction<ExchangeLogicModule, LogicModuleUpdateStoreItem>>();
	}
}
