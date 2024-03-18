#nullable disable

namespace LogicMonitor.Datamart.Migrations
{
	/// <inheritdoc />
	public partial class AddedLogcModuleUpdates : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "LogicModuleUpdates",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					LocalId = table.Column<int>(type: "integer", nullable: false),
					Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
					AppliesTo = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
					Category = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
					Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
					CollectionMethod = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
					Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
					Group = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
					Version = table.Column<long>(type: "bigint", nullable: false),
					LocalVersion = table.Column<long>(type: "bigint", nullable: false),
					AuditVersion = table.Column<long>(type: "bigint", nullable: false),
					RestLm = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
					RegistryVersion = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
					PublishedAtMilliseconds = table.Column<long>(type: "bigint", nullable: false),
					Quality = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
					Locator = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
					CurrentUuid = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
					Namespace = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
					Local = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
					Remote = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
					DatamartLastObserved = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					DatamartCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
					DatamartLastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_LogicModuleUpdates", x => x.Id);
				});
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "LogicModuleUpdates");
		}
	}
}
