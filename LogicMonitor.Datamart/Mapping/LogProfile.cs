namespace LogicMonitor.Datamart.Mapping;

public class LogProfile : Profile
{
	public LogProfile()
	{
		CreateMap<LogItem, LogStoreItem>()
			.ForMember(
				dest => dest.DatamartId,
				opts => opts.Ignore())
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
