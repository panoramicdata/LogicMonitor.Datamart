namespace LogicMonitor.Datamart.Mapping;

/// <summary>
/// AutoMapper profile for mapping LogicMonitor Website API objects to <see cref="WebsiteStoreItem"/>.
/// </summary>
public class WebsiteProfile : Profile
{
	/// <summary>
	/// Initializes a new instance of the <see cref="WebsiteProfile"/> class.
	/// </summary>
	public WebsiteProfile()
	{
		CreateWebsiteToWebsiteStoreItemMap();
		CreateWebsiteStoreItemToWebsiteMap();
	}

	private void CreateWebsiteToWebsiteStoreItemMap()
	{
		CreateMap<Website, WebsiteStoreItem>()
			.IgnoreMembers(
				dest => dest.Id, dest => dest.DatamartCreated, dest => dest.DatamartLastModified, dest => dest.DatamartLastObserved,
				dest => dest.WebsiteGroup, dest => dest.HostName, dest => dest.IsAlertingDisabled, dest => dest.WebsiteMethod,
				dest => dest.PageLoadAlertTimeInMs, dest => dest.PercentPacketsNotReceiveInTime, dest => dest.PacketsNotReceivedTimeoutMs, dest => dest.Schema,
				dest => dest.Script, dest => dest.WebsiteGroupId, dest => dest.StopMonitoringByWebsiteGroup)
			.ForMember(
				dest => dest.LogicMonitorId,
				opts => opts.MapFrom(src => src.Id));
	}

	private void CreateWebsiteStoreItemToWebsiteMap()
	{
		CreateMap<WebsiteStoreItem, Website>()
			.IgnoreMembers(
				dest => dest.Collectors, dest => dest.Checkpoints, dest => dest.Steps, dest => dest.Template,
				dest => dest.TestLocation, dest => dest.CustomProperties, dest => dest.UpdatedOnSeconds, dest => dest.RolePrivileges,
				dest => dest.GroupId, dest => dest.DisableAlerting, dest => dest.StopMonitoringByFolder, dest => dest.PacketsLossThresholdPercent,
				dest => dest.PacketsLossTimeoutMs, dest => dest.Host, dest => dest.PageLoadAlertTimeMs, dest => dest.HttpSchema,
				dest => dest.AlertingDisabledOn)
			.ForMember(
				dest => dest.Id,
				opts => opts.MapFrom(src => src.LogicMonitorId));
	}
}
