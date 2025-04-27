#nullable disable

namespace LogicMonitor.Datamart.Migrations.SqlServerMigrations
{
	/// <inheritdoc />
	public partial class AddedDeviceConfigSourceInstanceConfigLogicMonitorStringIdIndex : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_DeviceConfigSourceInstanceConfigs_DeviceConfigSourceInstances_DeviceConfigSourceInstanceId",
				table: "DeviceConfigSourceInstanceConfigs");

			migrationBuilder.AlterColumn<string>(
				name: "LogicMonitorStringId",
				table: "DeviceConfigSourceInstanceConfigs",
				type: "nvarchar(450)",
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(max)");

			migrationBuilder.CreateIndex(
				name: "IX_DeviceConfigSourceInstanceConfigs_LogicMonitorStringId",
				table: "DeviceConfigSourceInstanceConfigs",
				column: "LogicMonitorStringId");

			migrationBuilder.AddForeignKey(
				name: "FK_DeviceConfigSourceInstanceConfigs_DeviceConfigSourceInstances_DeviceConfigSourceInstanceId",
				table: "DeviceConfigSourceInstanceConfigs",
				column: "DeviceConfigSourceInstanceId",
				principalTable: "DeviceConfigSourceInstances",
				principalColumn: "Id",
				onDelete: ReferentialAction.Restrict);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_DeviceConfigSourceInstanceConfigs_DeviceConfigSourceInstances_DeviceConfigSourceInstanceId",
				table: "DeviceConfigSourceInstanceConfigs");

			migrationBuilder.DropIndex(
				name: "IX_DeviceConfigSourceInstanceConfigs_LogicMonitorStringId",
				table: "DeviceConfigSourceInstanceConfigs");

			migrationBuilder.AlterColumn<string>(
				name: "LogicMonitorStringId",
				table: "DeviceConfigSourceInstanceConfigs",
				type: "nvarchar(max)",
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(450)");

			migrationBuilder.AddForeignKey(
				name: "FK_DeviceConfigSourceInstanceConfigs_DeviceConfigSourceInstances_DeviceConfigSourceInstanceId",
				table: "DeviceConfigSourceInstanceConfigs",
				column: "DeviceConfigSourceInstanceId",
				principalTable: "DeviceConfigSourceInstances",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}
	}
}
