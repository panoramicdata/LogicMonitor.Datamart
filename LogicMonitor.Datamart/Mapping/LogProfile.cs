namespace LogicMonitor.Datamart.Mapping;

public class LogProfile : Profile
{
	public LogProfile()
	{
		CreateMap<LogItem, LogStoreItem>()
			.ForMember(
				dest => dest.Id,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.LogicMonitorId,
				opts => opts.MapFrom(src => src.Id))
			.ForMember(
				dest => dest.DatamartCreated,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DatamartLastModified,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.UserName,
				opts => opts.MapFrom(src => src.PerformedByUsername))

			.AfterMap<TruncateMappingAction<LogItem, LogStoreItem>>();
	}
}
