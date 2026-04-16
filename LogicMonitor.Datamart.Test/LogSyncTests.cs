namespace LogicMonitor.Datamart.Test;

/// <summary>
/// Verifies the LogicMonitor Log sync pipeline can retrieve and store recent log entries.
/// </summary>
/// <param name="iTestOutputHelper">xUnit output helper for test diagnostics.</param>
public class LogSyncTests(ITestOutputHelper iTestOutputHelper) : TestWithOutput(iTestOutputHelper)
{
	/// <summary>
	/// Runs the log sync for the past seven days and verifies the operation completes without error.
	/// </summary>
	[Fact]
	public async Task GetLogs()
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
