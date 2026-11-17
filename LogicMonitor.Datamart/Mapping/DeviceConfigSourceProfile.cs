namespace LogicMonitor.Datamart.Mapping;

/// <summary>
/// AutoMapper profile for mapping LogicMonitor DeviceConfigSource API objects to <see cref="ResourceConfigSourceStoreItem"/>.
/// </summary>
public class DeviceConfigSourceProfile : Profile
{
	/// <summary>
	/// Initializes a new instance of the <see cref="DeviceConfigSourceProfile"/> class.
	/// </summary>
	public DeviceConfigSourceProfile()
	{
		CreateResourceDataSourceToResourceConfigSourceStoreItemMap();
		CreateResourceConfigSourceStoreItemToResourceDataSourceMap();
	}

	private void CreateResourceDataSourceToResourceConfigSourceStoreItemMap()
	{
		CreateMap<ResourceDataSource, ResourceConfigSourceStoreItem>()
			.IgnoreMembers(
				dest => dest.Id, dest => dest.Device, dest => dest.DeviceId, dest => dest.ConfigSource,
				dest => dest.ConfigSourceId, dest => dest.DatamartCreated, dest => dest.DatamartLastModified, dest => dest.DatamartLastObserved,
				dest => dest.DeviceConfigSourceInstances)
			.ForMember(
				dest => dest.LogicMonitorId,
				opts => opts.MapFrom(src => src.Id));
	}

	private void CreateResourceConfigSourceStoreItemToResourceDataSourceMap()
	{
		CreateMap<ResourceConfigSourceStoreItem, ResourceDataSource>()
			.IgnoreMembers(
				dest => dest.AlertDisableStatus, dest => dest.AlertStatus, dest => dest.AlertStatusPriority, dest => dest.AlertingDisabledOn,
				dest => dest.CollectionMethod, dest => dest.DataSourceType, dest => dest.DataSourceName, dest => dest.DataSourceDescription,
				dest => dest.DataSourceDisplayName, dest => dest.DataSourceGraphs, dest => dest.DataSourceId, dest => dest.ResourceId,
				dest => dest.GroupName, dest => dest.DisabledInstanceGroups, dest => dest.InstanceAutoGroupEnabled, dest => dest.InstanceCount,
				dest => dest.MonitoringInstanceCount, dest => dest.NextAutoDiscoveryOnSeconds, dest => dest.OverviewGraphs, dest => dest.IsAutoDiscoveryEnabled,
				dest => dest.IsMonitoringDisabled, dest => dest.IsMultiple, dest => dest.SdtStatus, dest => dest.Status,
				dest => dest.SdtAt, dest => dest.ResourceName, dest => dest.ResourceDisplayName)
			.ForMember(
				dest => dest.Id,
				opts => opts.MapFrom(src => src.LogicMonitorId));
	}
}
