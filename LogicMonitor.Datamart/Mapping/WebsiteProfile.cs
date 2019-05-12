using AutoMapper;
using LogicMonitor.Api.Websites;
using LogicMonitor.Datamart.Models;

namespace LogicMonitor.Datamart.Mapping
{
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
					dest => dest.WebsiteGroup,
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
				;
		}
	}
}
