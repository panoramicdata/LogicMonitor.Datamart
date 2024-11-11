#nullable disable

namespace LogicMonitor.Datamart.Migrations.SqlServerMigrations
{
	/// <inheritdoc />
	public partial class AddedAutoTaskIntegration : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<int>(
				name: "AccountId",
				table: "Integrations",
				type: "int",
				nullable: true);

			migrationBuilder.AddColumn<int>(
				name: "CriticalPriority",
				table: "Integrations",
				type: "int",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "DueDateTime",
				table: "Integrations",
				type: "nvarchar(max)",
				nullable: true);

			migrationBuilder.AddColumn<int>(
				name: "ErrorPriority",
				table: "Integrations",
				type: "int",
				nullable: true);

			migrationBuilder.AddColumn<int>(
				name: "QueueId",
				table: "Integrations",
				type: "int",
				nullable: true);

			migrationBuilder.AddColumn<int>(
				name: "StatusAckTicket",
				table: "Integrations",
				type: "int",
				nullable: true);

			migrationBuilder.AddColumn<int>(
				name: "StatusCloseTicket",
				table: "Integrations",
				type: "int",
				nullable: true);

			migrationBuilder.AddColumn<int>(
				name: "StatusNewTicket",
				table: "Integrations",
				type: "int",
				nullable: true);

			migrationBuilder.AddColumn<int>(
				name: "StatusUpdateTicket",
				table: "Integrations",
				type: "int",
				nullable: true);

			migrationBuilder.AddColumn<int>(
				name: "WarnPriority",
				table: "Integrations",
				type: "int",
				nullable: true);

			migrationBuilder.AddColumn<int>(
				name: "Zone",
				table: "Integrations",
				type: "int",
				nullable: true);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "AccountId",
				table: "Integrations");

			migrationBuilder.DropColumn(
				name: "CriticalPriority",
				table: "Integrations");

			migrationBuilder.DropColumn(
				name: "DueDateTime",
				table: "Integrations");

			migrationBuilder.DropColumn(
				name: "ErrorPriority",
				table: "Integrations");

			migrationBuilder.DropColumn(
				name: "QueueId",
				table: "Integrations");

			migrationBuilder.DropColumn(
				name: "StatusAckTicket",
				table: "Integrations");

			migrationBuilder.DropColumn(
				name: "StatusCloseTicket",
				table: "Integrations");

			migrationBuilder.DropColumn(
				name: "StatusNewTicket",
				table: "Integrations");

			migrationBuilder.DropColumn(
				name: "StatusUpdateTicket",
				table: "Integrations");

			migrationBuilder.DropColumn(
				name: "WarnPriority",
				table: "Integrations");

			migrationBuilder.DropColumn(
				name: "Zone",
				table: "Integrations");
		}
	}
}
