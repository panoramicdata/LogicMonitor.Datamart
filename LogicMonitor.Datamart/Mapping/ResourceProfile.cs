namespace LogicMonitor.Datamart.Mapping;

/// <summary>
/// AutoMapper profile for mapping LogicMonitor Device API objects to <see cref="ResourceStoreItem"/>.
/// </summary>
public class ResourceProfile : Profile
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ResourceProfile"/> class.
	/// </summary>
	public ResourceProfile()
	{
		CreateResourceToResourceStoreItemMap();
		CreateResourceStoreItemToResourceMap();
	}

	private void CreateResourceToResourceStoreItemMap()
	{
		var map = CreateMap<Resource, ResourceStoreItem>()
			.IgnoreMembers(
				dest => dest.Id, dest => dest.PreferredCollector, dest => dest.PreferredCollectorId, dest => dest.DeviceDataSources,
				dest => dest.DatamartCreated, dest => dest.DatamartLastModified, dest => dest.DatamartLastObserved, dest => dest.LastAlertClosedTimeSeconds,
				dest => dest.LastTimeSeriesDataSyncDurationMs)
			.ForMember(
				dest => dest.LogicMonitorId,
				opts => opts.MapFrom(src => src.Id))
			.ForMember(
				dest => dest.DeviceType,
				opts => opts.MapFrom(src => src.ResourceType))
			.ForMember(
				dest => dest.DeviceGroupIdsString,
				opts => opts.MapFrom(src => src.ResourceGroupIdsString))
			.ForMember(
				dest => dest.DeviceStatus,
				opts => opts.MapFrom(src => src.ResourceStatus))
			.ForMember(
				dest => dest.RelatedDeviceId,
				opts => opts.MapFrom(src => src.RelatedResourceId));
		MapCustomProperties(map);
	}

	private static void MapCustomProperties(IMappingExpression<Resource, ResourceStoreItem> map)
	{
		for (var i = 1; i <= 20; i++)
		{
			var index = i;
			_ = map.ForMember($"Property{index}", opts => opts.MapFrom(src => CustomPropertyHandler.Get(src, index)));
		}
	}

	private void CreateResourceStoreItemToResourceMap()
	{
		CreateMap<ResourceStoreItem, Resource>()
			.IgnoreMembers(
				dest => dest.AlertingDisabledOn, dest => dest.AutoProperties, dest => dest.CustomProperties, dest => dest.InheritedProperties,
				dest => dest.Instances, dest => dest.ManualDiscoveryFlags, dest => dest.PreferredCollectorGroupId, dest => dest.PreferredCollectorGroupName,
				dest => dest.PreferredCollectorId, dest => dest.SystemProperties, dest => dest.LoadBalanceCollectorGroupId, dest => dest.AutoBalancedCollectorGroupId,
				dest => dest.CurrentLogCollectorId, dest => dest.DedicatedLogCollectorGroupId, dest => dest.IsPreferredLogCollectorConfigured, dest => dest.IsPreferredLogCollectorGroupConfigured,
				dest => dest.LogCollectorDescription, dest => dest.LogCollectorGroupId, dest => dest.LogCollectorGroupName, dest => dest.LogCollectorId,
				dest => dest.AwsTestResult, dest => dest.AwsTestResultCode, dest => dest.AzureTestResult, dest => dest.AzureTestResultCode,
				dest => dest.GcpTestResult, dest => dest.GcpTestResultCode, dest => dest.SaasTestResult, dest => dest.SaasTestResultCode,
				dest => dest.AutoVisualResult, dest => dest.ResourceIds, dest => dest.SyntheticsCollectorIds, dest => dest.RolePrivileges,
				dest => dest.Op, dest => dest.ContainsMultiValue, dest => dest.ResourceGroupIdsString, dest => dest.RelatedResourceId,
				dest => dest.ResourceStatus, dest => dest.ResourceType, dest => dest.Type, dest => dest.ClonedFromHostId,
				dest => dest.StopMonitoring, dest => dest.StopMonitoringByFolder, dest => dest.IndividualSmAlertEnable, dest => dest.IndividualAlertLevel,
				dest => dest.OverallAlertLevel, dest => dest.PollingInterval, dest => dest.Transition, dest => dest.GlobalSmAlertCond,
				dest => dest.UseDefaultLocationSetting, dest => dest.UseDefaultAlertSetting, dest => dest.Checkpoints, dest => dest.IsInternal,
				dest => dest.Collectors, dest => dest.TestLocation, dest => dest.GroupIds, dest => dest.Properties,
				dest => dest.Status, dest => dest.Template, dest => dest.LastUpdated, dest => dest.Steps,
				dest => dest.Domain, dest => dest.Count, dest => dest.Host, dest => dest.PercentPktsNotReceiveInTime,
				dest => dest.TimeoutInMSPktsNotReceive)
			.ForMember(
				dest => dest.Id,
				opts => opts.MapFrom(src => src.LogicMonitorId));
	}
}
