#nullable disable

namespace LogicMonitor.Datamart.Migrations.NpgsqlMigrations
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
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					DeviceConfigSourceInstanceId = table.Column<Guid>(type: "uuid", nullable: false),
					Config = table.Column<string>(type: "text", nullable: false),
					LogicMonitorStringId = table.Column<string>(type: "text", nullable: false),
					DatamartCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
					DatamartLastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
					LogicMonitorId = table.Column<int>(type: "integer", nullable: false),
					DatamartLastObserved = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_DeviceConfigSourceInstanceConfigs", x => x.Id);
					table.ForeignKey(
						name: "FK_DeviceConfigSourceInstanceConfigs_DeviceConfigSourceInstanc~",
						column: x => x.DeviceConfigSourceInstanceId,
						principalTable: "DeviceConfigSourceInstances",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_DeviceConfigSourceInstanceConfigs_DeviceConfigSourceInstanc~",
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
