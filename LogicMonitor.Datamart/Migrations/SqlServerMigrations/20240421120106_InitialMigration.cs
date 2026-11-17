#nullable disable

namespace LogicMonitor.Datamart.Migrations.SqlServerMigrations;

/// <inheritdoc />
public partial class InitialMigration : Migration
{
	private static readonly string[] _includesAlertsFasterPercentageAvailabilityIndexedColumns = ["Id", "Severity", "ClearValue", "MonitorObjectId", "ResourceTemplateName", "InstanceId", "InstanceName"];
	private static readonly string[] _alertsFasterPercentageAvailabilityIndexedColumns = ["StartOnSeconds", "EndOnSeconds", "IsCleared", "InScheduledDownTime", "MonitorObjectGroup0Id", "MonitorObjectGroup1Id", "MonitorObjectGroup2Id", "MonitorObjectGroup3Id", "MonitorObjectGroup4Id", "MonitorObjectGroup5Id", "MonitorObjectGroup6Id", "MonitorObjectGroup7Id", "MonitorObjectGroup8Id", "MonitorObjectGroup9Id"];
	private static readonly string[] _monitorObjectGroupsIndexColumns = ["FullPath", "MonitoredObjectType"];

	/// <inheritdoc />
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.CreateTable(
			name: "CollectorGroups",
			columns: table => new
			{
				Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
				Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
				CreatedOnTimeStampSeconds = table.Column<long>(type: "bigint", nullable: false),
				CollectorCount = table.Column<int>(type: "int", nullable: false),
				DatamartCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				DatamartLastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				LogicMonitorId = table.Column<int>(type: "int", nullable: false),
				DatamartLastObserved = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_CollectorGroups", x => x.Id);
			});

		migrationBuilder.CreateTable(
			name: "ConfigSources",
			columns: table => new
			{
				Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
				Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
				DatamartCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				DatamartLastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				LogicMonitorId = table.Column<int>(type: "int", nullable: false),
				DatamartLastObserved = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_ConfigSources", x => x.Id);
			});

		migrationBuilder.CreateTable(
			name: "DataSources",
			columns: table => new
			{
				Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
				Group = table.Column<string>(type: "nvarchar(max)", nullable: false),
				AppliesTo = table.Column<string>(type: "nvarchar(max)", nullable: false),
				Technology = table.Column<string>(type: "nvarchar(max)", nullable: false),
				Tags = table.Column<string>(type: "nvarchar(max)", nullable: false),
				Checksum = table.Column<string>(type: "nvarchar(max)", nullable: false),
				LineageId = table.Column<string>(type: "nvarchar(max)", nullable: false),
				InstallationMetadataOriginRegistryId = table.Column<string>(type: "nvarchar(max)", nullable: true),
				InstallationMetadataOriginLineageId = table.Column<string>(type: "nvarchar(max)", nullable: true),
				InstallationMetadataOriginAuthorCompanyUuid = table.Column<string>(type: "nvarchar(max)", nullable: true),
				InstallationMetadataOriginAuthorNamespace = table.Column<string>(type: "nvarchar(max)", nullable: true),
				InstallationMetadataOriginVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
				InstallationMetadataOriginChecksum = table.Column<string>(type: "nvarchar(max)", nullable: true),
				InstallationMetadataAuditedRegistryId = table.Column<string>(type: "nvarchar(max)", nullable: true),
				InstallationMetadataAuditedVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
				InstallationMetadataTargetLineageId = table.Column<string>(type: "nvarchar(max)", nullable: true),
				InstallationMetadataTargetLastPublishedId = table.Column<string>(type: "nvarchar(max)", nullable: true),
				InstallationMetadataTargetLastPublishedVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
				InstallationMetadataTargetLastPublishedChecksum = table.Column<string>(type: "nvarchar(max)", nullable: true),
				InstallationMetadataLogicModuleType = table.Column<string>(type: "nvarchar(max)", nullable: true),
				InstallationMetadataLogicModuleId = table.Column<int>(type: "int", nullable: true),
				InstallationMetadataIsChangedFromOrigin = table.Column<bool>(type: "bit", nullable: true),
				InstallationMetadataIsChangedFromTargetLastPublished = table.Column<bool>(type: "bit", nullable: true),
				Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
				DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
				Version = table.Column<string>(type: "nvarchar(max)", nullable: false),
				AuditVersion = table.Column<string>(type: "nvarchar(max)", nullable: false),
				PayloadVersion = table.Column<string>(type: "nvarchar(max)", nullable: false),
				HasMultiInstances = table.Column<bool>(type: "bit", nullable: false),
				UseWildValueAsUuid = table.Column<bool>(type: "bit", nullable: false),
				PollingIntervalSeconds = table.Column<int>(type: "int", nullable: false),
				CollectionMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
				CollectionAttributeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
				CollectionAttributeIp = table.Column<string>(type: "nvarchar(max)", nullable: true),
				LastTimeSeriesDataSyncDurationMs = table.Column<long>(type: "bigint", nullable: true),
				DatamartCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				DatamartLastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				LogicMonitorId = table.Column<int>(type: "int", nullable: false),
				DatamartLastObserved = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_DataSources", x => x.Id);
			});

