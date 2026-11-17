namespace LogicMonitor.Datamart.Mapping;

/// <summary>
/// AutoMapper profile for mapping LogicMonitor Collector API objects to <see cref="CollectorStoreItem"/>.
/// </summary>
public class CollectorProfile : Profile
{
	/// <summary>
	/// Initializes a new instance of the <see cref="CollectorProfile"/> class.
	/// </summary>
	public CollectorProfile()
	{
		CreateCollectorToCollectorStoreItemMap();
		CreateCollectorStoreItemToCollectorMap();
	}

	private void CreateCollectorToCollectorStoreItemMap()
	{
		CreateMap<Collector, CollectorStoreItem>()
			.IgnoreMembers(
				dest => dest.Id, dest => dest.CollectorGroupId, dest => dest.Devices, dest => dest.DatamartCreated,
				dest => dest.DatamartLastModified, dest => dest.DatamartLastObserved, dest => dest.Name, dest => dest.CollectorGroup)
			.ForMember(
				dest => dest.LogicMonitorId,
				opts => opts.MapFrom(src => src.Id))
			.ForMember(
				dest => dest.DeviceCount,
				opts => opts.MapFrom(src => src.ResourceCount))
			.ForMember(
				dest => dest.EnableFailOverOnCollectorDevice,
				opts => opts.MapFrom(src => src.EnableFailOverOnCollectorResource))
			.ForMember(
				dest => dest.HasFailOverDevice,
				opts => opts.MapFrom(src => src.HasFailOverResource))
			.ForMember(
				dest => dest.NeedAutoCreateCollectorDevice,
				opts => opts.MapFrom(src => src.NeedAutoCreateCollectorResource))
			.ForMember(
				dest => dest.SpecifiedCollectorDeviceGroupId,
				opts => opts.MapFrom(src => src.SpecifiedCollectorResourceGroupId))
			.ForMember(
				dest => dest.UserVisibleDeviceCount,
				opts => opts.MapFrom(src => src.UserVisibleResourceCount))
			.ForMember(
				dest => dest.LogicMonitorDeviceId,
				opts => opts.MapFrom(src => src.ResourceId))
			.ForMember(
				dest => dest.OnetimeDowngradeInfo,
				opts => opts.MapFrom(src => src.OneTimeDowngradeInfo));
	}

	private void CreateCollectorStoreItemToCollectorMap()
	{
		CreateMap<CollectorStoreItem, Collector>()
			.IgnoreMembers(
				dest => dest.IsAdminAccount, dest => dest.GroupId, dest => dest.GroupName, dest => dest.AutomaticUpgradeInfo,
				dest => dest.CustomProperties, dest => dest.EscalationChain, dest => dest.NextUpgradeInfo, dest => dest.OneTimeUpgradeInfo,
				dest => dest.PredefinedConfiguration, dest => dest.InstanceCount, dest => dest.BearerToken, dest => dest.Type,
				dest => dest.ConfigurationFields, dest => dest.CopyUrl, dest => dest.DownloadUrl, dest => dest.EncodedConfigData,
				dest => dest.Format, dest => dest.IsEncoded, dest => dest.IsLmLogsEnabled, dest => dest.OtelId,
				dest => dest.OtelVersion, dest => dest.IsSyntheticsEnables, dest => dest.SdtCount, dest => dest.SpecifiedCollectorResourceGroupId,
				dest => dest.CalculatedThreshold, dest => dest.ArchType, dest => dest.CpAgent, dest => dest.LogCollectorGroupId,
				dest => dest.LogCollectorGroupName)
			.ForMember(
				dest => dest.Id,
				opts => opts.MapFrom(src => src.LogicMonitorId))
			.ForMember(
				dest => dest.ResourceId,
				opts => opts.MapFrom(src => src.LogicMonitorDeviceId))
			.ForMember(
				dest => dest.ResourceCount,
				opts => opts.MapFrom(src => src.DeviceCount))
			.ForMember(
				dest => dest.EnableFailOverOnCollectorResource,
				opts => opts.MapFrom(src => src.EnableFailOverOnCollectorDevice))
			.ForMember(
				dest => dest.HasFailOverResource,
				opts => opts.MapFrom(src => src.HasFailOverDevice))
			.ForMember(
				dest => dest.NeedAutoCreateCollectorResource,
				opts => opts.MapFrom(src => src.NeedAutoCreateCollectorDevice))
			.ForMember(
				dest => dest.UserVisibleResourceCount,
				opts => opts.MapFrom(src => src.UserVisibleDeviceCount))
			.ForMember(
				dest => dest.OneTimeDowngradeInfo,
				opts => opts.MapFrom(src => src.OnetimeDowngradeInfo));
	}
}
