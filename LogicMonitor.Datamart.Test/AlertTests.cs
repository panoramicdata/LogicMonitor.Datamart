using LogicMonitor.Api.Devices;
using LogicMonitor.Datamart.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace LogicMonitor.Datamart.Test
{
	public class AlertTests : TestWithOutput
	{
		private readonly ILogger _logger;

		public AlertTests(ITestOutputHelper iTestOutputHelper) : base(iTestOutputHelper)
			=> _logger = ITestOutputHelper.BuildLoggerFor<AlertTests>();

		[Fact]
		public async void UpdateDevices()
		{
			_logger.LogInformation("Getting device dimensions...");
			var utcNow = DateTime.UtcNow;

			await DatamartClient
				.AddOrUpdate<Device, DeviceStoreItem>(context => context.Devices, default)
				.ConfigureAwait(false);
		}

		[Fact]
		public async void Get1HourOfAlerts()
		{
			_logger.LogInformation("Getting alerts...");
			//var startDateTimeUtc = DateTime.UtcNow.AddHours(-1);
			var startDateTimeUtc = DateTime.UtcNow.AddDays(-30);

			var updatedAlertStats = await new AlertSync(
					DatamartClient,
					startDateTimeUtc,
					ITestOutputHelper.BuildLoggerFor<AlertSync>())
				.DifferentialLoopTaskAsync(default)
				.ConfigureAwait(false);

			Assert.NotNull(updatedAlertStats);
			_logger.LogInformation($"New    : {updatedAlertStats.New}");
			_logger.LogInformation($"Updated: {updatedAlertStats.Updated}");
		}

		[Fact]
		public async void GetCachedAlertsAsync()
		{
			_logger.LogInformation("Getting alerts...");
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
}
