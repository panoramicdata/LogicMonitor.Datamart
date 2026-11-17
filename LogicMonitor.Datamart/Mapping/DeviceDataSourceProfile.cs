namespace LogicMonitor.Datamart.Mapping;

/// <summary>
/// AutoMapper profile for mapping LogicMonitor DeviceDataSource API objects to <see cref="ResourceDataSourceStoreItem"/>.
/// </summary>
public class DeviceDataSourceProfile : Profile
{
	/// <summary>
	/// Initializes a new instance of the <see cref="DeviceDataSourceProfile"/> class.
	/// </summary>
	public DeviceDataSourceProfile()
	{
		CreateResourceDataSourceToResourceDataSourceStoreItemMap();
		CreateResourceDataSourceStoreItemToResourceDataSourceMap();
	}

	private void CreateResourceDataSourceToResourceDataSourceStoreItemMap()
	{
		CreateMap<ResourceDataSource, ResourceDataSourceStoreItem>()
			.IgnoreMembers(
				dest => dest.Id, dest => dest.Device, dest => dest.DeviceId, dest => dest.DataSource,
				dest => dest.DataSource, dest => dest.DataSourceId, dest => dest.DatamartCreated, dest => dest.DatamartLastModified,
				dest => dest.DatamartLastObserved, dest => dest.DeviceDataSourceInstances)
			.ForMember(
				dest => dest.LogicMonitorId,
				opts => opts.MapFrom(src => src.Id));
	}

	private void CreateResourceDataSourceStoreItemToResourceDataSourceMap()
	{
		CreateMap<ResourceDataSourceStoreItem, ResourceDataSource>()
			.IgnoreMembers(
				dest => dest.AlertDisableStatus, dest => dest.AlertStatus, dest => dest.AlertStatusPriority, dest => dest.AlertingDisabledOn,
				dest => dest.CollectionMethod, dest => dest.DataSourceType, dest => dest.DataSourceDisplayName, dest => dest.DataSourceGraphs,
				dest => dest.DataSourceId, dest => dest.ResourceId, dest => dest.GroupName, dest => dest.DisabledInstanceGroups,
				dest => dest.InstanceAutoGroupEnabled, dest => dest.InstanceCount, dest => dest.MonitoringInstanceCount, dest => dest.NextAutoDiscoveryOnSeconds,
				dest => dest.OverviewGraphs, dest => dest.IsAutoDiscoveryEnabled, dest => dest.IsMonitoringDisabled, dest => dest.IsMultiple,
				dest => dest.SdtStatus, dest => dest.Status, dest => dest.SdtAt, dest => dest.ResourceName,
				dest => dest.ResourceDisplayName, dest => dest.DataSourceName, dest => dest.DataSourceDescription)
			.ForMember(
				dest => dest.Id,
				opts => opts.MapFrom(src => src.LogicMonitorId));
	}
}
