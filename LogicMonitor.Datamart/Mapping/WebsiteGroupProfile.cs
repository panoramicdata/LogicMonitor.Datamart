namespace LogicMonitor.Datamart.Mapping;

public class WebsiteGroupProfile : Profile
{
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
			;
	}
}
