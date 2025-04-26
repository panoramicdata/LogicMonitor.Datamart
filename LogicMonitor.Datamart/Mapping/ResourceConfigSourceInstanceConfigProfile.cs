namespace LogicMonitor.Datamart.Mapping;

public class ResourceConfigSourceInstanceConfigProfile : Profile
{
	public ResourceConfigSourceInstanceConfigProfile()
	{
		CreateMap<ResourceDataSourceInstanceConfig, ResourceConfigSourceInstanceConfigStoreItem>()
			.ForMember(
				dest => dest.Id,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.LogicMonitorId,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.LogicMonitorStringId,
				opts => opts.MapFrom(src => src.Id))
			.ForMember(
				dest => dest.DeviceConfigSourceInstanceId,
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
				dest => dest.DeviceConfigSourceInstance,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.LogicMonitorStringId,
				opts => opts.Ignore())
			;
	}
}
