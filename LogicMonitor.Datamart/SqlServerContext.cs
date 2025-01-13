using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LogicMonitor.Datamart;

public class SqlServerContext : Context
{
	public SqlServerContext() : base()
	{
	}

	public SqlServerContext(DbContextOptions<Context> options) : base(options)
	{
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		if (!optionsBuilder.IsConfigured)
		{
			optionsBuilder.UseSqlServer("Server=127.0.0.1;Port=1455;Database=LogicMonitorDatamart;User Id=sa;Password=XXX");
		}

		optionsBuilder.ConfigureWarnings(warnings => warnings.Log(RelationalEventId.PendingModelChangesWarning));
	}
}
