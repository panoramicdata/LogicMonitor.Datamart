#nullable disable

namespace LogicMonitor.Datamart.Migrations.NpgsqlMigrations;

/// <inheritdoc />
public partial class DeviceDataSourceInstanceNamesAndDescriptions : Migration
{
	/// <inheritdoc />
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.AddColumn<string>(
			name: "Description",
			table: "DeviceDataSourceInstances",
			type: "text",
			nullable: false,
			defaultValue: "");

		migrationBuilder.AddColumn<bool>(
			name: "Name",
			table: "DeviceDataSourceInstances",
			type: "boolean",
			nullable: false,
			defaultValue: false);
	}

	/// <inheritdoc />
	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropColumn(
			name: "Description",
			table: "DeviceDataSourceInstances");

		migrationBuilder.DropColumn(
			name: "Name",
			table: "DeviceDataSourceInstances");
	}
}
