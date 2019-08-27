using Microsoft.Extensions.Logging;
using System;
using Xunit;
using Xunit.Abstractions;

namespace LogicMonitor.Datamart.Test
{
	public class LogSyncTests : TestWithOutput
	{
		private readonly ILogger _logger;

		public LogSyncTests(ITestOutputHelper iTestOutputHelper)
		 : base(iTestOutputHelper)
		 => _logger = ITestOutputHelper.BuildLoggerFor<AlertTests>();

		[Fact]
		public async void GetLogs()
		{
			_logger.LogInformation("Getting logs...");
			var startDateTimeUtc = DateTimeOffset.UtcNow.AddDays(-7);
			//var startDateTimeUtc = DateTimeOffset.ParseExact("2019-04-01", "yyyy-MM-dd", CultureInfo.InvariantCulture).UtcDateTime;

			await new LogSync(
					DatamartClient,
					startDateTimeUtc,
					ITestOutputHelper.BuildLoggerFor<LogSync>())
				.ExecuteAsync(default)
				.ConfigureAwait(false);
		}
	}
}
