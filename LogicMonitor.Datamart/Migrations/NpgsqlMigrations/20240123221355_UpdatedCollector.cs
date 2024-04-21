#nullable disable

namespace LogicMonitor.Datamart.Migrations
{
	/// <inheritdoc />
	public partial class UpdatedCollector : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<bool>(
				name: "IsLmLogsEnabled",
				table: "Collectors",
				type: "boolean",
				nullable: false,
				defaultValue: false);

			migrationBuilder.AddColumn<bool>(
				name: "IsLmLogsSyslogEnabled",
				table: "Collectors",
				type: "boolean",
				nullable: false,
				defaultValue: false);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "IsLmLogsEnabled",
				table: "Collectors");

			migrationBuilder.DropColumn(
				name: "IsLmLogsSyslogEnabled",
				table: "Collectors");
		}
	}
}