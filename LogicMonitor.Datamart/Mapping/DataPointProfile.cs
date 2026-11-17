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
		CreateDataPointConfigurationItemToDataSourceDataPointStoreItemMap();
	}

	private void CreateDataPointConfigurationItemToDataSourceDataPointStoreItemMap()
	{
		CreateMap<DataPointConfigurationItem, DataSourceDataPointStoreItem>()
			.IgnoreMembers(
				dest => dest.Id, dest => dest.DataSourceId, dest => dest.DataSource, dest => dest.DeviceDataSourceInstanceDataPoints,
				dest => dest.DatamartCreated, dest => dest.DatamartLastModified, dest => dest.DatamartLastObserved, dest => dest.GlobalAlertExpression,
				dest => dest.DataSourceGraphId)
			.ForMember(
				dest => dest.LogicMonitorId,
				opts => opts.MapFrom(src => 0));
	}
}
