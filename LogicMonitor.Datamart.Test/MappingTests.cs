using LogicMonitor.Api.Collectors;
using LogicMonitor.Api.LogicModules;
using LogicMonitor.Api.Logs;
using LogicMonitor.Api.Settings;
using LogicMonitor.Api.Websites;
using LogicMonitor.Datamart.Mapping;

namespace LogicMonitor.Datamart.Test;

/// <summary>
/// Tests that exercise the actual mapping at runtime to catch type conversion
/// issues (e.g. Int64 -> Int32) that configuration validation alone does not detect.
/// </summary>
public class MappingTests
{
	private static readonly MapperConfiguration _mapperConfig = new(cfg => cfg.AddMaps(typeof(DatamartClient).Assembly));
	private static readonly Mapper _mapper = new Mapper(_mapperConfig);

	static MappingTests()
	{
		CustomPropertyHandler.Configure([]);
	}

	/// <summary>
	/// Maps an <see cref="Alert"/> to <see cref="AlertStoreItem"/> and verifies a non-null result.
	/// </summary>
	[Fact]
	public void WhenMappingAlert_ThenStoreItemIsCreated()
		=> _mapper.Map<Alert, AlertStoreItem>(new Alert()).Should().NotBeNull();

	/// <summary>
	/// Maps an <see cref="AlertRule"/> to <see cref="AlertRuleStoreItem"/> and verifies a non-null result.
	/// </summary>
	[Fact]
	public void WhenMappingAlertRule_ThenStoreItemIsCreated()
		=> _mapper.Map<AlertRule, AlertRuleStoreItem>(new AlertRule()).Should().NotBeNull();

	/// <summary>
	/// Maps a <see cref="Collector"/> to <see cref="CollectorStoreItem"/> and verifies a non-null result.
	/// </summary>
	[Fact]
	public void WhenMappingCollector_ThenStoreItemIsCreated()
		=> _mapper.Map<Collector, CollectorStoreItem>(new Collector()).Should().NotBeNull();

	/// <summary>
	/// Maps a <see cref="CollectorGroup"/> to <see cref="CollectorGroupStoreItem"/> and verifies a non-null result.
	/// </summary>
	[Fact]
	public void WhenMappingCollectorGroup_ThenStoreItemIsCreated()
		=> _mapper.Map<CollectorGroup, CollectorGroupStoreItem>(new CollectorGroup()).Should().NotBeNull();

	/// <summary>
	/// Maps a <see cref="ConfigSource"/> to <see cref="ConfigSourceStoreItem"/> and verifies a non-null result.
	/// </summary>
	[Fact]
	public void WhenMappingConfigSource_ThenStoreItemIsCreated()
		=> _mapper.Map<ConfigSource, ConfigSourceStoreItem>(new ConfigSource()).Should().NotBeNull();

	/// <summary>
	/// Maps a <see cref="DataSource"/> to <see cref="DataSourceStoreItem"/> and verifies a non-null result.
	/// </summary>
	[Fact]
	public void WhenMappingDataSource_ThenStoreItemIsCreated()
		=> _mapper.Map<DataSource, DataSourceStoreItem>(new DataSource()).Should().NotBeNull();

	/// <summary>
	/// Maps a <see cref="DataSource"/> with null installation metadata and verifies the result is non-null.
	/// </summary>
	[Fact]
	public void WhenMappingDataSourceWithNullInstallationMetadata_ThenStoreItemIsCreated()
		=> _mapper.Map<DataSource, DataSourceStoreItem>(new DataSource { InstallationMetadata = null! }).Should().NotBeNull();

	/// <summary>
	/// Maps a <see cref="DataSourceGraph"/> to <see cref="DataSourceGraphStoreItem"/> and verifies a non-null result.
	/// </summary>
	[Fact]
	public void WhenMappingDataSourceGraph_ThenStoreItemIsCreated()
		=> _mapper.Map<DataSourceGraph, DataSourceGraphStoreItem>(new DataSourceGraph()).Should().NotBeNull();

