namespace LogicMonitor.Datamart.Mapping;

public class EventSourceProfile : Profile
{
	public EventSourceProfile()
	{
		CreateMap<EventSource, EventSourceStoreItem>()
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
				dest => dest.DatamartLastObserved,
				opts => opts.Ignore())
			;
		CreateMap<EventSourceStoreItem, EventSource>()
			.ForMember(
				dest => dest.Id,
				opts => opts.MapFrom(src => src.LogicMonitorId))
			.ForMember(
				dest => dest.AlertBodyTemplate,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AlertEffectiveIntervalMinutes,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AlertSubjectTemplate,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AlertLevel,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AppliesTo,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AuditVersion,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Checksum,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.ClearAfterAcknowledgement,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Type,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Filters,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.GroovyScript,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Group,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.LinuxCommandLine,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.LineageId,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.LogFiles,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.LinuxScript,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Number,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.ScheduleMinutes,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.ScriptType,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.SuppressDuplicates,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Tags,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Technology,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Version,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.WindowsCommandLine,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.WindowsScript,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Dimension,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.InstallationMetadata,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AccessGroupIds,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AccessGroups,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.ColumnInstanceName,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Query,
				opts => opts.Ignore())
				;
	}
}