		migrationBuilder.CreateTable(
			name: "DeviceGroups",
			columns: table => new
			{
				Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
				Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
				AlertDisableStatus = table.Column<int>(type: "int", nullable: false),
				AlertEnable = table.Column<bool>(type: "bit", nullable: false),
				AlertStatus = table.Column<int>(type: "int", nullable: false),
				AppliesTo = table.Column<string>(type: "nvarchar(max)", nullable: false),
				AutoVisualResult = table.Column<string>(type: "nvarchar(max)", nullable: true),
				ClusterAlertStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
				ClusterAlertStatusPriority = table.Column<int>(type: "int", nullable: false),
				DefaultCollectorDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
				DefaultCollectorId = table.Column<int>(type: "int", nullable: false),
				DefaultAgentId = table.Column<int>(type: "int", nullable: false),
				AwsDeviceCount = table.Column<int>(type: "int", nullable: false),
				AwsRegionsInfo = table.Column<string>(type: "nvarchar(max)", nullable: false),
				AwsTestResult = table.Column<string>(type: "nvarchar(max)", nullable: true),
				AwsTestResultCode = table.Column<int>(type: "int", nullable: false),
				AzureDeviceCount = table.Column<int>(type: "int", nullable: false),
				AzureRegionsInfo = table.Column<string>(type: "nvarchar(max)", nullable: false),
				AzureTestResult = table.Column<string>(type: "nvarchar(max)", nullable: true),
				AzureTestResultCode = table.Column<int>(type: "int", nullable: false),
				GcpDeviceCount = table.Column<int>(type: "int", nullable: false),
				GcpRegionsInfo = table.Column<string>(type: "nvarchar(max)", nullable: false),
				GcpTestResult = table.Column<string>(type: "nvarchar(max)", nullable: true),
				GcpTestResultCode = table.Column<int>(type: "int", nullable: false),
				IsNetflowEnabled = table.Column<bool>(type: "bit", nullable: false),
				HasNetflowEnabledDevices = table.Column<bool>(type: "bit", nullable: false),
				CreatedOnTimestampUtc = table.Column<int>(type: "int", nullable: true),
				DeviceCount = table.Column<int>(type: "int", nullable: false),
				DeviceGroupType = table.Column<int>(type: "int", nullable: false),
				DirectDeviceCount = table.Column<int>(type: "int", nullable: false),
				DirectSubGroupCount = table.Column<int>(type: "int", nullable: false),
				AlertStatusPriority = table.Column<int>(type: "int", nullable: false),
				EffectiveAlertEnabled = table.Column<bool>(type: "bit", nullable: false),
				FullPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
				GroupStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
				IsAlertingDisabled = table.Column<bool>(type: "bit", nullable: false),
				ParentId = table.Column<int>(type: "int", nullable: false),
				SdtStatus = table.Column<int>(type: "int", nullable: false),
				UserPermission = table.Column<int>(type: "int", nullable: false),
				DatamartCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				DatamartLastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				LogicMonitorId = table.Column<int>(type: "int", nullable: false),
				DatamartLastObserved = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_DeviceGroups", x => x.Id);
			});

		migrationBuilder.CreateTable(
			name: "EscalationChains",
			columns: table => new
			{
				Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
				Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
				EnableThrottling = table.Column<bool>(type: "bit", nullable: false),
				ThrottlingPeriodMinutes = table.Column<int>(type: "int", nullable: false),
				ThrottlingAlertCount = table.Column<int>(type: "int", nullable: false),
				InAlerting = table.Column<bool>(type: "bit", nullable: false),
				DatamartCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				DatamartLastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				LogicMonitorId = table.Column<int>(type: "int", nullable: false),
				DatamartLastObserved = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_EscalationChains", x => x.Id);
			});

		migrationBuilder.CreateTable(
			name: "EventSources",
			columns: table => new
			{
				Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
				Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
				DatamartCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				DatamartLastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				LogicMonitorId = table.Column<int>(type: "int", nullable: false),
				DatamartLastObserved = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_EventSources", x => x.Id);
			});

		migrationBuilder.CreateTable(
			name: "LogicModuleUpdates",
			columns: table => new
			{
				Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				LocalId = table.Column<int>(type: "int", nullable: false),
				Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
				AppliesTo = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
				Category = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
				Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
				CollectionMethod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
				Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
				Group = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
				Version = table.Column<long>(type: "bigint", nullable: false),
				LocalVersion = table.Column<long>(type: "bigint", nullable: false),
				AuditVersion = table.Column<long>(type: "bigint", nullable: false),
				RestLm = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
				RegistryVersion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
				PublishedAtMilliseconds = table.Column<long>(type: "bigint", nullable: false),
				Quality = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
				Locator = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
				CurrentUuid = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
				Namespace = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
				Local = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
				Remote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
				DatamartLastObserved = table.Column<DateTime>(type: "datetime2", nullable: false),
				DatamartCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				DatamartLastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_LogicModuleUpdates", x => x.Id);
			});

		migrationBuilder.CreateTable(
			name: "LogItems",
			columns: table => new
			{
				Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				LogicMonitorId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
				UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
				HappenedOnTimeStampUtc = table.Column<long>(type: "bigint", nullable: false),
				SessionId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
				Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
				IpAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
				DatamartCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				DatamartLastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_LogItems", x => x.Id);
			});

		migrationBuilder.CreateTable(
			name: "MonitorObjectGroups",
			columns: table => new
			{
				Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				FullPath = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
				MonitoredObjectType = table.Column<int>(type: "int", nullable: false),
				DatamartCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				DatamartLastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_MonitorObjectGroups", x => x.Id);
			});

		migrationBuilder.CreateTable(
			name: "WebsiteGroups",
			columns: table => new
			{
				Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
				Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
				AlertStatus = table.Column<int>(type: "int", nullable: false),
				AlertStatusPriority = table.Column<int>(type: "int", nullable: false),
				AlertDisableStatus = table.Column<int>(type: "int", nullable: false),
				DirectWebsiteCount = table.Column<int>(type: "int", nullable: false),
				DirectWebsiteGroupCount = table.Column<int>(type: "int", nullable: false),
				DisableAlerting = table.Column<bool>(type: "bit", nullable: false),
				FullPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
				GroupStatus = table.Column<int>(type: "int", nullable: false),
				HasWebsitesDisabled = table.Column<bool>(type: "bit", nullable: false),
				ParentId = table.Column<int>(type: "int", nullable: false),
				SdtStatus = table.Column<int>(type: "int", nullable: false),
				WebsiteCount = table.Column<int>(type: "int", nullable: false),
				StopMonitoring = table.Column<bool>(type: "bit", nullable: true),
				UserPermissionString = table.Column<int>(type: "int", nullable: false),
				DatamartCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				DatamartLastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				LogicMonitorId = table.Column<int>(type: "int", nullable: false),
				DatamartLastObserved = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_WebsiteGroups", x => x.Id);
			});

