using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicMonitor.Datamart.Migrations.SqlServerMigrations
{
    /// <inheritdoc />
    public partial class AddedExcludingSdtColumnsToTimeSeriesDataAggregationStoreItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AvailabilityPercent2ExcludingSdt",
                table: "TimeSeriesDataAggregations",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "AvailabilityPercentExcludingSdt",
                table: "TimeSeriesDataAggregations",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Centile05ExcludingSdt",
                table: "TimeSeriesDataAggregations",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Centile10ExcludingSdt",
                table: "TimeSeriesDataAggregations",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Centile25ExcludingSdt",
                table: "TimeSeriesDataAggregations",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Centile50ExcludingSdt",
                table: "TimeSeriesDataAggregations",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Centile75ExcludingSdt",
                table: "TimeSeriesDataAggregations",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Centile90ExcludingSdt",
                table: "TimeSeriesDataAggregations",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Centile95ExcludingSdt",
                table: "TimeSeriesDataAggregations",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DataCountExcludingSdt",
                table: "TimeSeriesDataAggregations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "FirstExcludingSdt",
                table: "TimeSeriesDataAggregations",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "FirstWithDataExcludingSdt",
                table: "TimeSeriesDataAggregations",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LastExcludingSdt",
                table: "TimeSeriesDataAggregations",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LastWithDataExcludingSdt",
                table: "TimeSeriesDataAggregations",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MaxExcludingSdt",
                table: "TimeSeriesDataAggregations",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MinExcludingSdt",
                table: "TimeSeriesDataAggregations",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NoDataCountExcludingSdt",
                table: "TimeSeriesDataAggregations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "SumExcludingSdt",
                table: "TimeSeriesDataAggregations",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "SumSquaredExcludingSdt",
                table: "TimeSeriesDataAggregations",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailabilityPercent2ExcludingSdt",
                table: "TimeSeriesDataAggregations");

            migrationBuilder.DropColumn(
                name: "AvailabilityPercentExcludingSdt",
                table: "TimeSeriesDataAggregations");

            migrationBuilder.DropColumn(
                name: "Centile05ExcludingSdt",
                table: "TimeSeriesDataAggregations");

            migrationBuilder.DropColumn(
                name: "Centile10ExcludingSdt",
                table: "TimeSeriesDataAggregations");

            migrationBuilder.DropColumn(
                name: "Centile25ExcludingSdt",
                table: "TimeSeriesDataAggregations");

            migrationBuilder.DropColumn(
                name: "Centile50ExcludingSdt",
                table: "TimeSeriesDataAggregations");

            migrationBuilder.DropColumn(
                name: "Centile75ExcludingSdt",
                table: "TimeSeriesDataAggregations");

            migrationBuilder.DropColumn(
                name: "Centile90ExcludingSdt",
                table: "TimeSeriesDataAggregations");

            migrationBuilder.DropColumn(
                name: "Centile95ExcludingSdt",
                table: "TimeSeriesDataAggregations");

            migrationBuilder.DropColumn(
                name: "DataCountExcludingSdt",
                table: "TimeSeriesDataAggregations");

            migrationBuilder.DropColumn(
                name: "FirstExcludingSdt",
                table: "TimeSeriesDataAggregations");

            migrationBuilder.DropColumn(
                name: "FirstWithDataExcludingSdt",
                table: "TimeSeriesDataAggregations");

            migrationBuilder.DropColumn(
                name: "LastExcludingSdt",
                table: "TimeSeriesDataAggregations");

            migrationBuilder.DropColumn(
                name: "LastWithDataExcludingSdt",
                table: "TimeSeriesDataAggregations");

            migrationBuilder.DropColumn(
                name: "MaxExcludingSdt",
                table: "TimeSeriesDataAggregations");

            migrationBuilder.DropColumn(
                name: "MinExcludingSdt",
                table: "TimeSeriesDataAggregations");

            migrationBuilder.DropColumn(
                name: "NoDataCountExcludingSdt",
                table: "TimeSeriesDataAggregations");

            migrationBuilder.DropColumn(
                name: "SumExcludingSdt",
                table: "TimeSeriesDataAggregations");

            migrationBuilder.DropColumn(
                name: "SumSquaredExcludingSdt",
                table: "TimeSeriesDataAggregations");
        }
    }
}
