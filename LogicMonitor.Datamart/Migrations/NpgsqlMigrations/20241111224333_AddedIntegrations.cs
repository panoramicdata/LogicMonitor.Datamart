#nullable disable

namespace LogicMonitor.Datamart.Migrations.NpgsqlMigrations
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
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					Type = table.Column<string>(type: "text", nullable: false),
					Extra = table.Column<string>(type: "text", nullable: false),
					AckHeaders = table.Column<string>(type: "text", nullable: true),
					AckMethod = table.Column<string>(type: "text", nullable: true),
					AckOAuthVersion = table.Column<string>(type: "text", nullable: true),
					AckOAuthGrantType = table.Column<string>(type: "text", nullable: true),
					AckOAuthAccessTokenUrl = table.Column<string>(type: "text", nullable: true),
					AckOAuthClientId = table.Column<string>(type: "text", nullable: true),
					AckOAuthClientSecret = table.Column<string>(type: "text", nullable: true),
					AckOAuthScope = table.Column<string>(type: "text", nullable: true),
					AckPassword = table.Column<string>(type: "text", nullable: true),
					AckPayload = table.Column<string>(type: "text", nullable: true),
					AckPayloadFormat = table.Column<string>(type: "text", nullable: true),
					AckUrl = table.Column<string>(type: "text", nullable: true),
					AckUsername = table.Column<string>(type: "text", nullable: true),
					AckAlertDataType = table.Column<string>(type: "text", nullable: true),
					AlertDataType = table.Column<string>(type: "text", nullable: true),
					ClearHeaders = table.Column<string>(type: "text", nullable: true),
					ClearMethod = table.Column<string>(type: "text", nullable: true),
					ClearOAuthVersion = table.Column<string>(type: "text", nullable: true),
					ClearOAuthGrantType = table.Column<string>(type: "text", nullable: true),
					ClearOAuthAccessTokenUrl = table.Column<string>(type: "text", nullable: true),
					ClearOAuthClientId = table.Column<string>(type: "text", nullable: true),
					ClearOAuthClientSecret = table.Column<string>(type: "text", nullable: true),
					ClearOAuthScope = table.Column<string>(type: "text", nullable: true),
					ClearPassword = table.Column<string>(type: "text", nullable: true),
					ClearPayload = table.Column<string>(type: "text", nullable: true),
					ClearPayloadFormat = table.Column<string>(type: "text", nullable: true),
					ClearUrl = table.Column<string>(type: "text", nullable: true),
					ClearUsername = table.Column<string>(type: "text", nullable: true),
					EnabledStatus = table.Column<string>(type: "text", nullable: true),
					Headers = table.Column<string>(type: "text", nullable: true),
					Method = table.Column<string>(type: "text", nullable: true),
					ParseMethod = table.Column<string>(type: "text", nullable: true),
					ParseExpression = table.Column<string>(type: "text", nullable: true),
					Payload = table.Column<string>(type: "text", nullable: true),
					PayloadFormat = table.Column<string>(type: "text", nullable: true),
					UpdateMethod = table.Column<string>(type: "text", nullable: true),
					UpdateUrl = table.Column<string>(type: "text", nullable: true),
					Url = table.Column<string>(type: "text", nullable: true),
					Username = table.Column<string>(type: "text", nullable: true),
					OAuthVersion = table.Column<string>(type: "text", nullable: true),
					OAuthGrantType = table.Column<string>(type: "text", nullable: true),
					OAuthAccessTokenUrl = table.Column<string>(type: "text", nullable: true),
					OAuthClientId = table.Column<string>(type: "text", nullable: true),
					OAuthClientSecret = table.Column<string>(type: "text", nullable: true),
					OAuthScope = table.Column<string>(type: "text", nullable: true),
					Password = table.Column<string>(type: "text", nullable: true),
					UpdateHeaders = table.Column<string>(type: "text", nullable: true),
					UpdatePassword = table.Column<string>(type: "text", nullable: true),
					UpdateDataMethod = table.Column<string>(type: "text", nullable: true),
					UpdateDataUrl = table.Column<string>(type: "text", nullable: true),
					UpdateDataUsername = table.Column<string>(type: "text", nullable: true),
					UpdateDataPassword = table.Column<string>(type: "text", nullable: true),
					UpdateDataPayload = table.Column<string>(type: "text", nullable: true),
					UpdateDataPayloadFormat = table.Column<string>(type: "text", nullable: true),
					UpdateDataHeaders = table.Column<string>(type: "text", nullable: true),
					UpdateDataAlertDataType = table.Column<string>(type: "text", nullable: true),
					UpdateAlertDataType = table.Column<string>(type: "text", nullable: true),
					ClearAlertDataType = table.Column<string>(type: "text", nullable: true),
					UpdateDataOAuthVersion = table.Column<string>(type: "text", nullable: true),
					UpdateDataOAuthGrantType = table.Column<string>(type: "text", nullable: true),
					UpdateDataOAuthAccessTokenUrl = table.Column<string>(type: "text", nullable: true),
					UpdateDataOAuthClientId = table.Column<string>(type: "text", nullable: true),
					UpdateDataOAuthClientSecret = table.Column<string>(type: "text", nullable: true),
					UpdateDataOAuthScope = table.Column<string>(type: "text", nullable: true),
					UpdatePayload = table.Column<string>(type: "text", nullable: true),
					UpdatePayloadFormat = table.Column<string>(type: "text", nullable: true),
					UpdateUsername = table.Column<string>(type: "text", nullable: true),
					UpdateOAuthVersion = table.Column<string>(type: "text", nullable: true),
					UpdateOAuthGrantType = table.Column<string>(type: "text", nullable: true),
					UpdateOAuthAccessTokenUrl = table.Column<string>(type: "text", nullable: true),
					UpdateOAuthClientId = table.Column<string>(type: "text", nullable: true),
					UpdateOAuthClientSecret = table.Column<string>(type: "text", nullable: true),
					UpdateOAuthScope = table.Column<string>(type: "text", nullable: true),
					DatamartCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
					DatamartLastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
					LogicMonitorId = table.Column<int>(type: "integer", nullable: false),
					DatamartLastObserved = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
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