	/// <summary>
	/// Maps an <see cref="EscalationChain"/> to <see cref="EscalationChainStoreItem"/> and verifies a non-null result.
	/// </summary>
	[Fact]
	public void WhenMappingEscalationChain_ThenStoreItemIsCreated()
		=> _mapper.Map<EscalationChain, EscalationChainStoreItem>(new EscalationChain()).Should().NotBeNull();

	/// <summary>
	/// Maps an <see cref="EventSource"/> to <see cref="EventSourceStoreItem"/> and verifies a non-null result.
	/// </summary>
	[Fact]
	public void WhenMappingEventSource_ThenStoreItemIsCreated()
		=> _mapper.Map<EventSource, EventSourceStoreItem>(new EventSource()).Should().NotBeNull();

	/// <summary>
	/// Maps an <see cref="Integration"/> to <see cref="IntegrationStoreItem"/> and verifies a non-null result.
	/// </summary>
	[Fact]
	public void WhenMappingIntegration_ThenStoreItemIsCreated()
		=> _mapper.Map<Integration, IntegrationStoreItem>(new Integration()).Should().NotBeNull();

	/// <summary>
	/// Maps an <see cref="HttpIntegration"/> to <see cref="IntegrationStoreItem"/> and verifies a non-null result.
	/// </summary>
	[Fact]
	public void WhenMappingHttpIntegration_ThenStoreItemIsCreated()
		=> _mapper.Map<HttpIntegration, IntegrationStoreItem>(new HttpIntegration()).Should().NotBeNull();

	/// <summary>
	/// Maps an <see cref="EmailIntegration"/> to <see cref="IntegrationStoreItem"/> and verifies a non-null result.
	/// </summary>
	[Fact]
	public void WhenMappingEmailIntegration_ThenStoreItemIsCreated()
		=> _mapper.Map<EmailIntegration, IntegrationStoreItem>(new EmailIntegration()).Should().NotBeNull();

	/// <summary>
	/// Maps an <see cref="AutoTaskIntegration"/> to <see cref="IntegrationStoreItem"/> and verifies a non-null result.
	/// </summary>
	[Fact]
	public void WhenMappingAutoTaskIntegration_ThenStoreItemIsCreated()
		=> _mapper.Map<AutoTaskIntegration, IntegrationStoreItem>(new AutoTaskIntegration()).Should().NotBeNull();

	/// <summary>
	/// Maps a <see cref="LogicModuleUpdate"/> to <see cref="LogicModuleUpdateStoreItem"/> and verifies a non-null result.
	/// </summary>
	[Fact]
	public void WhenMappingLogicModuleUpdate_ThenStoreItemIsCreated()
		=> _mapper.Map<LogicModuleUpdate, LogicModuleUpdateStoreItem>(new LogicModuleUpdate()).Should().NotBeNull();

	/// <summary>
	/// Maps a <see cref="LogItem"/> to <see cref="LogStoreItem"/> and verifies a non-null result.
	/// </summary>
	[Fact]
	public void WhenMappingLogItem_ThenStoreItemIsCreated()
		=> _mapper.Map<LogItem, LogStoreItem>(new LogItem()).Should().NotBeNull();

	/// <summary>
	/// Maps a <see cref="Resource"/> to <see cref="ResourceStoreItem"/> and verifies a non-null result.
	/// </summary>
	[Fact]
	public void WhenMappingResource_ThenStoreItemIsCreated()
		=> _mapper.Map<Resource, ResourceStoreItem>(new Resource()).Should().NotBeNull();

	/// <summary>
	/// Maps a <see cref="ResourceGroup"/> to <see cref="ResourceGroupStoreItem"/> and verifies a non-null result.
	/// </summary>
	[Fact]
	public void WhenMappingResourceGroup_ThenStoreItemIsCreated()
		=> _mapper.Map<ResourceGroup, ResourceGroupStoreItem>(new ResourceGroup()).Should().NotBeNull();

