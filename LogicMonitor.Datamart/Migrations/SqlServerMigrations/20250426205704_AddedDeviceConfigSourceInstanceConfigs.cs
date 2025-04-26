#nullable disable

namespace LogicMonitor.Datamart.Migrations.SqlServerMigrations
{
	/// <inheritdoc />
	public partial class AddedDeviceConfigSourceInstanceConfigs : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "DeviceConfigSourceInstanceConfigs",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					DeviceConfigSourceInstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Config = table.Column<string>(type: "nvarchar(max)", nullable: false),
					LogicMonitorStringId = table.Column<string>(type: "nvarchar(max)", nullable: false),
					DatamartCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
					DatamartLastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
					LogicMonitorId = table.Column<int>(type: "int", nullable: false),
					DatamartLastObserved = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_DeviceConfigSourceInstanceConfigs", x => x.Id);
					table.ForeignKey(
						name: "FK_DeviceConfigSourceInstanceConfigs_DeviceConfigSourceInstances_DeviceConfigSourceInstanceId",
						column: x => x.DeviceConfigSourceInstanceId,
						principalTable: "DeviceConfigSourceInstances",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_DeviceConfigSourceInstanceConfigs_DeviceConfigSourceInstanceId",
				table: "DeviceConfigSourceInstanceConfigs",
				column: "DeviceConfigSourceInstanceId");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "DeviceConfigSourceInstanceConfigs");
		}
	}
}
