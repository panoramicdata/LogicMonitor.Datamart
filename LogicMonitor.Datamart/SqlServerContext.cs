using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LogicMonitor.Datamart;

/// <summary>
/// A SQL Server implementation of <see cref="Context"/> using Microsoft.EntityFrameworkCore.SqlServer.
/// </summary>
public class SqlServerContext : Context
{
	/// <summary>
	/// Initializes a new instance of the <see cref="SqlServerContext"/> class.
	/// </summary>
	public SqlServerContext() : base()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="SqlServerContext"/> class with the specified options.
	/// </summary>
	/// <param name="options">The database context options.</param>
	public SqlServerContext(DbContextOptions<Context> options) : base(options)
	{
	}

	/// <inheritdoc />
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		if (!optionsBuilder.IsConfigured)
		{
			optionsBuilder.UseSqlServer("Server=127.0.0.1;Port=1455;Database=LogicMonitorDatamart;User Id=sa;Password=XXX");
		}

		optionsBuilder.ConfigureWarnings(warnings => warnings.Log(RelationalEventId.PendingModelChangesWarning));
	}
}
