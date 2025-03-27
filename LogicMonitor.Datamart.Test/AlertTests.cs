namespace LogicMonitor.Datamart.Test;

public class AlertTests(ITestOutputHelper iTestOutputHelper) : TestWithOutput(iTestOutputHelper)
{
	[Fact]
	public async Task UpdateDevices()
	{
		var utcNow = DateTime.UtcNow;

		await DatamartClient
			.AddOrUpdate<Resource, ResourceStoreItem>(
				context => context.Devices,
				true,
				LoggerFactory.CreateLogger(nameof(AlertTests)),
				TestNotificationReceiver,
				default)
			.ConfigureAwait(true);
	}

	[Fact]
	public async Task Get24HoursOfAlerts()
	{
		var startDateTimeUtc = DateTime.UtcNow.AddHours(-24);

		var updatedAlertStats = await new AlertSync(
				DatamartClient,
				startDateTimeUtc,
				LoggerFactory)
			.DifferentialLoopTaskAsync(default)
			.ConfigureAwait(true);

		updatedAlertStats.Should().NotBeNull();
	}

	[Fact]
	public async Task GetCachedAlertsAsync()
	{
		var startDateTimeUtc = DateTimeOffset.UtcNow.AddDays(-10);
		var endDateTimeUtc = DateTimeOffset.UtcNow.AddDays(-5);
		var result = await DatamartClient.GetCachedAlertsAsync(
			startDateTimeUtc.ToUnixTimeSeconds(),
			endDateTimeUtc.ToUnixTimeSeconds(),
			true,
			null,
			null,
			null,
			new AckFilter(),
			["PDL"],
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			Api.Filters.OrderDirection.Asc,
			new SdtFilter(),
			null,
			true
			).ConfigureAwait(true);
		result.Should().NotBeNull();
	}
}
