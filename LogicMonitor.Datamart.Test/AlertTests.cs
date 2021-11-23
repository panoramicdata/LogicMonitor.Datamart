namespace LogicMonitor.Datamart.Test;

public class AlertTests : TestWithOutput
{
	public AlertTests(ITestOutputHelper iTestOutputHelper) : base(iTestOutputHelper)
	{
	}

	[Fact]
	public async void UpdateDevices()
	{
		var utcNow = DateTime.UtcNow;

		await DatamartClient
			.AddOrUpdate<Device, DeviceStoreItem>(context => context.Devices, LoggerFactory.CreateLogger(nameof(AlertTests)), default)
			.ConfigureAwait(false);
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
			.ConfigureAwait(false);

		Assert.NotNull(updatedAlertStats);
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
			new Api.Alerts.AckFilter(),
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
			new Api.Alerts.SdtFilter(),
			null,
			true
			).ConfigureAwait(false);
		Assert.NotNull(result);
	}
}
