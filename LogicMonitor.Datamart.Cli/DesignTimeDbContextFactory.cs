namespace LogicMonitor.Datamart.Cli;

/// <summary>
/// This class is used for migrations to create a context - it is located through reflection and naming convention to find the DesignTimeDbContextFactory
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<Context>
{
	/// <summary>
	/// Creates a <see cref="Context"/> instance for Entity Framework design-time operations such as migrations.
	/// </summary>
	/// <param name="args">Design-time arguments supplied by Entity Framework tooling.</param>
	/// <returns>A configured <see cref="Context"/> that targets the local Datamart database.</returns>
	public Context CreateDbContext(string[] args)
	{
		var configuration = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json")
			.Build();

		var databaseServer = configuration["DatabaseServer"];
		var databaseName = configuration["DatabaseName"];

		var connectionString = $"server={databaseServer};database={databaseName};Trusted_Connection=True;Application Name=LogicMonitor.Datamart.DesignTime";
		var builder = new DbContextOptionsBuilder<Context>();
		builder.UseSqlServer(connectionString);
		return new Context(builder.Options);
	}
}
