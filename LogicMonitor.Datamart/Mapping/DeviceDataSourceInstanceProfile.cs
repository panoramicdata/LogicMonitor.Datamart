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
					dest => dest.LastMeasurementUpdatedTimeSeconds,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.DataMeasures,
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
		}
	}
}
