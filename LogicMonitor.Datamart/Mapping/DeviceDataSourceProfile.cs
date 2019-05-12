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
					dest => dest.DatamartId,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.DatamartCreatedUtc,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.DatamartLastModifiedUtc,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.Name,
					opts => opts.MapFrom(d => $"{d.DeviceDisplayName}/{d.DataSourceName}"))
				.ForMember(
					dest => dest.Description,
					opts => opts.MapFrom(d => $"DataSource {d.DataSourceName} on device {d.DeviceDisplayName}"))
				.ForMember(
					dest => dest.DeviceDataSourceInstances,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.DataSource,
					opts => opts.Ignore())
				;
		}
	}
}
