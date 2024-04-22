#nullable disable

namespace LogicMonitor.Datamart.Migrations.NpgsqlMigrations
{
	/// <inheritdoc />
	public partial class TimeSeriesDataSyncTimings : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<long>(
				name: "LastTimeSeriesDataSyncDurationMs",
				table: "Devices",
				type: "bigint",
				nullable: true);

			migrationBuilder.AddColumn<long>(
				name: "LastTimeSeriesDataSyncDurationMs",
				table: "DataSources",
				type: "bigint",
				nullable: true);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "LastTimeSeriesDataSyncDurationMs",
				table: "Devices");

			migrationBuilder.DropColumn(
				name: "LastTimeSeriesDataSyncDurationMs",
				table: "DataSources");
		}
	}
}
