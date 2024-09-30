using System.Globalization;

namespace LogicMonitor.Datamart.Mapping;

public class DataSourceGraphProfile : Profile
{
	public DataSourceGraphProfile()
	{
		CreateMap<DataSourceGraph, DataSourceGraphStoreItem>()
			.ForMember(
				dest => dest.Id,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DataSource,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.IsOverview,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DataSourceId,
				opts => opts.MapFrom(src => Guid.Empty))
			.ForMember(
				dest => dest.LogicMonitorId,
				opts => opts.MapFrom(src => src.Id))
			.ForMember(
				dest => dest.IsBase1024,
				opts => opts.MapFrom(src => src.Base1024))
			.ForMember(
				dest => dest.IsRigid,
				opts => opts.MapFrom(src => src.Rigid))
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
				dest => dest.DataPoints,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.MaxValue,
				opts => opts.MapFrom(src => GetDouble(src.MaxValue)))
			.ForMember(
				dest => dest.MinValue,
				opts => opts.MapFrom(src => GetDouble(src.MinValue)))
		;

		CreateMap<DataSourceGraphStoreItem, DataSourceGraph>()
			.ForMember(
				dest => dest.Id,
				opts => opts.MapFrom(src => src.LogicMonitorId))
			.ForMember(
				dest => dest.Rigid,
				opts => opts.MapFrom(src => src.IsRigid))
			.ForMember(
				dest => dest.Base1024,
				opts => opts.MapFrom(src => src.IsBase1024))
			.ForMember(
				dest => dest.MaxValue,
				opts => opts.MapFrom(src => src.MaxValue == null ? "NaN" : src.MaxValue.Value.ToString("N0", CultureInfo.InvariantCulture)))
			.ForMember(
				dest => dest.MinValue,
				opts => opts.MapFrom(src => src.MinValue == null ? "NaN" : src.MinValue.Value.ToString("N0", CultureInfo.InvariantCulture)))
			.ForMember(
				dest => dest.Lines,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Aggregated,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DataPoints,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DataSourceId,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.VirtualDataPoints,
				opts => opts.Ignore())
			;
	}

	private static double? GetDouble(string text)
		=> double.TryParse(text, out var maxValue)
			? (maxValue is double.NaN ? null : maxValue)
			: null;
}
