namespace LogicMonitor.Datamart.Test;

/// <summary>
/// Exercises alert-related Datamart synchronization and cached retrieval scenarios.
/// </summary>
/// <param name="iTestOutputHelper">xUnit output helper used for logging.</param>
public class AlertTests(ITestOutputHelper iTestOutputHelper) : TestWithOutput(iTestOutputHelper)
{
	/// <summary>
	/// Updates device entities required by downstream alert synchronization flows.
	/// </summary>
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

	/// <summary>
	/// Synchronizes alert data for the previous 24-hour window and validates a result is returned.
	/// </summary>
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

	/// <summary>
	/// Retrieves cached alerts for a historical range using explicit filter arguments.
	/// </summary>
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
