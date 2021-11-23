namespace LogicMonitor.Datamart.Migrations;

public partial class AddedlastObservedUtc : Migration
{
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.AddColumn<DateTime>(
			 name: "DatamartLastObservedUtc",
			 table: "Websites",
			 nullable: false,
			 defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

		migrationBuilder.AddColumn<DateTime>(
			 name: "DatamartLastObservedUtc",
			 table: "WebsiteGroups",
			 nullable: false,
			 defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

		migrationBuilder.AddColumn<DateTime>(
			 name: "DatamartLastObservedUtc",
			 table: "EventSources",
			 nullable: false,
			 defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

		migrationBuilder.AddColumn<DateTime>(
			 name: "DatamartLastObservedUtc",
			 table: "EscalationChains",
			 nullable: false,
			 defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

		migrationBuilder.AddColumn<DateTime>(
			 name: "DatamartLastObservedUtc",
			 table: "Devices",
			 nullable: false,
			 defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

		migrationBuilder.AddColumn<DateTime>(
			 name: "DatamartLastObservedUtc",
			 table: "DeviceGroups",
			 nullable: false,
			 defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

		migrationBuilder.AddColumn<DateTime>(
			 name: "DatamartLastObservedUtc",
			 table: "DeviceDataSources",
			 nullable: false,
			 defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

		migrationBuilder.AddColumn<DateTime>(
			 name: "DatamartLastObservedUtc",
			 table: "DeviceDataSourceInstances",
			 nullable: false,
			 defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

		migrationBuilder.AddColumn<DateTime>(
			 name: "DatamartLastObservedUtc",
			 table: "DataSources",
			 nullable: false,
			 defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

		migrationBuilder.AddColumn<DateTime>(
			 name: "DatamartLastObservedUtc",
			 table: "DataSourceDataPoints",
			 nullable: false,
			 defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

		migrationBuilder.AddColumn<DateTime>(
			 name: "DatamartLastObservedUtc",
			 table: "ConfigSources",
			 nullable: false,
			 defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

		migrationBuilder.AddColumn<DateTime>(
			 name: "DatamartLastObservedUtc",
			 table: "Collectors",
			 nullable: false,
			 defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

		migrationBuilder.AddColumn<DateTime>(
			 name: "DatamartLastObservedUtc",
			 table: "CollectorGroups",
			 nullable: false,
			 defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

		migrationBuilder.AddColumn<DateTime>(
			 name: "DatamartLastObservedUtc",
			 table: "AlertRules",
			 nullable: false,
			 defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
	}

	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropColumn(
			 name: "DatamartLastObservedUtc",
			 table: "Websites");

		migrationBuilder.DropColumn(
			 name: "DatamartLastObservedUtc",
			 table: "WebsiteGroups");

		migrationBuilder.DropColumn(
			 name: "DatamartLastObservedUtc",
			 table: "EventSources");

		migrationBuilder.DropColumn(
			 name: "DatamartLastObservedUtc",
			 table: "EscalationChains");

		migrationBuilder.DropColumn(
			 name: "DatamartLastObservedUtc",
			 table: "Devices");

		migrationBuilder.DropColumn(
			 name: "DatamartLastObservedUtc",
			 table: "DeviceGroups");

		migrationBuilder.DropColumn(
			 name: "DatamartLastObservedUtc",
			 table: "DeviceDataSources");

		migrationBuilder.DropColumn(
			 name: "DatamartLastObservedUtc",
			 table: "DeviceDataSourceInstances");

		migrationBuilder.DropColumn(
			 name: "DatamartLastObservedUtc",
			 table: "DataSources");

		migrationBuilder.DropColumn(
			 name: "DatamartLastObservedUtc",
			 table: "DataSourceDataPoints");

		migrationBuilder.DropColumn(
			 name: "DatamartLastObservedUtc",
			 table: "ConfigSources");

		migrationBuilder.DropColumn(
			 name: "DatamartLastObservedUtc",
			 table: "Collectors");

		migrationBuilder.DropColumn(
			 name: "DatamartLastObservedUtc",
			 table: "CollectorGroups");

		migrationBuilder.DropColumn(
			 name: "DatamartLastObservedUtc",
			 table: "AlertRules");
	}
}
