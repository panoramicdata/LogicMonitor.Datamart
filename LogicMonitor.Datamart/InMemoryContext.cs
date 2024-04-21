namespace LogicMonitor.Datamart;

public class InMemoryContext : Context
{
	public InMemoryContext() : base()
	{
	}

	public InMemoryContext(DbContextOptions<Context> options) : base(options)
	{
	}
}
