#nullable disable

namespace LogicMonitor.Datamart.Migrations.NpgsqlMigrations
{
	/// <inheritdoc />
	public partial class Added_Missing_IsEncodedProperty : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<bool>(
				name: "IsEncoded",
				table: "Collectors",
				type: "boolean",
				nullable: false,
				defaultValue: false);

			migrationBuilder.AddColumn<string>(
				name: "OtelVerison",
				table: "Collectors",
				type: "text",
				nullable: false,
				defaultValue: "");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "IsEncoded",
				table: "Collectors");

			migrationBuilder.DropColumn(
				name: "OtelVerison",
				table: "Collectors");
		}
	}
}
