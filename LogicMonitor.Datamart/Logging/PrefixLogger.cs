namespace LogicMonitor.Datamart.Logging;

public class PrefixLogger : ILogger
{
	private readonly string _prefix;
	private readonly ILogger _logger;

	public PrefixLogger(string prefix, ILoggerFactory loggerFactory, string categoryName = null)
	{
		_prefix = prefix ?? throw new ArgumentNullException(nameof(prefix));
		_logger = categoryName != null
			? loggerFactory.CreateLogger(categoryName)
			: loggerFactory?.CreateLogger<PrefixLogger>() ?? throw new ArgumentNullException(nameof(loggerFactory));
	}

	public IDisposable BeginScope<TState>(TState state) => _logger.BeginScope(state);

	public bool IsEnabled(LogLevel logLevel) => _logger.IsEnabled(logLevel);

	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
	{
		var message = formatter != null && exception != null
			? formatter(state, exception)
			: state.ToString();

		if (exception == null)
		{
			_logger.Log(logLevel, eventId, _prefix + ": " + message);
		}
		else
		{
			_logger.Log(logLevel, eventId, _prefix + ": " + message, exception, formatter);
		}
	}
}