		migrationBuilder.CreateTable(
			name: "Collectors",
			columns: table => new
			{
				Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				CollectorGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
				Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
				AckComment = table.Column<string>(type: "nvarchar(max)", nullable: false),
				Acked = table.Column<bool>(type: "bit", nullable: false),
				AckedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
				AckedOnLocalString = table.Column<string>(type: "nvarchar(max)", nullable: false),
				AckedOnUtcTimestampUtc = table.Column<long>(type: "bigint", nullable: true),
				Architecture = table.Column<string>(type: "nvarchar(max)", nullable: false),
				BackupCollectorId = table.Column<int>(type: "int", nullable: false),
				Build = table.Column<int>(type: "int", nullable: false),
				CanDowngrade = table.Column<bool>(type: "bit", nullable: false),
				CanDowngradeReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
				ClearSent = table.Column<bool>(type: "bit", nullable: false),
				CollectorConfiguration = table.Column<string>(type: "nvarchar(max)", nullable: false),
				Configuration = table.Column<string>(type: "nvarchar(max)", nullable: true),
				ConfigurationVersion = table.Column<int>(type: "int", nullable: false),
				CreatedOnLocalString = table.Column<string>(type: "nvarchar(max)", nullable: false),
				CreatedOnTimeStampUtc = table.Column<long>(type: "bigint", nullable: false),
				Credential = table.Column<string>(type: "nvarchar(max)", nullable: true),
				Credential2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
				DeviceCount = table.Column<int>(type: "int", nullable: false),
				Ea = table.Column<bool>(type: "bit", nullable: false),
				EnableFailBack = table.Column<bool>(type: "bit", nullable: false),
				EnableFailOverOnCollectorDevice = table.Column<bool>(type: "bit", nullable: false),
				EscalationChainId = table.Column<int>(type: "int", nullable: false),
				HasFailOverDevice = table.Column<bool>(type: "bit", nullable: false),
				HostName = table.Column<string>(type: "nvarchar(max)", nullable: false),
				InSdt = table.Column<bool>(type: "bit", nullable: false),
				IsDown = table.Column<bool>(type: "bit", nullable: false),
				IsEncoded = table.Column<bool>(type: "bit", nullable: false),
				IsLmLogsEnabled = table.Column<bool>(type: "bit", nullable: false),
				IsLmLogsSyslogEnabled = table.Column<bool>(type: "bit", nullable: false),
				LastSentNotificationOnLocal = table.Column<string>(type: "nvarchar(max)", nullable: false),
				LastSentNotificationOnTimeStampUtc = table.Column<int>(type: "int", nullable: false),
				LogicMonitorDeviceId = table.Column<int>(type: "int", nullable: false),
				NeedAutoCreateCollectorDevice = table.Column<bool>(type: "bit", nullable: false),
				NetscanVersion = table.Column<int>(type: "int", nullable: false),
				NextRecipient = table.Column<int>(type: "int", nullable: false),
				OnetimeDowngradeInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
				OtelVerison = table.Column<string>(type: "nvarchar(max)", nullable: false),
				Platform = table.Column<string>(type: "nvarchar(max)", nullable: false),
				PreviousVersion = table.Column<int>(type: "int", nullable: false),
				ProxyConfiguration = table.Column<string>(type: "nvarchar(max)", nullable: false),
				ResendIntervalSeconds = table.Column<int>(type: "int", nullable: false),
				WebsiteConfiguration = table.Column<string>(type: "nvarchar(max)", nullable: false),
				WebsiteCount = table.Column<int>(type: "int", nullable: false),
				Size = table.Column<string>(type: "nvarchar(max)", nullable: false),
				SpecifiedCollectorDeviceGroupId = table.Column<int>(type: "int", nullable: false),
				Status = table.Column<int>(type: "int", nullable: false),
				SuppressAlertClear = table.Column<bool>(type: "bit", nullable: false),
				UpdatedOnLocalString = table.Column<string>(type: "nvarchar(max)", nullable: false),
				UpgradeTimeUtcSeconds = table.Column<long>(type: "bigint", nullable: false),
				UpdatedOnTimeStampUtc = table.Column<long>(type: "bigint", nullable: true),
				UptimeSeconds = table.Column<int>(type: "int", nullable: false),
				UserChangeOnLocal = table.Column<string>(type: "nvarchar(max)", nullable: false),
				UserChangeOnUtcSeconds = table.Column<long>(type: "bigint", nullable: false),
				UserPermission = table.Column<int>(type: "int", nullable: false),
				UserVisibleDeviceCount = table.Column<int>(type: "int", nullable: false),
				UserVisibleWebsiteCount = table.Column<int>(type: "int", nullable: false),
				WatchdogConfiguration = table.Column<string>(type: "nvarchar(max)", nullable: false),
				WatchdogUpdatedOnLocal = table.Column<string>(type: "nvarchar(max)", nullable: false),
				WatchdogUpdatedOnSeconds = table.Column<long>(type: "bigint", nullable: true),
				WrapperConfiguration = table.Column<string>(type: "nvarchar(max)", nullable: false),
				DatamartCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				DatamartLastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				LogicMonitorId = table.Column<int>(type: "int", nullable: false),
				DatamartLastObserved = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_Collectors", x => x.Id);
				table.ForeignKey(
					name: "FK_Collectors_CollectorGroups_CollectorGroupId",
					column: x => x.CollectorGroupId,
					principalTable: "CollectorGroups",
					principalColumn: "Id",
					onDelete: ReferentialAction.Restrict);
			});

