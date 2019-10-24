using Microsoft.Extensions.Logging;
using System;
using Xunit;
using Xunit.Abstractions;

namespace LogicMonitor.Datamart.Test
{
	public class DataTests : TestWithOutput
	{
		private readonly ILogger _logger;

		public DataTests(ITestOutputHelper iTestOutputHelper)
		 : base(iTestOutputHelper)
		 => _logger = ITestOutputHelper.BuildLoggerFor<AlertTests>();

		[Fact]
		public async void Get12HoursOfData()
		{
			_logger.LogInformation("Getting data...");
			try
			{
				await new DataSync(
						DatamartClient,
						Configuration,
						ITestOutputHelper.BuildLoggerFor<DataSync>())
					.ExecuteAsync(default)
					.ConfigureAwait(false);
			}
			catch (Exception e)
			{
				var a = 1;
			}
		}
	}
}
