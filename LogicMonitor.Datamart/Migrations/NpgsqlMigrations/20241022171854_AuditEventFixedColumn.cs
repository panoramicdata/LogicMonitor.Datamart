using System.Collections.ObjectModel;

#nullable disable

namespace LogicMonitor.Datamart.Migrations.NpgsqlMigrations
{
	/// <inheritdoc />
	public partial class AuditEventFixedColumn : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<string>(
				name: "ResourceIds",
				table: "AuditEvents",
				type: "character varying(50)",
				maxLength: 50,
				nullable: true,
				oldClrType: typeof(Collection<int>),
				oldType: "integer[]",
				oldMaxLength: 50,
				oldNullable: true);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<Collection<int>>(
				name: "ResourceIds",
				table: "AuditEvents",
				type: "integer[]",
				maxLength: 50,
				nullable: true,
				oldClrType: typeof(string),
				oldType: "character varying(50)",
				oldMaxLength: 50,
				oldNullable: true);
		}
	}
}
