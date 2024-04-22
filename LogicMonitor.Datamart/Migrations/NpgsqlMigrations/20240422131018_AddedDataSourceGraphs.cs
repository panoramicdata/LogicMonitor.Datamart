#nullable disable

namespace LogicMonitor.Datamart.Migrations.NpgsqlMigrations
{
	/// <inheritdoc />
	public partial class AddedDataSourceGraphs : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<Guid>(
				name: "DataSourceGraphId",
				table: "DataSourceDataPoints",
				type: "uuid",
				nullable: true);

			migrationBuilder.AddColumn<Guid>(
				name: "DataSourceGraphStoreItemId",
				table: "DataSourceDataPoints",
				type: "uuid",
				nullable: true);

			migrationBuilder.CreateTable(
				name: "DataSourceGraphs",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					DataSourceId = table.Column<Guid>(type: "uuid", nullable: false),
					Name = table.Column<string>(type: "text", nullable: false),
					Title = table.Column<string>(type: "text", nullable: false),
					VerticalLabel = table.Column<string>(type: "text", nullable: false),
					IsRigid = table.Column<bool>(type: "boolean", nullable: false),
					Width = table.Column<int>(type: "integer", nullable: false),
					Height = table.Column<int>(type: "integer", nullable: false),
					MaxValue = table.Column<double>(type: "double precision", nullable: true),
					MinValue = table.Column<double>(type: "double precision", nullable: true),
					DisplayPriority = table.Column<int>(type: "integer", nullable: false),
					Timescale = table.Column<string>(type: "text", nullable: false),
					IsBase1024 = table.Column<bool>(type: "boolean", nullable: false),
					DataSourceStoreItemId = table.Column<Guid>(type: "uuid", nullable: true),
					DatamartCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
					DatamartLastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
					LogicMonitorId = table.Column<int>(type: "integer", nullable: false),
					DatamartLastObserved = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
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
				name: "FK_DataSourceDataPoints_DataSourceGraphs_DataSourceGraphStoreI~",
				table: "DataSourceDataPoints",
				column: "DataSourceGraphStoreItemId",
				principalTable: "DataSourceGraphs",
				principalColumn: "Id");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_DataSourceDataPoints_DataSourceGraphs_DataSourceGraphStoreI~",
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
}