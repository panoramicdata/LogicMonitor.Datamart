using AutoMapper;
using LogicMonitor.Api.Devices;
using LogicMonitor.Datamart.Models;

namespace LogicMonitor.Datamart.Mapping
{
	public class DeviceGroupProfile : Profile
	{
		public DeviceGroupProfile()
		{
			CreateMap<DeviceGroup, DeviceGroupStoreItem>()
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
				;
			CreateMap<DeviceGroupStoreItem, DeviceGroup>()
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
				;
		}
	}
}