		migrationBuilder.CreateTable(
			name: "DataSourceDataPoints",
			columns: table => new
			{
				Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				DataSourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
				Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
				MeasurementUnit = table.Column<string>(type: "nvarchar(max)", nullable: false),
				GlobalAlertExpression = table.Column<string>(type: "nvarchar(max)", nullable: false),
				Calculation = table.Column<string>(type: "nvarchar(max)", nullable: false),
				PercentageAvailabilityCalculation = table.Column<string>(type: "nvarchar(max)", nullable: false),
				Tags = table.Column<string>(type: "nvarchar(max)", nullable: false),
				Property1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
				Property2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
				Property3 = table.Column<string>(type: "nvarchar(max)", nullable: false),
				Property4 = table.Column<string>(type: "nvarchar(max)", nullable: false),
				Property5 = table.Column<string>(type: "nvarchar(max)", nullable: false),
				Property6 = table.Column<string>(type: "nvarchar(max)", nullable: false),
				Property7 = table.Column<string>(type: "nvarchar(max)", nullable: false),
				Property8 = table.Column<string>(type: "nvarchar(max)", nullable: false),
				Property9 = table.Column<string>(type: "nvarchar(max)", nullable: false),
				Property10 = table.Column<string>(type: "nvarchar(max)", nullable: false),
				ResyncTimeSeriesData = table.Column<bool>(type: "bit", nullable: false),
				DatamartCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				DatamartLastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				LogicMonitorId = table.Column<int>(type: "int", nullable: false),
				DatamartLastObserved = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_DataSourceDataPoints", x => x.Id);
				table.ForeignKey(
					name: "FK_DataSourceDataPoints_DataSources_DataSourceId",
					column: x => x.DataSourceId,
					principalTable: "DataSources",
					principalColumn: "Id",
					onDelete: ReferentialAction.Cascade);
			});

		migrationBuilder.CreateTable(
			name: "AlertRules",
			columns: table => new
			{
				Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				EscalationChainId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
				Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
				Priority = table.Column<int>(type: "int", nullable: false),
				LevelString = table.Column<string>(type: "nvarchar(max)", nullable: false),
				DataSourceName = table.Column<string>(type: "nvarchar(max)", nullable: false),
				DataSourceInstanceName = table.Column<string>(type: "nvarchar(max)", nullable: false),
				DataPoint = table.Column<string>(type: "nvarchar(max)", nullable: false),
				EscalationChainIntervalMinutes = table.Column<int>(type: "int", nullable: false),
				SuppressAlertClear = table.Column<bool>(type: "bit", nullable: false),
				SuppressAlertAckSdt = table.Column<bool>(type: "bit", nullable: false),
				DatamartCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				DatamartLastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				LogicMonitorId = table.Column<int>(type: "int", nullable: false),
				DatamartLastObserved = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_AlertRules", x => x.Id);
				table.ForeignKey(
					name: "FK_AlertRules_EscalationChains_EscalationChainId",
					column: x => x.EscalationChainId,
					principalTable: "EscalationChains",
					principalColumn: "Id",
					onDelete: ReferentialAction.Cascade);
			});

		migrationBuilder.CreateTable(
			name: "Websites",
			columns: table => new
			{
				Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				WebsiteGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
				Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
				Count = table.Column<int>(type: "int", nullable: false),
				Domain = table.Column<string>(type: "nvarchar(max)", nullable: true),
				HostName = table.Column<string>(type: "nvarchar(max)", nullable: true),
				AlertDisableStatus = table.Column<int>(type: "int", nullable: false),
				AlertExpression = table.Column<string>(type: "nvarchar(max)", nullable: true),
				AlertStatus = table.Column<int>(type: "int", nullable: false),
				AlertStatusPriority = table.Column<int>(type: "int", nullable: false),
				IsAlertingDisabled = table.Column<bool>(type: "bit", nullable: false),
				GlobalSmAlertCond = table.Column<int>(type: "int", nullable: false),
				IndividualSmAlertEnable = table.Column<bool>(type: "bit", nullable: false),
				IndividualAlertLevel = table.Column<byte>(type: "tinyint", nullable: false),
				IgnoreSsl = table.Column<bool>(type: "bit", nullable: false),
				IsInternal = table.Column<bool>(type: "bit", nullable: false),
				WebsiteMethod = table.Column<int>(type: "int", nullable: false),
				OverallAlertLevel = table.Column<byte>(type: "tinyint", nullable: false),
				PageLoadAlertTimeInMs = table.Column<int>(type: "int", nullable: false),
				PercentPacketsNotReceiveInTime = table.Column<int>(type: "int", nullable: false),
				PacketsNotReceivedTimeoutMs = table.Column<int>(type: "int", nullable: false),
				PollingIntervalMinutes = table.Column<int>(type: "int", nullable: false),
				Schema = table.Column<string>(type: "nvarchar(max)", nullable: false),
				Script = table.Column<string>(type: "nvarchar(max)", nullable: true),
				SdtStatus = table.Column<int>(type: "int", nullable: false),
				StopMonitoring = table.Column<bool>(type: "bit", nullable: false),
				StopMonitoringByWebsiteGroup = table.Column<bool>(type: "bit", nullable: false),
				TriggerSslStatusAlerts = table.Column<bool>(type: "bit", nullable: false),
				TriggerSslExpirationAlerts = table.Column<bool>(type: "bit", nullable: false),
				Transition = table.Column<int>(type: "int", nullable: false),
				Type = table.Column<int>(type: "int", nullable: false),
				UseDefaultAlertSetting = table.Column<bool>(type: "bit", nullable: false),
				UseDefaultLocationSetting = table.Column<bool>(type: "bit", nullable: false),
				UserPermissionString = table.Column<int>(type: "int", nullable: false),
				Status = table.Column<int>(type: "int", nullable: false),
				DatamartCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				DatamartLastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				LogicMonitorId = table.Column<int>(type: "int", nullable: false),
				DatamartLastObserved = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_Websites", x => x.Id);
				table.ForeignKey(
					name: "FK_Websites_WebsiteGroups_WebsiteGroupId",
					column: x => x.WebsiteGroupId,
					principalTable: "WebsiteGroups",
					principalColumn: "Id",
					onDelete: ReferentialAction.Restrict);
			});

