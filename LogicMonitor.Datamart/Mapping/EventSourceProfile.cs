namespace LogicMonitor.Datamart.Mapping;

/// <summary>
/// AutoMapper profile for mapping LogicMonitor EventSource API objects to <see cref="EventSourceStoreItem"/>.
/// </summary>
public class EventSourceProfile : Profile
{
	/// <summary>
	/// Initializes a new instance of the <see cref="EventSourceProfile"/> class.
	/// </summary>
	public EventSourceProfile()
	{
		CreateEventSourceToEventSourceStoreItemMap();
		CreateEventSourceStoreItemToEventSourceMap();
	}

	private void CreateEventSourceToEventSourceStoreItemMap()
	{
		CreateMap<EventSource, EventSourceStoreItem>()
			.IgnoreMembers(dest => dest.Id, dest => dest.DatamartCreated, dest => dest.DatamartLastModified, dest => dest.DatamartLastObserved)
			.ForMember(
				dest => dest.LogicMonitorId,
				opts => opts.MapFrom(src => src.Id));
	}

	private void CreateEventSourceStoreItemToEventSourceMap()
	{
		CreateMap<EventSourceStoreItem, EventSource>()
			.IgnoreMembers(
				dest => dest.AlertBodyTemplate, dest => dest.AlertEffectiveIntervalMinutes, dest => dest.AlertSubjectTemplate, dest => dest.AlertLevel,
				dest => dest.AppliesTo, dest => dest.AuditVersion, dest => dest.Checksum, dest => dest.ClearAfterAcknowledgement,
				dest => dest.Type, dest => dest.Filters, dest => dest.GroovyScript, dest => dest.Group,
				dest => dest.LinuxCommandLine, dest => dest.LineageId, dest => dest.LogFiles, dest => dest.LinuxScript,
				dest => dest.Number, dest => dest.ScheduleMinutes, dest => dest.ScriptType, dest => dest.SuppressDuplicates,
				dest => dest.Tags, dest => dest.Technology, dest => dest.Version, dest => dest.WindowsCommandLine,
				dest => dest.WindowsScript, dest => dest.Dimension, dest => dest.InstallationMetadata, dest => dest.AccessGroupIds,
				dest => dest.AccessGroups, dest => dest.ColumnInstanceName, dest => dest.Query, dest => dest.IsInUse)
			.ForMember(
				dest => dest.Id,
				opts => opts.MapFrom(src => src.LogicMonitorId));
	}
}
