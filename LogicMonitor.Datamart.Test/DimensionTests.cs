using LogicMonitor.Api.LogicModules;

namespace LogicMonitor.Datamart.Test;

public class DimensionTests : TestWithOutput
{
	public DimensionTests(ITestOutputHelper iTestOutputHelper) : base(iTestOutputHelper)
	{
	}

	[Fact]
	public async Task GetDimensions()
	{
		await new DimensionSync(
				DatamartClient,
				Configuration,
				new List<string>
				{
					nameof(Device),
					nameof(DeviceDataSourceInstance),
				},
				LoggerFactory)
			.ExecuteAsync(default)
			.ConfigureAwait(false);
	}
}
