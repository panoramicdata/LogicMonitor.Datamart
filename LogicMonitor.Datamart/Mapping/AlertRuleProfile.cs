using AutoMapper;
using LogicMonitor.Api.Settings;
using LogicMonitor.Datamart.Models;

namespace LogicMonitor.Datamart.Mapping
{
	public class AlertRuleProfile : Profile
	{
		public AlertRuleProfile()
		{
			CreateMap<AlertRule, AlertRuleStoreItem>()
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
					dest => dest.Alerts,
					opts => opts.Ignore())
				;
			CreateMap<AlertRuleStoreItem, AlertRule>()
				.ForMember(
					dest => dest.Devices,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.DeviceGroups,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.EscalationChain,
					opts => opts.Ignore())
				;
		}
	}
}
