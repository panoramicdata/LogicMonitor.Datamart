using System.Collections.ObjectModel;

#nullable disable

namespace LogicMonitor.Datamart.Migrations.NpgsqlMigrations
{
	/// <inheritdoc />
	public partial class AddedAuditEvents : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "AuditEvents",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					LogicMonitorId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
					Host = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
					OriginalDescription = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
					SessionId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
					OriginatorType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
					EntityType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
					ActionType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
					OutcomeType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
					ResourceIds = table.Column<Collection<int>>(type: "integer[]", maxLength: 50, nullable: true),
					ResourceNames = table.Column<Collection<string>>(type: "text[]", maxLength: 200, nullable: true),
					LogicModuleId = table.Column<int>(type: "integer", nullable: true),
					LogicModuleName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
					LogicModuleVersion = table.Column<int>(type: "integer", nullable: true),
					InstanceId = table.Column<int>(type: "integer", nullable: true),
					InstanceName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
					CollectorId = table.Column<int>(type: "integer", nullable: true),
					CollectorName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
					CollectorDescription = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
					Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
					ApiTokenId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
					ApiPath = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
					ApiMethod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
					DataSourceNewInstanceIds = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
					DataSourceNewInstanceNames = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
					DataSourceDeletedInstanceIds = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
					DataSourceDeletedInstanceNames = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
					ResourceGroupName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
					ResourceGroupId = table.Column<int>(type: "integer", nullable: true),
					PropertyName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
					DeviceDataSourceId = table.Column<int>(type: "integer", nullable: true),
					MatchedRegExId = table.Column<int>(type: "integer", nullable: false),
					PropertyValue = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
					Time = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
					WildValue = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
					PerformedByUsername = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
					UserName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
					UserEmail = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
					UserId = table.Column<int>(type: "integer", nullable: true),
					UserRole = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
					AlertId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
					AlertNote = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
					MonthlyMetrics = table.Column<long>(type: "bigint", nullable: true),
					StartDownTime = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
					EndDownTime = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
					Command = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
					ResourceHostname = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
					RemoteSessionId = table.Column<long>(type: "bigint", maxLength: 50, nullable: true),
					RemoteSessionType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
					RestrictSso = table.Column<bool>(type: "boolean", maxLength: 50, nullable: true),
					CollectorGroupName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
					CollectorGroupId = table.Column<int>(type: "integer", nullable: true),
					RequestId = table.Column<long>(type: "bigint", nullable: true),
					DatamartCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
					DatamartLastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AuditEvents", x => x.Id);
				});
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "AuditEvents");
		}
	}
}
