#nullable disable

namespace LogicMonitor.Datamart.Migrations.NpgsqlMigrations;

/// <inheritdoc />
public partial class AlertResourceTemplateFields : Migration
{
	/// <inheritdoc />
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.AlterColumn<string>(
			name: "ResourceTemplateType",
			table: "Alerts",
			type: "character varying(10)",
			maxLength: 10,
			nullable: true,
			oldClrType: typeof(string),
			oldType: "character varying(10)",
			oldMaxLength: 10);

		migrationBuilder.AlterColumn<string>(
			name: "ResourceTemplateName",
			table: "Alerts",
			type: "character varying(50)",
			maxLength: 50,
			nullable: true,
			oldClrType: typeof(string),
			oldType: "character varying(50)",
			oldMaxLength: 50);
	}

	/// <inheritdoc />
	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.AlterColumn<string>(
			name: "ResourceTemplateType",
			table: "Alerts",
			type: "character varying(10)",
			maxLength: 10,
			nullable: false,
			defaultValue: "",
			oldClrType: typeof(string),
			oldType: "character varying(10)",
			oldMaxLength: 10,
			oldNullable: true);

		migrationBuilder.AlterColumn<string>(
			name: "ResourceTemplateName",
			table: "Alerts",
			type: "character varying(50)",
			maxLength: 50,
			nullable: false,
			defaultValue: "",
			oldClrType: typeof(string),
			oldType: "character varying(50)",
			oldMaxLength: 50,
			oldNullable: true);
	}
}
