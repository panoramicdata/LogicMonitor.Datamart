﻿using AutoMapper;
using LogicMonitor.Api.LogicModules;
using LogicMonitor.Datamart.Models;

namespace LogicMonitor.Datamart.Mapping
{
	public class DataSourceProfile : Profile
	{
		public DataSourceProfile()
		{
			CreateMap<DataSource, DataSourceStoreItem>()
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
					dest => dest.DeviceDataSources,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.DataPoints,
					opts => opts.Ignore())
				;
			CreateMap<DataSourceStoreItem, DataSource>()
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
					dest => dest.Collector,
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
					dest => dest.Version,
					opts => opts.Ignore())
				.ForMember(
					dest => dest.DataSourceDataPoints,
					opts => opts.Ignore())
				;
		}
	}
}
