using AutoMapper;
using LogicMonitor.Api.LogicModules;
using LogicMonitor.Datamart.Models;

namespace LogicMonitor.Datamart.Mapping
{
	public class DeviceDataSourceProfile : Profile
	{
		public DeviceDataSourceProfile()
		{
			CreateMap<DeviceDataSource, DeviceDataSourceStoreItem>()
				.ForMember(
					dest => dest.Device,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.DataSource,
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
					dest => dest.DeviceDataSourceInstances,
					opts => opts.Ignore())
				;

			CreateMap<DeviceDataSourceStoreItem, DeviceDataSource>()
				.ForMember(
					dest => dest.AlertDisableStatus,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.AlertStatus,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.AlertStatusPriority,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.AlertingDisabledOnSeconds,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.CollectionMethod,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.DataSourceType,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.DataSourceDisplayName,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.DataSourceGraphs,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.GroupName,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.DisabledInstanceGroups,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.InstanceAutoGroupEnabled,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.InstanceCount,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.MonitoringInstanceCount,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.NextAutoDiscoveryOnSeconds,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.OverviewGraphs,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.IsAutoDiscoveryEnabled,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.IsMonitoringDisabled,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.IsMultiple,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.SdtStatus,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.Status,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.SdtAt,
					opts => opts.Ignore())
				;
		}
	}
}
