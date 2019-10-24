using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LogicMonitor.Datamart.Migrations
{
	public partial class DataSourceDataPoints : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				 name: "DataSourceDataPoints",
				 columns: table => new
				 {
					 DatamartId = table.Column<int>(nullable: false)
							.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
					 DatamartCreatedUtc = table.Column<DateTime>(nullable: false),
					 DatamartLastModifiedUtc = table.Column<DateTime>(nullable: false),
					 Id = table.Column<int>(nullable: false),
					 DataSourceId = table.Column<int>(nullable: false),
					 Name = table.Column<string>(nullable: true),
					 Description = table.Column<string>(nullable: true),
					 MeasurementUnit = table.Column<string>(nullable: true)
				 },
				 constraints: table =>
				 {
					 table.PrimaryKey("PK_DataSourceDataPoints", x => x.DatamartId);
					 table.ForeignKey(
							  name: "FK_DataSourceDataPoints_DataSources_DataSourceId",
							  column: x => x.DataSourceId,
							  principalTable: "DataSources",
							  principalColumn: "DatamartId",
							  onDelete: ReferentialAction.Cascade);
				 });

			migrationBuilder.CreateIndex(
				 name: "IX_DataSourceDataPoints_DataSourceId",
				 table: "DataSourceDataPoints",
				 column: "DataSourceId");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				 name: "DataSourceDataPoints");
		}
	}
}
