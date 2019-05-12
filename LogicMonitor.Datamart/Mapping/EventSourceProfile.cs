using AutoMapper;
using LogicMonitor.Api.LogicModules;
using LogicMonitor.Datamart.Models;

namespace LogicMonitor.Datamart.Mapping
{
	public class EventSourceProfile : Profile
	{
		public EventSourceProfile()
		{
			CreateMap<EventSource, EventSourceStoreItem>()
				.ForMember(
					dest => dest.DatamartId,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.DatamartCreatedUtc,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.DatamartLastModifiedUtc,
					opts => opts.Ignore())
				;
			CreateMap<EventSourceStoreItem, EventSource>()
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
					dest => dest.CheckIntervalSeconds,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.ClearAfterAcknowledgement,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.EventSourceType,
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
					dest => dest.LogFiles,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.LinuxScript,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.Published,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.Schedule,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.ScriptType,
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
				;
		}
	}
}
