namespace LogicMonitor.Datamart.Mapping;

/// <summary>
/// AutoMapper profile for mapping LogicMonitor CollectorGroup API objects to <see cref="CollectorGroupStoreItem"/>.
/// </summary>
public class CollectorGroupProfile : Profile
{
	/// <summary>
	/// Initializes a new instance of the <see cref="CollectorGroupProfile"/> class.
	/// </summary>
	public CollectorGroupProfile()
	{
		CreateMap<CollectorGroup, CollectorGroupStoreItem>()
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
				dest => dest.Collectors,
				opts => opts.Ignore())
			;
		CreateMap<CollectorGroupStoreItem, CollectorGroup>()
			.ForMember(
				dest => dest.Id,
				opts => opts.MapFrom(src => src.LogicMonitorId))
			.ForMember(
				dest => dest.CustomProperties,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.UserPermission,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AutoBalance,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AutoBalanceStrategy,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AutoBalanceResourceCountThreshold,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AutoBalanceInstanceCountThreshold,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.VersionsMismatch,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Platform,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.InstanceCount,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.ResourceCount,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.MismatchVersion,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.HighestPriorityCollectorStatus,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.CalThreshold,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.PropertyForBalancing,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.PropertyForBalancingLastUpdatedOn,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.PropertyForBalancingUpdateLockedUpToMilliseconds,
				opts => opts.Ignore())
			;
	}
}
