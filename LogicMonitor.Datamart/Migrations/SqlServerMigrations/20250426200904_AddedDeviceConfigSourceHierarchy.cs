#nullable disable

namespace LogicMonitor.Datamart.Migrations.SqlServerMigrations
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
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					ConfigSourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					DatamartCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
					DatamartLastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
					LogicMonitorId = table.Column<int>(type: "int", nullable: false),
					DatamartLastObserved = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
					DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					DeviceConfigSourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					DatamartCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
					DatamartLastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
					LogicMonitorId = table.Column<int>(type: "int", nullable: false),
					DatamartLastObserved = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
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
					InstanceProperty10 = table.Column<string>(type: "nvarchar(max)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_DeviceConfigSourceInstances", x => x.Id);
					table.ForeignKey(
						name: "FK_DeviceConfigSourceInstances_DeviceConfigSources_DeviceConfigSourceId",
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
