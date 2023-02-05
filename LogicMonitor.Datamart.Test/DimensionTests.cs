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
				LoggerFactory)
			.ExecuteAsync(default)
			.ConfigureAwait(false);
	}
}