	/// <summary>
	/// Maps a <see cref="ResourceDataSource"/> to <see cref="ResourceDataSourceStoreItem"/> and verifies a non-null result.
	/// </summary>
	[Fact]
	public void WhenMappingResourceDataSource_ThenStoreItemIsCreated()
		=> _mapper.Map<ResourceDataSource, ResourceDataSourceStoreItem>(new ResourceDataSource()).Should().NotBeNull();

	/// <summary>
	/// Maps a <see cref="ResourceDataSourceInstance"/> to <see cref="ResourceDataSourceInstanceStoreItem"/> and verifies a non-null result.
	/// </summary>
	[Fact]
	public void WhenMappingResourceDataSourceInstance_ThenStoreItemIsCreated()
		=> _mapper.Map<ResourceDataSourceInstance, ResourceDataSourceInstanceStoreItem>(new ResourceDataSourceInstance()).Should().NotBeNull();

	/// <summary>
	/// Maps a <see cref="Website"/> to <see cref="WebsiteStoreItem"/> and verifies a non-null result.
	/// </summary>
	[Fact]
	public void WhenMappingWebsite_ThenStoreItemIsCreated()
		=> _mapper.Map<Website, WebsiteStoreItem>(new Website()).Should().NotBeNull();

	/// <summary>
	/// Maps a <see cref="WebsiteGroup"/> to <see cref="WebsiteGroupStoreItem"/> and verifies a non-null result.
	/// </summary>
	[Fact]
	public void WhenMappingWebsiteGroup_ThenStoreItemIsCreated()
		=> _mapper.Map<WebsiteGroup, WebsiteGroupStoreItem>(new WebsiteGroup()).Should().NotBeNull();

	// Reverse mappings: StoreItem -> API

	/// <summary>
	/// Maps an <see cref="AlertStoreItem"/> back to an <see cref="Alert"/> and verifies a non-null result.
	/// </summary>
	[Fact]
	public void WhenMappingAlertStoreItem_ThenAlertIsCreated()
		=> _mapper.Map<AlertStoreItem, Alert>(new AlertStoreItem()).Should().NotBeNull();

	/// <summary>
	/// Maps an <see cref="AlertRuleStoreItem"/> back to an <see cref="AlertRule"/> and verifies a non-null result.
	/// </summary>
	[Fact]
	public void WhenMappingAlertRuleStoreItem_ThenAlertRuleIsCreated()
		=> _mapper.Map<AlertRuleStoreItem, AlertRule>(new AlertRuleStoreItem()).Should().NotBeNull();

	/// <summary>
	/// Maps a <see cref="CollectorStoreItem"/> back to a <see cref="Collector"/> and verifies a non-null result.
	/// </summary>
	[Fact]
	public void WhenMappingCollectorStoreItem_ThenCollectorIsCreated()
		=> _mapper.Map<CollectorStoreItem, Collector>(new CollectorStoreItem()).Should().NotBeNull();

	/// <summary>
	/// Maps a <see cref="CollectorGroupStoreItem"/> back to a <see cref="CollectorGroup"/> and verifies a non-null result.
	/// </summary>
	[Fact]
	public void WhenMappingCollectorGroupStoreItem_ThenCollectorGroupIsCreated()
		=> _mapper.Map<CollectorGroupStoreItem, CollectorGroup>(new CollectorGroupStoreItem()).Should().NotBeNull();

	/// <summary>
	/// Maps a <see cref="ConfigSourceStoreItem"/> back to a <see cref="ConfigSource"/> and verifies a non-null result.
	/// </summary>
	[Fact]
	public void WhenMappingConfigSourceStoreItem_ThenConfigSourceIsCreated()
		=> _mapper.Map<ConfigSourceStoreItem, ConfigSource>(CreateConfigSourceStoreItem()).Should().NotBeNull();

