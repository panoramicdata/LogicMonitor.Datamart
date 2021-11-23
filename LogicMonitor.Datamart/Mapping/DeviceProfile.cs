namespace LogicMonitor.Datamart.Mapping;

public class DeviceProfile : Profile
{
	public DeviceProfile()
	{
		CreateMap<Device, DeviceStoreItem>()
			.ForMember(
				dest => dest.DeviceDataSources,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DeviceDataSourceInstances,
				opts => opts.Ignore())

			.ForMember(
				dest => dest.DatamartId,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DatamartCreatedUtc,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DatamartLastModifiedUtc,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DatamartLastObservedUtc,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.LastAlertClosedTimeSeconds,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Property1,
				opts => opts.MapFrom(src => CustomPropertyHandler.Get(src, 1)))
			.ForMember(
				dest => dest.Property2,
				opts => opts.MapFrom(src => CustomPropertyHandler.Get(src, 2)))
			.ForMember(
				dest => dest.Property3,
				opts => opts.MapFrom(src => CustomPropertyHandler.Get(src, 3)))
			.ForMember(
				dest => dest.Property4,
				opts => opts.MapFrom(src => CustomPropertyHandler.Get(src, 4)))
			.ForMember(
				dest => dest.Property5,
				opts => opts.MapFrom(src => CustomPropertyHandler.Get(src, 5)))
			;

		CreateMap<DeviceStoreItem, Device>()
			.ForMember(
				dest => dest.AlertingDisabledOn,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AutoProperties,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.CustomProperties,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.InheritedProperties,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Instances,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.ManualDiscoveryFlags,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.SystemProperties,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.LoadBalanceCollectorGroupId,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AutoBalancedCollectorGroupId,
				opts => opts.Ignore())
			;
	}
}
