#nullable disable

namespace LogicMonitor.Datamart.Migrations.SqlServerMigrations
{
	/// <inheritdoc />
	public partial class PropertyNameTypoCorrection : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.RenameColumn(
				name: "ToDeleteTimeinMs",
				table: "Devices",
				newName: "ToDeleteTimeInMs");

			migrationBuilder.RenameColumn(
				name: "DeletedTimeinMs",
				table: "Devices",
				newName: "DeletedTimeInMs");

			migrationBuilder.RenameColumn(
				name: "OtelVerison",
				table: "Collectors",
				newName: "OtelVersion");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.RenameColumn(
				name: "ToDeleteTimeInMs",
				table: "Devices",
				newName: "ToDeleteTimeinMs");

			migrationBuilder.RenameColumn(
				name: "DeletedTimeInMs",
				table: "Devices",
				newName: "DeletedTimeinMs");

			migrationBuilder.RenameColumn(
				name: "OtelVersion",
				table: "Collectors",
				newName: "OtelVerison");
		}
	}
}
