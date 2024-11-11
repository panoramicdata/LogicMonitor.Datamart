namespace LogicMonitor.Datamart.Mapping;

public class ResourceDataSourceInstanceProfile : Profile
{
	public ResourceDataSourceInstanceProfile()
	{
		CreateMap<ResourceDataSourceInstance, ResourceDataSourceInstanceStoreItem>()
			.ForMember(
				dest => dest.Id,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.LogicMonitorId,
				opts => opts.MapFrom(src => src.Id))
			.ForMember(
				dest => dest.DeviceDataSourceId,
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
				dest => dest.DeviceDataSource,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DeviceDataSourceInstanceDataPoints,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.LastWentMissing,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.SdtAt,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.SdtStatus,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.InstanceProperty1,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.InstanceProperty2,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.InstanceProperty3,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.InstanceProperty4,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.InstanceProperty5,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.InstanceProperty6,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.InstanceProperty7,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.InstanceProperty8,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.InstanceProperty9,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.InstanceProperty10,
				opts => opts.Ignore())
			;

		CreateMap<ResourceDataSourceInstanceStoreItem, ResourceDataSourceInstance>()
			.ForMember(
				dest => dest.Id,
				opts => opts.MapFrom(src => src.LogicMonitorId))
			.ForMember(
				dest => dest.AlertDisabledOn,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.CustomProperties,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DataSourceId,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.ResourceDataSourceId,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.ResourceDisplayName,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.ResourceId,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.SystemProperties,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.IsUncInstance,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AutoProperties,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.CollectorId,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DataSourceName,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DataSourceType,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.GroupsDisabledThisSource,
				opts => opts.Ignore())
		;
	}
}
