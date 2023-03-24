namespace LogicMonitor.Datamart.Mapping;

public class DeviceGroupProfile : Profile
{
	public DeviceGroupProfile()
	{
		CreateMap<DeviceGroup, DeviceGroupStoreItem>()
			.ForMember(
				dest => dest.Id,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.LogicMonitorId,
				opts => opts.MapFrom(src => src.Id))
			.ForMember(
				dest => dest.DatamartCreatedUtc,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DatamartLastModifiedUtc,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DatamartLastObservedUtc,
				opts => opts.Ignore())
			;
		CreateMap<DeviceGroupStoreItem, DeviceGroup>()
			.ForMember(
				dest => dest.Id,
				opts => opts.MapFrom(src => src.LogicMonitorId))
			.ForMember(
				dest => dest.AwsTestResult,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.CustomProperties,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DefaultCollectorGroupId,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DefaultCollectorGroupDescription,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DefaultLoadBalanceCollectorGroupId,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Devices,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Extra,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AlertingDisabledOn,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.SubGroups,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DefaultAutoBalancedCollectorGroupId,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.KubernetesDeviceCount,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.RolePrivileges,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.SaasTestResult,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.SaasTestResultCode,
				opts => opts.Ignore())
			;
	}
}
