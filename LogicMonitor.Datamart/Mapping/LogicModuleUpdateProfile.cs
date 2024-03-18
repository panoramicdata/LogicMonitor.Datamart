namespace LogicMonitor.Datamart.Mapping;

public class LogicModuleUpdateProfile : Profile
{
	public LogicModuleUpdateProfile()
	{
		CreateMap<LogicModuleUpdate, LogicModuleUpdateStoreItem>()
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
				dest => dest.Category,
				opts => opts.MapFrom(src => src.Category.ToString())) // Convert enum to string
			.ForMember(
				dest => dest.Type,
				opts => opts.MapFrom(src => src.Type.ToString())) // Convert enum to string
			.AfterMap<TruncateMappingAction<LogicModuleUpdate, LogicModuleUpdateStoreItem>>();
	}
}
