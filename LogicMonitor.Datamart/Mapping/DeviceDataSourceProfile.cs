namespace LogicMonitor.Datamart.Mapping;

public class DeviceDataSourceProfile : Profile
{
	public DeviceDataSourceProfile()
	{
		CreateMap<ResourceDataSource, DeviceDataSourceStoreItem>()
			.ForMember(
				dest => dest.Id,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.LogicMonitorId,
				opts => opts.MapFrom(src => src.Id))
			.ForMember(
				dest => dest.Device,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DeviceId,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DataSource,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DataSourceId,
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
				dest => dest.DeviceDataSourceInstances,
				opts => opts.Ignore())
			;

		CreateMap<DeviceDataSourceStoreItem, ResourceDataSource>()
			.ForMember(
				dest => dest.Id,
				opts => opts.MapFrom(src => src.LogicMonitorId))
			.ForMember(
				dest => dest.AlertDisableStatus,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AlertStatus,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AlertStatusPriority,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AlertingDisabledOn,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.CollectionMethod,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DataSourceType,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DataSourceDisplayName,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DataSourceGraphs,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DataSourceId,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.ResourceId,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.GroupName,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DisabledInstanceGroups,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.InstanceAutoGroupEnabled,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.InstanceCount,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.MonitoringInstanceCount,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.NextAutoDiscoveryOnSeconds,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.OverviewGraphs,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.IsAutoDiscoveryEnabled,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.IsMonitoringDisabled,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.IsMultiple,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.SdtStatus,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Status,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.SdtAt,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.ResourceName,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.ResourceDisplayName,
				opts => opts.Ignore())
			;
	}
}
