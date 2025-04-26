#nullable disable

namespace LogicMonitor.Datamart.Migrations.NpgsqlMigrations
{
	/// <inheritdoc />
	public partial class UpdatedConfigSourceAndDataSource : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<string>(
				name: "AppliesTo",
				table: "ConfigSources",
				type: "text",
				nullable: false,
				defaultValue: "");

			migrationBuilder.AddColumn<string>(
				name: "AuditVersion",
				table: "ConfigSources",
				type: "text",
				nullable: false,
				defaultValue: "");

			migrationBuilder.AddColumn<string>(
				name: "Checksum",
				table: "ConfigSources",
				type: "text",
				nullable: false,
				defaultValue: "");

			migrationBuilder.AddColumn<string>(
				name: "CollectionMethod",
				table: "ConfigSources",
				type: "text",
				nullable: false,
				defaultValue: "");

			migrationBuilder.AddColumn<string>(
				name: "DisplayName",
				table: "ConfigSources",
				type: "text",
				nullable: false,
				defaultValue: "");

			migrationBuilder.AddColumn<string>(
				name: "Group",
				table: "ConfigSources",
				type: "text",
				nullable: false,
				defaultValue: "");

			migrationBuilder.AddColumn<string>(
				name: "Version",
				table: "ConfigSources",
				type: "text",
				nullable: false,
				defaultValue: "");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "AppliesTo",
				table: "ConfigSources");

			migrationBuilder.DropColumn(
				name: "AuditVersion",
				table: "ConfigSources");

			migrationBuilder.DropColumn(
				name: "Checksum",
				table: "ConfigSources");

			migrationBuilder.DropColumn(
				name: "CollectionMethod",
				table: "ConfigSources");

			migrationBuilder.DropColumn(
				name: "DisplayName",
				table: "ConfigSources");

			migrationBuilder.DropColumn(
				name: "Group",
				table: "ConfigSources");

			migrationBuilder.DropColumn(
				name: "Version",
				table: "ConfigSources");
		}
	}
}
