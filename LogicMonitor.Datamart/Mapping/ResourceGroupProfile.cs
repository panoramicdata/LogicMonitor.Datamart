namespace LogicMonitor.Datamart.Mapping;

/// <summary>
/// AutoMapper profile for mapping LogicMonitor DeviceGroup API objects to <see cref="ResourceGroupStoreItem"/>.
/// </summary>
public class ResourceGroupProfile : Profile
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ResourceGroupProfile"/> class.
	/// </summary>
	public ResourceGroupProfile()
	{
		CreateResourceGroupToResourceGroupStoreItemMap();
		CreateResourceGroupStoreItemToResourceGroupMap();
	}

	private void CreateResourceGroupToResourceGroupStoreItemMap()
	{
		CreateMap<ResourceGroup, ResourceGroupStoreItem>()
			.IgnoreMembers(
				dest => dest.Id, dest => dest.DatamartCreated, dest => dest.DatamartLastModified, dest => dest.DatamartLastObserved,
				dest => dest.AlertEnable)
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
				opts => opts.MapFrom(src => (int)src.ResourceCount))
			.ForMember(
				dest => dest.DeviceGroupType,
				opts => opts.MapFrom(src => src.ResourceGroupType))
			.ForMember(
				dest => dest.DirectDeviceCount,
				opts => opts.MapFrom(src => (int)src.DirectResourceCount))
			.ForMember(
				dest => dest.AzureTestResultCode,
				opts => opts.MapFrom(src => (int)src.AzureTestResultCode))
			.ForMember(
				dest => dest.DirectSubGroupCount,
				opts => opts.MapFrom(src => (int)src.DirectSubGroupCount));
	}

	private void CreateResourceGroupStoreItemToResourceGroupMap()
	{
		CreateMap<ResourceGroupStoreItem, ResourceGroup>()
			.IgnoreMembers(
				dest => dest.AwsTestResult, dest => dest.CustomProperties, dest => dest.DefaultCollectorGroupId, dest => dest.DefaultCollectorGroupDescription,
				dest => dest.DefaultLoadBalanceCollectorGroupId, dest => dest.Resources, dest => dest.Extra, dest => dest.AlertingDisabledOn,
				dest => dest.SubGroups, dest => dest.DefaultAutoBalancedCollectorGroupId, dest => dest.KubernetesResourceCount, dest => dest.RolePrivileges,
				dest => dest.SaasTestResult, dest => dest.SaasTestResultCode, dest => dest.ServicesTemplatesId, dest => dest.IsAlertingEnabled,
				dest => dest.ResourceGroupType, dest => dest.DirectResourceCount, dest => dest.PropertyChangeWarningMessage, dest => dest.AwsResourceCount,
				dest => dest.AzureResourceCount, dest => dest.GcpResourceCount, dest => dest.ResourceCount, dest => dest.OciResourceCount,
				dest => dest.OciRegionsInfo, dest => dest.OciTestResultCode, dest => dest.OciTestResult, dest => dest.TenantIdentifier)
			.ForMember(
				dest => dest.Id,
				opts => opts.MapFrom(src => src.LogicMonitorId))
			.IgnoreAllPropertiesWithAnInaccessibleSetter();
	}
}
