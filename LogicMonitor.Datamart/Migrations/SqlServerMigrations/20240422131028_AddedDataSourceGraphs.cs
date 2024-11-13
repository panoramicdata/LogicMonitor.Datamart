#nullable disable

namespace LogicMonitor.Datamart.Migrations.SqlServerMigrations;

/// <inheritdoc />
public partial class AddedDataSourceGraphs : Migration
{
	/// <inheritdoc />
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.AddColumn<Guid>(
			name: "DataSourceGraphId",
			table: "DataSourceDataPoints",
			type: "uniqueidentifier",
			nullable: true);

		migrationBuilder.AddColumn<Guid>(
			name: "DataSourceGraphStoreItemId",
			table: "DataSourceDataPoints",
			type: "uniqueidentifier",
			nullable: true);

		migrationBuilder.CreateTable(
			name: "DataSourceGraphs",
			columns: table => new
			{
				Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				DataSourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
				Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
				VerticalLabel = table.Column<string>(type: "nvarchar(max)", nullable: false),
				IsRigid = table.Column<bool>(type: "bit", nullable: false),
				Width = table.Column<int>(type: "int", nullable: false),
				Height = table.Column<int>(type: "int", nullable: false),
				MaxValue = table.Column<double>(type: "float", nullable: true),
				MinValue = table.Column<double>(type: "float", nullable: true),
				DisplayPriority = table.Column<int>(type: "int", nullable: false),
				Timescale = table.Column<string>(type: "nvarchar(max)", nullable: false),
				IsBase1024 = table.Column<bool>(type: "bit", nullable: false),
				DataSourceStoreItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
				DatamartCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				DatamartLastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
				LogicMonitorId = table.Column<int>(type: "int", nullable: false),
				DatamartLastObserved = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_DataSourceGraphs", x => x.Id);
				table.ForeignKey(
					name: "FK_DataSourceGraphs_DataSources_DataSourceStoreItemId",
					column: x => x.DataSourceStoreItemId,
					principalTable: "DataSources",
					principalColumn: "Id");
			});

		migrationBuilder.CreateIndex(
			name: "IX_DataSourceDataPoints_DataSourceGraphStoreItemId",
			table: "DataSourceDataPoints",
			column: "DataSourceGraphStoreItemId");

		migrationBuilder.CreateIndex(
			name: "IX_DataSourceGraphs_DataSourceStoreItemId",
			table: "DataSourceGraphs",
			column: "DataSourceStoreItemId");

		migrationBuilder.AddForeignKey(
			name: "FK_DataSourceDataPoints_DataSourceGraphs_DataSourceGraphStoreItemId",
			table: "DataSourceDataPoints",
			column: "DataSourceGraphStoreItemId",
			principalTable: "DataSourceGraphs",
			principalColumn: "Id");
	}

	/// <inheritdoc />
	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropForeignKey(
			name: "FK_DataSourceDataPoints_DataSourceGraphs_DataSourceGraphStoreItemId",
			table: "DataSourceDataPoints");

		migrationBuilder.DropTable(
			name: "DataSourceGraphs");

		migrationBuilder.DropIndex(
			name: "IX_DataSourceDataPoints_DataSourceGraphStoreItemId",
			table: "DataSourceDataPoints");

		migrationBuilder.DropColumn(
			name: "DataSourceGraphId",
			table: "DataSourceDataPoints");

		migrationBuilder.DropColumn(
			name: "DataSourceGraphStoreItemId",
			table: "DataSourceDataPoints");
	}
}