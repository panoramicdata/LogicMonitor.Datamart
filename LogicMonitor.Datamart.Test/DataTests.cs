using LogicMonitor.Datamart.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace LogicMonitor.Datamart.Test
{
	public class DataTests : TestWithOutput
	{
		private readonly ILogger _logger;

		public DataTests(ITestOutputHelper iTestOutputHelper)
		 : base(iTestOutputHelper)
		 => _logger = ITestOutputHelper.BuildLoggerFor<AlertTests>();

		[Fact]
		public async void Get12HoursOfData()
		{
			_logger.LogInformation("Getting data...");
			var startDateTimeUtc = DateTimeOffset.UtcNow.AddHours(-12);
			startDateTimeUtc = new DateTimeOffset(startDateTimeUtc.Year, startDateTimeUtc.Month, startDateTimeUtc.Day, startDateTimeUtc.Hour, 0, 0, TimeSpan.Zero).UtcDateTime;
			const int lateArrivingDataWindowHours = 2;
			await new DataSync(
					DatamartClient,
					DataSourceSpecifications,
					startDateTimeUtc,
					lateArrivingDataWindowHours,
					ITestOutputHelper.BuildLoggerFor<DataSync>())
				.ExecuteAsync(default)
				.ConfigureAwait(false);
		}
	}
}
