using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicMonitor.Datamart.Migrations.NpgsqlMigrations
{
    /// <inheritdoc />
    public partial class AddedAvailabilityPercent2ToTimeSeriesDataAggregationStoreItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AvailabilityPercent2",
                table: "TimeSeriesDataAggregations",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailabilityPercent2",
                table: "TimeSeriesDataAggregations");
        }
    }
}
