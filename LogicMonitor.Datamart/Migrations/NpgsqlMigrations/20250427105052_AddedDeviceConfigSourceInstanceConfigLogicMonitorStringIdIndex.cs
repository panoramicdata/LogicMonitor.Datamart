#nullable disable

namespace LogicMonitor.Datamart.Migrations.NpgsqlMigrations
{
	/// <inheritdoc />
	public partial class AddedDeviceConfigSourceInstanceConfigLogicMonitorStringIdIndex : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_DeviceConfigSourceInstanceConfigs_DeviceConfigSourceInstanc~",
				table: "DeviceConfigSourceInstanceConfigs");

			migrationBuilder.CreateIndex(
				name: "IX_DeviceConfigSourceInstanceConfigs_LogicMonitorStringId",
				table: "DeviceConfigSourceInstanceConfigs",
				column: "LogicMonitorStringId");

			migrationBuilder.AddForeignKey(
				name: "FK_DeviceConfigSourceInstanceConfigs_DeviceConfigSourceInstanc~",
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
				name: "FK_DeviceConfigSourceInstanceConfigs_DeviceConfigSourceInstanc~",
				table: "DeviceConfigSourceInstanceConfigs");

			migrationBuilder.DropIndex(
				name: "IX_DeviceConfigSourceInstanceConfigs_LogicMonitorStringId",
				table: "DeviceConfigSourceInstanceConfigs");

			migrationBuilder.AddForeignKey(
				name: "FK_DeviceConfigSourceInstanceConfigs_DeviceConfigSourceInstanc~",
				table: "DeviceConfigSourceInstanceConfigs",
				column: "DeviceConfigSourceInstanceId",
				principalTable: "DeviceConfigSourceInstances",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}
	}
}
