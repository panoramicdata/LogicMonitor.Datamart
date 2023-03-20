#nullable disable

namespace LogicMonitor.Datamart.Migrations
{
	/// <inheritdoc />
	public partial class InitialMigration : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "CollectorGroups",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					Name = table.Column<string>(type: "text", nullable: false),
					Description = table.Column<string>(type: "text", nullable: false),
					CreatedOnTimeStampSeconds = table.Column<long>(type: "bigint", nullable: false),
					CollectorCount = table.Column<int>(type: "integer", nullable: false),
					DatamartCreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					DatamartLastModifiedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					LogicMonitorId = table.Column<int>(type: "integer", nullable: false),
					DatamartLastObservedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_CollectorGroups", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "ConfigSources",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					Name = table.Column<string>(type: "text", nullable: false),
					Description = table.Column<string>(type: "text", nullable: false),
					DatamartCreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					DatamartLastModifiedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					LogicMonitorId = table.Column<int>(type: "integer", nullable: false),
					DatamartLastObservedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_ConfigSources", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "DataSources",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					Name = table.Column<string>(type: "text", nullable: false),
					Description = table.Column<string>(type: "text", nullable: false),
					DatamartCreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					DatamartLastModifiedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					LogicMonitorId = table.Column<int>(type: "integer", nullable: false),
					DatamartLastObservedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_DataSources", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "DeviceGroups",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					Name = table.Column<string>(type: "text", nullable: false),
					Description = table.Column<string>(type: "text", nullable: false),
					AlertDisableStatus = table.Column<int>(type: "integer", nullable: false),
					AlertEnable = table.Column<bool>(type: "boolean", nullable: false),
					AlertStatus = table.Column<int>(type: "integer", nullable: false),
					AppliesTo = table.Column<string>(type: "text", nullable: false),
					AutoVisualResult = table.Column<string>(type: "text", nullable: true),
					ClusterAlertStatus = table.Column<string>(type: "text", nullable: false),
					ClusterAlertStatusPriority = table.Column<int>(type: "integer", nullable: false),
					DefaultCollectorDescription = table.Column<string>(type: "text", nullable: true),
					DefaultCollectorId = table.Column<int>(type: "integer", nullable: false),
					DefaultAgentId = table.Column<int>(type: "integer", nullable: false),
					AwsDeviceCount = table.Column<int>(type: "integer", nullable: false),
					AwsRegionsInfo = table.Column<string>(type: "text", nullable: false),
					AwsTestResult = table.Column<string>(type: "text", nullable: true),
					AwsTestResultCode = table.Column<int>(type: "integer", nullable: false),
					AzureDeviceCount = table.Column<int>(type: "integer", nullable: false),
					AzureRegionsInfo = table.Column<string>(type: "text", nullable: false),
					AzureTestResult = table.Column<string>(type: "text", nullable: true),
					AzureTestResultCode = table.Column<int>(type: "integer", nullable: false),
					GcpDeviceCount = table.Column<int>(type: "integer", nullable: false),
					GcpRegionsInfo = table.Column<string>(type: "text", nullable: false),
					GcpTestResult = table.Column<string>(type: "text", nullable: true),
					GcpTestResultCode = table.Column<int>(type: "integer", nullable: false),
					IsNetflowEnabled = table.Column<bool>(type: "boolean", nullable: false),
					HasNetflowEnabledDevices = table.Column<bool>(type: "boolean", nullable: false),
					CreatedOnTimestampUtc = table.Column<int>(type: "integer", nullable: true),
					DeviceCount = table.Column<int>(type: "integer", nullable: false),
					DeviceGroupType = table.Column<int>(type: "integer", nullable: false),
					DirectDeviceCount = table.Column<int>(type: "integer", nullable: false),
					DirectSubGroupCount = table.Column<int>(type: "integer", nullable: false),
					AlertStatusPriority = table.Column<int>(type: "integer", nullable: false),
					EffectiveAlertEnabled = table.Column<bool>(type: "boolean", nullable: false),
					FullPath = table.Column<string>(type: "text", nullable: false),
					GroupStatus = table.Column<string>(type: "text", nullable: false),
					IsAlertingDisabled = table.Column<bool>(type: "boolean", nullable: false),
					ParentId = table.Column<int>(type: "integer", nullable: false),
					SdtStatus = table.Column<int>(type: "integer", nullable: false),
					UserPermission = table.Column<int>(type: "integer", nullable: false),
					DatamartCreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					DatamartLastModifiedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					LogicMonitorId = table.Column<int>(type: "integer", nullable: false),
					DatamartLastObservedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_DeviceGroups", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "EscalationChains",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					Name = table.Column<string>(type: "text", nullable: false),
					Description = table.Column<string>(type: "text", nullable: false),
					EnableThrottling = table.Column<bool>(type: "boolean", nullable: false),
					ThrottlingPeriodMinutes = table.Column<int>(type: "integer", nullable: false),
					ThrottlingAlertCount = table.Column<int>(type: "integer", nullable: false),
					InAlerting = table.Column<bool>(type: "boolean", nullable: false),
					DatamartCreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					DatamartLastModifiedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					LogicMonitorId = table.Column<int>(type: "integer", nullable: false),
					DatamartLastObservedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_EscalationChains", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "EventSources",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					Name = table.Column<string>(type: "text", nullable: false),
					Description = table.Column<string>(type: "text", nullable: false),
					DatamartCreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					DatamartLastModifiedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					LogicMonitorId = table.Column<int>(type: "integer", nullable: false),
					DatamartLastObservedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_EventSources", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "LogItems",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					LogicMonitorId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
					UserName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
					HappenedOnTimeStampUtc = table.Column<long>(type: "bigint", nullable: false),
					SessionId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
					Description = table.Column<string>(type: "text", nullable: false),
					IpAddress = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
					DatamartCreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					DatamartLastModifiedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_LogItems", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "MonitorObjectGroups",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					FullPath = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
					MonitoredObjectType = table.Column<int>(type: "integer", nullable: false),
					DatamartCreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					DatamartLastModifiedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_MonitorObjectGroups", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "TimeSeriesDataAggregations",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					DataPointId = table.Column<Guid>(type: "uuid", nullable: false),
					PeriodStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					PeriodEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					Centile05 = table.Column<double>(type: "double precision", nullable: true),
					Centile10 = table.Column<double>(type: "double precision", nullable: true),
					Centile25 = table.Column<double>(type: "double precision", nullable: true),
					Centile75 = table.Column<double>(type: "double precision", nullable: true),
					Centile90 = table.Column<double>(type: "double precision", nullable: true),
					Centile95 = table.Column<double>(type: "double precision", nullable: true),
					First = table.Column<double>(type: "double precision", nullable: true),
					Last = table.Column<double>(type: "double precision", nullable: true),
					FirstWithData = table.Column<double>(type: "double precision", nullable: true),
					LastWithData = table.Column<double>(type: "double precision", nullable: true),
					Min = table.Column<double>(type: "double precision", nullable: true),
					Max = table.Column<double>(type: "double precision", nullable: true),
					Sum = table.Column<double>(type: "double precision", nullable: false),
					SumSquared = table.Column<double>(type: "double precision", nullable: false),
					DataCount = table.Column<int>(type: "integer", nullable: false),
					NoDataCount = table.Column<int>(type: "integer", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_TimeSeriesDataAggregations", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "WebsiteGroups",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					Name = table.Column<string>(type: "text", nullable: false),
					Description = table.Column<string>(type: "text", nullable: false),
					AlertStatus = table.Column<int>(type: "integer", nullable: false),
					AlertStatusPriority = table.Column<int>(type: "integer", nullable: false),
					AlertDisableStatus = table.Column<int>(type: "integer", nullable: false),
					DirectWebsiteCount = table.Column<int>(type: "integer", nullable: false),
					DirectWebsiteGroupCount = table.Column<int>(type: "integer", nullable: false),
					DisableAlerting = table.Column<bool>(type: "boolean", nullable: false),
					FullPath = table.Column<string>(type: "text", nullable: false),
					GroupStatus = table.Column<int>(type: "integer", nullable: false),
					HasWebsitesDisabled = table.Column<bool>(type: "boolean", nullable: false),
					ParentId = table.Column<int>(type: "integer", nullable: false),
					SdtStatus = table.Column<int>(type: "integer", nullable: false),
					WebsiteCount = table.Column<int>(type: "integer", nullable: false),
					StopMonitoring = table.Column<bool>(type: "boolean", nullable: true),
					UserPermissionString = table.Column<int>(type: "integer", nullable: false),
					DatamartCreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					DatamartLastModifiedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					LogicMonitorId = table.Column<int>(type: "integer", nullable: false),
					DatamartLastObservedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_WebsiteGroups", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "Collectors",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					CollectorGroupId = table.Column<Guid>(type: "uuid", nullable: false),
					Name = table.Column<string>(type: "text", nullable: false),
					Description = table.Column<string>(type: "text", nullable: false),
					AckComment = table.Column<string>(type: "text", nullable: false),
					Acked = table.Column<bool>(type: "boolean", nullable: false),
					AckedBy = table.Column<string>(type: "text", nullable: false),
					AckedOnLocalString = table.Column<string>(type: "text", nullable: false),
					AckedOnUtcTimestampUtc = table.Column<long>(type: "bigint", nullable: true),
					Architecture = table.Column<string>(type: "text", nullable: false),
					BackupCollectorId = table.Column<int>(type: "integer", nullable: false),
					Build = table.Column<int>(type: "integer", nullable: false),
					CanDowngrade = table.Column<bool>(type: "boolean", nullable: false),
					CanDowngradeReason = table.Column<string>(type: "text", nullable: false),
					ClearSent = table.Column<bool>(type: "boolean", nullable: false),
					CollectorConfiguration = table.Column<string>(type: "text", nullable: false),
					Configuration = table.Column<string>(type: "text", nullable: true),
					ConfigurationVersion = table.Column<int>(type: "integer", nullable: false),
					CreatedOnLocalString = table.Column<string>(type: "text", nullable: false),
					CreatedOnTimeStampUtc = table.Column<long>(type: "bigint", nullable: false),
					Credential = table.Column<string>(type: "text", nullable: true),
					Credential2 = table.Column<string>(type: "text", nullable: true),
					DeviceCount = table.Column<int>(type: "integer", nullable: false),
					Ea = table.Column<bool>(type: "boolean", nullable: false),
					EnableFailBack = table.Column<bool>(type: "boolean", nullable: false),
					EnableFailOverOnCollectorDevice = table.Column<bool>(type: "boolean", nullable: false),
					EscalationChainId = table.Column<int>(type: "integer", nullable: false),
					HasFailOverDevice = table.Column<bool>(type: "boolean", nullable: false),
					HostName = table.Column<string>(type: "text", nullable: false),
					InSdt = table.Column<bool>(type: "boolean", nullable: false),
					IsDown = table.Column<bool>(type: "boolean", nullable: false),
					LastSentNotificationOnLocal = table.Column<string>(type: "text", nullable: false),
					LastSentNotificationOnTimeStampUtc = table.Column<int>(type: "integer", nullable: false),
					LogicMonitorDeviceId = table.Column<int>(type: "integer", nullable: false),
					NeedAutoCreateCollectorDevice = table.Column<bool>(type: "boolean", nullable: false),
					NetscanVersion = table.Column<int>(type: "integer", nullable: false),
					NextRecipient = table.Column<int>(type: "integer", nullable: false),
					OnetimeDowngradeInfo = table.Column<string>(type: "text", nullable: true),
					Platform = table.Column<string>(type: "text", nullable: false),
					PreviousVersion = table.Column<int>(type: "integer", nullable: false),
					ProxyConfiguration = table.Column<string>(type: "text", nullable: false),
					ResendIntervalSeconds = table.Column<int>(type: "integer", nullable: false),
					WebsiteConfiguration = table.Column<string>(type: "text", nullable: false),
					WebsiteCount = table.Column<int>(type: "integer", nullable: false),
					Size = table.Column<string>(type: "text", nullable: false),
					SpecifiedCollectorDeviceGroupId = table.Column<int>(type: "integer", nullable: false),
					Status = table.Column<int>(type: "integer", nullable: false),
					SuppressAlertClear = table.Column<bool>(type: "boolean", nullable: false),
					UpdatedOnLocalString = table.Column<string>(type: "text", nullable: false),
					UpgradeTimeUtcSeconds = table.Column<long>(type: "bigint", nullable: false),
					UpdatedOnTimeStampUtc = table.Column<long>(type: "bigint", nullable: true),
					UptimeSeconds = table.Column<int>(type: "integer", nullable: false),
					UserChangeOnLocal = table.Column<string>(type: "text", nullable: false),
					UserChangeOnUtcSeconds = table.Column<long>(type: "bigint", nullable: false),
					UserPermission = table.Column<int>(type: "integer", nullable: false),
					UserVisibleDeviceCount = table.Column<int>(type: "integer", nullable: false),
					UserVisibleWebsiteCount = table.Column<int>(type: "integer", nullable: false),
					WatchdogConfiguration = table.Column<string>(type: "text", nullable: false),
					WatchdogUpdatedOnLocal = table.Column<string>(type: "text", nullable: false),
					WatchdogUpdatedOnSeconds = table.Column<long>(type: "bigint", nullable: true),
					WrapperConfiguration = table.Column<string>(type: "text", nullable: false),
					DatamartCreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					DatamartLastModifiedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					LogicMonitorId = table.Column<int>(type: "integer", nullable: false),
					DatamartLastObservedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					DataSourceId = table.Column<Guid>(type: "uuid", nullable: false),
					Name = table.Column<string>(type: "text", nullable: false),
					Description = table.Column<string>(type: "text", nullable: false),
					MeasurementUnit = table.Column<string>(type: "text", nullable: false),
					DatamartCreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					DatamartLastModifiedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					LogicMonitorId = table.Column<int>(type: "integer", nullable: false),
					DatamartLastObservedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					EscalationChainId = table.Column<Guid>(type: "uuid", nullable: false),
					Name = table.Column<string>(type: "text", nullable: false),
					Description = table.Column<string>(type: "text", nullable: true),
					Priority = table.Column<int>(type: "integer", nullable: false),
					LevelString = table.Column<string>(type: "text", nullable: false),
					DataSourceName = table.Column<string>(type: "text", nullable: false),
					DataSourceInstanceName = table.Column<string>(type: "text", nullable: false),
					DataPoint = table.Column<string>(type: "text", nullable: false),
					EscalationChainIntervalMinutes = table.Column<int>(type: "integer", nullable: false),
					SuppressAlertClear = table.Column<bool>(type: "boolean", nullable: false),
					SuppressAlertAckSdt = table.Column<bool>(type: "boolean", nullable: false),
					DatamartCreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					DatamartLastModifiedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					LogicMonitorId = table.Column<int>(type: "integer", nullable: false),
					DatamartLastObservedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					WebsiteGroupId = table.Column<Guid>(type: "uuid", nullable: false),
					Name = table.Column<string>(type: "text", nullable: false),
					Description = table.Column<string>(type: "text", nullable: false),
					Count = table.Column<int>(type: "integer", nullable: false),
					Domain = table.Column<string>(type: "text", nullable: true),
					HostName = table.Column<string>(type: "text", nullable: true),
					AlertDisableStatus = table.Column<int>(type: "integer", nullable: false),
					AlertExpression = table.Column<string>(type: "text", nullable: true),
					AlertStatus = table.Column<int>(type: "integer", nullable: false),
					AlertStatusPriority = table.Column<int>(type: "integer", nullable: false),
					IsAlertingDisabled = table.Column<bool>(type: "boolean", nullable: false),
					GlobalSmAlertCond = table.Column<int>(type: "integer", nullable: false),
					IndividualSmAlertEnable = table.Column<bool>(type: "boolean", nullable: false),
					IndividualAlertLevel = table.Column<byte>(type: "smallint", nullable: false),
					IgnoreSsl = table.Column<bool>(type: "boolean", nullable: false),
					IsInternal = table.Column<bool>(type: "boolean", nullable: false),
					WebsiteMethod = table.Column<int>(type: "integer", nullable: false),
					OverallAlertLevel = table.Column<byte>(type: "smallint", nullable: false),
					PageLoadAlertTimeInMs = table.Column<int>(type: "integer", nullable: false),
					PercentPacketsNotReceiveInTime = table.Column<int>(type: "integer", nullable: false),
					PacketsNotReceivedTimeoutMs = table.Column<int>(type: "integer", nullable: false),
					PollingIntervalMinutes = table.Column<int>(type: "integer", nullable: false),
					Schema = table.Column<string>(type: "text", nullable: false),
					Script = table.Column<string>(type: "text", nullable: true),
					SdtStatus = table.Column<int>(type: "integer", nullable: false),
					StopMonitoring = table.Column<bool>(type: "boolean", nullable: false),
					StopMonitoringByWebsiteGroup = table.Column<bool>(type: "boolean", nullable: false),
					TriggerSslStatusAlerts = table.Column<bool>(type: "boolean", nullable: false),
					TriggerSslExpirationAlerts = table.Column<bool>(type: "boolean", nullable: false),
					Transition = table.Column<int>(type: "integer", nullable: false),
					Type = table.Column<int>(type: "integer", nullable: false),
					UseDefaultAlertSetting = table.Column<bool>(type: "boolean", nullable: false),
					UseDefaultLocationSetting = table.Column<bool>(type: "boolean", nullable: false),
					UserPermissionString = table.Column<int>(type: "integer", nullable: false),
					Status = table.Column<int>(type: "integer", nullable: false),
					DatamartCreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					DatamartLastModifiedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					LogicMonitorId = table.Column<int>(type: "integer", nullable: false),
					DatamartLastObservedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					PreferredCollectorId = table.Column<Guid>(type: "uuid", nullable: true),
					Name = table.Column<string>(type: "text", nullable: false),
					Description = table.Column<string>(type: "text", nullable: false),
					AlertDisableStatus = table.Column<int>(type: "integer", nullable: false),
					AlertStatusPriority = table.Column<int>(type: "integer", nullable: false),
					AlertStatus = table.Column<int>(type: "integer", nullable: false),
					AncestorHasDisabledLogicModule = table.Column<bool>(type: "boolean", nullable: false),
					AutoPropertiesAssignedOnSeconds = table.Column<long>(type: "bigint", nullable: true),
					AutoPropertiesUpdatedOnSeconds = table.Column<long>(type: "bigint", nullable: true),
					AwsState = table.Column<int>(type: "integer", nullable: false),
					AzureState = table.Column<int>(type: "integer", nullable: false),
					GcpState = table.Column<int>(type: "integer", nullable: false),
					CanUseRemoteSession = table.Column<bool>(type: "boolean", nullable: false),
					CollectorDescription = table.Column<string>(type: "text", nullable: false),
					CreatedOnSeconds = table.Column<long>(type: "bigint", nullable: true),
					CurrentCollectorId = table.Column<int>(type: "integer", nullable: false),
					DeletedTimeinMs = table.Column<long>(type: "bigint", nullable: false),
					DeviceType = table.Column<int>(type: "integer", nullable: false),
					IsAlertingDisabled = table.Column<bool>(type: "boolean", nullable: false),
					DisplayName = table.Column<string>(type: "text", nullable: false),
					EffectiveAlertEnabled = table.Column<bool>(type: "boolean", nullable: false),
					EnableNetflow = table.Column<bool>(type: "boolean", nullable: false),
					HasActiveInstance = table.Column<bool>(type: "boolean", nullable: false),
					HasDisabledSubResource = table.Column<bool>(type: "boolean", nullable: false),
					HasMore = table.Column<bool>(type: "boolean", nullable: false),
					DeviceGroupIdsString = table.Column<string>(type: "text", nullable: false),
					DeviceStatus = table.Column<byte>(type: "smallint", nullable: false),
					LastDataTimeSeconds = table.Column<long>(type: "bigint", nullable: true),
					LastRawDataTimeSeconds = table.Column<long>(type: "bigint", nullable: true),
					Link = table.Column<string>(type: "text", nullable: true),
					NetflowCollectorId = table.Column<int>(type: "integer", nullable: false),
					NetflowCollectorDescription = table.Column<string>(type: "text", nullable: true),
					NetflowCollectorGroupId = table.Column<int>(type: "integer", nullable: false),
					NetflowCollectorGroupName = table.Column<string>(type: "text", nullable: true),
					RelatedDeviceId = table.Column<int>(type: "integer", nullable: false),
					ScanConfigId = table.Column<int>(type: "integer", nullable: false),
					SdtStatus = table.Column<int>(type: "integer", nullable: false),
					ToDeleteTimeinMs = table.Column<long>(type: "bigint", nullable: false),
					UptimeInSeconds = table.Column<int>(type: "integer", nullable: false),
					UpdatedOnSeconds = table.Column<long>(type: "bigint", nullable: true),
					UserPermission = table.Column<int>(type: "integer", nullable: false),
					LastAlertClosedTimeSeconds = table.Column<long>(type: "bigint", nullable: false),
					Property1 = table.Column<string>(type: "text", nullable: true),
					Property2 = table.Column<string>(type: "text", nullable: true),
					Property3 = table.Column<string>(type: "text", nullable: true),
					Property4 = table.Column<string>(type: "text", nullable: true),
					Property5 = table.Column<string>(type: "text", nullable: true),
					DatamartCreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					DatamartLastModifiedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					LogicMonitorId = table.Column<int>(type: "integer", nullable: false),
					DatamartLastObservedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					AlertRuleId = table.Column<Guid>(type: "uuid", nullable: true),
					LogicMonitorId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
					AlertType = table.Column<int>(type: "integer", nullable: false),
					InternalId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
					StartOnSeconds = table.Column<int>(type: "integer", nullable: false),
					EndOnSeconds = table.Column<int>(type: "integer", nullable: false),
					Acked = table.Column<bool>(type: "boolean", nullable: false),
					AckedOnSeconds = table.Column<int>(type: "integer", nullable: false),
					AckedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
					AckComment = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
					AlertRuleName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
					AlertEscalationChainName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
					AlertEscalationChainId = table.Column<int>(type: "integer", nullable: true),
					AlertEscalationSubChainId = table.Column<int>(type: "integer", nullable: true),
					NextRecipient = table.Column<int>(type: "integer", nullable: false),
					AlertRecipients = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
					Severity = table.Column<int>(type: "integer", nullable: false),
					IsCleared = table.Column<bool>(type: "boolean", nullable: false),
					InScheduledDownTime = table.Column<bool>(type: "boolean", nullable: false),
					Value = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
					Thresholds = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
					ClearValue = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
					MonitorObjectType = table.Column<int>(type: "integer", nullable: false),
					MonitorObjectId = table.Column<int>(type: "integer", nullable: true),
					MonitorObjectName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
					MonitorObjectGroup0Id = table.Column<Guid>(type: "uuid", nullable: true),
					MonitorObjectGroup1Id = table.Column<Guid>(type: "uuid", nullable: true),
					MonitorObjectGroup2Id = table.Column<Guid>(type: "uuid", nullable: true),
					MonitorObjectGroup3Id = table.Column<Guid>(type: "uuid", nullable: true),
					MonitorObjectGroup4Id = table.Column<Guid>(type: "uuid", nullable: true),
					MonitorObjectGroup5Id = table.Column<Guid>(type: "uuid", nullable: true),
					MonitorObjectGroup6Id = table.Column<Guid>(type: "uuid", nullable: true),
					MonitorObjectGroup7Id = table.Column<Guid>(type: "uuid", nullable: true),
					MonitorObjectGroup8Id = table.Column<Guid>(type: "uuid", nullable: true),
					MonitorObjectGroup9Id = table.Column<Guid>(type: "uuid", nullable: true),
					ResourceId = table.Column<int>(type: "integer", nullable: true),
					ResourceTemplateId = table.Column<int>(type: "integer", nullable: true),
					ResourceTemplateType = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
					ResourceTemplateName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
					InstanceId = table.Column<int>(type: "integer", nullable: false),
					InstanceName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
					InstanceDescription = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
					DataPointName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
					DataPointId = table.Column<int>(type: "integer", nullable: false),
					DetailMessageSubject = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
					DetailMessageBody = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
					CustomColumn1 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
					CustomColumn2 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
					CustomColumn3 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
					CustomColumn4 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
					CustomColumn5 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
					EnableAnomalyAlertSuppression = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
					EnableAnomalyAlertGeneration = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
					Tenant = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
					DependencyRole = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
					DependencyRoutingState = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
					IsActiveDiscoveryAlert = table.Column<bool>(type: "boolean", maxLength: 50, nullable: false),
					ActiveDiscoveryAlertDescription = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
					IsAnomaly = table.Column<bool>(type: "boolean", nullable: false),
					Suppressor = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
					SuppressedDescending = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
					DatamartCreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					DatamartLastModifiedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					DataSourceId = table.Column<Guid>(type: "uuid", nullable: false),
					DeviceId = table.Column<Guid>(type: "uuid", nullable: false),
					AssignedOnSeconds = table.Column<long>(type: "bigint", nullable: false),
					CreatedOnSeconds = table.Column<long>(type: "bigint", nullable: false),
					UpdatedOnSeconds = table.Column<long>(type: "bigint", nullable: false),
					DatamartCreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					DatamartLastModifiedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					LogicMonitorId = table.Column<int>(type: "integer", nullable: false),
					DatamartLastObservedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					DeviceDataSourceId = table.Column<Guid>(type: "uuid", nullable: false),
					AlertDisableStatus = table.Column<int>(type: "integer", nullable: false),
					AlertStatus = table.Column<int>(type: "integer", nullable: false),
					AlertStatusPriority = table.Column<int>(type: "integer", nullable: false),
					DisableAlerting = table.Column<bool>(type: "boolean", nullable: false),
					DisplayName = table.Column<string>(type: "text", nullable: false),
					GroupId = table.Column<int>(type: "integer", nullable: false),
					GroupName = table.Column<string>(type: "text", nullable: false),
					LastCollectedTimeSeconds = table.Column<long>(type: "bigint", nullable: false),
					LastUpdatedTimeSeconds = table.Column<long>(type: "bigint", nullable: false),
					LockDescription = table.Column<bool>(type: "boolean", nullable: false),
					StopMonitoring = table.Column<bool>(type: "boolean", nullable: false),
					SdtStatus = table.Column<int>(type: "integer", nullable: false),
					SdtAt = table.Column<string>(type: "text", nullable: true),
					WildValue = table.Column<string>(type: "text", nullable: false),
					WildValue2 = table.Column<string>(type: "text", nullable: false),
					DataCompleteToUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
					LastWentMissingUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
					DatamartCreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					DatamartLastModifiedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					LogicMonitorId = table.Column<int>(type: "integer", nullable: false),
					DatamartLastObservedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_DeviceDataSourceInstances", x => x.Id);
					table.ForeignKey(
						name: "FK_DeviceDataSourceInstances_DeviceDataSources_DeviceDataSourc~",
						column: x => x.DeviceDataSourceId,
						principalTable: "DeviceDataSources",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
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
				columns: new[] { "StartOnSeconds", "EndOnSeconds", "IsCleared", "InScheduledDownTime", "MonitorObjectGroup0Id", "MonitorObjectGroup1Id", "MonitorObjectGroup2Id", "MonitorObjectGroup3Id", "MonitorObjectGroup4Id", "MonitorObjectGroup5Id", "MonitorObjectGroup6Id", "MonitorObjectGroup7Id", "MonitorObjectGroup8Id", "MonitorObjectGroup9Id" })
				.Annotation("Npgsql:IndexInclude", new[] { "Id", "Severity", "ClearValue", "MonitorObjectId", "ResourceTemplateName", "InstanceId", "InstanceName" });

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
				name: "IX_DeviceDataSourceInstances_DeviceDataSourceId",
				table: "DeviceDataSourceInstances",
				column: "DeviceDataSourceId");

			migrationBuilder.CreateIndex(
				name: "IX_DeviceDataSourceInstances_LastWentMissingUtc",
				table: "DeviceDataSourceInstances",
				column: "LastWentMissingUtc");

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
				columns: new[] { "FullPath", "MonitoredObjectType" });

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
				name: "DataSourceDataPoints");

			migrationBuilder.DropTable(
				name: "DeviceDataSourceInstances");

			migrationBuilder.DropTable(
				name: "DeviceGroups");

			migrationBuilder.DropTable(
				name: "EventSources");

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
				name: "DeviceDataSources");

			migrationBuilder.DropTable(
				name: "WebsiteGroups");

			migrationBuilder.DropTable(
				name: "EscalationChains");

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
}
