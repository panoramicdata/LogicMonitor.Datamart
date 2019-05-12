using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace LogicMonitor.Datamart.Cli
{
	/// <summary>
	/// This class is used for migrations to create a context - it is located through reflection and naming convention to find the DesignTimeDbContextFactory
	/// </summary>
	public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<Context>
	{
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
}
