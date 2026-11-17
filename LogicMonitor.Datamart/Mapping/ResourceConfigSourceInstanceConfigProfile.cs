namespace LogicMonitor.Datamart.Mapping;

/// <summary>
/// AutoMapper profile for mapping LogicMonitor DeviceConfigSourceInstanceConfig API objects to <see cref="ResourceConfigSourceInstanceConfigStoreItem"/>.
/// </summary>
public class ResourceConfigSourceInstanceConfigProfile : Profile
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ResourceConfigSourceInstanceConfigProfile"/> class.
	/// </summary>
	public ResourceConfigSourceInstanceConfigProfile()
	{
		CreateResourceDataSourceInstanceConfigToResourceConfigSourceInstanceConfigStoreItemMap();
	}

	private void CreateResourceDataSourceInstanceConfigToResourceConfigSourceInstanceConfigStoreItemMap()
	{
		CreateMap<ResourceDataSourceInstanceConfig, ResourceConfigSourceInstanceConfigStoreItem>()
			.IgnoreMembers(
				dest => dest.Id, dest => dest.LogicMonitorId, dest => dest.DeviceConfigSourceInstanceId, dest => dest.DatamartCreated,
				dest => dest.DatamartLastModified, dest => dest.DatamartLastObserved, dest => dest.DeviceConfigSourceInstance, dest => dest.LogicMonitorStringId)
			.ForMember(
				dest => dest.LogicMonitorStringId,
				opts => opts.MapFrom(src => src.Id));
	}
}
