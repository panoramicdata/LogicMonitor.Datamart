#nullable disable

namespace LogicMonitor.Datamart.Migrations
{
	/// <inheritdoc />
	public partial class CalculationsAndTags : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<string>(
				name: "Calculation",
				table: "DataSourceDataPoints",
				type: "text",
				nullable: false,
				defaultValue: "");

			migrationBuilder.AddColumn<string>(
				name: "Tags",
				table: "DataSourceDataPoints",
				type: "text",
				nullable: false,
				defaultValue: "");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "Calculation",
				table: "DataSourceDataPoints");

			migrationBuilder.DropColumn(
				name: "Tags",
				table: "DataSourceDataPoints");
		}
	}
}