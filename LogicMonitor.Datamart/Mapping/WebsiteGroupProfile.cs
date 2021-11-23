namespace LogicMonitor.Datamart.Mapping;

public class WebsiteGroupProfile : Profile
{
	public WebsiteGroupProfile()
	{
		CreateMap<WebsiteGroup, WebsiteGroupStoreItem>()
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
				dest => dest.Websites,
				opts => opts.Ignore())
			;
		CreateMap<WebsiteGroupStoreItem, WebsiteGroup>()
			.ForMember(
				dest => dest.CustomProperties,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.ChildWebsiteGroups,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.TestLocation,
				opts => opts.Ignore())
			;
	}
}
