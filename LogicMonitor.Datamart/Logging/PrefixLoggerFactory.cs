using Microsoft.Extensions.Logging;

namespace LogicMonitor.Datamart.Logging
{
	public class PrefixLoggerFactory : ILoggerFactory
	{
		private readonly string _prefix;
		private readonly ILoggerFactory _loggerFactory;
		private bool disposed; // To detect redundant calls

		public PrefixLoggerFactory(string prefix, ILoggerFactory loggerFactory)
		{
			_prefix = prefix;
			_loggerFactory = loggerFactory;
		}

		public void AddProvider(ILoggerProvider provider)
			=> _loggerFactory.AddProvider(provider);

		public ILogger CreateLogger(string categoryName)
			=> new PrefixLogger(_prefix, _loggerFactory.CreateLogger(categoryName));

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					_loggerFactory.Dispose();
				}
				disposed = true;
			}
		}

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
#pragma warning disable CA1063 // Implement IDisposable Correctly
		public void Dispose() => Dispose(true);
#pragma warning restore CA1063 // Implement IDisposable Correctly
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
	}
}
