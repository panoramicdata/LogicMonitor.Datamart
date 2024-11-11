using System.Globalization;

namespace LogicMonitor.Datamart.Mapping;

public class AuditEventProfile : Profile
{
	public AuditEventProfile()
	{
		CreateMap<AuditEvent, AuditEventStoreItem>()
			.ForMember(
				dest => dest.Id,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.LogicMonitorId,
				opts => opts.MapFrom(src => src.Id))
			.ForMember(
				dest => dest.DatamartCreated,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DatamartLastModified,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DeviceDataSourceId,
				opts => opts.MapFrom(src => src.ResourceDataSourceId))
			.ForMember(
				dest => dest.OriginatorType,
				opts => opts.MapFrom(src => src.OriginatorType.ToString()))
			.ForMember(
				dest => dest.EntityType,
				opts => opts.MapFrom(src => src.EntityType.ToString()))
			.ForMember(
				dest => dest.ActionType,
				opts => opts.MapFrom(src => src.ActionType.ToString()))
			.ForMember(
				dest => dest.OutcomeType,
				opts => opts.MapFrom(src => src.OutcomeType.ToString()))
			.ForMember(
				dest => dest.ResourceIds,
				opts => opts.MapFrom(src => src.ResourceIds == null ? null : string.Join(',', src.ResourceIds.Select(x => x.ToString(CultureInfo.InvariantCulture)))))
			.ForMember(
				dest => dest.ResourceNames,
				opts => opts.MapFrom(src => src.ResourceNames == null ? null : string.Join(',', src.ResourceNames)))
			.ForMember(
				dest => dest.DataSourceNewInstanceIds,
				opts => opts.MapFrom(src => src.DataSourceNewInstanceIds == null ? null : string.Join(',', src.DataSourceNewInstanceIds.Select(x => x.ToString(CultureInfo.InvariantCulture)))))
			.ForMember(
				dest => dest.DataSourceNewInstanceNames,
				opts => opts.MapFrom(src => src.DataSourceNewInstanceNames == null ? null : string.Join(',', src.DataSourceNewInstanceNames.Select(x => x.ToString(CultureInfo.InvariantCulture)))))
			.ForMember(
				dest => dest.DataSourceDeletedInstanceIds,
				opts => opts.MapFrom(src => src.DataSourceDeletedInstanceIds == null ? null : string.Join(',', src.DataSourceDeletedInstanceIds.Select(x => x.ToString(CultureInfo.InvariantCulture)))))
			.ForMember(
				dest => dest.DataSourceDeletedInstanceNames,
				opts => opts.MapFrom(src => src.DataSourceDeletedInstanceNames == null ? null : string.Join(',', src.DataSourceDeletedInstanceNames.Select(x => x.ToString(CultureInfo.InvariantCulture)))))
			.AfterMap<TruncateMappingAction<AuditEvent, AuditEventStoreItem>>();
	}
}
