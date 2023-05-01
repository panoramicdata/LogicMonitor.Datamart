#nullable disable

namespace LogicMonitor.Datamart.Migrations
{
	/// <inheritdoc />
	public partial class RestructuredDataPoints : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql("DELETE FROM \"TimeSeriesDataAggregations\"");
			migrationBuilder.Sql("DELETE FROM \"DeviceDataSourceInstances\"");
			migrationBuilder.Sql("DELETE FROM \"DeviceDataSources\"");
			migrationBuilder.Sql("DELETE FROM \"DataSourceDataPoints\"");

			migrationBuilder.RenameColumn(
				name: "DataPointId",
				table: "TimeSeriesDataAggregations",
				newName: "DeviceDataSourceInstanceDataPointId");

			migrationBuilder.CreateTable(
				name: "DeviceDataSourceInstanceDataPoints",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					DeviceDataSourceInstanceId = table.Column<Guid>(type: "uuid", nullable: false),
					DataSourceDataPointId = table.Column<Guid>(type: "uuid", nullable: false),
					DataSourceStoreItemId = table.Column<Guid>(type: "uuid", nullable: true),
					DatamartCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
					DatamartLastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
					LogicMonitorId = table.Column<int>(type: "integer", nullable: false),
					DatamartLastObserved = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_DeviceDataSourceInstanceDataPoints", x => x.Id);
					table.ForeignKey(
						name: "FK_DeviceDataSourceInstanceDataPoints_DataSourceDataPoints_Dat~",
						column: x => x.DataSourceDataPointId,
						principalTable: "DataSourceDataPoints",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_DeviceDataSourceInstanceDataPoints_DataSources_DataSourceSt~",
						column: x => x.DataSourceStoreItemId,
						principalTable: "DataSources",
						principalColumn: "Id");
					table.ForeignKey(
						name: "FK_DeviceDataSourceInstanceDataPoints_DeviceDataSourceInstance~",
						column: x => x.DeviceDataSourceInstanceId,
						principalTable: "DeviceDataSourceInstances",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_TimeSeriesDataAggregations_DeviceDataSourceInstanceDataPoin~",
				table: "TimeSeriesDataAggregations",
				column: "DeviceDataSourceInstanceDataPointId");

			migrationBuilder.CreateIndex(
				name: "IX_DeviceDataSourceInstanceDataPoints_DataSourceDataPointId",
				table: "DeviceDataSourceInstanceDataPoints",
				column: "DataSourceDataPointId");

			migrationBuilder.CreateIndex(
				name: "IX_DeviceDataSourceInstanceDataPoints_DataSourceStoreItemId",
				table: "DeviceDataSourceInstanceDataPoints",
				column: "DataSourceStoreItemId");

			migrationBuilder.CreateIndex(
				name: "IX_DeviceDataSourceInstanceDataPoints_DeviceDataSourceInstance~",
				table: "DeviceDataSourceInstanceDataPoints",
				column: "DeviceDataSourceInstanceId");

			migrationBuilder.AddForeignKey(
				name: "FK_TimeSeriesDataAggregations_DeviceDataSourceInstanceDataPoin~",
				table: "TimeSeriesDataAggregations",
				column: "DeviceDataSourceInstanceDataPointId",
				principalTable: "DeviceDataSourceInstanceDataPoints",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_TimeSeriesDataAggregations_DeviceDataSourceInstanceDataPoin~",
				table: "TimeSeriesDataAggregations");

			migrationBuilder.DropTable(
				name: "DeviceDataSourceInstanceDataPoints");

			migrationBuilder.DropIndex(
				name: "IX_TimeSeriesDataAggregations_DeviceDataSourceInstanceDataPoin~",
				table: "TimeSeriesDataAggregations");

			migrationBuilder.RenameColumn(
				name: "DeviceDataSourceInstanceDataPointId",
				table: "TimeSeriesDataAggregations",
				newName: "DataPointId");
		}
	}
}