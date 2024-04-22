#nullable disable

namespace LogicMonitor.Datamart.Migrations.NpgsqlMigrations
{
	/// <inheritdoc />
	public partial class AddOverviewGraphs : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_DataSourceGraphs_DataSources_DataSourceStoreItemId",
				table: "DataSourceGraphs");

			migrationBuilder.DropIndex(
				name: "IX_DataSourceGraphs_DataSourceStoreItemId",
				table: "DataSourceGraphs");

			migrationBuilder.DropColumn(
				name: "DataSourceStoreItemId",
				table: "DataSourceGraphs");

			migrationBuilder.AddColumn<bool>(
				name: "IsOverview",
				table: "DataSourceGraphs",
				type: "boolean",
				nullable: false,
				defaultValue: false);

			migrationBuilder.CreateIndex(
				name: "IX_DataSourceGraphs_DataSourceId",
				table: "DataSourceGraphs",
				column: "DataSourceId");

			migrationBuilder.AddForeignKey(
				name: "FK_DataSourceGraphs_DataSources_DataSourceId",
				table: "DataSourceGraphs",
				column: "DataSourceId",
				principalTable: "DataSources",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_DataSourceGraphs_DataSources_DataSourceId",
				table: "DataSourceGraphs");

			migrationBuilder.DropIndex(
				name: "IX_DataSourceGraphs_DataSourceId",
				table: "DataSourceGraphs");

			migrationBuilder.DropColumn(
				name: "IsOverview",
				table: "DataSourceGraphs");

			migrationBuilder.AddColumn<Guid>(
				name: "DataSourceStoreItemId",
				table: "DataSourceGraphs",
				type: "uuid",
				nullable: true);

			migrationBuilder.CreateIndex(
				name: "IX_DataSourceGraphs_DataSourceStoreItemId",
				table: "DataSourceGraphs",
				column: "DataSourceStoreItemId");

			migrationBuilder.AddForeignKey(
				name: "FK_DataSourceGraphs_DataSources_DataSourceStoreItemId",
				table: "DataSourceGraphs",
				column: "DataSourceStoreItemId",
				principalTable: "DataSources",
				principalColumn: "Id");
		}
	}
}