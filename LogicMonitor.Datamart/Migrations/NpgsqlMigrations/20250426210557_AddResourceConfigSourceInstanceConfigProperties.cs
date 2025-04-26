#nullable disable

namespace LogicMonitor.Datamart.Migrations.NpgsqlMigrations
{
	/// <inheritdoc />
	public partial class AddResourceConfigSourceInstanceConfigProperties : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<string>(
				name: "ChangeStatus",
				table: "DeviceConfigSourceInstanceConfigs",
				type: "text",
				nullable: false,
				defaultValue: "");

			migrationBuilder.AddColumn<string>(
				name: "ConfigErrorMessage",
				table: "DeviceConfigSourceInstanceConfigs",
				type: "text",
				nullable: false,
				defaultValue: "");

			migrationBuilder.AddColumn<int>(
				name: "ConfigStatus",
				table: "DeviceConfigSourceInstanceConfigs",
				type: "integer",
				nullable: false,
				defaultValue: 0);

			migrationBuilder.AddColumn<DateTimeOffset>(
				name: "PollUtc",
				table: "DeviceConfigSourceInstanceConfigs",
				type: "timestamp with time zone",
				nullable: false,
				defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

			migrationBuilder.AddColumn<string>(
				name: "Version",
				table: "DeviceConfigSourceInstanceConfigs",
				type: "text",
				nullable: false,
				defaultValue: "");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "ChangeStatus",
				table: "DeviceConfigSourceInstanceConfigs");

			migrationBuilder.DropColumn(
				name: "ConfigErrorMessage",
				table: "DeviceConfigSourceInstanceConfigs");

			migrationBuilder.DropColumn(
				name: "ConfigStatus",
				table: "DeviceConfigSourceInstanceConfigs");

			migrationBuilder.DropColumn(
				name: "PollUtc",
				table: "DeviceConfigSourceInstanceConfigs");

			migrationBuilder.DropColumn(
				name: "Version",
				table: "DeviceConfigSourceInstanceConfigs");
		}
	}
}
