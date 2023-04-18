#nullable disable

namespace LogicMonitor.Datamart.Migrations
{
	/// <inheritdoc />
	public partial class AddedDeviceProperties : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<int>(
				name: "CriticalCount",
				table: "TimeSeriesDataAggregations",
				type: "integer",
				nullable: false,
				defaultValue: 0);

			migrationBuilder.AddColumn<int>(
				name: "ErrorCount",
				table: "TimeSeriesDataAggregations",
				type: "integer",
				nullable: false,
				defaultValue: 0);

			migrationBuilder.AddColumn<int>(
				name: "NormalCount",
				table: "TimeSeriesDataAggregations",
				type: "integer",
				nullable: false,
				defaultValue: 0);

			migrationBuilder.AddColumn<int>(
				name: "WarningCount",
				table: "TimeSeriesDataAggregations",
				type: "integer",
				nullable: false,
				defaultValue: 0);

			migrationBuilder.AddColumn<string>(
				name: "Property10",
				table: "Devices",
				type: "text",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "Property11",
				table: "Devices",
				type: "text",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "Property12",
				table: "Devices",
				type: "text",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "Property13",
				table: "Devices",
				type: "text",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "Property14",
				table: "Devices",
				type: "text",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "Property15",
				table: "Devices",
				type: "text",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "Property16",
				table: "Devices",
				type: "text",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "Property17",
				table: "Devices",
				type: "text",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "Property18",
				table: "Devices",
				type: "text",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "Property19",
				table: "Devices",
				type: "text",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "Property20",
				table: "Devices",
				type: "text",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "Property6",
				table: "Devices",
				type: "text",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "Property7",
				table: "Devices",
				type: "text",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "Property8",
				table: "Devices",
				type: "text",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "Property9",
				table: "Devices",
				type: "text",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "EffectiveAlertExpression",
				table: "DeviceDataSourceInstances",
				type: "text",
				nullable: false,
				defaultValue: "");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "CriticalCount",
				table: "TimeSeriesDataAggregations");

			migrationBuilder.DropColumn(
				name: "ErrorCount",
				table: "TimeSeriesDataAggregations");

			migrationBuilder.DropColumn(
				name: "NormalCount",
				table: "TimeSeriesDataAggregations");

			migrationBuilder.DropColumn(
				name: "WarningCount",
				table: "TimeSeriesDataAggregations");

			migrationBuilder.DropColumn(
				name: "Property10",
				table: "Devices");

			migrationBuilder.DropColumn(
				name: "Property11",
				table: "Devices");

			migrationBuilder.DropColumn(
				name: "Property12",
				table: "Devices");

			migrationBuilder.DropColumn(
				name: "Property13",
				table: "Devices");

			migrationBuilder.DropColumn(
				name: "Property14",
				table: "Devices");

			migrationBuilder.DropColumn(
				name: "Property15",
				table: "Devices");

			migrationBuilder.DropColumn(
				name: "Property16",
				table: "Devices");

			migrationBuilder.DropColumn(
				name: "Property17",
				table: "Devices");

			migrationBuilder.DropColumn(
				name: "Property18",
				table: "Devices");

			migrationBuilder.DropColumn(
				name: "Property19",
				table: "Devices");

			migrationBuilder.DropColumn(
				name: "Property20",
				table: "Devices");

			migrationBuilder.DropColumn(
				name: "Property6",
				table: "Devices");

			migrationBuilder.DropColumn(
				name: "Property7",
				table: "Devices");

			migrationBuilder.DropColumn(
				name: "Property8",
				table: "Devices");

			migrationBuilder.DropColumn(
				name: "Property9",
				table: "Devices");

			migrationBuilder.DropColumn(
				name: "EffectiveAlertExpression",
				table: "DeviceDataSourceInstances");
		}
	}
}