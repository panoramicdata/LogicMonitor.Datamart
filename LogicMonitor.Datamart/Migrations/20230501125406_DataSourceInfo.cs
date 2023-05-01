#nullable disable

namespace LogicMonitor.Datamart.Migrations
{
	/// <inheritdoc />
	public partial class DataSourceInfo : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<string>(
				name: "AppliesTo",
				table: "DataSources",
				type: "text",
				nullable: false,
				defaultValue: "");

			migrationBuilder.AddColumn<string>(
				name: "AuditVersion",
				table: "DataSources",
				type: "text",
				nullable: false,
				defaultValue: "");

			migrationBuilder.AddColumn<string>(
				name: "CollectionMethod",
				table: "DataSources",
				type: "text",
				nullable: false,
				defaultValue: "");

			migrationBuilder.AddColumn<string>(
				name: "Collector",
				table: "DataSources",
				type: "text",
				nullable: false,
				defaultValue: "");

			migrationBuilder.AddColumn<string>(
				name: "DisplayName",
				table: "DataSources",
				type: "text",
				nullable: false,
				defaultValue: "");

			migrationBuilder.AddColumn<bool>(
				name: "HasMultiInstances",
				table: "DataSources",
				type: "boolean",
				nullable: false,
				defaultValue: false);

			migrationBuilder.AddColumn<int>(
				name: "PollingIntervalSeconds",
				table: "DataSources",
				type: "integer",
				nullable: false,
				defaultValue: 0);

			migrationBuilder.AddColumn<string>(
				name: "Technology",
				table: "DataSources",
				type: "text",
				nullable: false,
				defaultValue: "");

			migrationBuilder.AddColumn<string>(
				name: "Version",
				table: "DataSources",
				type: "text",
				nullable: false,
				defaultValue: "");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "AppliesTo",
				table: "DataSources");

			migrationBuilder.DropColumn(
				name: "AuditVersion",
				table: "DataSources");

			migrationBuilder.DropColumn(
				name: "CollectionMethod",
				table: "DataSources");

			migrationBuilder.DropColumn(
				name: "Collector",
				table: "DataSources");

			migrationBuilder.DropColumn(
				name: "DisplayName",
				table: "DataSources");

			migrationBuilder.DropColumn(
				name: "HasMultiInstances",
				table: "DataSources");

			migrationBuilder.DropColumn(
				name: "PollingIntervalSeconds",
				table: "DataSources");

			migrationBuilder.DropColumn(
				name: "Technology",
				table: "DataSources");

			migrationBuilder.DropColumn(
				name: "Version",
				table: "DataSources");
		}
	}
}