	/// <summary>
	/// Maps a <see cref="DataSourceStoreItem"/> back to a <see cref="DataSource"/> and verifies a non-null result.
	/// </summary>
	[Fact]
	public void WhenMappingDataSourceStoreItem_ThenDataSourceIsCreated()
		=> _mapper.Map<DataSourceStoreItem, DataSource>(CreateDataSourceStoreItem()).Should().NotBeNull();

	/// <summary>
	/// Maps a <see cref="DataSourceGraphStoreItem"/> back to a <see cref="DataSourceGraph"/> and verifies a non-null result.
	/// </summary>
	[Fact]
	public void WhenMappingDataSourceGraphStoreItem_ThenDataSourceGraphIsCreated()
		=> _mapper.Map<DataSourceGraphStoreItem, DataSourceGraph>(CreateDataSourceGraphStoreItem()).Should().NotBeNull();

	/// <summary>
	/// Maps an <see cref="EscalationChainStoreItem"/> back to an <see cref="EscalationChain"/> and verifies a non-null result.
	/// </summary>
	[Fact]
	public void WhenMappingEscalationChainStoreItem_ThenEscalationChainIsCreated()
		=> _mapper.Map<EscalationChainStoreItem, EscalationChain>(new EscalationChainStoreItem()).Should().NotBeNull();

	/// <summary>
	/// Maps an <see cref="EventSourceStoreItem"/> back to an <see cref="EventSource"/> and verifies a non-null result.
	/// </summary>
	[Fact]
	public void WhenMappingEventSourceStoreItem_ThenEventSourceIsCreated()
		=> _mapper.Map<EventSourceStoreItem, EventSource>(new EventSourceStoreItem()).Should().NotBeNull();

	/// <summary>
	/// Maps a <see cref="ResourceStoreItem"/> back to a <see cref="Resource"/> and verifies a non-null result.
	/// </summary>
	[Fact]
	public void WhenMappingResourceStoreItem_ThenResourceIsCreated()
		=> _mapper.Map<ResourceStoreItem, Resource>(new ResourceStoreItem()).Should().NotBeNull();

	/// <summary>
	/// Maps a <see cref="ResourceGroupStoreItem"/> back to a <see cref="ResourceGroup"/> and verifies a non-null result.
	/// </summary>
	[Fact]
	public void WhenMappingResourceGroupStoreItem_ThenResourceGroupIsCreated()
		=> _mapper.Map<ResourceGroupStoreItem, ResourceGroup>(new ResourceGroupStoreItem()).Should().NotBeNull();

	/// <summary>
	/// Maps a <see cref="ResourceDataSourceStoreItem"/> back to a <see cref="ResourceDataSource"/> and verifies a non-null result.
	/// </summary>
	[Fact]
	public void WhenMappingResourceDataSourceStoreItem_ThenResourceDataSourceIsCreated()
		=> _mapper.Map<ResourceDataSourceStoreItem, ResourceDataSource>(new ResourceDataSourceStoreItem()).Should().NotBeNull();

	/// <summary>
	/// Maps a <see cref="ResourceConfigSourceStoreItem"/> back to a <see cref="ResourceDataSource"/> and verifies a non-null result.
	/// </summary>
	[Fact]
	public void WhenMappingResourceConfigSourceStoreItem_ThenResourceDataSourceIsCreated()
		=> _mapper.Map<ResourceConfigSourceStoreItem, ResourceDataSource>(new ResourceConfigSourceStoreItem()).Should().NotBeNull();

	/// <summary>
	/// Maps a <see cref="ResourceDataSourceInstanceStoreItem"/> back to a <see cref="ResourceDataSourceInstance"/> and verifies a non-null result.
	/// </summary>
	[Fact]
	public void WhenMappingResourceDataSourceInstanceStoreItem_ThenInstanceIsCreated()
		=> _mapper.Map<ResourceDataSourceInstanceStoreItem, ResourceDataSourceInstance>(new ResourceDataSourceInstanceStoreItem()).Should().NotBeNull();

