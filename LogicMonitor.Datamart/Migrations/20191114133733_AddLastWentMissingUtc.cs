using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LogicMonitor.Datamart.Migrations
{
    public partial class AddLastWentMissingUtc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastWentMissingUtc",
                table: "DeviceDataSourceInstances",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeviceDataSourceInstances_LastWentMissingUtc",
                table: "DeviceDataSourceInstances",
                column: "LastWentMissingUtc");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DeviceDataSourceInstances_LastWentMissingUtc",
                table: "DeviceDataSourceInstances");

            migrationBuilder.DropColumn(
                name: "LastWentMissingUtc",
                table: "DeviceDataSourceInstances");
        }
    }
}
