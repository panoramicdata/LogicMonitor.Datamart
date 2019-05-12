using AutoMapper;
using LogicMonitor.Api.Devices;
using LogicMonitor.Datamart.Models;

namespace LogicMonitor.Datamart.Mapping
{
	public class DeviceProfile : Profile
	{
		public DeviceProfile()
		{
			CreateMap<Device, DeviceStoreItem>()
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
				.ForMember(
					dest => dest.LastAlertClosedTimeSeconds,
					opts => opts.Ignore())
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
}
