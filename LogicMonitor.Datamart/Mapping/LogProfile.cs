namespace LogicMonitor.Datamart.Mapping;

/// <summary>
/// AutoMapper profile for mapping LogicMonitor Log API objects to <see cref="LogStoreItem"/>.
/// </summary>
public class LogProfile : Profile
{
	/// <summary>
	/// Initializes a new instance of the <see cref="LogProfile"/> class.
	/// </summary>
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