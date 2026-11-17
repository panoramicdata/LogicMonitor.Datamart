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
		CreateCollectorGroupToCollectorGroupStoreItemMap();
		CreateCollectorGroupStoreItemToCollectorGroupMap();
	}

	private void CreateCollectorGroupToCollectorGroupStoreItemMap()
	{
		CreateMap<CollectorGroup, CollectorGroupStoreItem>()
			.IgnoreMembers(
				dest => dest.Id, dest => dest.DatamartCreated, dest => dest.DatamartLastModified, dest => dest.DatamartLastObserved,
				dest => dest.Collectors)
			.ForMember(
				dest => dest.LogicMonitorId,
				opts => opts.MapFrom(src => src.Id));
	}

	private void CreateCollectorGroupStoreItemToCollectorGroupMap()
	{
		CreateMap<CollectorGroupStoreItem, CollectorGroup>()
			.IgnoreMembers(
				dest => dest.CustomProperties, dest => dest.UserPermission, dest => dest.AutoBalance, dest => dest.AutoBalanceStrategy,
				dest => dest.AutoBalanceResourceCountThreshold, dest => dest.AutoBalanceInstanceCountThreshold, dest => dest.VersionsMismatch, dest => dest.Platform,
				dest => dest.InstanceCount, dest => dest.ResourceCount, dest => dest.MismatchVersion, dest => dest.HighestPriorityCollectorStatus,
				dest => dest.CalThreshold, dest => dest.PropertyForBalancing, dest => dest.PropertyForBalancingLastUpdatedOn, dest => dest.PropertyForBalancingUpdateLockedUpToMilliseconds)
			.ForMember(
				dest => dest.Id,
				opts => opts.MapFrom(src => src.LogicMonitorId));
	}
}
