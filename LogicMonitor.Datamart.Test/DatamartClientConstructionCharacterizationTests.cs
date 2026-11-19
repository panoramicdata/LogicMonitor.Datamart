using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Npgsql;

namespace LogicMonitor.Datamart.Test;

/// <summary>
/// Offline characterization tests of how the client builds its database options. No connection is opened.
/// </summary>
public class DatamartClientConstructionCharacterizationTests
{
	private static DatamartClient Client(DatabaseType type, Action<Configuration>? configure = null)
		=> InMemoryDatamart.CreateClient(configuration =>
		{
			configuration.DatabaseType = type;
			configuration.DatabaseName = "TheDatabase";
			configuration.DatabaseServerName = "db.example";
			configuration.DatabaseServerPort = null;
			configuration.DatabaseUsername = "the user";
			configuration.DatabasePassword = "pa;ss'\"word";
			configure?.Invoke(configuration);
		});

	/// <summary>
	/// SQL Server uses a built connection string with the default port, encryption, certificate trust and application name.
	/// </summary>
	[Fact]
	public void SqlServer_DefaultPort_BuildsConnectionString()
	{
		using var context = Client(DatabaseType.SqlServer).GetContext();

		var builder = new SqlConnectionStringBuilder(context.Database.GetConnectionString());
		builder.DataSource.Should().Be("db.example,1433");
		builder.InitialCatalog.Should().Be("TheDatabase");
		builder.UserID.Should().Be("the user");
		builder.Password.Should().Be("pa;ss'\"word");
		builder.ApplicationName.Should().Be("LogicMonitor.Datamart");
		builder.TrustServerCertificate.Should().BeTrue();
		builder.Encrypt.Should().Be(SqlConnectionEncryptOption.Mandatory);
		builder.ConnectTimeout.Should().Be(30);
		builder.Authentication.Should().Be(SqlAuthenticationMethod.NotSpecified);
		context.Database.GetCommandTimeout().Should().Be(600);
		context.Database.CreateExecutionStrategy().RetriesOnFailure.Should().BeTrue();
	}

	/// <summary>
	/// A configured port, authentication method and command timeout flow into the SQL Server options.
	/// </summary>
	[Fact]
	public void SqlServer_ExplicitSettings_AreUsed()
	{
		using var context = Client(DatabaseType.SqlServer, configuration =>
		{
			configuration.DatabaseServerPort = 14330;
			configuration.SqlServerAuthenticationMethod = SqlAuthenticationMethod.ActiveDirectoryDefault;
			configuration.SqlCommandTimeoutSeconds = 42;
		}).GetContext();

		var builder = new SqlConnectionStringBuilder(context.Database.GetConnectionString());
		builder.DataSource.Should().Be("db.example,14330");
		builder.Authentication.Should().Be(SqlAuthenticationMethod.ActiveDirectoryDefault);
		context.Database.GetCommandTimeout().Should().Be(42);
	}

	/// <summary>
	/// Postgres uses a built connection string with the default port and a UTC timezone.
	/// </summary>
	[Fact]
	public void Postgres_DefaultPort_BuildsConnectionString()
	{
		using var context = Client(DatabaseType.Postgres).GetContext();

		var builder = new NpgsqlConnectionStringBuilder(context.Database.GetConnectionString());
		builder.Host.Should().Be("db.example");
		builder.Port.Should().Be(5432);
		builder.Database.Should().Be("TheDatabase");
		builder.Username.Should().Be("the user");
		builder.Password.Should().Be("pa;ss'\"word");
		builder.Timezone.Should().Be("UTC");
		context.Database.CreateExecutionStrategy().RetriesOnFailure.Should().BeTrue();
	}

	/// <summary>
	/// A configured Postgres port is used.
	/// </summary>
	[Fact]
	public void Postgres_ExplicitPort_IsUsed()
	{
		using var context = Client(DatabaseType.Postgres, configuration => configuration.DatabaseServerPort = 6543).GetContext();

		new NpgsqlConnectionStringBuilder(context.Database.GetConnectionString()).Port.Should().Be(6543);
	}

	/// <summary>
	/// The in-memory type builds an in-memory context.
	/// </summary>
	[Fact]
	public void InMemory_BuildsInMemoryContext()
	{
		using var context = Client(DatabaseType.InMemory).GetContext();

		context.Should().BeOfType<InMemoryContext>();
		context.Database.ProviderName.Should().Be("Microsoft.EntityFrameworkCore.InMemory");
	}

	/// <summary>
	/// The None type configures no provider at all.
	/// </summary>
	[Fact]
	public void None_ConfiguresNoProvider()
		=> Client(DatabaseType.None).DbContextOptions.Extensions.Should().BeEmpty();

	/// <summary>
	/// A database type the client cannot build options for is rejected at construction.
	/// </summary>
	[Fact]
	public void UnsupportedType_Throws()
	{
		var act = () => Client((DatabaseType)99);

		act.Should().Throw<NotSupportedException>();
	}

	/// <summary>
	/// Sensitive data logging is only enabled on request.
	/// </summary>
	[Theory]
	[InlineData(false)]
	[InlineData(true)]
	public void SensitiveDataLogging_FollowsConfiguration(bool enabled)
	{
		var client = Client(DatabaseType.InMemory, configuration => configuration.EnableSensitiveDatabaseLogging = enabled);

		var extension = client.DbContextOptions.FindExtension<CoreOptionsExtension>();

		(extension?.IsSensitiveDataLoggingEnabled ?? false).Should().Be(enabled);
	}
}
