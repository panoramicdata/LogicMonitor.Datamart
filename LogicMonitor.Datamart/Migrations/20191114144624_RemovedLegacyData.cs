using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LogicMonitor.Datamart.Migrations
{
    public partial class RemovedLegacyData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceDataSourceInstanceData");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_DeviceDataSourceInstances_Id",
                table: "DeviceDataSourceInstances");

            migrationBuilder.DropColumn(
                name: "AlertStatusPriority",
                table: "DeviceDataSources");

            migrationBuilder.DropColumn(
                name: "DataSourceDescription",
                table: "DeviceDataSources");

            migrationBuilder.DropColumn(
                name: "DataSourceDisplayName",
                table: "DeviceDataSources");

            migrationBuilder.DropColumn(
                name: "DataSourceName",
                table: "DeviceDataSources");

            migrationBuilder.DropColumn(
                name: "DataSourceType",
                table: "DeviceDataSources");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "DeviceDataSources");

            migrationBuilder.DropColumn(
                name: "DeviceDisplayName",
                table: "DeviceDataSources");

            migrationBuilder.DropColumn(
                name: "DeviceName",
                table: "DeviceDataSources");

            migrationBuilder.DropColumn(
                name: "GroupName",
                table: "DeviceDataSources");

            migrationBuilder.DropColumn(
                name: "InstanceAutoGroupEnabled",
                table: "DeviceDataSources");

            migrationBuilder.DropColumn(
                name: "InstanceCount",
                table: "DeviceDataSources");

            migrationBuilder.DropColumn(
                name: "IsAutoDiscoveryEnabled",
                table: "DeviceDataSources");

            migrationBuilder.DropColumn(
                name: "IsMonitoringDisabled",
                table: "DeviceDataSources");

            migrationBuilder.DropColumn(
                name: "IsMultiple",
                table: "DeviceDataSources");

            migrationBuilder.DropColumn(
                name: "MonitoringInstanceCount",
                table: "DeviceDataSources");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "DeviceDataSources");

            migrationBuilder.DropColumn(
                name: "NextAutoDiscoveryOnSeconds",
                table: "DeviceDataSources");

            migrationBuilder.DropColumn(
                name: "DeviceDisplayName",
                table: "DeviceDataSourceInstances");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceDataSources_DeviceId",
                table: "DeviceDataSources",
                column: "DeviceId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeviceDataSources_Devices_DeviceId",
                table: "DeviceDataSources",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeviceDataSources_Devices_DeviceId",
                table: "DeviceDataSources");

            migrationBuilder.DropIndex(
                name: "IX_DeviceDataSources_DeviceId",
                table: "DeviceDataSources");

            migrationBuilder.AddColumn<int>(
                name: "AlertStatusPriority",
                table: "DeviceDataSources",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DataSourceDescription",
                table: "DeviceDataSources",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DataSourceDisplayName",
                table: "DeviceDataSources",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DataSourceName",
                table: "DeviceDataSources",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DataSourceType",
                table: "DeviceDataSources",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "DeviceDataSources",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeviceDisplayName",
                table: "DeviceDataSources",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeviceName",
                table: "DeviceDataSources",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GroupName",
                table: "DeviceDataSources",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "InstanceAutoGroupEnabled",
                table: "DeviceDataSources",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "InstanceCount",
                table: "DeviceDataSources",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoDiscoveryEnabled",
                table: "DeviceDataSources",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMonitoringDisabled",
                table: "DeviceDataSources",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMultiple",
                table: "DeviceDataSources",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MonitoringInstanceCount",
                table: "DeviceDataSources",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "DeviceDataSources",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "NextAutoDiscoveryOnSeconds",
                table: "DeviceDataSources",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "DeviceDisplayName",
                table: "DeviceDataSourceInstances",
                nullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_DeviceDataSourceInstances_Id",
                table: "DeviceDataSourceInstances",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "DeviceDataSourceInstanceData",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DataPointName = table.Column<string>(nullable: true),
                    DateTime = table.Column<DateTime>(nullable: false),
                    DeviceDataSourceInstanceId = table.Column<int>(nullable: false),
                    Value = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceDataSourceInstanceData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceDataSourceInstanceData_DeviceDataSourceInstances_DeviceDataSourceInstanceId",
                        column: x => x.DeviceDataSourceInstanceId,
                        principalTable: "DeviceDataSourceInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceDataSourceInstanceData_DateTime",
                table: "DeviceDataSourceInstanceData",
                column: "DateTime");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceDataSourceInstanceData_DeviceDataSourceInstanceId_DataPointName",
                table: "DeviceDataSourceInstanceData",
                columns: new[] { "DeviceDataSourceInstanceId", "DataPointName" });
        }
    }
}
