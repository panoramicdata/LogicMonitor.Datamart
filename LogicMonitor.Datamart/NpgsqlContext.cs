using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LogicMonitor.Datamart;

/// <summary>
/// A PostgreSQL implementation of <see cref="Context"/> using Npgsql.
/// </summary>
public class NpgsqlContext : Context
{
	/// <summary>
	/// Initializes a new instance of the <see cref="NpgsqlContext"/> class.
	/// </summary>
	public NpgsqlContext() : base()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="NpgsqlContext"/> class with the specified options.
	/// </summary>
	/// <param name="options">The database context options.</param>
	public NpgsqlContext(DbContextOptions<Context> options) : base(options)
	{
	}

	/// <inheritdoc />
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		if (!optionsBuilder.IsConfigured)
		{
			optionsBuilder.UseNpgsql("Server=127.0.0.1;Port=5432;Database=XXX;User Id=XXX;Password=XXX;Timezone=UTC");
		}

		optionsBuilder.ConfigureWarnings(warnings => warnings.Log(RelationalEventId.PendingModelChangesWarning));
	}
}
