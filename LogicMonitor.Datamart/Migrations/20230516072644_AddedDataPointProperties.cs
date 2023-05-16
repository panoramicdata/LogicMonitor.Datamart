using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicMonitor.Datamart.Migrations
{
    /// <inheritdoc />
    public partial class AddedDataPointProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Property1",
                table: "DataSourceDataPoints",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Property10",
                table: "DataSourceDataPoints",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Property2",
                table: "DataSourceDataPoints",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Property3",
                table: "DataSourceDataPoints",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Property4",
                table: "DataSourceDataPoints",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Property5",
                table: "DataSourceDataPoints",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Property6",
                table: "DataSourceDataPoints",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Property7",
                table: "DataSourceDataPoints",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Property8",
                table: "DataSourceDataPoints",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Property9",
                table: "DataSourceDataPoints",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Property1",
                table: "DataSourceDataPoints");

            migrationBuilder.DropColumn(
                name: "Property10",
                table: "DataSourceDataPoints");

            migrationBuilder.DropColumn(
                name: "Property2",
                table: "DataSourceDataPoints");

            migrationBuilder.DropColumn(
                name: "Property3",
                table: "DataSourceDataPoints");

            migrationBuilder.DropColumn(
                name: "Property4",
                table: "DataSourceDataPoints");

            migrationBuilder.DropColumn(
                name: "Property5",
                table: "DataSourceDataPoints");

            migrationBuilder.DropColumn(
                name: "Property6",
                table: "DataSourceDataPoints");

            migrationBuilder.DropColumn(
                name: "Property7",
                table: "DataSourceDataPoints");

            migrationBuilder.DropColumn(
                name: "Property8",
                table: "DataSourceDataPoints");

            migrationBuilder.DropColumn(
                name: "Property9",
                table: "DataSourceDataPoints");
        }
    }
}
