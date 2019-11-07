using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LogicMonitor.Datamart.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlertRules",
                columns: table => new
                {
                    DatamartId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DatamartCreatedUtc = table.Column<DateTime>(nullable: false),
                    DatamartLastModifiedUtc = table.Column<DateTime>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Priority = table.Column<int>(nullable: false),
                    LevelString = table.Column<string>(nullable: true),
                    DataSourceName = table.Column<string>(nullable: true),
                    DataSourceInstanceName = table.Column<string>(nullable: true),
                    DataPoint = table.Column<string>(nullable: true),
                    EscalationChainIntervalMinutes = table.Column<int>(nullable: false),
                    EscalationChainId = table.Column<int>(nullable: false),
                    SuppressAlertClear = table.Column<bool>(nullable: false),
                    SuppressAlertAckSdt = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertRules", x => x.DatamartId);
                    table.UniqueConstraint("AK_AlertRules_Id", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CollectorGroups",
                columns: table => new
                {
                    DatamartId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DatamartCreatedUtc = table.Column<DateTime>(nullable: false),
                    DatamartLastModifiedUtc = table.Column<DateTime>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    CreatedOnTimeStampSeconds = table.Column<long>(nullable: false),
                    CollectorCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectorGroups", x => x.DatamartId);
                    table.UniqueConstraint("AK_CollectorGroups_Id", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConfigSources",
                columns: table => new
                {
                    DatamartId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DatamartCreatedUtc = table.Column<DateTime>(nullable: false),
                    DatamartLastModifiedUtc = table.Column<DateTime>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigSources", x => x.DatamartId);
                });

            migrationBuilder.CreateTable(
                name: "DataSources",
                columns: table => new
                {
                    DatamartId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DatamartCreatedUtc = table.Column<DateTime>(nullable: false),
                    DatamartLastModifiedUtc = table.Column<DateTime>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataSources", x => x.DatamartId);
                    table.UniqueConstraint("AK_DataSources_Id", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeviceGroups",
                columns: table => new
                {
                    DatamartId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DatamartCreatedUtc = table.Column<DateTime>(nullable: false),
                    DatamartLastModifiedUtc = table.Column<DateTime>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    AlertDisableStatus = table.Column<int>(nullable: false),
                    AlertEnable = table.Column<bool>(nullable: false),
                    AlertStatus = table.Column<int>(nullable: false),
                    AppliesTo = table.Column<string>(nullable: true),
                    AutoVisualResult = table.Column<string>(nullable: true),
                    ClusterAlertStatus = table.Column<string>(nullable: true),
                    ClusterAlertStatusPriority = table.Column<int>(nullable: false),
                    DefaultCollectorDescription = table.Column<string>(nullable: true),
                    DefaultCollectorId = table.Column<int>(nullable: false),
                    DefaultAgentId = table.Column<int>(nullable: false),
                    AwsDeviceCount = table.Column<int>(nullable: false),
                    AwsRegionsInfo = table.Column<string>(nullable: true),
                    AwsTestResult = table.Column<string>(nullable: true),
                    AwsTestResultCode = table.Column<int>(nullable: false),
                    AzureDeviceCount = table.Column<int>(nullable: false),
                    AzureRegionsInfo = table.Column<string>(nullable: true),
                    AzureTestResult = table.Column<string>(nullable: true),
                    AzureTestResultCode = table.Column<int>(nullable: false),
                    GcpDeviceCount = table.Column<int>(nullable: false),
                    GcpRegionsInfo = table.Column<string>(nullable: true),
                    GcpTestResult = table.Column<string>(nullable: true),
                    GcpTestResultCode = table.Column<int>(nullable: false),
                    IsNetflowEnabled = table.Column<bool>(nullable: false),
                    HasNetflowEnabledDevices = table.Column<bool>(nullable: false),
                    CreatedOnTimestampUtc = table.Column<int>(nullable: true),
                    DeviceCount = table.Column<int>(nullable: false),
                    DeviceGroupType = table.Column<int>(nullable: false),
                    DirectDeviceCount = table.Column<int>(nullable: false),
                    DirectSubGroupCount = table.Column<int>(nullable: false),
                    AlertStatusPriority = table.Column<int>(nullable: false),
                    EffectiveAlertEnabled = table.Column<bool>(nullable: false),
                    FullPath = table.Column<string>(nullable: true),
                    GroupStatus = table.Column<string>(nullable: true),
                    IsAlertingDisabled = table.Column<bool>(nullable: false),
                    ParentId = table.Column<int>(nullable: false),
                    SdtStatus = table.Column<int>(nullable: false),
                    UserPermission = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceGroups", x => x.DatamartId);
                });

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    DatamartId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DatamartCreatedUtc = table.Column<DateTime>(nullable: false),
                    DatamartLastModifiedUtc = table.Column<DateTime>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    AlertDisableStatus = table.Column<int>(nullable: false),
                    AlertStatusPriority = table.Column<int>(nullable: false),
                    AlertStatus = table.Column<int>(nullable: false),
                    AncestorHasDisabledLogicModule = table.Column<bool>(nullable: false),
                    AutoPropertiesAssignedOnSeconds = table.Column<long>(nullable: true),
                    AutoPropertiesUpdatedOnSeconds = table.Column<long>(nullable: true),
                    AwsState = table.Column<int>(nullable: false),
                    AzureState = table.Column<int>(nullable: false),
                    GcpState = table.Column<int>(nullable: false),
                    CanUseRemoteSession = table.Column<bool>(nullable: false),
                    CollectorDescription = table.Column<string>(nullable: true),
                    CreatedOnSeconds = table.Column<long>(nullable: true),
                    CurrentCollectorId = table.Column<int>(nullable: false),
                    DeletedTimeinMs = table.Column<long>(nullable: false),
                    DeviceType = table.Column<int>(nullable: false),
                    IsAlertingDisabled = table.Column<bool>(nullable: false),
                    DisplayName = table.Column<string>(nullable: true),
                    EffectiveAlertEnabled = table.Column<bool>(nullable: false),
                    EnableNetflow = table.Column<bool>(nullable: false),
                    HasActiveInstance = table.Column<bool>(nullable: false),
                    HasDisabledSubResource = table.Column<bool>(nullable: false),
                    HasMore = table.Column<bool>(nullable: false),
                    DeviceGroupIdsString = table.Column<string>(nullable: true),
                    DeviceStatus = table.Column<byte>(nullable: false),
                    LastDataTimeSeconds = table.Column<long>(nullable: true),
                    LastRawDataTimeSeconds = table.Column<long>(nullable: true),
                    Link = table.Column<string>(nullable: true),
                    NetflowCollectorId = table.Column<int>(nullable: false),
                    NetflowCollectorDescription = table.Column<string>(nullable: true),
                    NetflowCollectorGroupId = table.Column<int>(nullable: false),
                    NetflowCollectorGroupName = table.Column<string>(nullable: true),
                    PreferredCollectorId = table.Column<int>(nullable: false),
                    PreferredCollectorGroupId = table.Column<int>(nullable: false),
                    PreferredCollectorGroupName = table.Column<string>(nullable: true),
                    RelatedDeviceId = table.Column<int>(nullable: false),
                    ScanConfigId = table.Column<int>(nullable: false),
                    SdtStatus = table.Column<int>(nullable: false),
                    ToDeleteTimeinMs = table.Column<long>(nullable: false),
                    UptimeInSeconds = table.Column<int>(nullable: false),
                    UpdatedOnSeconds = table.Column<long>(nullable: true),
                    UserPermission = table.Column<int>(nullable: false),
                    LastAlertClosedTimeSeconds = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.DatamartId);
                    table.UniqueConstraint("AK_Devices_Id", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EscalationChains",
                columns: table => new
                {
                    DatamartId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DatamartCreatedUtc = table.Column<DateTime>(nullable: false),
                    DatamartLastModifiedUtc = table.Column<DateTime>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    EnableThrottling = table.Column<bool>(nullable: false),
                    ThrottlingPeriodMinutes = table.Column<int>(nullable: false),
                    ThrottlingAlertCount = table.Column<int>(nullable: false),
                    InAlerting = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EscalationChains", x => x.DatamartId);
                });

            migrationBuilder.CreateTable(
                name: "EventSources",
                columns: table => new
                {
                    DatamartId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DatamartCreatedUtc = table.Column<DateTime>(nullable: false),
                    DatamartLastModifiedUtc = table.Column<DateTime>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventSources", x => x.DatamartId);
                });

            migrationBuilder.CreateTable(
                name: "LogItems",
                columns: table => new
                {
                    DatamartId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DatamartCreatedUtc = table.Column<DateTime>(nullable: false),
                    DatamartLastModifiedUtc = table.Column<DateTime>(nullable: false),
                    Id = table.Column<string>(maxLength: 50, nullable: true),
                    UserName = table.Column<string>(maxLength: 100, nullable: true),
                    HappenedOnTimeStampUtc = table.Column<long>(nullable: false),
                    SessionId = table.Column<string>(maxLength: 50, nullable: true),
                    Description = table.Column<string>(nullable: true),
                    IpAddress = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogItems", x => x.DatamartId);
                });

            migrationBuilder.CreateTable(
                name: "MonitorObjectGroups",
                columns: table => new
                {
                    DatamartId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DatamartCreatedUtc = table.Column<DateTime>(nullable: false),
                    DatamartLastModifiedUtc = table.Column<DateTime>(nullable: false),
                    FullPath = table.Column<string>(maxLength: 200, nullable: true),
                    MonitoredObjectType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitorObjectGroups", x => x.DatamartId);
                });

            migrationBuilder.CreateTable(
                name: "WebsiteGroups",
                columns: table => new
                {
                    DatamartId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DatamartCreatedUtc = table.Column<DateTime>(nullable: false),
                    DatamartLastModifiedUtc = table.Column<DateTime>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    AlertStatus = table.Column<int>(nullable: false),
                    AlertStatusPriority = table.Column<int>(nullable: false),
                    AlertDisableStatus = table.Column<int>(nullable: false),
                    DirectWebsiteCount = table.Column<int>(nullable: false),
                    DirectWebsiteGroupCount = table.Column<int>(nullable: false),
                    DisableAlerting = table.Column<bool>(nullable: false),
                    FullPath = table.Column<string>(nullable: true),
                    GroupStatus = table.Column<int>(nullable: false),
                    HasWebsitesDisabled = table.Column<bool>(nullable: false),
                    ParentId = table.Column<int>(nullable: false),
                    SdtStatus = table.Column<int>(nullable: false),
                    WebsiteCount = table.Column<int>(nullable: false),
                    StopMonitoring = table.Column<bool>(nullable: true),
                    UserPermissionString = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebsiteGroups", x => x.DatamartId);
                    table.UniqueConstraint("AK_WebsiteGroups_Id", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Collectors",
                columns: table => new
                {
                    DatamartId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DatamartCreatedUtc = table.Column<DateTime>(nullable: false),
                    DatamartLastModifiedUtc = table.Column<DateTime>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    AckComment = table.Column<string>(nullable: true),
                    Acked = table.Column<bool>(nullable: false),
                    AckedBy = table.Column<string>(nullable: true),
                    AckedOnLocalString = table.Column<string>(nullable: true),
                    AckedOnUtcTimestampUtc = table.Column<long>(nullable: true),
                    Architecture = table.Column<string>(nullable: true),
                    BackupCollectorId = table.Column<int>(nullable: false),
                    Build = table.Column<int>(nullable: false),
                    CanDowngrade = table.Column<bool>(nullable: false),
                    CanDowngradeReason = table.Column<string>(nullable: true),
                    ClearSent = table.Column<bool>(nullable: false),
                    CollectorConfiguration = table.Column<string>(nullable: true),
                    DeviceId = table.Column<int>(nullable: false),
                    GroupId = table.Column<int>(nullable: false),
                    GroupName = table.Column<string>(nullable: true),
                    Configuration = table.Column<string>(nullable: true),
                    ConfigurationVersion = table.Column<int>(nullable: false),
                    CreatedOnLocalString = table.Column<string>(nullable: true),
                    CreatedOnTimeStampUtc = table.Column<long>(nullable: false),
                    Credential = table.Column<string>(nullable: true),
                    Credential2 = table.Column<string>(nullable: true),
                    DeviceCount = table.Column<int>(nullable: false),
                    Ea = table.Column<bool>(nullable: false),
                    EnableFailBack = table.Column<bool>(nullable: false),
                    EnableFailOverOnCollectorDevice = table.Column<bool>(nullable: false),
                    EscalationChainId = table.Column<int>(nullable: false),
                    HasFailOverDevice = table.Column<bool>(nullable: false),
                    HostName = table.Column<string>(nullable: true),
                    InSdt = table.Column<bool>(nullable: false),
                    IsDown = table.Column<bool>(nullable: false),
                    LastSentNotificationOnLocal = table.Column<string>(nullable: true),
                    LastSentNotificationOnTimeStampUtc = table.Column<int>(nullable: false),
                    NeedAutoCreateCollectorDevice = table.Column<bool>(nullable: false),
                    NetscanVersion = table.Column<int>(nullable: false),
                    NextRecipient = table.Column<int>(nullable: false),
                    OnetimeDowngradeInfo = table.Column<string>(nullable: true),
                    Platform = table.Column<string>(nullable: true),
                    PreviousVersion = table.Column<int>(nullable: false),
                    ProxyConfiguration = table.Column<string>(nullable: true),
                    ResendIntervalSeconds = table.Column<int>(nullable: false),
                    WebsiteConfiguration = table.Column<string>(nullable: true),
                    WebsiteCount = table.Column<int>(nullable: false),
                    Size = table.Column<string>(nullable: true),
                    SpecifiedCollectorDeviceGroupId = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    SuppressAlertClear = table.Column<bool>(nullable: false),
                    UpdatedOnLocalString = table.Column<string>(nullable: true),
                    UpgradeTimeUtcSeconds = table.Column<long>(nullable: false),
                    UpdatedOnTimeStampUtc = table.Column<long>(nullable: true),
                    UptimeSeconds = table.Column<int>(nullable: false),
                    UserChangeOnLocal = table.Column<string>(nullable: true),
                    UserChangeOnUtcSeconds = table.Column<long>(nullable: false),
                    UserPermission = table.Column<int>(nullable: false),
                    UserVisibleDeviceCount = table.Column<int>(nullable: false),
                    UserVisibleWebsiteCount = table.Column<int>(nullable: false),
                    WatchdogConfiguration = table.Column<string>(nullable: true),
                    WatchdogUpdatedOnLocal = table.Column<string>(nullable: true),
                    WatchdogUpdatedOnSeconds = table.Column<long>(nullable: true),
                    WrapperConfiguration = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collectors", x => x.DatamartId);
                    table.ForeignKey(
                        name: "FK_Collectors_CollectorGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "CollectorGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DataSourceDataPoints",
                columns: table => new
                {
                    DatamartId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DatamartCreatedUtc = table.Column<DateTime>(nullable: false),
                    DatamartLastModifiedUtc = table.Column<DateTime>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    DataSourceId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    MeasurementUnit = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataSourceDataPoints", x => x.DatamartId);
                    table.ForeignKey(
                        name: "FK_DataSourceDataPoints_DataSources_DataSourceId",
                        column: x => x.DataSourceId,
                        principalTable: "DataSources",
                        principalColumn: "DatamartId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeviceDataSources",
                columns: table => new
                {
                    DatamartId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DatamartCreatedUtc = table.Column<DateTime>(nullable: false),
                    DatamartLastModifiedUtc = table.Column<DateTime>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    AlertStatusPriority = table.Column<int>(nullable: false),
                    AssignedOnSeconds = table.Column<long>(nullable: false),
                    CreatedOnSeconds = table.Column<long>(nullable: false),
                    DataSourceType = table.Column<string>(nullable: true),
                    UpdatedOnSeconds = table.Column<long>(nullable: false),
                    DataSourceId = table.Column<int>(nullable: false),
                    DataSourceName = table.Column<string>(nullable: true),
                    DataSourceDescription = table.Column<string>(nullable: true),
                    DataSourceDisplayName = table.Column<string>(nullable: true),
                    DeviceId = table.Column<int>(nullable: false),
                    DeviceName = table.Column<string>(nullable: true),
                    DeviceDisplayName = table.Column<string>(nullable: true),
                    GroupName = table.Column<string>(nullable: true),
                    InstanceAutoGroupEnabled = table.Column<bool>(nullable: false),
                    InstanceCount = table.Column<int>(nullable: false),
                    MonitoringInstanceCount = table.Column<int>(nullable: false),
                    NextAutoDiscoveryOnSeconds = table.Column<long>(nullable: false),
                    IsAutoDiscoveryEnabled = table.Column<bool>(nullable: false),
                    IsMonitoringDisabled = table.Column<bool>(nullable: false),
                    IsMultiple = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceDataSources", x => x.DatamartId);
                    table.UniqueConstraint("AK_DeviceDataSources_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceDataSources_DataSources_DataSourceId",
                        column: x => x.DataSourceId,
                        principalTable: "DataSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Alerts",
                columns: table => new
                {
                    DatamartId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DatamartCreatedUtc = table.Column<DateTime>(nullable: false),
                    DatamartLastModifiedUtc = table.Column<DateTime>(nullable: false),
                    Id = table.Column<string>(maxLength: 20, nullable: true),
                    AlertType = table.Column<int>(nullable: false),
                    InternalId = table.Column<string>(maxLength: 20, nullable: true),
                    StartOnSeconds = table.Column<int>(nullable: false),
                    EndOnSeconds = table.Column<int>(nullable: false),
                    Acked = table.Column<bool>(nullable: false),
                    AckedOnSeconds = table.Column<int>(nullable: false),
                    AckedBy = table.Column<string>(maxLength: 50, nullable: true),
                    AckComment = table.Column<string>(maxLength: 50, nullable: true),
                    AlertRuleName = table.Column<string>(maxLength: 50, nullable: true),
                    AlertRuleId = table.Column<int>(nullable: false),
                    AlertEscalationChainName = table.Column<string>(maxLength: 50, nullable: true),
                    AlertEscalationChainId = table.Column<int>(nullable: false),
                    AlertEscalationSubChainId = table.Column<int>(nullable: false),
                    NextRecipient = table.Column<int>(nullable: false),
                    AlertRecipients = table.Column<string>(maxLength: 200, nullable: true),
                    Severity = table.Column<int>(nullable: false),
                    IsCleared = table.Column<bool>(nullable: false),
                    InScheduledDownTime = table.Column<bool>(nullable: false),
                    Value = table.Column<string>(maxLength: 50, nullable: true),
                    Thresholds = table.Column<string>(maxLength: 50, nullable: true),
                    ClearValue = table.Column<string>(maxLength: 50, nullable: true),
                    MonitorObjectType = table.Column<int>(nullable: false),
                    MonitorObjectId = table.Column<int>(nullable: true),
                    MonitorObjectName = table.Column<string>(maxLength: 50, nullable: true),
                    MonitorObjectGroup0Id = table.Column<int>(nullable: true),
                    MonitorObjectGroup1Id = table.Column<int>(nullable: true),
                    MonitorObjectGroup2Id = table.Column<int>(nullable: true),
                    MonitorObjectGroup3Id = table.Column<int>(nullable: true),
                    MonitorObjectGroup4Id = table.Column<int>(nullable: true),
                    MonitorObjectGroup5Id = table.Column<int>(nullable: true),
                    MonitorObjectGroup6Id = table.Column<int>(nullable: true),
                    MonitorObjectGroup7Id = table.Column<int>(nullable: true),
                    MonitorObjectGroup8Id = table.Column<int>(nullable: true),
                    MonitorObjectGroup9Id = table.Column<int>(nullable: true),
                    ResourceId = table.Column<int>(nullable: true),
                    ResourceTemplateId = table.Column<int>(nullable: true),
                    ResourceTemplateType = table.Column<string>(maxLength: 10, nullable: true),
                    ResourceTemplateName = table.Column<string>(maxLength: 50, nullable: true),
                    InstanceId = table.Column<int>(nullable: false),
                    InstanceName = table.Column<string>(maxLength: 50, nullable: true),
                    InstanceDescription = table.Column<string>(maxLength: 1000, nullable: true),
                    DataPointName = table.Column<string>(maxLength: 50, nullable: true),
                    DataPointId = table.Column<int>(nullable: false),
                    DetailMessageSubject = table.Column<string>(maxLength: 200, nullable: true),
                    DetailMessageBody = table.Column<string>(maxLength: 1000, nullable: true),
                    CustomColumn1 = table.Column<string>(maxLength: 50, nullable: true),
                    CustomColumn2 = table.Column<string>(maxLength: 50, nullable: true),
                    CustomColumn3 = table.Column<string>(maxLength: 50, nullable: true),
                    CustomColumn4 = table.Column<string>(maxLength: 50, nullable: true),
                    CustomColumn5 = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerts", x => x.DatamartId);
                    table.ForeignKey(
                        name: "FK_Alerts_AlertRules_AlertRuleId",
                        column: x => x.AlertRuleId,
                        principalTable: "AlertRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Alerts_MonitorObjectGroups_MonitorObjectGroup0Id",
                        column: x => x.MonitorObjectGroup0Id,
                        principalTable: "MonitorObjectGroups",
                        principalColumn: "DatamartId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Alerts_MonitorObjectGroups_MonitorObjectGroup1Id",
                        column: x => x.MonitorObjectGroup1Id,
                        principalTable: "MonitorObjectGroups",
                        principalColumn: "DatamartId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Alerts_MonitorObjectGroups_MonitorObjectGroup2Id",
                        column: x => x.MonitorObjectGroup2Id,
                        principalTable: "MonitorObjectGroups",
                        principalColumn: "DatamartId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Alerts_MonitorObjectGroups_MonitorObjectGroup3Id",
                        column: x => x.MonitorObjectGroup3Id,
                        principalTable: "MonitorObjectGroups",
                        principalColumn: "DatamartId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Alerts_MonitorObjectGroups_MonitorObjectGroup4Id",
                        column: x => x.MonitorObjectGroup4Id,
                        principalTable: "MonitorObjectGroups",
                        principalColumn: "DatamartId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Alerts_MonitorObjectGroups_MonitorObjectGroup5Id",
                        column: x => x.MonitorObjectGroup5Id,
                        principalTable: "MonitorObjectGroups",
                        principalColumn: "DatamartId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Alerts_MonitorObjectGroups_MonitorObjectGroup6Id",
                        column: x => x.MonitorObjectGroup6Id,
                        principalTable: "MonitorObjectGroups",
                        principalColumn: "DatamartId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Alerts_MonitorObjectGroups_MonitorObjectGroup7Id",
                        column: x => x.MonitorObjectGroup7Id,
                        principalTable: "MonitorObjectGroups",
                        principalColumn: "DatamartId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Alerts_MonitorObjectGroups_MonitorObjectGroup8Id",
                        column: x => x.MonitorObjectGroup8Id,
                        principalTable: "MonitorObjectGroups",
                        principalColumn: "DatamartId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Alerts_MonitorObjectGroups_MonitorObjectGroup9Id",
                        column: x => x.MonitorObjectGroup9Id,
                        principalTable: "MonitorObjectGroups",
                        principalColumn: "DatamartId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Websites",
                columns: table => new
                {
                    DatamartId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DatamartCreatedUtc = table.Column<DateTime>(nullable: false),
                    DatamartLastModifiedUtc = table.Column<DateTime>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Count = table.Column<int>(nullable: false),
                    Domain = table.Column<string>(nullable: true),
                    HostName = table.Column<string>(nullable: true),
                    AlertDisableStatus = table.Column<int>(nullable: false),
                    AlertExpression = table.Column<string>(nullable: true),
                    AlertStatus = table.Column<int>(nullable: false),
                    AlertStatusPriority = table.Column<int>(nullable: false),
                    IsAlertingDisabled = table.Column<bool>(nullable: false),
                    GlobalSmAlertCond = table.Column<int>(nullable: false),
                    IndividualSmAlertEnable = table.Column<bool>(nullable: false),
                    IndividualAlertLevel = table.Column<byte>(nullable: false),
                    IgnoreSsl = table.Column<bool>(nullable: false),
                    IsInternal = table.Column<bool>(nullable: false),
                    WebsiteMethod = table.Column<int>(nullable: false),
                    OverallAlertLevel = table.Column<byte>(nullable: false),
                    PageLoadAlertTimeInMs = table.Column<int>(nullable: false),
                    PercentPacketsNotReceiveInTime = table.Column<int>(nullable: false),
                    PacketsNotReceivedTimeoutMs = table.Column<int>(nullable: false),
                    PollingIntervalMinutes = table.Column<int>(nullable: false),
                    Schema = table.Column<string>(nullable: true),
                    Script = table.Column<string>(nullable: true),
                    SdtStatus = table.Column<int>(nullable: false),
                    WebsiteGroupId = table.Column<int>(nullable: false),
                    StopMonitoring = table.Column<bool>(nullable: false),
                    StopMonitoringByWebsiteGroup = table.Column<bool>(nullable: false),
                    TriggerSslStatusAlerts = table.Column<bool>(nullable: false),
                    TriggerSslExpirationAlerts = table.Column<bool>(nullable: false),
                    Transition = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    UseDefaultAlertSetting = table.Column<bool>(nullable: false),
                    UseDefaultLocationSetting = table.Column<bool>(nullable: false),
                    UserPermissionString = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Websites", x => x.DatamartId);
                    table.ForeignKey(
                        name: "FK_Websites_WebsiteGroups_WebsiteGroupId",
                        column: x => x.WebsiteGroupId,
                        principalTable: "WebsiteGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeviceDataSourceInstances",
                columns: table => new
                {
                    DatamartId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DatamartCreatedUtc = table.Column<DateTime>(nullable: false),
                    DatamartLastModifiedUtc = table.Column<DateTime>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    AlertDisableStatus = table.Column<int>(nullable: false),
                    AlertStatus = table.Column<int>(nullable: false),
                    AlertStatusPriority = table.Column<int>(nullable: false),
                    DataSourceId = table.Column<int>(nullable: true),
                    DeviceDisplayName = table.Column<string>(nullable: true),
                    DeviceDataSourceId = table.Column<int>(nullable: false),
                    DeviceId = table.Column<int>(nullable: true),
                    DisableAlerting = table.Column<bool>(nullable: false),
                    DisplayName = table.Column<string>(nullable: true),
                    GroupId = table.Column<int>(nullable: false),
                    GroupName = table.Column<string>(nullable: true),
                    LastCollectedTimeSeconds = table.Column<long>(nullable: false),
                    LastUpdatedTimeSeconds = table.Column<long>(nullable: false),
                    LockDescription = table.Column<bool>(nullable: false),
                    StopMonitoring = table.Column<bool>(nullable: false),
                    SdtStatus = table.Column<int>(nullable: false),
                    SdtAt = table.Column<string>(nullable: true),
                    WildValue = table.Column<string>(nullable: true),
                    WildValue2 = table.Column<string>(nullable: true),
                    LastMeasurementUpdatedTimeSeconds = table.Column<long>(nullable: false),
                    LastAggregationHourWrittenUtc = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceDataSourceInstances", x => x.DatamartId);
                    table.UniqueConstraint("AK_DeviceDataSourceInstances_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceDataSourceInstances_DeviceDataSources_DeviceDataSourceId",
                        column: x => x.DeviceDataSourceId,
                        principalTable: "DeviceDataSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeviceDataSourceInstances_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeviceDataSourceInstanceData",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateTime = table.Column<DateTime>(nullable: false),
                    DeviceDataSourceInstanceId = table.Column<int>(nullable: false),
                    DataPointName = table.Column<string>(nullable: true),
                    Value = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceDataSourceInstanceData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceDataSourceInstanceData_DeviceDataSourceInstances_DeviceDataSourceInstanceId",
                        column: x => x.DeviceDataSourceInstanceId,
                        principalTable: "DeviceDataSourceInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_AlertRuleId",
                table: "Alerts",
                column: "AlertRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_DataPointId",
                table: "Alerts",
                column: "DataPointId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_DataPointName",
                table: "Alerts",
                column: "DataPointName");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_EndOnSeconds",
                table: "Alerts",
                column: "EndOnSeconds");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_Id",
                table: "Alerts",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_InstanceId",
                table: "Alerts",
                column: "InstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_InstanceName",
                table: "Alerts",
                column: "InstanceName");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_InternalId",
                table: "Alerts",
                column: "InternalId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_IsCleared",
                table: "Alerts",
                column: "IsCleared");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_MonitorObjectGroup0Id",
                table: "Alerts",
                column: "MonitorObjectGroup0Id");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_MonitorObjectGroup1Id",
                table: "Alerts",
                column: "MonitorObjectGroup1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_MonitorObjectGroup2Id",
                table: "Alerts",
                column: "MonitorObjectGroup2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_MonitorObjectGroup3Id",
                table: "Alerts",
                column: "MonitorObjectGroup3Id");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_MonitorObjectGroup4Id",
                table: "Alerts",
                column: "MonitorObjectGroup4Id");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_MonitorObjectGroup5Id",
                table: "Alerts",
                column: "MonitorObjectGroup5Id");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_MonitorObjectGroup6Id",
                table: "Alerts",
                column: "MonitorObjectGroup6Id");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_MonitorObjectGroup7Id",
                table: "Alerts",
                column: "MonitorObjectGroup7Id");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_MonitorObjectGroup8Id",
                table: "Alerts",
                column: "MonitorObjectGroup8Id");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_MonitorObjectGroup9Id",
                table: "Alerts",
                column: "MonitorObjectGroup9Id");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_MonitorObjectId",
                table: "Alerts",
                column: "MonitorObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_MonitorObjectName",
                table: "Alerts",
                column: "MonitorObjectName");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_MonitorObjectType",
                table: "Alerts",
                column: "MonitorObjectType");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_ResourceId",
                table: "Alerts",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_ResourceTemplateId",
                table: "Alerts",
                column: "ResourceTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_ResourceTemplateName",
                table: "Alerts",
                column: "ResourceTemplateName");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_ResourceTemplateType",
                table: "Alerts",
                column: "ResourceTemplateType");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_Severity",
                table: "Alerts",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_StartOnSeconds",
                table: "Alerts",
                column: "StartOnSeconds");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_FasterPercentageAvailability",
                table: "Alerts",
                columns: new[] { "StartOnSeconds", "EndOnSeconds", "IsCleared", "InScheduledDownTime", "MonitorObjectGroup0Id", "MonitorObjectGroup1Id", "MonitorObjectGroup2Id", "MonitorObjectGroup3Id", "MonitorObjectGroup4Id", "MonitorObjectGroup5Id", "MonitorObjectGroup6Id", "MonitorObjectGroup7Id", "MonitorObjectGroup8Id", "MonitorObjectGroup9Id" })
                .Annotation("SqlServer:Include", new[] { "Id", "Severity", "ClearValue", "MonitorObjectId", "ResourceTemplateName", "InstanceId", "InstanceName" });

            migrationBuilder.CreateIndex(
                name: "IX_Collectors_GroupId",
                table: "Collectors",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_DataSourceDataPoints_DataSourceId",
                table: "DataSourceDataPoints",
                column: "DataSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceDataSourceInstanceData_DateTime",
                table: "DeviceDataSourceInstanceData",
                column: "DateTime");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceDataSourceInstanceData_DeviceDataSourceInstanceId_DataPointName",
                table: "DeviceDataSourceInstanceData",
                columns: new[] { "DeviceDataSourceInstanceId", "DataPointName" });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceDataSourceInstances_DeviceDataSourceId",
                table: "DeviceDataSourceInstances",
                column: "DeviceDataSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceDataSourceInstances_DeviceId",
                table: "DeviceDataSourceInstances",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceDataSources_DataSourceId",
                table: "DeviceDataSources",
                column: "DataSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_MonitorObjectGroups_FullPath_MonitoredObjectType",
                table: "MonitorObjectGroups",
                columns: new[] { "FullPath", "MonitoredObjectType" });

            migrationBuilder.CreateIndex(
                name: "IX_Websites_WebsiteGroupId",
                table: "Websites",
                column: "WebsiteGroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alerts");

            migrationBuilder.DropTable(
                name: "Collectors");

            migrationBuilder.DropTable(
                name: "ConfigSources");

            migrationBuilder.DropTable(
                name: "DataSourceDataPoints");

            migrationBuilder.DropTable(
                name: "DeviceDataSourceInstanceData");

            migrationBuilder.DropTable(
                name: "DeviceGroups");

            migrationBuilder.DropTable(
                name: "EscalationChains");

            migrationBuilder.DropTable(
                name: "EventSources");

            migrationBuilder.DropTable(
                name: "LogItems");

            migrationBuilder.DropTable(
                name: "Websites");

            migrationBuilder.DropTable(
                name: "AlertRules");

            migrationBuilder.DropTable(
                name: "MonitorObjectGroups");

            migrationBuilder.DropTable(
                name: "CollectorGroups");

            migrationBuilder.DropTable(
                name: "DeviceDataSourceInstances");

            migrationBuilder.DropTable(
                name: "WebsiteGroups");

            migrationBuilder.DropTable(
                name: "DeviceDataSources");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "DataSources");
        }
    }
}
