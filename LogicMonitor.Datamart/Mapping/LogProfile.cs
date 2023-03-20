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
				dest => dest.DatamartCreatedUtc,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DatamartLastModifiedUtc,
				opts => opts.Ignore())

			.AfterMap<TruncateMappingAction<LogItem, LogStoreItem>>()
			;
	}
}
