namespace LogicMonitor.Datamart.Test;

public class AlertTests(ITestOutputHelper iTestOutputHelper) : TestWithOutput(iTestOutputHelper)
{
	[Fact]
	public async void UpdateDevices()
	{
		var utcNow = DateTime.UtcNow;

		await DatamartClient
			.AddOrUpdate<Device, DeviceStoreItem>(context => context.Devices, LoggerFactory.CreateLogger(nameof(AlertTests)), default)
			.ConfigureAwait(true);
	}

	[Fact]
	public async void Get24HoursOfAlerts()
	{
		var startDateTimeUtc = DateTime.UtcNow.AddHours(-24);
		//var startDateTimeUtc = DateTime.UtcNow.AddDays(-30);

		var updatedAlertStats = await new AlertSync(
				DatamartClient,
				startDateTimeUtc,
				LoggerFactory)
			.DifferentialLoopTaskAsync(default)
			.ConfigureAwait(true);

		updatedAlertStats.Should().NotBeNull();
	}

	[Fact]
	public async void GetCachedAlertsAsync()
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
			new List<string> { "PDL" },
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
