namespace LogicMonitor.Datamart.Mapping;

/// <summary>
/// AutoMapper profile for mapping LogicMonitor WebsiteGroup API objects to <see cref="WebsiteGroupStoreItem"/>.
/// </summary>
public class WebsiteGroupProfile : Profile
{
	/// <summary>
	/// Initializes a new instance of the <see cref="WebsiteGroupProfile"/> class.
	/// </summary>
	public WebsiteGroupProfile()
	{
		CreateMap<WebsiteGroup, WebsiteGroupStoreItem>()
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
			.ForMember(
				dest => dest.Websites,
				opts => opts.Ignore())
			;
		CreateMap<WebsiteGroupStoreItem, WebsiteGroup>()
			.ForMember(
				dest => dest.Id,
				opts => opts.MapFrom(src => src.LogicMonitorId))
			.ForMember(
				dest => dest.CustomProperties,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.ChildWebsiteGroups,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.TestLocation,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.RolePrivileges,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AlertingDisabledOn,
				opts => opts.Ignore())
			;
	}
}
