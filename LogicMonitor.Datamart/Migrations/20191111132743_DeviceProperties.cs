using Microsoft.EntityFrameworkCore.Migrations;

namespace LogicMonitor.Datamart.Migrations
{
    public partial class DeviceProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Property1",
                table: "Devices",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Property2",
                table: "Devices",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Property3",
                table: "Devices",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Property4",
                table: "Devices",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Property5",
                table: "Devices",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Property1",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Property2",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Property3",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Property4",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Property5",
                table: "Devices");
        }
    }
}
