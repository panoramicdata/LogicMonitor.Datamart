using Xunit;
using Xunit.Abstractions;

namespace LogicMonitor.Datamart.Test
{
	public class DimensionTests : TestWithOutput
	{
		public DimensionTests(ITestOutputHelper iTestOutputHelper) : base(iTestOutputHelper)
		{
		}

		[Fact]
		public async void GetDimensions()
		{
			await new DimensionSync(
					DatamartClient,
					Configuration,
					LoggerFactory)
				.ExecuteAsync(default)
				.ConfigureAwait(false);
		}
	}
}
