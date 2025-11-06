using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicMonitor.Datamart.Migrations.SqlServerMigrations
{
    /// <inheritdoc />
    public partial class AddedAlertLevelSdtCountsToTimeSeriesDataAggregationStoreItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CriticalSdtCount",
                table: "TimeSeriesDataAggregations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ErrorSdtCount",
                table: "TimeSeriesDataAggregations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NormalOrSdtCount",
                table: "TimeSeriesDataAggregations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WarningSdtCount",
                table: "TimeSeriesDataAggregations",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CriticalSdtCount",
                table: "TimeSeriesDataAggregations");

            migrationBuilder.DropColumn(
                name: "ErrorSdtCount",
                table: "TimeSeriesDataAggregations");

            migrationBuilder.DropColumn(
                name: "NormalOrSdtCount",
                table: "TimeSeriesDataAggregations");

            migrationBuilder.DropColumn(
                name: "WarningSdtCount",
                table: "TimeSeriesDataAggregations");
        }
    }
}
