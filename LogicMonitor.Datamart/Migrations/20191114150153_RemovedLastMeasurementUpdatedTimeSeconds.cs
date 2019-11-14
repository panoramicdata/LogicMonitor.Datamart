using Microsoft.EntityFrameworkCore.Migrations;

namespace LogicMonitor.Datamart.Migrations
{
    public partial class RemovedLastMeasurementUpdatedTimeSeconds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastMeasurementUpdatedTimeSeconds",
                table: "DeviceDataSourceInstances");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "LastMeasurementUpdatedTimeSeconds",
                table: "DeviceDataSourceInstances",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
