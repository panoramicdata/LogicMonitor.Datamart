#nullable disable

namespace LogicMonitor.Datamart.Migrations
{
	/// <inheritdoc />
	public partial class ChangedInstanceNameToString : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<string>(
				name: "Name",
				table: "DeviceDataSourceInstances",
				type: "text",
				nullable: false,
				oldClrType: typeof(bool),
				oldType: "boolean");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<bool>(
				name: "Name",
				table: "DeviceDataSourceInstances",
				type: "boolean",
				nullable: false,
				oldClrType: typeof(string),
				oldType: "text");
		}
	}
}