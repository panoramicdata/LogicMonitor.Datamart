namespace LogicMonitor.Datamart.Migrations;

public partial class AlertRuleIsNullable : Migration
{
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.AlterColumn<int>(
			name: "AlertRuleId",
			table: "Alerts",
			nullable: true,
			oldClrType: typeof(int));

		migrationBuilder.AlterColumn<int>(
			name: "AlertEscalationSubChainId",
			table: "Alerts",
			nullable: true,
			oldClrType: typeof(int));

		migrationBuilder.AlterColumn<int>(
			name: "AlertEscalationChainId",
			table: "Alerts",
			nullable: true,
			oldClrType: typeof(int));
	}

	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.AlterColumn<int>(
			name: "AlertRuleId",
			table: "Alerts",
			nullable: false,
			oldClrType: typeof(int),
			oldNullable: true);

		migrationBuilder.AlterColumn<int>(
			name: "AlertEscalationSubChainId",
			table: "Alerts",
			nullable: false,
			oldClrType: typeof(int),
			oldNullable: true);

		migrationBuilder.AlterColumn<int>(
			name: "AlertEscalationChainId",
			table: "Alerts",
			nullable: false,
			oldClrType: typeof(int),
			oldNullable: true);
	}
}
