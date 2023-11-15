namespace LogicMonitor.Datamart.Test;

public class LogSyncTests(ITestOutputHelper iTestOutputHelper) : TestWithOutput(iTestOutputHelper)
{
	[Fact]
	public async void GetLogs()
	{
		var startDateTimeUtc = DateTimeOffset.UtcNow.AddDays(-7);
		//var startDateTimeUtc = DateTimeOffset.ParseExact("2019-04-01", "yyyy-MM-dd", CultureInfo.InvariantCulture).UtcDateTime;

		await new LogSync(
				DatamartClient,
				startDateTimeUtc,
				LoggerFactory)
			.ExecuteAsync(default)
			.ConfigureAwait(true);
	}
}
