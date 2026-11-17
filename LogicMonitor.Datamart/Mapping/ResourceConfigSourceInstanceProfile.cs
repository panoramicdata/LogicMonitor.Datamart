namespace LogicMonitor.Datamart.Mapping;

/// <summary>
/// AutoMapper profile for mapping LogicMonitor DeviceConfigSourceInstance API objects to <see cref="ResourceConfigSourceInstanceStoreItem"/>.
/// </summary>
public class ResourceConfigSourceInstanceProfile : Profile
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ResourceConfigSourceInstanceProfile"/> class.
	/// </summary>
	public ResourceConfigSourceInstanceProfile()
	{
		CreateResourceDataSourceInstanceToResourceConfigSourceInstanceStoreItemMap();
	}

	private void CreateResourceDataSourceInstanceToResourceConfigSourceInstanceStoreItemMap()
	{
		CreateMap<ResourceDataSourceInstance, ResourceConfigSourceInstanceStoreItem>()
			.IgnoreMembers(
				dest => dest.Id, dest => dest.DeviceConfigSourceId, dest => dest.DeviceConfigSourceInstanceConfigs, dest => dest.DatamartCreated,
				dest => dest.DatamartLastModified, dest => dest.DatamartLastObserved, dest => dest.DeviceConfigSource, dest => dest.LastWentMissing,
				dest => dest.SdtAt, dest => dest.SdtStatus, dest => dest.InstanceProperty1, dest => dest.InstanceProperty2,
				dest => dest.InstanceProperty3, dest => dest.InstanceProperty4, dest => dest.InstanceProperty5, dest => dest.InstanceProperty6,
				dest => dest.InstanceProperty7, dest => dest.InstanceProperty8, dest => dest.InstanceProperty9, dest => dest.InstanceProperty10)
			.ForMember(
				dest => dest.LogicMonitorId,
				opts => opts.MapFrom(src => src.Id));
	}
}
