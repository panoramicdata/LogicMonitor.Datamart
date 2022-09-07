#nullable disable

namespace LogicMonitor.Datamart.Migrations
{
	public partial class NullableField_Website_Script : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<string>(
				name: "Script",
				table: "Websites",
				type: "nvarchar(max)",
				nullable: true,
				oldClrType: typeof(string),
				oldType: "nvarchar(max)");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<string>(
				name: "Script",
				table: "Websites",
				type: "nvarchar(max)",
				nullable: false,
				defaultValue: "",
				oldClrType: typeof(string),
				oldType: "nvarchar(max)",
				oldNullable: true);
		}
	}
}
