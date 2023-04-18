namespace LogicMonitor.Datamart.Mapping;

public class DeviceDataSourceInstanceProfile : Profile
{
	public DeviceDataSourceInstanceProfile()
	{
		CreateMap<DeviceDataSourceInstance, DeviceDataSourceInstanceStoreItem>()
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
				dest => dest.DataCompleteTo,
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
			;

		CreateMap<DeviceDataSourceInstanceStoreItem, DeviceDataSourceInstance>()
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
				dest => dest.DeviceDataSourceId,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DeviceDisplayName,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DeviceId,
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
			.ForMember(
				dest => dest.Name,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Description,
				opts => opts.Ignore())
		;
	}
}
