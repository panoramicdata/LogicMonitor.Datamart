﻿// <auto-generated />
using System;
using LogicMonitor.Datamart;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LogicMonitor.Datamart.Migrations
{
    [DbContext(typeof(Context))]
    [Migration("20191114150153_RemovedLastMeasurementUpdatedTimeSeconds")]
    partial class RemovedLastMeasurementUpdatedTimeSeconds
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("LogicMonitor.Datamart.Models.AlertRuleStoreItem", b =>
                {
                    b.Property<int>("DatamartId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("DataPoint");

                    b.Property<string>("DataSourceInstanceName");

                    b.Property<string>("DataSourceName");

                    b.Property<DateTime>("DatamartCreatedUtc");

                    b.Property<DateTime>("DatamartLastModifiedUtc");

                    b.Property<string>("Description");

                    b.Property<int>("EscalationChainId");

                    b.Property<int>("EscalationChainIntervalMinutes");

                    b.Property<int>("Id");

                    b.Property<string>("LevelString");

                    b.Property<string>("Name");

                    b.Property<int>("Priority");

                    b.Property<bool>("SuppressAlertAckSdt");

                    b.Property<bool>("SuppressAlertClear");

                    b.HasKey("DatamartId");

                    b.ToTable("AlertRules");
                });

            modelBuilder.Entity("LogicMonitor.Datamart.Models.AlertStoreItem", b =>
                {
                    b.Property<int>("DatamartId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AckComment")
                        .HasMaxLength(50);

                    b.Property<bool>("Acked");

                    b.Property<string>("AckedBy")
                        .HasMaxLength(50);

                    b.Property<int>("AckedOnSeconds");

                    b.Property<int>("AlertEscalationChainId");

                    b.Property<string>("AlertEscalationChainName")
                        .HasMaxLength(50);

                    b.Property<int>("AlertEscalationSubChainId");

                    b.Property<string>("AlertRecipients")
                        .HasMaxLength(200);

                    b.Property<int>("AlertRuleId");

                    b.Property<string>("AlertRuleName")
                        .HasMaxLength(50);

                    b.Property<int>("AlertType");

                    b.Property<string>("ClearValue")
                        .HasMaxLength(50);

                    b.Property<string>("CustomColumn1")
                        .HasMaxLength(50);

                    b.Property<string>("CustomColumn2")
                        .HasMaxLength(50);

                    b.Property<string>("CustomColumn3")
                        .HasMaxLength(50);

                    b.Property<string>("CustomColumn4")
                        .HasMaxLength(50);

                    b.Property<string>("CustomColumn5")
                        .HasMaxLength(50);

                    b.Property<int>("DataPointId");

                    b.Property<string>("DataPointName")
                        .HasMaxLength(50);

                    b.Property<DateTime>("DatamartCreatedUtc");

                    b.Property<DateTime>("DatamartLastModifiedUtc");

                    b.Property<string>("DetailMessageBody")
                        .HasMaxLength(1000);

                    b.Property<string>("DetailMessageSubject")
                        .HasMaxLength(200);

                    b.Property<int>("EndOnSeconds");

                    b.Property<string>("Id")
                        .HasMaxLength(20);

                    b.Property<bool>("InScheduledDownTime");

                    b.Property<string>("InstanceDescription")
                        .HasMaxLength(1000);

                    b.Property<int>("InstanceId");

                    b.Property<string>("InstanceName")
                        .HasMaxLength(50);

                    b.Property<string>("InternalId")
                        .HasMaxLength(20);

                    b.Property<bool>("IsCleared");

                    b.Property<int?>("MonitorObjectGroup0Id");

                    b.Property<int?>("MonitorObjectGroup1Id");

                    b.Property<int?>("MonitorObjectGroup2Id");

                    b.Property<int?>("MonitorObjectGroup3Id");

                    b.Property<int?>("MonitorObjectGroup4Id");

                    b.Property<int?>("MonitorObjectGroup5Id");

                    b.Property<int?>("MonitorObjectGroup6Id");

                    b.Property<int?>("MonitorObjectGroup7Id");

                    b.Property<int?>("MonitorObjectGroup8Id");

                    b.Property<int?>("MonitorObjectGroup9Id");

                    b.Property<int?>("MonitorObjectId");

                    b.Property<string>("MonitorObjectName")
                        .HasMaxLength(50);

                    b.Property<int>("MonitorObjectType");

                    b.Property<int>("NextRecipient");

                    b.Property<int?>("ResourceId");

                    b.Property<int?>("ResourceTemplateId");

                    b.Property<string>("ResourceTemplateName")
                        .HasMaxLength(50);

                    b.Property<string>("ResourceTemplateType")
                        .HasMaxLength(10);

                    b.Property<int>("Severity");

                    b.Property<int>("StartOnSeconds");

                    b.Property<string>("Thresholds")
                        .HasMaxLength(50);

                    b.Property<string>("Value")
                        .HasMaxLength(50);

                    b.HasKey("DatamartId");

                    b.HasIndex("AlertRuleId");

                    b.HasIndex("DataPointId");

                    b.HasIndex("DataPointName");

                    b.HasIndex("EndOnSeconds");

                    b.HasIndex("Id");

                    b.HasIndex("InstanceId");

                    b.HasIndex("InstanceName");

                    b.HasIndex("InternalId");

                    b.HasIndex("IsCleared");

                    b.HasIndex("MonitorObjectGroup0Id");

                    b.HasIndex("MonitorObjectGroup1Id");

                    b.HasIndex("MonitorObjectGroup2Id");

                    b.HasIndex("MonitorObjectGroup3Id");

                    b.HasIndex("MonitorObjectGroup4Id");

                    b.HasIndex("MonitorObjectGroup5Id");

                    b.HasIndex("MonitorObjectGroup6Id");

                    b.HasIndex("MonitorObjectGroup7Id");

                    b.HasIndex("MonitorObjectGroup8Id");

                    b.HasIndex("MonitorObjectGroup9Id");

                    b.HasIndex("MonitorObjectId");

                    b.HasIndex("MonitorObjectName");

                    b.HasIndex("MonitorObjectType");

                    b.HasIndex("ResourceId");

                    b.HasIndex("ResourceTemplateId");

                    b.HasIndex("ResourceTemplateName");

                    b.HasIndex("ResourceTemplateType");

                    b.HasIndex("Severity");

                    b.HasIndex("StartOnSeconds");

                    b.HasIndex("StartOnSeconds", "EndOnSeconds", "IsCleared", "InScheduledDownTime", "MonitorObjectGroup0Id", "MonitorObjectGroup1Id", "MonitorObjectGroup2Id", "MonitorObjectGroup3Id", "MonitorObjectGroup4Id", "MonitorObjectGroup5Id", "MonitorObjectGroup6Id", "MonitorObjectGroup7Id", "MonitorObjectGroup8Id", "MonitorObjectGroup9Id")
                        .HasName("IX_Alerts_FasterPercentageAvailability")
                        .HasAnnotation("SqlServer:Include", new[] { "Id", "Severity", "ClearValue", "MonitorObjectId", "ResourceTemplateName", "InstanceId", "InstanceName" });

                    b.ToTable("Alerts");
                });

            modelBuilder.Entity("LogicMonitor.Datamart.Models.CollectorGroupStoreItem", b =>
                {
                    b.Property<int>("DatamartId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CollectorCount");

                    b.Property<long>("CreatedOnTimeStampSeconds");

                    b.Property<DateTime>("DatamartCreatedUtc");

                    b.Property<DateTime>("DatamartLastModifiedUtc");

                    b.Property<string>("Description");

                    b.Property<int>("Id");

                    b.Property<string>("Name");

                    b.HasKey("DatamartId");

                    b.ToTable("CollectorGroups");
                });

            modelBuilder.Entity("LogicMonitor.Datamart.Models.CollectorStoreItem", b =>
                {
                    b.Property<int>("DatamartId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AckComment");

                    b.Property<bool>("Acked");

                    b.Property<string>("AckedBy");

                    b.Property<string>("AckedOnLocalString");

                    b.Property<long?>("AckedOnUtcTimestampUtc");

                    b.Property<string>("Architecture");

                    b.Property<int>("BackupCollectorId");

                    b.Property<int>("Build");

                    b.Property<bool>("CanDowngrade");

                    b.Property<string>("CanDowngradeReason");

                    b.Property<bool>("ClearSent");

                    b.Property<string>("CollectorConfiguration");

                    b.Property<string>("Configuration");

                    b.Property<int>("ConfigurationVersion");

                    b.Property<string>("CreatedOnLocalString");

                    b.Property<long>("CreatedOnTimeStampUtc");

                    b.Property<string>("Credential");

                    b.Property<string>("Credential2");

                    b.Property<DateTime>("DatamartCreatedUtc");

                    b.Property<DateTime>("DatamartLastModifiedUtc");

                    b.Property<string>("Description");

                    b.Property<int>("DeviceCount");

                    b.Property<int>("DeviceId");

                    b.Property<bool>("Ea");

                    b.Property<bool>("EnableFailBack");

                    b.Property<bool>("EnableFailOverOnCollectorDevice");

                    b.Property<int>("EscalationChainId");

                    b.Property<int>("GroupId");

                    b.Property<string>("GroupName");

                    b.Property<bool>("HasFailOverDevice");

                    b.Property<string>("HostName");

                    b.Property<int>("Id");

                    b.Property<bool>("InSdt");

                    b.Property<bool>("IsDown");

                    b.Property<string>("LastSentNotificationOnLocal");

                    b.Property<int>("LastSentNotificationOnTimeStampUtc");

                    b.Property<string>("Name");

                    b.Property<bool>("NeedAutoCreateCollectorDevice");

                    b.Property<int>("NetscanVersion");

                    b.Property<int>("NextRecipient");

                    b.Property<string>("OnetimeDowngradeInfo");

                    b.Property<string>("Platform");

                    b.Property<int>("PreviousVersion");

                    b.Property<string>("ProxyConfiguration");

                    b.Property<int>("ResendIntervalSeconds");

                    b.Property<string>("Size");

                    b.Property<int>("SpecifiedCollectorDeviceGroupId");

                    b.Property<int>("Status");

                    b.Property<bool>("SuppressAlertClear");

                    b.Property<string>("UpdatedOnLocalString");

                    b.Property<long?>("UpdatedOnTimeStampUtc");

                    b.Property<long>("UpgradeTimeUtcSeconds");

                    b.Property<int>("UptimeSeconds");

                    b.Property<string>("UserChangeOnLocal");

                    b.Property<long>("UserChangeOnUtcSeconds");

                    b.Property<int>("UserPermission");

                    b.Property<int>("UserVisibleDeviceCount");

                    b.Property<int>("UserVisibleWebsiteCount");

                    b.Property<string>("WatchdogConfiguration");

                    b.Property<string>("WatchdogUpdatedOnLocal");

                    b.Property<long?>("WatchdogUpdatedOnSeconds");

                    b.Property<string>("WebsiteConfiguration");

                    b.Property<int>("WebsiteCount");

                    b.Property<string>("WrapperConfiguration");

                    b.HasKey("DatamartId");

                    b.HasIndex("GroupId");

                    b.ToTable("Collectors");
                });

            modelBuilder.Entity("LogicMonitor.Datamart.Models.ConfigSourceStoreItem", b =>
                {
                    b.Property<int>("DatamartId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DatamartCreatedUtc");

                    b.Property<DateTime>("DatamartLastModifiedUtc");

                    b.Property<string>("Description");

                    b.Property<int>("Id");

                    b.Property<string>("Name");

                    b.HasKey("DatamartId");

                    b.ToTable("ConfigSources");
                });

            modelBuilder.Entity("LogicMonitor.Datamart.Models.DataSourceDataPointStoreItem", b =>
                {
                    b.Property<int>("DatamartId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("DataSourceId");

                    b.Property<DateTime>("DatamartCreatedUtc");

                    b.Property<DateTime>("DatamartLastModifiedUtc");

                    b.Property<string>("Description");

                    b.Property<int>("Id");

                    b.Property<string>("MeasurementUnit");

                    b.Property<string>("Name");

                    b.HasKey("DatamartId");

                    b.HasIndex("DataSourceId");

                    b.ToTable("DataSourceDataPoints");
                });

            modelBuilder.Entity("LogicMonitor.Datamart.Models.DataSourceStoreItem", b =>
                {
                    b.Property<int>("DatamartId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DatamartCreatedUtc");

                    b.Property<DateTime>("DatamartLastModifiedUtc");

                    b.Property<string>("Description");

                    b.Property<int>("Id");

                    b.Property<string>("Name");

                    b.HasKey("DatamartId");

                    b.ToTable("DataSources");
                });

            modelBuilder.Entity("LogicMonitor.Datamart.Models.DeviceDataSourceInstanceStoreItem", b =>
                {
                    b.Property<int>("DatamartId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AlertDisableStatus");

                    b.Property<int>("AlertStatus");

                    b.Property<int>("AlertStatusPriority");

                    b.Property<int?>("DataSourceId");

                    b.Property<DateTime>("DatamartCreatedUtc");

                    b.Property<DateTime>("DatamartLastModifiedUtc");

                    b.Property<int>("DeviceDataSourceId");

                    b.Property<int?>("DeviceId");

                    b.Property<bool>("DisableAlerting");

                    b.Property<string>("DisplayName");

                    b.Property<int>("GroupId");

                    b.Property<string>("GroupName");

                    b.Property<int>("Id");

                    b.Property<DateTime?>("LastAggregationHourWrittenUtc");

                    b.Property<long>("LastCollectedTimeSeconds");

                    b.Property<long>("LastUpdatedTimeSeconds");

                    b.Property<DateTime?>("LastWentMissingUtc");

                    b.Property<bool>("LockDescription");

                    b.Property<string>("SdtAt");

                    b.Property<int>("SdtStatus");

                    b.Property<bool>("StopMonitoring");

                    b.Property<string>("WildValue");

                    b.Property<string>("WildValue2");

                    b.HasKey("DatamartId");

                    b.HasIndex("DeviceDataSourceId");

                    b.HasIndex("DeviceId");

                    b.HasIndex("LastWentMissingUtc");

                    b.ToTable("DeviceDataSourceInstances");
                });

            modelBuilder.Entity("LogicMonitor.Datamart.Models.DeviceDataSourceStoreItem", b =>
                {
                    b.Property<int>("DatamartId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("AssignedOnSeconds");

                    b.Property<long>("CreatedOnSeconds");

                    b.Property<int>("DataSourceId");

                    b.Property<DateTime>("DatamartCreatedUtc");

                    b.Property<DateTime>("DatamartLastModifiedUtc");

                    b.Property<int>("DeviceId");

                    b.Property<int>("Id");

                    b.Property<long>("UpdatedOnSeconds");

                    b.HasKey("DatamartId");

                    b.HasIndex("DataSourceId");

                    b.HasIndex("DeviceId");

                    b.ToTable("DeviceDataSources");
                });

            modelBuilder.Entity("LogicMonitor.Datamart.Models.DeviceGroupStoreItem", b =>
                {
                    b.Property<int>("DatamartId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AlertDisableStatus");

                    b.Property<bool>("AlertEnable");

                    b.Property<int>("AlertStatus");

                    b.Property<int>("AlertStatusPriority");

                    b.Property<string>("AppliesTo");

                    b.Property<string>("AutoVisualResult");

                    b.Property<int>("AwsDeviceCount");

                    b.Property<string>("AwsRegionsInfo");

                    b.Property<string>("AwsTestResult");

                    b.Property<int>("AwsTestResultCode");

                    b.Property<int>("AzureDeviceCount");

                    b.Property<string>("AzureRegionsInfo");

                    b.Property<string>("AzureTestResult");

                    b.Property<int>("AzureTestResultCode");

                    b.Property<string>("ClusterAlertStatus");

                    b.Property<int>("ClusterAlertStatusPriority");

                    b.Property<int?>("CreatedOnTimestampUtc");

                    b.Property<DateTime>("DatamartCreatedUtc");

                    b.Property<DateTime>("DatamartLastModifiedUtc");

                    b.Property<int>("DefaultAgentId");

                    b.Property<string>("DefaultCollectorDescription");

                    b.Property<int>("DefaultCollectorId");

                    b.Property<string>("Description");

                    b.Property<int>("DeviceCount");

                    b.Property<int>("DeviceGroupType");

                    b.Property<int>("DirectDeviceCount");

                    b.Property<int>("DirectSubGroupCount");

                    b.Property<bool>("EffectiveAlertEnabled");

                    b.Property<string>("FullPath");

                    b.Property<int>("GcpDeviceCount");

                    b.Property<string>("GcpRegionsInfo");

                    b.Property<string>("GcpTestResult");

                    b.Property<int>("GcpTestResultCode");

                    b.Property<string>("GroupStatus");

                    b.Property<bool>("HasNetflowEnabledDevices");

                    b.Property<int>("Id");

                    b.Property<bool>("IsAlertingDisabled");

                    b.Property<bool>("IsNetflowEnabled");

                    b.Property<string>("Name");

                    b.Property<int>("ParentId");

                    b.Property<int>("SdtStatus");

                    b.Property<int>("UserPermission");

                    b.HasKey("DatamartId");

                    b.ToTable("DeviceGroups");
                });

            modelBuilder.Entity("LogicMonitor.Datamart.Models.DeviceStoreItem", b =>
                {
                    b.Property<int>("DatamartId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AlertDisableStatus");

                    b.Property<int>("AlertStatus");

                    b.Property<int>("AlertStatusPriority");

                    b.Property<bool>("AncestorHasDisabledLogicModule");

                    b.Property<long?>("AutoPropertiesAssignedOnSeconds");

                    b.Property<long?>("AutoPropertiesUpdatedOnSeconds");

                    b.Property<int>("AwsState");

                    b.Property<int>("AzureState");

                    b.Property<bool>("CanUseRemoteSession");

                    b.Property<string>("CollectorDescription");

                    b.Property<long?>("CreatedOnSeconds");

                    b.Property<int>("CurrentCollectorId");

                    b.Property<DateTime>("DatamartCreatedUtc");

                    b.Property<DateTime>("DatamartLastModifiedUtc");

                    b.Property<long>("DeletedTimeinMs");

                    b.Property<string>("Description");

                    b.Property<string>("DeviceGroupIdsString");

                    b.Property<byte>("DeviceStatus");

                    b.Property<int>("DeviceType");

                    b.Property<string>("DisplayName");

                    b.Property<bool>("EffectiveAlertEnabled");

                    b.Property<bool>("EnableNetflow");

                    b.Property<int>("GcpState");

                    b.Property<bool>("HasActiveInstance");

                    b.Property<bool>("HasDisabledSubResource");

                    b.Property<bool>("HasMore");

                    b.Property<int>("Id");

                    b.Property<bool>("IsAlertingDisabled");

                    b.Property<long>("LastAlertClosedTimeSeconds");

                    b.Property<long?>("LastDataTimeSeconds");

                    b.Property<long?>("LastRawDataTimeSeconds");

                    b.Property<string>("Link");

                    b.Property<string>("Name");

                    b.Property<string>("NetflowCollectorDescription");

                    b.Property<int>("NetflowCollectorGroupId");

                    b.Property<string>("NetflowCollectorGroupName");

                    b.Property<int>("NetflowCollectorId");

                    b.Property<int>("PreferredCollectorGroupId");

                    b.Property<string>("PreferredCollectorGroupName");

                    b.Property<int>("PreferredCollectorId");

                    b.Property<string>("Property1");

                    b.Property<string>("Property2");

                    b.Property<string>("Property3");

                    b.Property<string>("Property4");

                    b.Property<string>("Property5");

                    b.Property<int>("RelatedDeviceId");

                    b.Property<int>("ScanConfigId");

                    b.Property<int>("SdtStatus");

                    b.Property<long>("ToDeleteTimeinMs");

                    b.Property<long?>("UpdatedOnSeconds");

                    b.Property<int>("UptimeInSeconds");

                    b.Property<int>("UserPermission");

                    b.HasKey("DatamartId");

                    b.ToTable("Devices");
                });

            modelBuilder.Entity("LogicMonitor.Datamart.Models.EscalationChainStoreItem", b =>
                {
                    b.Property<int>("DatamartId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DatamartCreatedUtc");

                    b.Property<DateTime>("DatamartLastModifiedUtc");

                    b.Property<string>("Description");

                    b.Property<bool>("EnableThrottling");

                    b.Property<int>("Id");

                    b.Property<bool>("InAlerting");

                    b.Property<string>("Name");

                    b.Property<int>("ThrottlingAlertCount");

                    b.Property<int>("ThrottlingPeriodMinutes");

                    b.HasKey("DatamartId");

                    b.ToTable("EscalationChains");
                });

            modelBuilder.Entity("LogicMonitor.Datamart.Models.EventSourceStoreItem", b =>
                {
                    b.Property<int>("DatamartId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DatamartCreatedUtc");

                    b.Property<DateTime>("DatamartLastModifiedUtc");

                    b.Property<string>("Description");

                    b.Property<int>("Id");

                    b.Property<string>("Name");

                    b.HasKey("DatamartId");

                    b.ToTable("EventSources");
                });

            modelBuilder.Entity("LogicMonitor.Datamart.Models.LogStoreItem", b =>
                {
                    b.Property<int>("DatamartId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DatamartCreatedUtc");

                    b.Property<DateTime>("DatamartLastModifiedUtc");

                    b.Property<string>("Description");

                    b.Property<long>("HappenedOnTimeStampUtc");

                    b.Property<string>("Id")
                        .HasMaxLength(50);

                    b.Property<string>("IpAddress")
                        .HasMaxLength(200);

                    b.Property<string>("SessionId")
                        .HasMaxLength(50);

                    b.Property<string>("UserName")
                        .HasMaxLength(100);

                    b.HasKey("DatamartId");

                    b.ToTable("LogItems");
                });

            modelBuilder.Entity("LogicMonitor.Datamart.Models.MonitorObjectGroupStoreItem", b =>
                {
                    b.Property<int>("DatamartId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DatamartCreatedUtc");

                    b.Property<DateTime>("DatamartLastModifiedUtc");

                    b.Property<string>("FullPath")
                        .HasMaxLength(200);

                    b.Property<int>("MonitoredObjectType");

                    b.HasKey("DatamartId");

                    b.HasIndex("FullPath", "MonitoredObjectType");

                    b.ToTable("MonitorObjectGroups");
                });

            modelBuilder.Entity("LogicMonitor.Datamart.Models.WebsiteGroupStoreItem", b =>
                {
                    b.Property<int>("DatamartId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AlertDisableStatus");

                    b.Property<int>("AlertStatus");

                    b.Property<int>("AlertStatusPriority");

                    b.Property<DateTime>("DatamartCreatedUtc");

                    b.Property<DateTime>("DatamartLastModifiedUtc");

                    b.Property<string>("Description");

                    b.Property<int>("DirectWebsiteCount");

                    b.Property<int>("DirectWebsiteGroupCount");

                    b.Property<bool>("DisableAlerting");

                    b.Property<string>("FullPath");

                    b.Property<int>("GroupStatus");

                    b.Property<bool>("HasWebsitesDisabled");

                    b.Property<int>("Id");

                    b.Property<string>("Name");

                    b.Property<int>("ParentId");

                    b.Property<int>("SdtStatus");

                    b.Property<bool?>("StopMonitoring");

                    b.Property<int>("UserPermissionString");

                    b.Property<int>("WebsiteCount");

                    b.HasKey("DatamartId");

                    b.ToTable("WebsiteGroups");
                });

            modelBuilder.Entity("LogicMonitor.Datamart.Models.WebsiteStoreItem", b =>
                {
                    b.Property<int>("DatamartId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AlertDisableStatus");

                    b.Property<string>("AlertExpression");

                    b.Property<int>("AlertStatus");

                    b.Property<int>("AlertStatusPriority");

                    b.Property<int>("Count");

                    b.Property<DateTime>("DatamartCreatedUtc");

                    b.Property<DateTime>("DatamartLastModifiedUtc");

                    b.Property<string>("Description");

                    b.Property<string>("Domain");

                    b.Property<int>("GlobalSmAlertCond");

                    b.Property<string>("HostName");

                    b.Property<int>("Id");

                    b.Property<bool>("IgnoreSsl");

                    b.Property<byte>("IndividualAlertLevel");

                    b.Property<bool>("IndividualSmAlertEnable");

                    b.Property<bool>("IsAlertingDisabled");

                    b.Property<bool>("IsInternal");

                    b.Property<string>("Name");

                    b.Property<byte>("OverallAlertLevel");

                    b.Property<int>("PacketsNotReceivedTimeoutMs");

                    b.Property<int>("PageLoadAlertTimeInMs");

                    b.Property<int>("PercentPacketsNotReceiveInTime");

                    b.Property<int>("PollingIntervalMinutes");

                    b.Property<string>("Schema");

                    b.Property<string>("Script");

                    b.Property<int>("SdtStatus");

                    b.Property<int>("Status");

                    b.Property<bool>("StopMonitoring");

                    b.Property<bool>("StopMonitoringByWebsiteGroup");

                    b.Property<int>("Transition");

                    b.Property<bool>("TriggerSslExpirationAlerts");

                    b.Property<bool>("TriggerSslStatusAlerts");

                    b.Property<int>("Type");

                    b.Property<bool>("UseDefaultAlertSetting");

                    b.Property<bool>("UseDefaultLocationSetting");

                    b.Property<int>("UserPermissionString");

                    b.Property<int>("WebsiteGroupId");

                    b.Property<int>("WebsiteMethod");

                    b.HasKey("DatamartId");

                    b.HasIndex("WebsiteGroupId");

                    b.ToTable("Websites");
                });

            modelBuilder.Entity("LogicMonitor.Datamart.Models.AlertStoreItem", b =>
                {
                    b.HasOne("LogicMonitor.Datamart.Models.AlertRuleStoreItem", "AlertRule")
                        .WithMany("Alerts")
                        .HasForeignKey("AlertRuleId")
                        .HasPrincipalKey("Id")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("LogicMonitor.Datamart.Models.MonitorObjectGroupStoreItem", "MonitorObjectGroup0")
                        .WithMany("AlertsFromGroup0")
                        .HasForeignKey("MonitorObjectGroup0Id")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("LogicMonitor.Datamart.Models.MonitorObjectGroupStoreItem", "MonitorObjectGroup1")
                        .WithMany("AlertsFromGroup1")
                        .HasForeignKey("MonitorObjectGroup1Id")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("LogicMonitor.Datamart.Models.MonitorObjectGroupStoreItem", "MonitorObjectGroup2")
                        .WithMany("AlertsFromGroup2")
                        .HasForeignKey("MonitorObjectGroup2Id")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("LogicMonitor.Datamart.Models.MonitorObjectGroupStoreItem", "MonitorObjectGroup3")
                        .WithMany("AlertsFromGroup3")
                        .HasForeignKey("MonitorObjectGroup3Id")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("LogicMonitor.Datamart.Models.MonitorObjectGroupStoreItem", "MonitorObjectGroup4")
                        .WithMany("AlertsFromGroup4")
                        .HasForeignKey("MonitorObjectGroup4Id")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("LogicMonitor.Datamart.Models.MonitorObjectGroupStoreItem", "MonitorObjectGroup5")
                        .WithMany("AlertsFromGroup5")
                        .HasForeignKey("MonitorObjectGroup5Id")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("LogicMonitor.Datamart.Models.MonitorObjectGroupStoreItem", "MonitorObjectGroup6")
                        .WithMany("AlertsFromGroup6")
                        .HasForeignKey("MonitorObjectGroup6Id")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("LogicMonitor.Datamart.Models.MonitorObjectGroupStoreItem", "MonitorObjectGroup7")
                        .WithMany("AlertsFromGroup7")
                        .HasForeignKey("MonitorObjectGroup7Id")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("LogicMonitor.Datamart.Models.MonitorObjectGroupStoreItem", "MonitorObjectGroup8")
                        .WithMany("AlertsFromGroup8")
                        .HasForeignKey("MonitorObjectGroup8Id")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("LogicMonitor.Datamart.Models.MonitorObjectGroupStoreItem", "MonitorObjectGroup9")
                        .WithMany("AlertsFromGroup9")
                        .HasForeignKey("MonitorObjectGroup9Id")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("LogicMonitor.Datamart.Models.CollectorStoreItem", b =>
                {
                    b.HasOne("LogicMonitor.Datamart.Models.CollectorGroupStoreItem", "CollectorGroup")
                        .WithMany("Collectors")
                        .HasForeignKey("GroupId")
                        .HasPrincipalKey("Id")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("LogicMonitor.Datamart.Models.DataSourceDataPointStoreItem", b =>
                {
                    b.HasOne("LogicMonitor.Datamart.Models.DataSourceStoreItem", "DataSource")
                        .WithMany("DataPoints")
                        .HasForeignKey("DataSourceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("LogicMonitor.Datamart.Models.DeviceDataSourceInstanceStoreItem", b =>
                {
                    b.HasOne("LogicMonitor.Datamart.Models.DeviceDataSourceStoreItem", "DeviceDataSource")
                        .WithMany("DeviceDataSourceInstances")
                        .HasForeignKey("DeviceDataSourceId")
                        .HasPrincipalKey("Id")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("LogicMonitor.Datamart.Models.DeviceStoreItem", "Device")
                        .WithMany("DeviceDataSourceInstances")
                        .HasForeignKey("DeviceId")
                        .HasPrincipalKey("Id")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("LogicMonitor.Datamart.Models.DeviceDataSourceStoreItem", b =>
                {
                    b.HasOne("LogicMonitor.Datamart.Models.DataSourceStoreItem", "DataSource")
                        .WithMany("DeviceDataSources")
                        .HasForeignKey("DataSourceId")
                        .HasPrincipalKey("Id")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("LogicMonitor.Datamart.Models.DeviceStoreItem", "Device")
                        .WithMany("DeviceDataSources")
                        .HasForeignKey("DeviceId")
                        .HasPrincipalKey("Id")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("LogicMonitor.Datamart.Models.WebsiteStoreItem", b =>
                {
                    b.HasOne("LogicMonitor.Datamart.Models.WebsiteGroupStoreItem", "WebsiteGroup")
                        .WithMany("Websites")
                        .HasForeignKey("WebsiteGroupId")
                        .HasPrincipalKey("Id")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}