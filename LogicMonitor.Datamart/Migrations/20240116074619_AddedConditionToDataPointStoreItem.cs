using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicMonitor.Datamart.Migrations
{
    /// <inheritdoc />
    public partial class AddedConditionToDataPointStoreItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Condition",
                table: "DataSourceDataPoints",
                type: "text",
                nullable: false,
                defaultValue: "true");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Condition",
                table: "DataSourceDataPoints");
        }
    }
}
