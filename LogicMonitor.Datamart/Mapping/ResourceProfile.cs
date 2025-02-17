﻿namespace LogicMonitor.Datamart.Mapping;

public class ResourceProfile : Profile
{
	public ResourceProfile()
	{
		CreateMap<Resource, ResourceStoreItem>()
			.ForMember(
				dest => dest.Id,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.LogicMonitorId,
				opts => opts.MapFrom(src => src.Id))
			.ForMember(
				dest => dest.PreferredCollector,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.PreferredCollectorId,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DeviceDataSources,
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
				dest => dest.LastAlertClosedTimeSeconds,
				opts => opts.Ignore())
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
				opts => opts.MapFrom(src => src.RelatedResourceId))
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
			.ForMember(
				dest => dest.Property6,
				opts => opts.MapFrom(src => CustomPropertyHandler.Get(src, 6)))
			.ForMember(
				dest => dest.Property7,
				opts => opts.MapFrom(src => CustomPropertyHandler.Get(src, 7)))
			.ForMember(
				dest => dest.Property8,
				opts => opts.MapFrom(src => CustomPropertyHandler.Get(src, 8)))
			.ForMember(
				dest => dest.Property9,
				opts => opts.MapFrom(src => CustomPropertyHandler.Get(src, 9)))
			.ForMember(
				dest => dest.Property10,
				opts => opts.MapFrom(src => CustomPropertyHandler.Get(src, 10)))
			.ForMember(
				dest => dest.Property11,
				opts => opts.MapFrom(src => CustomPropertyHandler.Get(src, 11)))
			.ForMember(
				dest => dest.Property12,
				opts => opts.MapFrom(src => CustomPropertyHandler.Get(src, 12)))
			.ForMember(
				dest => dest.Property13,
				opts => opts.MapFrom(src => CustomPropertyHandler.Get(src, 13)))
			.ForMember(
				dest => dest.Property14,
				opts => opts.MapFrom(src => CustomPropertyHandler.Get(src, 14)))
			.ForMember(
				dest => dest.Property15,
				opts => opts.MapFrom(src => CustomPropertyHandler.Get(src, 15)))
			.ForMember(
				dest => dest.Property16,
				opts => opts.MapFrom(src => CustomPropertyHandler.Get(src, 16)))
			.ForMember(
				dest => dest.Property17,
				opts => opts.MapFrom(src => CustomPropertyHandler.Get(src, 17)))
			.ForMember(
				dest => dest.Property18,
				opts => opts.MapFrom(src => CustomPropertyHandler.Get(src, 18)))
			.ForMember(
				dest => dest.Property19,
				opts => opts.MapFrom(src => CustomPropertyHandler.Get(src, 19)))
			.ForMember(
				dest => dest.Property20,
				opts => opts.MapFrom(src => CustomPropertyHandler.Get(src, 20)))
			.ForMember(
				dest => dest.LastTimeSeriesDataSyncDurationMs,
				opts => opts.Ignore())
			;

		CreateMap<ResourceStoreItem, Resource>()
			.ForMember(
				dest => dest.Id,
				opts => opts.MapFrom(src => src.LogicMonitorId))
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
				dest => dest.PreferredCollectorGroupId,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.PreferredCollectorGroupName,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.PreferredCollectorId,
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
			.ForMember(
				dest => dest.CurrentLogCollectorId,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.IsPreferredLogCollectorConfigured,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.LogCollectorDescription,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.LogCollectorGroupId,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.LogCollectorGroupName,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.LogCollectorId,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AwsTestResult,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AwsTestResultCode,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AzureTestResult,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AzureTestResultCode,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.GcpTestResult,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.GcpTestResultCode,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.SaasTestResult,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.SaasTestResultCode,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AutoVisualResult,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.ResourceIds,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.SyntheticsCollectorIds,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.RolePrivileges,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Op,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.ContainsMultiValue,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.ResourceGroupIdsString,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.RelatedResourceId,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.ResourceStatus,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.ResourceType,
				opts => opts.Ignore())
			;
	}
}
