using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicMonitor.Datamart.Migrations.NpgsqlMigrations
{
    /// <inheritdoc />
    public partial class MadeDataSourcesCollectionAttributeIpNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CollectionAttributeIp",
                table: "DataSources",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CollectionAttributeIp",
                table: "DataSources",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
