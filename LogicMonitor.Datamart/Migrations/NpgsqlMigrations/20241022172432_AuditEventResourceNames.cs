using System.Collections.ObjectModel;

#nullable disable

namespace LogicMonitor.Datamart.Migrations.NpgsqlMigrations;

/// <inheritdoc />
public partial class AuditEventResourceNames : Migration
{
	/// <inheritdoc />
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.AlterColumn<string>(
			name: "ResourceNames",
			table: "AuditEvents",
			type: "character varying(200)",
			maxLength: 200,
			nullable: true,
			oldClrType: typeof(Collection<string>),
			oldType: "text[]",
			oldMaxLength: 200,
			oldNullable: true);
	}

	/// <inheritdoc />
	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.AlterColumn<Collection<string>>(
			name: "ResourceNames",
			table: "AuditEvents",
			type: "text[]",
			maxLength: 200,
			nullable: true,
			oldClrType: typeof(string),
			oldType: "character varying(200)",
			oldMaxLength: 200,
			oldNullable: true);
	}
}
