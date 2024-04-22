using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicMonitor.Datamart.Migrations.NpgsqlMigrations
{
    /// <inheritdoc />
    public partial class DatabaseChangesForNCalcProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Condition",
                table: "DataSourceDataPoints");

            migrationBuilder.AddColumn<string>(
                name: "InstanceProperty1",
                table: "DeviceDataSourceInstances",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InstanceProperty10",
                table: "DeviceDataSourceInstances",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InstanceProperty2",
                table: "DeviceDataSourceInstances",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InstanceProperty3",
                table: "DeviceDataSourceInstances",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InstanceProperty4",
                table: "DeviceDataSourceInstances",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InstanceProperty5",
                table: "DeviceDataSourceInstances",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InstanceProperty6",
                table: "DeviceDataSourceInstances",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InstanceProperty7",
                table: "DeviceDataSourceInstances",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InstanceProperty8",
                table: "DeviceDataSourceInstances",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InstanceProperty9",
                table: "DeviceDataSourceInstances",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InstanceDatapointProperty1",
                table: "DeviceDataSourceInstanceDataPoints",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InstanceDatapointProperty10",
                table: "DeviceDataSourceInstanceDataPoints",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InstanceDatapointProperty2",
                table: "DeviceDataSourceInstanceDataPoints",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InstanceDatapointProperty3",
                table: "DeviceDataSourceInstanceDataPoints",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InstanceDatapointProperty4",
                table: "DeviceDataSourceInstanceDataPoints",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InstanceDatapointProperty5",
                table: "DeviceDataSourceInstanceDataPoints",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InstanceDatapointProperty6",
                table: "DeviceDataSourceInstanceDataPoints",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InstanceDatapointProperty7",
                table: "DeviceDataSourceInstanceDataPoints",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InstanceDatapointProperty8",
                table: "DeviceDataSourceInstanceDataPoints",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InstanceDatapointProperty9",
                table: "DeviceDataSourceInstanceDataPoints",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InstanceProperty1",
                table: "DeviceDataSourceInstances");

            migrationBuilder.DropColumn(
                name: "InstanceProperty10",
                table: "DeviceDataSourceInstances");

            migrationBuilder.DropColumn(
                name: "InstanceProperty2",
                table: "DeviceDataSourceInstances");

            migrationBuilder.DropColumn(
                name: "InstanceProperty3",
                table: "DeviceDataSourceInstances");

            migrationBuilder.DropColumn(
                name: "InstanceProperty4",
                table: "DeviceDataSourceInstances");

            migrationBuilder.DropColumn(
                name: "InstanceProperty5",
                table: "DeviceDataSourceInstances");

            migrationBuilder.DropColumn(
                name: "InstanceProperty6",
                table: "DeviceDataSourceInstances");

            migrationBuilder.DropColumn(
                name: "InstanceProperty7",
                table: "DeviceDataSourceInstances");

            migrationBuilder.DropColumn(
                name: "InstanceProperty8",
                table: "DeviceDataSourceInstances");

            migrationBuilder.DropColumn(
                name: "InstanceProperty9",
                table: "DeviceDataSourceInstances");

            migrationBuilder.DropColumn(
                name: "InstanceDatapointProperty1",
                table: "DeviceDataSourceInstanceDataPoints");

            migrationBuilder.DropColumn(
                name: "InstanceDatapointProperty10",
                table: "DeviceDataSourceInstanceDataPoints");

            migrationBuilder.DropColumn(
                name: "InstanceDatapointProperty2",
                table: "DeviceDataSourceInstanceDataPoints");

            migrationBuilder.DropColumn(
                name: "InstanceDatapointProperty3",
                table: "DeviceDataSourceInstanceDataPoints");

            migrationBuilder.DropColumn(
                name: "InstanceDatapointProperty4",
                table: "DeviceDataSourceInstanceDataPoints");

            migrationBuilder.DropColumn(
                name: "InstanceDatapointProperty5",
                table: "DeviceDataSourceInstanceDataPoints");

            migrationBuilder.DropColumn(
                name: "InstanceDatapointProperty6",
                table: "DeviceDataSourceInstanceDataPoints");

            migrationBuilder.DropColumn(
                name: "InstanceDatapointProperty7",
                table: "DeviceDataSourceInstanceDataPoints");

            migrationBuilder.DropColumn(
                name: "InstanceDatapointProperty8",
                table: "DeviceDataSourceInstanceDataPoints");

            migrationBuilder.DropColumn(
                name: "InstanceDatapointProperty9",
                table: "DeviceDataSourceInstanceDataPoints");

            migrationBuilder.AddColumn<string>(
                name: "Condition",
                table: "DataSourceDataPoints",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
