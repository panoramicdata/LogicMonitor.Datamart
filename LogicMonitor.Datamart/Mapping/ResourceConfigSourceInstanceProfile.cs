namespace LogicMonitor.Datamart.Mapping;

public class ResourceConfigSourceInstanceProfile : Profile
{
	public ResourceConfigSourceInstanceProfile()
	{
		CreateMap<ResourceDataSourceInstance, ResourceConfigSourceInstanceStoreItem>()
			.ForMember(
				dest => dest.Id,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.LogicMonitorId,
				opts => opts.MapFrom(src => src.Id))
			.ForMember(
				dest => dest.DeviceConfigSourceId,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DeviceConfigSourceInstanceConfigs,
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
				dest => dest.DeviceConfigSource,
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
	}
}
