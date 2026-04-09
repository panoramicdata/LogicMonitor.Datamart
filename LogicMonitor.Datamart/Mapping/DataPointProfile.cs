namespace LogicMonitor.Datamart.Mapping;

/// <summary>
/// AutoMapper profile for mapping LogicMonitor DataPoint API objects to <see cref="DataSourceDataPointStoreItem"/>.
/// </summary>
public class DataPointProfile : Profile
{
	/// <summary>
	/// Initializes a new instance of the <see cref="DataPointProfile"/> class.
	/// </summary>
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
