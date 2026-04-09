namespace LogicMonitor.Datamart.Mapping;

/// <summary>
/// AutoMapper profile for mapping LogicMonitor LogicModuleUpdate API objects to <see cref="LogicModuleUpdateStoreItem"/>.
/// </summary>
public class LogicModuleUpdateProfile : Profile
{
	/// <summary>
	/// Initializes a new instance of the <see cref="LogicModuleUpdateProfile"/> class.
	/// </summary>
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
