#nullable disable

namespace LogicMonitor.Datamart.Migrations.NpgsqlMigrations
{
	/// <inheritdoc />
	public partial class AddedDeviceConfigSourceHierarchy : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "DeviceConfigSources",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					ConfigSourceId = table.Column<Guid>(type: "uuid", nullable: false),
					DatamartCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
					DatamartLastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
					LogicMonitorId = table.Column<int>(type: "integer", nullable: false),
					DatamartLastObserved = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
					DeviceId = table.Column<Guid>(type: "uuid", nullable: false),
					AssignedOnSeconds = table.Column<long>(type: "bigint", nullable: false),
					CreatedOnSeconds = table.Column<long>(type: "bigint", nullable: false),
					UpdatedOnSeconds = table.Column<long>(type: "bigint", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_DeviceConfigSources", x => x.Id);
					table.ForeignKey(
						name: "FK_DeviceConfigSources_ConfigSources_ConfigSourceId",
						column: x => x.ConfigSourceId,
						principalTable: "ConfigSources",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_DeviceConfigSources_Devices_DeviceId",
						column: x => x.DeviceId,
						principalTable: "Devices",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "DeviceConfigSourceInstances",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					DeviceConfigSourceId = table.Column<Guid>(type: "uuid", nullable: false),
					DatamartCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
					DatamartLastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
					LogicMonitorId = table.Column<int>(type: "integer", nullable: false),
					DatamartLastObserved = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
					AlertDisableStatus = table.Column<int>(type: "integer", nullable: false),
					AlertStatus = table.Column<int>(type: "integer", nullable: false),
					AlertStatusPriority = table.Column<int>(type: "integer", nullable: false),
					Description = table.Column<string>(type: "text", nullable: false),
					DisableAlerting = table.Column<bool>(type: "boolean", nullable: false),
					DisplayName = table.Column<string>(type: "text", nullable: false),
					GroupId = table.Column<int>(type: "integer", nullable: false),
					GroupName = table.Column<string>(type: "text", nullable: false),
					LastCollectedTimeSeconds = table.Column<long>(type: "bigint", nullable: false),
					LastUpdatedTimeSeconds = table.Column<long>(type: "bigint", nullable: false),
					LockDescription = table.Column<bool>(type: "boolean", nullable: false),
					Name = table.Column<string>(type: "text", nullable: false),
					StopMonitoring = table.Column<bool>(type: "boolean", nullable: false),
					SdtStatus = table.Column<int>(type: "integer", nullable: false),
					SdtAt = table.Column<string>(type: "text", nullable: true),
					WildValue = table.Column<string>(type: "text", nullable: false),
					WildValue2 = table.Column<string>(type: "text", nullable: false),
					LastWentMissing = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
					InstanceProperty1 = table.Column<string>(type: "text", nullable: false),
					InstanceProperty2 = table.Column<string>(type: "text", nullable: false),
					InstanceProperty3 = table.Column<string>(type: "text", nullable: false),
					InstanceProperty4 = table.Column<string>(type: "text", nullable: false),
					InstanceProperty5 = table.Column<string>(type: "text", nullable: false),
					InstanceProperty6 = table.Column<string>(type: "text", nullable: false),
					InstanceProperty7 = table.Column<string>(type: "text", nullable: false),
					InstanceProperty8 = table.Column<string>(type: "text", nullable: false),
					InstanceProperty9 = table.Column<string>(type: "text", nullable: false),
					InstanceProperty10 = table.Column<string>(type: "text", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_DeviceConfigSourceInstances", x => x.Id);
					table.ForeignKey(
						name: "FK_DeviceConfigSourceInstances_DeviceConfigSources_DeviceConfi~",
						column: x => x.DeviceConfigSourceId,
						principalTable: "DeviceConfigSources",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_DeviceConfigSourceInstances_DeviceConfigSourceId",
				table: "DeviceConfigSourceInstances",
				column: "DeviceConfigSourceId");

			migrationBuilder.CreateIndex(
				name: "IX_DeviceConfigSources_ConfigSourceId",
				table: "DeviceConfigSources",
				column: "ConfigSourceId");

			migrationBuilder.CreateIndex(
				name: "IX_DeviceConfigSources_DeviceId",
				table: "DeviceConfigSources",
				column: "DeviceId");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "DeviceConfigSourceInstances");

			migrationBuilder.DropTable(
				name: "DeviceConfigSources");
		}
	}
}
