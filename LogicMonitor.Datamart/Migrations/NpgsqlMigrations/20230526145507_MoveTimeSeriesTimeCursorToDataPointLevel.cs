#nullable disable

namespace LogicMonitor.Datamart.Migrations.NpgsqlMigrations
{
	/// <inheritdoc />
	public partial class MoveTimeSeriesTimeCursorToDataPointLevel : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<DateTimeOffset>(
				name: "DataCompleteTo",
				table: "DeviceDataSourceInstanceDataPoints",
				type: "timestamp with time zone",
				nullable: true);

			// Copy the data from DeviceDataSourceInstance to DeviceDataSourceInstanceDataPoint using postgres
			migrationBuilder.Sql("""
UPDATE "DeviceDataSourceInstanceDataPoints"
SET "DataCompleteTo" = "DeviceDataSourceInstances"."DataCompleteTo"
FROM "DeviceDataSourceInstances"
WHERE "DeviceDataSourceInstances"."Id" = "DeviceDataSourceInstanceDataPoints"."DeviceDataSourceInstanceId"
""");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "DataCompleteTo",
				table: "DeviceDataSourceInstanceDataPoints");
		}
	}
}
