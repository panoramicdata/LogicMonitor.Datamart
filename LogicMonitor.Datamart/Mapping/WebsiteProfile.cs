namespace LogicMonitor.Datamart.Mapping;

public class WebsiteProfile : Profile
{
	public WebsiteProfile()
	{
		CreateMap<Website, WebsiteStoreItem>()
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
				dest => dest.DatamartLastObservedUtc,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.WebsiteGroup,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.HostName,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.IsAlertingDisabled,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.WebsiteMethod,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.PageLoadAlertTimeInMs,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.PercentPacketsNotReceiveInTime,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.PacketsNotReceivedTimeoutMs,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Schema,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Script,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.WebsiteGroupId,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.StopMonitoringByWebsiteGroup,
				opts => opts.Ignore())
			;

		CreateMap<WebsiteStoreItem, Website>()
			.ForMember(
				dest => dest.Collectors,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Checkpoints,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Steps,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Template,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.TestLocation,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.CustomProperties,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.UpdatedOnSeconds,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.RolePrivileges,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.GroupId,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DisableAlerting,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.StopMonitoringByFolder,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.PacketsLossThresholdPercent,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.PacketsLossTimeoutMs,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Host,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.PageLoadAlertTimeMs,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.HttpSchema,
				opts => opts.Ignore())
			;
	}
}
