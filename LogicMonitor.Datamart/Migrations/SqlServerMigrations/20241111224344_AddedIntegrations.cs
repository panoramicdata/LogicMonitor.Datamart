#nullable disable

namespace LogicMonitor.Datamart.Migrations.SqlServerMigrations
{
	/// <inheritdoc />
	public partial class AddedIntegrations : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "Integrations",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
					Extra = table.Column<string>(type: "nvarchar(max)", nullable: false),
					AckHeaders = table.Column<string>(type: "nvarchar(max)", nullable: true),
					AckMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
					AckOAuthVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
					AckOAuthGrantType = table.Column<string>(type: "nvarchar(max)", nullable: true),
					AckOAuthAccessTokenUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
					AckOAuthClientId = table.Column<string>(type: "nvarchar(max)", nullable: true),
					AckOAuthClientSecret = table.Column<string>(type: "nvarchar(max)", nullable: true),
					AckOAuthScope = table.Column<string>(type: "nvarchar(max)", nullable: true),
					AckPassword = table.Column<string>(type: "nvarchar(max)", nullable: true),
					AckPayload = table.Column<string>(type: "nvarchar(max)", nullable: true),
					AckPayloadFormat = table.Column<string>(type: "nvarchar(max)", nullable: true),
					AckUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
					AckUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
					AckAlertDataType = table.Column<string>(type: "nvarchar(max)", nullable: true),
					AlertDataType = table.Column<string>(type: "nvarchar(max)", nullable: true),
					ClearHeaders = table.Column<string>(type: "nvarchar(max)", nullable: true),
					ClearMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
					ClearOAuthVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
					ClearOAuthGrantType = table.Column<string>(type: "nvarchar(max)", nullable: true),
					ClearOAuthAccessTokenUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
					ClearOAuthClientId = table.Column<string>(type: "nvarchar(max)", nullable: true),
					ClearOAuthClientSecret = table.Column<string>(type: "nvarchar(max)", nullable: true),
					ClearOAuthScope = table.Column<string>(type: "nvarchar(max)", nullable: true),
					ClearPassword = table.Column<string>(type: "nvarchar(max)", nullable: true),
					ClearPayload = table.Column<string>(type: "nvarchar(max)", nullable: true),
					ClearPayloadFormat = table.Column<string>(type: "nvarchar(max)", nullable: true),
					ClearUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
					ClearUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
					EnabledStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
					Headers = table.Column<string>(type: "nvarchar(max)", nullable: true),
					Method = table.Column<string>(type: "nvarchar(max)", nullable: true),
					ParseMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
					ParseExpression = table.Column<string>(type: "nvarchar(max)", nullable: true),
					Payload = table.Column<string>(type: "nvarchar(max)", nullable: true),
					PayloadFormat = table.Column<string>(type: "nvarchar(max)", nullable: true),
					UpdateMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
					UpdateUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
					Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
					Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
					OAuthVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
					OAuthGrantType = table.Column<string>(type: "nvarchar(max)", nullable: true),
					OAuthAccessTokenUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
					OAuthClientId = table.Column<string>(type: "nvarchar(max)", nullable: true),
					OAuthClientSecret = table.Column<string>(type: "nvarchar(max)", nullable: true),
					OAuthScope = table.Column<string>(type: "nvarchar(max)", nullable: true),
					Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
					UpdateHeaders = table.Column<string>(type: "nvarchar(max)", nullable: true),
					UpdatePassword = table.Column<string>(type: "nvarchar(max)", nullable: true),
					UpdateDataMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
					UpdateDataUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
					UpdateDataUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
					UpdateDataPassword = table.Column<string>(type: "nvarchar(max)", nullable: true),
					UpdateDataPayload = table.Column<string>(type: "nvarchar(max)", nullable: true),
					UpdateDataPayloadFormat = table.Column<string>(type: "nvarchar(max)", nullable: true),
					UpdateDataHeaders = table.Column<string>(type: "nvarchar(max)", nullable: true),
					UpdateDataAlertDataType = table.Column<string>(type: "nvarchar(max)", nullable: true),
					UpdateAlertDataType = table.Column<string>(type: "nvarchar(max)", nullable: true),
					ClearAlertDataType = table.Column<string>(type: "nvarchar(max)", nullable: true),
					UpdateDataOAuthVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
					UpdateDataOAuthGrantType = table.Column<string>(type: "nvarchar(max)", nullable: true),
					UpdateDataOAuthAccessTokenUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
					UpdateDataOAuthClientId = table.Column<string>(type: "nvarchar(max)", nullable: true),
					UpdateDataOAuthClientSecret = table.Column<string>(type: "nvarchar(max)", nullable: true),
					UpdateDataOAuthScope = table.Column<string>(type: "nvarchar(max)", nullable: true),
					UpdatePayload = table.Column<string>(type: "nvarchar(max)", nullable: true),
					UpdatePayloadFormat = table.Column<string>(type: "nvarchar(max)", nullable: true),
					UpdateUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
					UpdateOAuthVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
					UpdateOAuthGrantType = table.Column<string>(type: "nvarchar(max)", nullable: true),
					UpdateOAuthAccessTokenUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
					UpdateOAuthClientId = table.Column<string>(type: "nvarchar(max)", nullable: true),
					UpdateOAuthClientSecret = table.Column<string>(type: "nvarchar(max)", nullable: true),
					UpdateOAuthScope = table.Column<string>(type: "nvarchar(max)", nullable: true),
					DatamartCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
					DatamartLastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
					LogicMonitorId = table.Column<int>(type: "int", nullable: false),
					DatamartLastObserved = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Integrations", x => x.Id);
				});
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "Integrations");
		}
	}
}