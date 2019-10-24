using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace LogicMonitor.Datamart.Test
{
	public class DimensionTests : TestWithOutput
	{
		private readonly ILogger _logger;

		public DimensionTests(ITestOutputHelper iTestOutputHelper) : base(iTestOutputHelper)
			=> _logger = ITestOutputHelper.BuildLoggerFor<AlertTests>();

		[Fact]
		public async void GetDimensions()
		{
			_logger.LogInformation("Getting Dimensions...");

			await new DimensionSync(
					DatamartClient,
					Configuration,
					ITestOutputHelper.BuildLoggerFor<DimensionSync>())
				.ExecuteAsync(default)
				.ConfigureAwait(false);
		}
	}
}
