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
		CreateWebsiteGroupToWebsiteGroupStoreItemMap();
		CreateWebsiteGroupStoreItemToWebsiteGroupMap();
	}

	private void CreateWebsiteGroupToWebsiteGroupStoreItemMap()
	{
		CreateMap<WebsiteGroup, WebsiteGroupStoreItem>()
			.IgnoreMembers(
				dest => dest.Id, dest => dest.DatamartCreated, dest => dest.DatamartLastModified, dest => dest.DatamartLastObserved,
				dest => dest.Websites)
			.ForMember(
				dest => dest.LogicMonitorId,
				opts => opts.MapFrom(src => src.Id));
	}

	private void CreateWebsiteGroupStoreItemToWebsiteGroupMap()
	{
		CreateMap<WebsiteGroupStoreItem, WebsiteGroup>()
			.IgnoreMembers(
				dest => dest.CustomProperties, dest => dest.ChildWebsiteGroups, dest => dest.TestLocation, dest => dest.RolePrivileges,
				dest => dest.AlertingDisabledOn)
			.ForMember(
				dest => dest.Id,
				opts => opts.MapFrom(src => src.LogicMonitorId));
	}
}
