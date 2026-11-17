namespace LogicMonitor.Datamart.Mapping;

/// <summary>
/// AutoMapper profile for mapping LogicMonitor DataSourceGraph API objects to <see cref="DataSourceGraphStoreItem"/>.
/// </summary>
public class DataSourceGraphProfile : Profile
{
	/// <summary>
	/// Initializes a new instance of the <see cref="DataSourceGraphProfile"/> class.
	/// </summary>
	public DataSourceGraphProfile()
	{
		CreateDataSourceGraphToDataSourceGraphStoreItemMap();
		CreateDataSourceGraphStoreItemToDataSourceGraphMap();
	}

	private void CreateDataSourceGraphToDataSourceGraphStoreItemMap()
	{
		CreateMap<DataSourceGraph, DataSourceGraphStoreItem>()
			.IgnoreMembers(
				dest => dest.Id, dest => dest.DataSource, dest => dest.IsOverview, dest => dest.DatamartCreated,
				dest => dest.DatamartLastModified, dest => dest.DatamartLastObserved, dest => dest.DataPoints)
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
				dest => dest.MaxValue,
				opts => opts.MapFrom(src => GetDouble(src.MaxValue)))
			.ForMember(
				dest => dest.MinValue,
				opts => opts.MapFrom(src => GetDouble(src.MinValue)))
			.ForMember(
				dest => dest.Timescale,
				opts => opts.MapFrom(src => src.TimeScale));
	}

	private void CreateDataSourceGraphStoreItemToDataSourceGraphMap()
	{
		CreateMap<DataSourceGraphStoreItem, DataSourceGraph>()
			.IgnoreMembers(
				dest => dest.Lines, dest => dest.Aggregated, dest => dest.DataPoints, dest => dest.DataSourceId,
				dest => dest.VirtualDataPoints)
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
				dest => dest.TimeScale,
				opts => opts.MapFrom(src => src.Timescale));
	}

	private static double? GetDouble(string text)
		=> double.TryParse(text, out var maxValue)
			? (maxValue is double.NaN ? null : maxValue)
			: null;
}
