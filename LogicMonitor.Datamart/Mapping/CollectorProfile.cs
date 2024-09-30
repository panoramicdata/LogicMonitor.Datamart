namespace LogicMonitor.Datamart.Mapping;

public class CollectorProfile : Profile
{
	public CollectorProfile()
	{
		CreateMap<Collector, CollectorStoreItem>()
			.ForMember(
				dest => dest.Id,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.LogicMonitorId,
				opts => opts.MapFrom(src => src.Id))
			.ForMember(
				dest => dest.CollectorGroupId,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Devices,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.LogicMonitorDeviceId,
				opts => opts.MapFrom(src => src.DeviceId))
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
				dest => dest.Name,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.CollectorGroup,
				opts => opts.Ignore())
			;

		CreateMap<CollectorStoreItem, Collector>()
			.ForMember(
				dest => dest.Id,
				opts => opts.MapFrom(src => src.LogicMonitorId))
			.ForMember(
				dest => dest.DeviceId,
				opts => opts.MapFrom(src => src.LogicMonitorDeviceId))
			.ForMember(
				dest => dest.GroupId,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.GroupName,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AutomaticUpgradeInfo,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.CustomProperties,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.EscalationChain,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.NextUpgradeInfo,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.OneTimeUpgradeInfo,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.PredefinedConfiguration,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.InstanceCount,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.BearerToken,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Type,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.ConfigurationFields,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.CopyUrl,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DownloadUrl,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.EncodedConfigData,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Format,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.IsEncoded,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.IsLmLogsEnabled,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.OtelId,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.OtelVersion,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.IsSyntheticsEnables,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.SpecifiedCollectorResourceGroupId,
				opts => opts.Ignore())
			;
	}
}
