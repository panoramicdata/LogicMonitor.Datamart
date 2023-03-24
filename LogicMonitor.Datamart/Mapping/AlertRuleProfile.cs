namespace LogicMonitor.Datamart.Mapping;

public class AlertRuleProfile : Profile
{
	public AlertRuleProfile()
	{
		CreateMap<AlertRule, AlertRuleStoreItem>()
			.ForMember(
				dest => dest.Id,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.EscalationChainId,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.EscalationChain,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.LogicMonitorId,
				opts => opts.MapFrom(src => src.Id))
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
				dest => dest.AlertStoreItems,
				opts => opts.Ignore())
			;
		CreateMap<AlertRuleStoreItem, AlertRule>()
			.ForMember(
				dest => dest.Id,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.EscalationChain,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.EscalationChainId,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Devices,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DeviceGroups,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.EscalationChain,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.ResourceProperties,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.SendAnomalySuppressedAlert,
				opts => opts.Ignore())
			;
	}
}
