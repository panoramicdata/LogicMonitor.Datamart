namespace LogicMonitor.Datamart.Mapping;

public class DataPointProfile : Profile
{
	public DataPointProfile()
	{
		CreateMap<DataPointConfigurationItem, DataSourceDataPointStoreItem>()
			.ForMember(
				dest => dest.Id,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DataSourceId,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DataSource,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DeviceDataSourceInstanceDataPoints,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.LogicMonitorId,
				opts => opts.MapFrom(src => 0))
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
				dest => dest.GlobalAlertExpression,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DataSourceGraphId,
				opts => opts.Ignore())
			;
	}
}