		migrationBuilder.CreateTable(
			name: "Devices",
			columns: table => new
			{
				Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				PreferredCollectorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
				Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
				Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
				AlertDisableStatus = table.Column<int>(type: "int", nullable: false),
				AlertStatusPriority = table.Column<int>(type: "int", nullable: false),
				AlertStatus = table.Column<int>(type: "int", nullable: false),
				AncestorHasDisabledLogicModule = table.Column<bool>(type: "bit", nullable: false),
				AutoPropertiesAssignedOnSeconds = table.Column<long>(type: "bigint", nullable: true),
				AutoPropertiesUpdatedOnSeconds = table.Column<long>(type: "bigint", nullable: true),
				AwsState = table.Column<int>(type: "int", nullable: false),
				AzureState = table.Column<int>(type: "int", nullable: false),
				GcpState = table.Column<int>(type: "int", nullable: false),
				CanUseRemoteSession = table.Column<bool>(type: "bit", nullable: false),
				CollectorDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
				CreatedOnSeconds = table.Column<long>(type: "bigint", nullable: true),
				CurrentCollectorId = table.Column<int>(type: "int", nullable: false),
				DeletedTimeinMs = table.Column<long>(type: "bigint", nullable: false),
				DeviceType = table.Column<int>(type: "int", nullable: false),
				IsAlertingDisabled = table.Column<bool>(type: "bit", nullable: false),
				DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
				EffectiveAlertEnabled = table.Column<bool>(type: "bit", nullable: false),
				EnableNetflow = table.Column<bool>(type: "bit", nullable: false),
				HasActiveInstance = table.Column<bool>(type: "bit", nullable: false),
				HasDisabledSubResource = table.Column<bool>(type: "bit", nullable: false),
				HasMore = table.Column<bool>(type: "bit", nullable: false),
				DeviceGroupIdsString = table.Column<string>(type: "nvarchar(max)", nullable: false),
				DeviceStatus = table.Column<byte>(type: "tinyint", nullable: false),
				LastDataTimeSeconds = table.Column<long>(type: "bigint", nullable: true),
				LastRawDataTimeSeconds = table.Column<long>(type: "bigint", nullable: true),
				Link = table.Column<string>(type: "nvarchar(max)", nullable: true),
				NetflowCollectorId = table.Column<int>(type: "int", nullable: false),
				NetflowCollectorDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
				NetflowCollectorGroupId = table.Column<int>(type: "int", nullable: false),
				NetflowCollectorGroupName = table.Column<string>(type: "nvarchar(max)", nullable: true),
				RelatedDeviceId = table.Column<int>(type: "int", nullable: false),
				ScanConfigId = table.Column<int>(type: "int", nullable: false),
				SdtStatus = table.Column<int>(type: "int", nullable: false),
				ToDeleteTimeinMs = table.Column<long>(type: "bigint", nullable: false),
				UptimeInSeconds = table.Column<int>(type: "int", nullable: false),
				UpdatedOnSeconds = table.Column<long>(type: "bigint", nullable: true),
				UserPermission = table.Column<int>(type: "int", nullable: false),
				LastAlertClosedTimeSeconds = table.Column<long>(type: "bigint", nullable: false),
				Property1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
				Property2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
				Property3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
				Property4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
				Property5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
				Property6 = table.Column<string>(type: "nvarchar(max)", nullable: true),
				Property7 = table.Column<string>(type: "nvarchar(max)", nullable: true),
				Property8 = table.Column<string>(type: "nvarchar(max)", nullable: true),
				Property9 = table.Column<string>(type: "nvarchar(max)", nullable: true),
				Property10 = table.Column<string>(type: "nvarchar(max)", nullable: true),
				Property11 = table.Column<string>(type: "nvarchar(max)", nullable: true),
				Property12 = table.Column<string>(type: "nvarchar(max)", nullable: true),
				Property13 = table.Column<string>(type: "nvarchar(max)", nullable: true),
				Property14 = table.Column<string>(type: "nvarchar(max)", nullable: true),
				Property15 = table.Column<string>(type: "nvarchar(max)", nullable: true),
				Property16 = table.Column<string>(type: "nvarchar(max)", nullable: true),
				Property17 = table.Column<string>(type: "nvarchar(max)", nullable: true),
				Property18 = table.Column<string>(type: "nvarchar(max)", nullable: true),
				Property19 = table.Column<string>(type: "nvarchar(max)", nullable: true),
				Property20 = table.Column<string>(type: "nvarchar(max)", nullable: true),
				LastTimeSeriesDataSyncDurationMs = table.Column<long>(type: "bigint", nullable: true),
				DatamartCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				DatamartLastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				LogicMonitorId = table.Column<int>(type: "int", nullable: false),
				DatamartLastObserved = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_Devices", x => x.Id);
				table.ForeignKey(
					name: "FK_Devices_Collectors_PreferredCollectorId",
					column: x => x.PreferredCollectorId,
					principalTable: "Collectors",
					principalColumn: "Id");
			});

