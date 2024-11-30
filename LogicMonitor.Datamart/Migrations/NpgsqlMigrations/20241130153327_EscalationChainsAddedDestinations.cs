#nullable disable

namespace LogicMonitor.Datamart.Migrations.NpgsqlMigrations;

/// <inheritdoc />
public partial class EscalationChainsAddedDestinations : Migration
{
	/// <inheritdoc />
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.AddColumn<string>(
			name: "CcDestination",
			table: "EscalationChains",
			type: "text",
			nullable: false,
			defaultValue: "");

		migrationBuilder.AddColumn<string>(
			name: "CcDestinations",
			table: "EscalationChains",
			type: "text",
			nullable: false,
			defaultValue: "");

		migrationBuilder.AddColumn<string>(
			name: "Destination",
			table: "EscalationChains",
			type: "text",
			nullable: false,
			defaultValue: "");

		migrationBuilder.AddColumn<string>(
			name: "Destinations",
			table: "EscalationChains",
			type: "text",
			nullable: false,
			defaultValue: "");
	}

	/// <inheritdoc />
	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropColumn(
			name: "CcDestination",
			table: "EscalationChains");

		migrationBuilder.DropColumn(
			name: "CcDestinations",
			table: "EscalationChains");

		migrationBuilder.DropColumn(
			name: "Destination",
			table: "EscalationChains");

		migrationBuilder.DropColumn(
			name: "Destinations",
			table: "EscalationChains");
	}
}
