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
		CreateAlertRuleToAlertRuleStoreItemMap();
		CreateAlertRuleStoreItemToAlertRuleMap();
	}

	private void CreateAlertRuleToAlertRuleStoreItemMap()
	{
		CreateMap<AlertRule, AlertRuleStoreItem>()
			.IgnoreMembers(
				dest => dest.Id, dest => dest.EscalationChainId, dest => dest.EscalationChain, dest => dest.DatamartCreated,
				dest => dest.DatamartLastModified, dest => dest.DatamartLastObserved, dest => dest.AlertStoreItems)
			.ForMember(
				dest => dest.LogicMonitorId,
				opts => opts.MapFrom(src => src.Id));
	}

	private void CreateAlertRuleStoreItemToAlertRuleMap()
	{
		CreateMap<AlertRuleStoreItem, AlertRule>()
			.IgnoreMembers(
				dest => dest.Id, dest => dest.EscalationChain, dest => dest.EscalationChainId, dest => dest.Resources,
				dest => dest.ResourceGroups, dest => dest.EscalationChain, dest => dest.ResourceProperties, dest => dest.SendAnomalySuppressedAlert)
			.IgnoreAllPropertiesWithAnInaccessibleSetter();
	}
}