	/// <summary>
	/// Maps a <see cref="WebsiteStoreItem"/> back to a <see cref="Website"/> and verifies a non-null result.
	/// </summary>
	[Fact]
	public void WhenMappingWebsiteStoreItem_ThenWebsiteIsCreated()
		=> _mapper.Map<WebsiteStoreItem, Website>(new WebsiteStoreItem()).Should().NotBeNull();

	/// <summary>
	/// Maps a <see cref="WebsiteGroupStoreItem"/> back to a <see cref="WebsiteGroup"/> and verifies a non-null result.
	/// </summary>
	[Fact]
	public void WhenMappingWebsiteGroupStoreItem_ThenWebsiteGroupIsCreated()
		=> _mapper.Map<WebsiteGroupStoreItem, WebsiteGroup>(new WebsiteGroupStoreItem()).Should().NotBeNull();

	// Value mapping tests

	/// <summary>
	/// Verifies that the LogicMonitor ID is correctly transferred when mapping a <see cref="ResourceGroup"/>.
	/// </summary>
	[Fact]
	public void WhenMappingResourceGroupWithId_ThenLogicMonitorIdIsMapped()
	{
		var source = new ResourceGroup { Id = 42 };

		var result = _mapper.Map<ResourceGroup, ResourceGroupStoreItem>(source);

		result.LogicMonitorId.Should().Be(42);
	}

	/// <summary>
	/// Verifies that the Name property survives a round-trip mapping from <see cref="ResourceGroup"/> to store item and back.
	/// </summary>
	[Fact]
	public void WhenRoundTrippingResourceGroup_ThenNameIsPreserved()
	{
		var source = new ResourceGroup { Id = 99, Name = "TestGroup" };
		var storeItem = _mapper.Map<ResourceGroup, ResourceGroupStoreItem>(source);
		var roundTripped = _mapper.Map<ResourceGroupStoreItem, ResourceGroup>(storeItem);

		roundTripped.Name.Should().Be("TestGroup");
	}

	/// <summary>
	/// Verifies that the InternalId property survives a round-trip mapping from <see cref="Alert"/> to store item and back.
	/// </summary>
	[Fact]
	public void WhenRoundTrippingAlert_ThenInternalIdIsPreserved()
	{
		var source = new Alert { Id = "AL123", InternalId = "INT456" };
		var storeItem = _mapper.Map<Alert, AlertStoreItem>(source);
		var roundTripped = _mapper.Map<AlertStoreItem, Alert>(storeItem);

		roundTripped.InternalId.Should().Be("INT456");
	}

	/// <summary>
	/// Verifies that the integer AuditVersion on a <see cref="DataSource"/> is mapped to a string on the store item.
	/// </summary>
	[Fact]
	public void WhenMappingDataSourceWithAuditVersion_ThenAuditVersionIsString()
	{
		var source = new DataSource { AuditVersion = 42 };

		var result = _mapper.Map<DataSource, DataSourceStoreItem>(source);

		result.AuditVersion.Should().Be("42");
	}

	/// <summary>
	/// Verifies that the integer Version on a <see cref="DataSource"/> is mapped to a string on the store item.
	/// </summary>
	[Fact]
	public void WhenMappingDataSourceWithVersion_ThenVersionIsString()
	{
		var source = new DataSource { Version = 7 };

		var result = _mapper.Map<DataSource, DataSourceStoreItem>(source);

		result.Version.Should().Be("7");
	}

	/// <summary>
	/// Verifies that the integer AuditVersion on a <see cref="ConfigSource"/> is mapped to a string on the store item.
	/// </summary>
	[Fact]
	public void WhenMappingConfigSourceWithAuditVersion_ThenAuditVersionIsString()
	{
		var source = new ConfigSource { AuditVersion = 99 };

		var result = _mapper.Map<ConfigSource, ConfigSourceStoreItem>(source);

		result.AuditVersion.Should().Be("99");
	}

