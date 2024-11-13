#nullable disable

namespace LogicMonitor.Datamart.Migrations.SqlServerMigrations;

/// <inheritdoc />
public partial class AddedEmailIntegrationSupport : Migration
{
	/// <inheritdoc />
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.AddColumn<string>(
			name: "Body",
			table: "Integrations",
			type: "nvarchar(max)",
			nullable: true);

		migrationBuilder.AddColumn<string>(
			name: "Description",
			table: "Integrations",
			type: "nvarchar(max)",
			nullable: true);

		migrationBuilder.AddColumn<string>(
			name: "Name",
			table: "Integrations",
			type: "nvarchar(max)",
			nullable: true);

		migrationBuilder.AddColumn<string>(
			name: "Receivers",
			table: "Integrations",
			type: "nvarchar(max)",
			nullable: true);

		migrationBuilder.AddColumn<string>(
			name: "Sender",
			table: "Integrations",
			type: "nvarchar(max)",
			nullable: true);

		migrationBuilder.AddColumn<string>(
			name: "Subject",
			table: "Integrations",
			type: "nvarchar(max)",
			nullable: true);
	}

	/// <inheritdoc />
	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropColumn(
			name: "Body",
			table: "Integrations");

		migrationBuilder.DropColumn(
			name: "Description",
			table: "Integrations");

		migrationBuilder.DropColumn(
			name: "Name",
			table: "Integrations");

		migrationBuilder.DropColumn(
			name: "Receivers",
			table: "Integrations");

		migrationBuilder.DropColumn(
			name: "Sender",
			table: "Integrations");

		migrationBuilder.DropColumn(
			name: "Subject",
			table: "Integrations");
	}
}
