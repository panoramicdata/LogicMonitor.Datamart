namespace LogicMonitor.Datamart.Mapping;

public class DataSourceProfile : Profile
{
	public DataSourceProfile()
	{
		CreateMap<DataSource, DataSourceStoreItem>()
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
				dest => dest.DeviceDataSources,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DataPoints,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.LastTimeSeriesDataSyncDurationMs,
				opts => opts.Ignore())
			;

		CreateMap<DataSourceStoreItem, DataSource>()
			.ForMember(
				dest => dest.Id,
				opts => opts.MapFrom(src => src.LogicMonitorId))
			.ForMember(
				dest => dest.AgdMethod,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AgdParams,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AppliesTo,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AuditVersion,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AutoDiscoveryDeleteInactive,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AutoDiscoveryGroovyscript,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AutoDiscoveryIntervalMinutes,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AutoDiscoveryLinuxCmdLine,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AutoDiscoveryLinuxScript,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AutoDiscoveryWindowsCommandLine,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AutoDiscoveryWindowsScript,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.CollectionMethod,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.CollectionMethod,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.CollectorAttribute,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DisplayName,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.EnableEriDiscovery,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.EriDiscoveryInterval,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.EriDiscoveryConfig,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Group,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.HasMultiInstances,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.HasUnacknowledgedAlert,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.IsAutoDiscoveryDisabled,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.IsAutoDiscoveryEnabled,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.AutoDiscoveryConfiguration,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.PollingIntervalSeconds,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Published,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Tags,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Technology,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.UseWildValueAsUuid,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Version,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.DataSourceDataPoints,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.PayloadVersion,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.Checksum,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.LineageId,
				opts => opts.Ignore())
			.ForMember(
				dest => dest.InstallationMetadata,
				opts => opts.Ignore())
			;
	}
}
