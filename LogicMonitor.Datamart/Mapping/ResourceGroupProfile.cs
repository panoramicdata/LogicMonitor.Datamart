namespace LogicMonitor.Datamart.Mapping;

public class ResourceGroupProfile : Profile
{
	public ResourceGroupProfile()
	{
		CreateMap<ResourceGroup, ResourceGroupStoreItem>()
			.ForMember(
				dest => dest.Id,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.LogicMonitorId,
				opts => opts.MapFrom(src => src.Id))
			.ForMember(
				dest => dest.AwsDeviceCount,
				opts => opts.MapFrom(src => src.AwsResourceCount))
			.ForMember(
				dest => dest.AzureDeviceCount,
				opts => opts.MapFrom(src => src.AzureResourceCount))
			.ForMember(
				dest => dest.GcpDeviceCount,
				opts => opts.MapFrom(src => src.GcpResourceCount))
			.ForMember(
				dest => dest.DeviceCount,
				opts => opts.MapFrom(src => src.ResourceCount))
			.ForMember(
				dest => dest.DeviceGroupType,
				opts => opts.MapFrom(src => src.ResourceGroupType))
			.ForMember(
				dest => dest.DirectDeviceCount,
				opts => opts.MapFrom(src => src.DirectResourceCount))
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
				dest => dest.AlertEnable,
				opts => opts.Ignore())
			;
		CreateMap<ResourceGroupStoreItem, ResourceGroup>()
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
				dest => dest.Resources,
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
				dest => dest.KubernetesResourceCount,
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
			.ForMember(
				dest => dest.ServicesTemplatesId,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.IsAlertingEnabled,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.ResourceGroupType,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DirectResourceCount,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.PropertyChangeWarningMessage,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AwsResourceCount,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AzureResourceCount,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.GcpResourceCount,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.ResourceCount,
				opts => opts.Ignore())
			.IgnoreAllPropertiesWithAnInaccessibleSetter()
			;
	}
}
