using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace LogicMonitor.Datamart.Test
{
	public class DataTests : TestWithOutput
	{
		public DataTests(ITestOutputHelper iTestOutputHelper)
		 : base(iTestOutputHelper)
		{
		}

		[Fact]
		public async void Get12HoursOfData()
		{
			await new DataSync(
					DatamartClient,
					Configuration,
					LoggerFactory)
				.ExecuteAsync(default)
				.ConfigureAwait(false);
		}
	}
}
