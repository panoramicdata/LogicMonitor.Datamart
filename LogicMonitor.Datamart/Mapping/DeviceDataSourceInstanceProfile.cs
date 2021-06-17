using AutoMapper;
using LogicMonitor.Api.LogicModules;
using LogicMonitor.Datamart.Models;

namespace LogicMonitor.Datamart.Mapping
{
	public class DeviceDataSourceInstanceProfile : Profile
	{
		public DeviceDataSourceInstanceProfile()
		{
			CreateMap<DeviceDataSourceInstance, DeviceDataSourceInstanceStoreItem>()
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
					dest => dest.DeviceDataSource,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.Device,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.LastAggregationHourWrittenUtc,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.LastWentMissingUtc,
					opts => opts.Ignore())
				;

			CreateMap<DeviceDataSourceInstanceStoreItem, DeviceDataSourceInstance>()
				.ForMember(
					dest => dest.AlertingDisabledOn,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.CustomProperties,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.SystemProperties,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.AutoProperties,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.DataSourceName,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.DataSourceType,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.GroupsDisabledThisSource,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.Name,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.Description,
					opts => opts.Ignore())
			;
		}
	}
}
