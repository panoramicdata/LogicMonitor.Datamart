namespace LogicMonitor.Datamart.Test;

public class DataTests : TestWithOutput
{
	public DataTests(ITestOutputHelper iTestOutputHelper)
	 : base(iTestOutputHelper)
	{
	}

	[Fact]
	public async void HighResolutionDataSync()
	{
		await new HighResolutionDataSync(
				DatamartClient,
				Configuration,
				LoggerFactory)
			.ExecuteAsync(default)
			.ConfigureAwait(false);
	}

	[Fact]
	public async void LowResolutionDataSync()
	{
		await new LowResolutionDataSync(
				DatamartClient,
				Configuration,
				LoggerFactory)
			.ExecuteAsync(default)
			.ConfigureAwait(false);
	}
}
