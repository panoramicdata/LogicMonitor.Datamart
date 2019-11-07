using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LogicMonitor.Datamart.Migrations
{
    public partial class MoveToMultipleAggregationTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceDataSourceInstanceAggregatedData");

            migrationBuilder.DropColumn(
                name: "DataSourceName",
                table: "DeviceDataSourceInstances");

            migrationBuilder.DropColumn(
                name: "DataSourceType",
                table: "DeviceDataSourceInstances");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DataSourceName",
                table: "DeviceDataSourceInstances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DataSourceType",
                table: "DeviceDataSourceInstances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DeviceDataSourceInstanceAggregatedData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataCount = table.Column<int>(type: "int", nullable: false),
                    DataPointId = table.Column<int>(type: "int", nullable: false),
                    DeviceDataSourceInstanceId = table.Column<int>(type: "int", nullable: false),
                    Hour = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Max = table.Column<double>(type: "float", nullable: true),
                    Min = table.Column<double>(type: "float", nullable: true),
                    NoDataCount = table.Column<int>(type: "int", nullable: false),
                    Sum = table.Column<double>(type: "float", nullable: true),
                    SumSquared = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceDataSourceInstanceAggregatedData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceDataSourceInstanceAggregatedData_DataSourceDataPoints_DataPointId",
                        column: x => x.DataPointId,
                        principalTable: "DataSourceDataPoints",
                        principalColumn: "DatamartId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeviceDataSourceInstanceAggregatedData_DeviceDataSourceInstances_DeviceDataSourceInstanceId",
                        column: x => x.DeviceDataSourceInstanceId,
                        principalTable: "DeviceDataSourceInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceDataSourceInstanceAggregatedData_DataPointId",
                table: "DeviceDataSourceInstanceAggregatedData",
                column: "DataPointId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceDataSourceInstanceAggregatedData_DeviceDataSourceInstanceId_DataPointId_Hour",
                table: "DeviceDataSourceInstanceAggregatedData",
                columns: new[] { "DeviceDataSourceInstanceId", "DataPointId", "Hour" });
        }
    }
}
