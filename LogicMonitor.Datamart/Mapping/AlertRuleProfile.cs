namespace LogicMonitor.Datamart.Mapping;

/// <summary>
/// AutoMapper profile for mapping LogicMonitor AlertRule API objects to <see cref="AlertRuleStoreItem"/>.
/// </summary>
public class AlertRuleProfile : Profile
{
	/// <summary>
	/// Initializes a new instance of the <see cref="AlertRuleProfile"/> class.
	/// </summary>
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
				dest => dest.DatamartCreated,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DatamartLastModified,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DatamartLastObserved,
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
				dest => dest.Resources,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.ResourceGroups,
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
			.IgnoreAllPropertiesWithAnInaccessibleSetter()
			;
	}
}
