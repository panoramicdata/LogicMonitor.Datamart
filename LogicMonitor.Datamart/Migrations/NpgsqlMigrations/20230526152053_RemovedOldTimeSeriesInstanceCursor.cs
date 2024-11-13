#nullable disable

namespace LogicMonitor.Datamart.Migrations.NpgsqlMigrations;

/// <inheritdoc />
public partial class RemovedOldTimeSeriesInstanceCursor : Migration
{
	/// <inheritdoc />
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropColumn(
			name: "DataCompleteTo",
			table: "DeviceDataSourceInstances");
	}

	/// <inheritdoc />
	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.AddColumn<DateTimeOffset>(
			name: "DataCompleteTo",
			table: "DeviceDataSourceInstances",
			type: "timestamp with time zone",
			nullable: true);
	}
}