		migrationBuilder.CreateTable(
			name: "Alerts",
			columns: table => new
			{
				Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				AlertRuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
				LogicMonitorId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
				AlertType = table.Column<int>(type: "int", nullable: false),
				InternalId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
				StartOnSeconds = table.Column<int>(type: "int", nullable: false),
				EndOnSeconds = table.Column<int>(type: "int", nullable: false),
				Acked = table.Column<bool>(type: "bit", nullable: false),
				AckedOnSeconds = table.Column<int>(type: "int", nullable: false),
				AckedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
				AckComment = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
				AlertRuleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
				AlertEscalationChainName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
				AlertEscalationChainId = table.Column<int>(type: "int", nullable: true),
				AlertEscalationSubChainId = table.Column<int>(type: "int", nullable: true),
				NextRecipient = table.Column<int>(type: "int", nullable: false),
				AlertRecipients = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
				Severity = table.Column<int>(type: "int", nullable: false),
				IsCleared = table.Column<bool>(type: "bit", nullable: false),
				InScheduledDownTime = table.Column<bool>(type: "bit", nullable: false),
				Value = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
				Thresholds = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
				ClearValue = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
				MonitorObjectType = table.Column<int>(type: "int", nullable: false),
				MonitorObjectId = table.Column<int>(type: "int", nullable: true),
				MonitorObjectName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
				MonitorObjectGroup0Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
				MonitorObjectGroup1Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
				MonitorObjectGroup2Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
				MonitorObjectGroup3Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
				MonitorObjectGroup4Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
				MonitorObjectGroup5Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
				MonitorObjectGroup6Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
				MonitorObjectGroup7Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
				MonitorObjectGroup8Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
				MonitorObjectGroup9Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
				ResourceId = table.Column<int>(type: "int", nullable: true),
				ResourceTemplateId = table.Column<int>(type: "int", nullable: true),
				ResourceTemplateType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
				ResourceTemplateName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
				InstanceId = table.Column<int>(type: "int", nullable: false),
				InstanceName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
				InstanceDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
				DataPointName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
				DataPointId = table.Column<int>(type: "int", nullable: false),
				DetailMessageSubject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
				DetailMessageBody = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
				CustomColumn1 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
				CustomColumn2 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
				CustomColumn3 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
				CustomColumn4 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
				CustomColumn5 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
				EnableAnomalyAlertSuppression = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
				EnableAnomalyAlertGeneration = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
				Tenant = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
				DependencyRole = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
				DependencyRoutingState = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
				IsActiveDiscoveryAlert = table.Column<bool>(type: "bit", maxLength: 50, nullable: false),
				ActiveDiscoveryAlertDescription = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
				IsAnomaly = table.Column<bool>(type: "bit", nullable: false),
				Suppressor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
				SuppressedDescending = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
				DatamartCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				DatamartLastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_Alerts", x => x.Id);
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
					principalColumn: "Id",
					onDelete: ReferentialAction.Restrict);
				table.ForeignKey(
					name: "FK_Alerts_MonitorObjectGroups_MonitorObjectGroup1Id",
					column: x => x.MonitorObjectGroup1Id,
					principalTable: "MonitorObjectGroups",
					principalColumn: "Id",
					onDelete: ReferentialAction.Restrict);
				table.ForeignKey(
					name: "FK_Alerts_MonitorObjectGroups_MonitorObjectGroup2Id",
					column: x => x.MonitorObjectGroup2Id,
					principalTable: "MonitorObjectGroups",
					principalColumn: "Id",
					onDelete: ReferentialAction.Restrict);
				table.ForeignKey(
					name: "FK_Alerts_MonitorObjectGroups_MonitorObjectGroup3Id",
					column: x => x.MonitorObjectGroup3Id,
					principalTable: "MonitorObjectGroups",
					principalColumn: "Id",
					onDelete: ReferentialAction.Restrict);
				table.ForeignKey(
					name: "FK_Alerts_MonitorObjectGroups_MonitorObjectGroup4Id",
					column: x => x.MonitorObjectGroup4Id,
					principalTable: "MonitorObjectGroups",
					principalColumn: "Id",
					onDelete: ReferentialAction.Restrict);
				table.ForeignKey(
					name: "FK_Alerts_MonitorObjectGroups_MonitorObjectGroup5Id",
					column: x => x.MonitorObjectGroup5Id,
					principalTable: "MonitorObjectGroups",
					principalColumn: "Id",
					onDelete: ReferentialAction.Restrict);
				table.ForeignKey(
					name: "FK_Alerts_MonitorObjectGroups_MonitorObjectGroup6Id",
					column: x => x.MonitorObjectGroup6Id,
					principalTable: "MonitorObjectGroups",
					principalColumn: "Id",
					onDelete: ReferentialAction.Restrict);
				table.ForeignKey(
					name: "FK_Alerts_MonitorObjectGroups_MonitorObjectGroup7Id",
					column: x => x.MonitorObjectGroup7Id,
					principalTable: "MonitorObjectGroups",
					principalColumn: "Id",
					onDelete: ReferentialAction.Restrict);
				table.ForeignKey(
					name: "FK_Alerts_MonitorObjectGroups_MonitorObjectGroup8Id",
					column: x => x.MonitorObjectGroup8Id,
					principalTable: "MonitorObjectGroups",
					principalColumn: "Id",
					onDelete: ReferentialAction.Restrict);
				table.ForeignKey(
					name: "FK_Alerts_MonitorObjectGroups_MonitorObjectGroup9Id",
					column: x => x.MonitorObjectGroup9Id,
					principalTable: "MonitorObjectGroups",
					principalColumn: "Id",
					onDelete: ReferentialAction.Restrict);
			});

		migrationBuilder.CreateTable(
			name: "DeviceDataSources",
			columns: table => new
			{
				Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				DataSourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				AssignedOnSeconds = table.Column<long>(type: "bigint", nullable: false),
				CreatedOnSeconds = table.Column<long>(type: "bigint", nullable: false),
				UpdatedOnSeconds = table.Column<long>(type: "bigint", nullable: false),
				DatamartCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				DatamartLastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				LogicMonitorId = table.Column<int>(type: "int", nullable: false),
				DatamartLastObserved = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_DeviceDataSources", x => x.Id);
				table.ForeignKey(
					name: "FK_DeviceDataSources_DataSources_DataSourceId",
					column: x => x.DataSourceId,
					principalTable: "DataSources",
					principalColumn: "Id",
					onDelete: ReferentialAction.Restrict);
				table.ForeignKey(
					name: "FK_DeviceDataSources_Devices_DeviceId",
					column: x => x.DeviceId,
					principalTable: "Devices",
					principalColumn: "Id",
					onDelete: ReferentialAction.Restrict);
			});

		migrationBuilder.CreateTable(
			name: "DeviceDataSourceInstances",
			columns: table => new
			{
				Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				DeviceDataSourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				AlertDisableStatus = table.Column<int>(type: "int", nullable: false),
				AlertStatus = table.Column<int>(type: "int", nullable: false),
				AlertStatusPriority = table.Column<int>(type: "int", nullable: false),
				Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
				DisableAlerting = table.Column<bool>(type: "bit", nullable: false),
				DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
				GroupId = table.Column<int>(type: "int", nullable: false),
				GroupName = table.Column<string>(type: "nvarchar(max)", nullable: false),
				LastCollectedTimeSeconds = table.Column<long>(type: "bigint", nullable: false),
				LastUpdatedTimeSeconds = table.Column<long>(type: "bigint", nullable: false),
				LockDescription = table.Column<bool>(type: "bit", nullable: false),
				Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
				StopMonitoring = table.Column<bool>(type: "bit", nullable: false),
				SdtStatus = table.Column<int>(type: "int", nullable: false),
				SdtAt = table.Column<string>(type: "nvarchar(max)", nullable: true),
				WildValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
				WildValue2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
				LastWentMissing = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
				InstanceProperty1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
				InstanceProperty2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
				InstanceProperty3 = table.Column<string>(type: "nvarchar(max)", nullable: false),
				InstanceProperty4 = table.Column<string>(type: "nvarchar(max)", nullable: false),
				InstanceProperty5 = table.Column<string>(type: "nvarchar(max)", nullable: false),
				InstanceProperty6 = table.Column<string>(type: "nvarchar(max)", nullable: false),
				InstanceProperty7 = table.Column<string>(type: "nvarchar(max)", nullable: false),
				InstanceProperty8 = table.Column<string>(type: "nvarchar(max)", nullable: false),
				InstanceProperty9 = table.Column<string>(type: "nvarchar(max)", nullable: false),
				InstanceProperty10 = table.Column<string>(type: "nvarchar(max)", nullable: false),
				DatamartCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				DatamartLastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				LogicMonitorId = table.Column<int>(type: "int", nullable: false),
				DatamartLastObserved = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_DeviceDataSourceInstances", x => x.Id);
				table.ForeignKey(
					name: "FK_DeviceDataSourceInstances_DeviceDataSources_DeviceDataSourceId",
					column: x => x.DeviceDataSourceId,
					principalTable: "DeviceDataSources",
					principalColumn: "Id",
					onDelete: ReferentialAction.Restrict);
			});

		migrationBuilder.CreateTable(
			name: "DeviceDataSourceInstanceDataPoints",
			columns: table => new
			{
				Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				DeviceDataSourceInstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				DataSourceDataPointId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				DataCompleteTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
				InstanceDatapointProperty1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
				InstanceDatapointProperty2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
				InstanceDatapointProperty3 = table.Column<string>(type: "nvarchar(max)", nullable: false),
				InstanceDatapointProperty4 = table.Column<string>(type: "nvarchar(max)", nullable: false),
				InstanceDatapointProperty5 = table.Column<string>(type: "nvarchar(max)", nullable: false),
				InstanceDatapointProperty6 = table.Column<string>(type: "nvarchar(max)", nullable: false),
				InstanceDatapointProperty7 = table.Column<string>(type: "nvarchar(max)", nullable: false),
				InstanceDatapointProperty8 = table.Column<string>(type: "nvarchar(max)", nullable: false),
				InstanceDatapointProperty9 = table.Column<string>(type: "nvarchar(max)", nullable: false),
				InstanceDatapointProperty10 = table.Column<string>(type: "nvarchar(max)", nullable: false),
				DataSourceStoreItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
				DatamartCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				DatamartLastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				LogicMonitorId = table.Column<int>(type: "int", nullable: false),
				DatamartLastObserved = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_DeviceDataSourceInstanceDataPoints", x => x.Id);
				table.ForeignKey(
					name: "FK_DeviceDataSourceInstanceDataPoints_DataSourceDataPoints_DataSourceDataPointId",
					column: x => x.DataSourceDataPointId,
					principalTable: "DataSourceDataPoints",
					principalColumn: "Id",
					onDelete: ReferentialAction.Cascade);
				table.ForeignKey(
					name: "FK_DeviceDataSourceInstanceDataPoints_DataSources_DataSourceStoreItemId",
					column: x => x.DataSourceStoreItemId,
					principalTable: "DataSources",
					principalColumn: "Id");
				table.ForeignKey(
					name: "FK_DeviceDataSourceInstanceDataPoints_DeviceDataSourceInstances_DeviceDataSourceInstanceId",
					column: x => x.DeviceDataSourceInstanceId,
					principalTable: "DeviceDataSourceInstances",
					principalColumn: "Id",
					onDelete: ReferentialAction.Cascade);
			});

		migrationBuilder.CreateTable(
			name: "TimeSeriesDataAggregations",
			columns: table => new
			{
				Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				DeviceDataSourceInstanceDataPointId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				PeriodStart = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				PeriodEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				Centile05 = table.Column<double>(type: "float", nullable: true),
				Centile10 = table.Column<double>(type: "float", nullable: true),
				Centile25 = table.Column<double>(type: "float", nullable: true),
				Centile50 = table.Column<double>(type: "float", nullable: true),
				Centile75 = table.Column<double>(type: "float", nullable: true),
				Centile90 = table.Column<double>(type: "float", nullable: true),
				Centile95 = table.Column<double>(type: "float", nullable: true),
				AvailabilityPercent = table.Column<double>(type: "float", nullable: true),
				First = table.Column<double>(type: "float", nullable: true),
				Last = table.Column<double>(type: "float", nullable: true),
				FirstWithData = table.Column<double>(type: "float", nullable: true),
				LastWithData = table.Column<double>(type: "float", nullable: true),
				Min = table.Column<double>(type: "float", nullable: true),
				Max = table.Column<double>(type: "float", nullable: true),
				Sum = table.Column<double>(type: "float", nullable: false),
				SumSquared = table.Column<double>(type: "float", nullable: false),
				DataCount = table.Column<int>(type: "int", nullable: false),
				NoDataCount = table.Column<int>(type: "int", nullable: false),
				NormalCount = table.Column<int>(type: "int", nullable: true),
				WarningCount = table.Column<int>(type: "int", nullable: true),
				ErrorCount = table.Column<int>(type: "int", nullable: true),
				CriticalCount = table.Column<int>(type: "int", nullable: true)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_TimeSeriesDataAggregations", x => x.Id);
				table.ForeignKey(
					name: "FK_TimeSeriesDataAggregations_DeviceDataSourceInstanceDataPoints_DeviceDataSourceInstanceDataPointId",
					column: x => x.DeviceDataSourceInstanceDataPointId,
					principalTable: "DeviceDataSourceInstanceDataPoints",
					principalColumn: "Id",
					onDelete: ReferentialAction.Cascade);
			});

		migrationBuilder.CreateIndex(
			name: "IX_AlertRules_EscalationChainId",
			table: "AlertRules",
			column: "EscalationChainId");

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
			name: "IX_Alerts_FasterPercentageAvailability",
			table: "Alerts",
			columns: _alertsFasterPercentageAvailabilityIndexedColumns)
			.Annotation("SqlServer:Include", _includesAlertsFasterPercentageAvailabilityIndexedColumns);

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
			name: "IX_Collectors_CollectorGroupId",
			table: "Collectors",
			column: "CollectorGroupId");

		migrationBuilder.CreateIndex(
			name: "IX_DataSourceDataPoints_DataSourceId",
			table: "DataSourceDataPoints",
			column: "DataSourceId");

		migrationBuilder.CreateIndex(
			name: "IX_DeviceDataSourceInstanceDataPoints_DataSourceDataPointId",
			table: "DeviceDataSourceInstanceDataPoints",
			column: "DataSourceDataPointId");

		migrationBuilder.CreateIndex(
			name: "IX_DeviceDataSourceInstanceDataPoints_DataSourceStoreItemId",
			table: "DeviceDataSourceInstanceDataPoints",
			column: "DataSourceStoreItemId");

		migrationBuilder.CreateIndex(
			name: "IX_DeviceDataSourceInstanceDataPoints_DeviceDataSourceInstanceId",
			table: "DeviceDataSourceInstanceDataPoints",
			column: "DeviceDataSourceInstanceId");

		migrationBuilder.CreateIndex(
			name: "IX_DeviceDataSourceInstances_DeviceDataSourceId",
			table: "DeviceDataSourceInstances",
			column: "DeviceDataSourceId");

		migrationBuilder.CreateIndex(
			name: "IX_DeviceDataSourceInstances_LastWentMissing",
			table: "DeviceDataSourceInstances",
			column: "LastWentMissing");

		migrationBuilder.CreateIndex(
			name: "IX_DeviceDataSources_DataSourceId",
			table: "DeviceDataSources",
			column: "DataSourceId");

		migrationBuilder.CreateIndex(
			name: "IX_DeviceDataSources_DeviceId",
			table: "DeviceDataSources",
			column: "DeviceId");

		migrationBuilder.CreateIndex(
			name: "IX_Devices_PreferredCollectorId",
			table: "Devices",
			column: "PreferredCollectorId");

		migrationBuilder.CreateIndex(
			name: "IX_MonitorObjectGroups_FullPath_MonitoredObjectType",
			table: "MonitorObjectGroups",
			columns: _monitorObjectGroupsIndexColumns);

		migrationBuilder.CreateIndex(
			name: "IX_TimeSeriesDataAggregations_DeviceDataSourceInstanceDataPointId",
			table: "TimeSeriesDataAggregations",
			column: "DeviceDataSourceInstanceDataPointId");

		migrationBuilder.CreateIndex(
			name: "IX_Websites_WebsiteGroupId",
			table: "Websites",
			column: "WebsiteGroupId");
	}

	/// <inheritdoc />
	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropTable(
			name: "Alerts");

		migrationBuilder.DropTable(
			name: "ConfigSources");

		migrationBuilder.DropTable(
			name: "DeviceGroups");

		migrationBuilder.DropTable(
			name: "EventSources");

		migrationBuilder.DropTable(
			name: "LogicModuleUpdates");

		migrationBuilder.DropTable(
			name: "LogItems");

		migrationBuilder.DropTable(
			name: "TimeSeriesDataAggregations");

		migrationBuilder.DropTable(
			name: "Websites");

		migrationBuilder.DropTable(
			name: "AlertRules");

		migrationBuilder.DropTable(
			name: "MonitorObjectGroups");

		migrationBuilder.DropTable(
			name: "DeviceDataSourceInstanceDataPoints");

		migrationBuilder.DropTable(
			name: "WebsiteGroups");

		migrationBuilder.DropTable(
			name: "EscalationChains");

		migrationBuilder.DropTable(
			name: "DataSourceDataPoints");

		migrationBuilder.DropTable(
			name: "DeviceDataSourceInstances");

		migrationBuilder.DropTable(
			name: "DeviceDataSources");

		migrationBuilder.DropTable(
			name: "DataSources");

		migrationBuilder.DropTable(
			name: "Devices");

		migrationBuilder.DropTable(
			name: "Collectors");

		migrationBuilder.DropTable(
			name: "CollectorGroups");
	}
}
