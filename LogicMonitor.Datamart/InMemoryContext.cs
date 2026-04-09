namespace LogicMonitor.Datamart;

/// <summary>
/// An in-memory implementation of <see cref="Context"/> for testing purposes.
/// </summary>
public class InMemoryContext : Context
{
	/// <summary>
	/// Initializes a new instance of the <see cref="InMemoryContext"/> class.
	/// </summary>
	public InMemoryContext() : base()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="InMemoryContext"/> class with the specified options.
	/// </summary>
	/// <param name="options">The database context options.</param>
	public InMemoryContext(DbContextOptions<Context> options) : base(options)
	{
	}
}