	// Self-mapping tests (T -> T)

	/// <summary>
	/// Verifies that self-mapping a <see cref="DataSourceGraphStoreItem"/> produces a new distinct instance with the same property values.
	/// </summary>
	[Fact]
	public void WhenSelfMappingDataSourceGraphStoreItem_ThenNewInstanceIsCreated()
	{
		var source = CreateDataSourceGraphStoreItem();
		source.Name = "TestGraph";
		source.Title = "Test Title";
		source.Width = 800;

		var result = _mapper.Map<DataSourceGraphStoreItem, DataSourceGraphStoreItem>(source);

		result.Should().NotBeSameAs(source);
		result.Name.Should().Be("TestGraph");
		result.Title.Should().Be("Test Title");
		result.Width.Should().Be(800);
	}

	/// <summary>
	/// Verifies that mapping a <see cref="DataSourceGraphStoreItem"/> onto an existing target instance updates the target's properties.
	/// </summary>
	[Fact]
	public void WhenSelfMappingDataSourceGraphStoreItemToExisting_ThenTargetIsUpdated()
	{
		var source = CreateDataSourceGraphStoreItem();
		source.Name = "UpdatedGraph";
		source.Height = 600;

		var target = CreateDataSourceGraphStoreItem();
		target.Name = "OriginalGraph";

		_mapper.Map(source, target);

		target.Name.Should().Be("UpdatedGraph");
		target.Height.Should().Be(600);
	}

	private static ConfigSourceStoreItem CreateConfigSourceStoreItem() => new()
	{
		AppliesTo = string.Empty,
		AuditVersion = string.Empty,
		Checksum = string.Empty,
		CollectionMethod = string.Empty,
		Description = string.Empty,
		DisplayName = string.Empty,
		Group = string.Empty,
		Name = string.Empty,
		Version = string.Empty
	};

	private static DataSourceStoreItem CreateDataSourceStoreItem() => new()
	{
		AppliesTo = string.Empty,
		AuditVersion = string.Empty,
		Checksum = string.Empty,
		CollectionMethod = string.Empty,
		Description = string.Empty,
		DisplayName = string.Empty,
		Group = string.Empty,
		Name = string.Empty,
		Version = string.Empty,
		Technology = string.Empty,
		Tags = string.Empty,
		LineageId = string.Empty,
		InstallationMetadataOriginRegistryId = null,
		InstallationMetadataOriginLineageId = null,
		InstallationMetadataOriginAuthorCompanyUuid = null,
		InstallationMetadataOriginAuthorNamespace = null,
		InstallationMetadataOriginVersion = null,
		InstallationMetadataOriginChecksum = null,
		InstallationMetadataAuditedRegistryId = null,
		InstallationMetadataAuditedVersion = null,
		InstallationMetadataTargetLineageId = null,
		InstallationMetadataTargetLastPublishedId = null,
		InstallationMetadataTargetLastPublishedVersion = null,
		InstallationMetadataTargetLastPublishedChecksum = null,
		InstallationMetadataLogicModuleType = null,
		InstallationMetadataLogicModuleId = null,
		InstallationMetadataIsChangedFromOrigin = null,
		InstallationMetadataIsChangedFromTargetLastPublished = null,
		PayloadVersion = string.Empty,
		HasMultiInstances = false,
		UseWildValueAsUuid = false,
		PollingIntervalSeconds = 0,
		CollectionAttributeName = null,
		CollectionAttributeIp = null,
		LastTimeSeriesDataSyncDurationMs = null
	};

	private static DataSourceGraphStoreItem CreateDataSourceGraphStoreItem() => new()
	{
		Name = string.Empty,
		Title = string.Empty,
		VerticalLabel = string.Empty,
		IsRigid = false,
		Width = 0,
		Height = 0,
		MaxValue = null,
		MinValue = null,
		DisplayPriority = 0,
		Timescale = string.Empty,
		IsBase1024 = false
	};
}
