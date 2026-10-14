using LogicMonitor.Api;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;

namespace LogicMonitor.Datamart.Test;

/// <summary>
/// The Postgres connection string that DatamartClient builds from its Configuration. Runs offline.
/// </summary>
public class PostgresConnectionStringTests
{
	/// <summary>
	/// MS-24748 A password containing connection string syntax reaches Postgres unchanged.
	/// </summary>
	/// <param name="password">The configured database password.</param>
	[Theory]
	[InlineData("Secret;x")]
	[InlineData("Se;cr;x")]
	[InlineData("\"Secret")]
	[InlineData("a'b\"c;d=e")]
	[InlineData(" padded ")]
	public void Password_WithConnectionStringSyntax_ReachesPostgresUnchanged(string password)
	{
		using var datamartClient = CreatePostgresClient(password);
		using var context = datamartClient.GetContext();

		// The call that threw "Couldn't set x;timezone" in production
		_ = context.Database.GetDbConnection();

		new NpgsqlConnectionStringBuilder(context.Database.GetConnectionString())
			.Password
			.Should()
			.Be(password);
	}

	/// <summary>
	/// Every configured value, and the UTC time zone, reaches the connection string.
	/// </summary>
	[Fact]
	public void ConnectionString_CarriesEveryConfiguredValue()
	{
		using var datamartClient = CreatePostgresClient("Secret123");
		using var context = datamartClient.GetContext();

		var connectionString = new NpgsqlConnectionStringBuilder(context.Database.GetConnectionString());

		connectionString.Host.Should().Be("db.contoso.example");
		connectionString.Port.Should().Be(5433);
		connectionString.Database.Should().Be("DataMagic_123456");
		connectionString.Username.Should().Be("datamagic");
		connectionString.Password.Should().Be("Secret123");
		connectionString.Timezone.Should().Be("UTC");
	}

	private static DatamartClient CreatePostgresClient(string password)
		=> new(
			new Configuration
			{
				Name = "Contoso",
				DatabaseType = DatabaseType.Postgres,
				DatabaseServerName = "db.contoso.example",
				DatabaseServerPort = 5433,
				DatabaseName = "DataMagic_123456",
				DatabaseUsername = "datamagic",
				DatabasePassword = password,
				LogicMonitorClientOptions = new LogicMonitorClientOptions
				{
					Account = "contoso",
					AccessId = "access-id",
					AccessKey = "access-key",
				},
			},
			NullLoggerFactory.Instance);
}
