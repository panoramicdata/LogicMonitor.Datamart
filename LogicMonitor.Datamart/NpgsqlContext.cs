namespace LogicMonitor.Datamart;

public class NpgsqlContext : Context
{
	public NpgsqlContext() : base()
	{
	}

	public NpgsqlContext(DbContextOptions<Context> options) : base(options)
	{
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		if (!optionsBuilder.IsConfigured)
		{
			optionsBuilder.UseNpgsql("Server=127.0.0.1;Port=5432;Database=XXX;User Id=XXX;Password=XXX;");
		}
	}
}
