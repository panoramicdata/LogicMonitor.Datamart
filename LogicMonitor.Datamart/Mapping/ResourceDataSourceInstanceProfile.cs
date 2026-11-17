namespace LogicMonitor.Datamart.Mapping;

/// <summary>
/// AutoMapper profile for mapping LogicMonitor DeviceDataSourceInstance API objects to <see cref="ResourceDataSourceInstanceStoreItem"/>.
/// </summary>
public class ResourceDataSourceInstanceProfile : Profile
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ResourceDataSourceInstanceProfile"/> class.
	/// </summary>
	public ResourceDataSourceInstanceProfile()
	{
		CreateResourceDataSourceInstanceToResourceDataSourceInstanceStoreItemMap();
		CreateResourceDataSourceInstanceStoreItemToResourceDataSourceInstanceMap();
	}

	private void CreateResourceDataSourceInstanceToResourceDataSourceInstanceStoreItemMap()
	{
		CreateMap<ResourceDataSourceInstance, ResourceDataSourceInstanceStoreItem>()
			.IgnoreMembers(
				dest => dest.Id, dest => dest.DeviceDataSourceId, dest => dest.DatamartCreated, dest => dest.DatamartLastModified,
				dest => dest.DatamartLastObserved, dest => dest.DeviceDataSource, dest => dest.DeviceDataSourceInstanceDataPoints, dest => dest.LastWentMissing,
				dest => dest.SdtAt, dest => dest.SdtStatus, dest => dest.InstanceProperty1, dest => dest.InstanceProperty2,
				dest => dest.InstanceProperty3, dest => dest.InstanceProperty4, dest => dest.InstanceProperty5, dest => dest.InstanceProperty6,
				dest => dest.InstanceProperty7, dest => dest.InstanceProperty8, dest => dest.InstanceProperty9, dest => dest.InstanceProperty10)
			.ForMember(
				dest => dest.LogicMonitorId,
				opts => opts.MapFrom(src => src.Id));
	}

	private void CreateResourceDataSourceInstanceStoreItemToResourceDataSourceInstanceMap()
	{
		CreateMap<ResourceDataSourceInstanceStoreItem, ResourceDataSourceInstance>()
			.IgnoreMembers(
				dest => dest.AlertDisabledOn, dest => dest.CustomProperties, dest => dest.DataSourceId, dest => dest.ResourceDataSourceId,
				dest => dest.ResourceDisplayName, dest => dest.ResourceId, dest => dest.SystemProperties, dest => dest.IsUncInstance,
				dest => dest.AutoProperties, dest => dest.CollectorId, dest => dest.DataSourceName, dest => dest.DataSourceType,
				dest => dest.GroupsDisabledThisSource)
			.ForMember(
				dest => dest.Id,
				opts => opts.MapFrom(src => src.LogicMonitorId));
	}
}
