using Divergic.Logging.Xunit;
using LogicMonitor.Datamart.Config;
using LogicMonitor.Datamart.Logging;
using LogicMonitor.Datamart.Test.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace LogicMonitor.Datamart.Test
{
	public abstract class TestWithOutput
	{
		protected static DateTimeOffset TwelveHoursAgo = DateTimeOffset.UtcNow.AddHours(-12);

		protected static Configuration Configuration = new Configuration
		{
			Name = "Test",
			AggregationDurationMinutes = 15,
			LateArrivingDataWindowHours = 2,
			StartDateTimeUtc = new DateTimeOffset(
				TwelveHoursAgo.Year,
				TwelveHoursAgo.Month,
				TwelveHoursAgo.Day,
				TwelveHoursAgo.Hour,
				0,
				0,
				TimeSpan.Zero).UtcDateTime,

			DataSources = new List<DataSourceConfigurationItem>
			{
				new DataSourceConfigurationItem{
					Name = "WinCPU",
					//AggregationDurationMinutes = 60,
					DataPoints = new List<DataPointConfigurationItem>
					{
						new DataPointConfigurationItem
							{
								Name = "CPUBusyPercent",
								MeasurementUnit = "%",
							},
						new DataPointConfigurationItem
							{
								Name= "ProcessorQueueLength",
								MeasurementUnit = "count"
							}
					}
				}
			}
		};

		protected TestWithOutput(ITestOutputHelper iTestOutputHelper)
		{
			ITestOutputHelper = iTestOutputHelper;
			var nowUtc = DateTimeOffset.UtcNow;
			StartEpoch = nowUtc.AddDays(-30).ToUnixTimeSeconds();
			EndEpoch = nowUtc.ToUnixTimeSeconds();
			var configuration = LoadConfiguration("appsettings.json");
			var logicMonitorCredentials = configuration.LogicMonitorCredentials;
			var loggerFactory = new PrefixLoggerFactory(logicMonitorCredentials.Account, LogFactory.Create(iTestOutputHelper));
			Configuration.LogicMonitorCredential = new LogicMonitorCredential
			{
				Subdomain = logicMonitorCredentials.Account,
				AccessId = logicMonitorCredentials.AccessId,
				AccessKey = logicMonitorCredentials.AccessKey,
			};
			Configuration.DatabaseType = DatabaseType.SqlServer;
			Configuration.DatabaseServerName = configuration.DatabaseServer;
			Configuration.DatabaseName = configuration.DatabaseName;

			DatamartClient = new DatamartClient(
				Configuration,
				loggerFactory);

			DatamartClient.EnsureDatabaseCreatedAndSchemaUpdatedAsync().GetAwaiter().GetResult();

			Stopwatch = Stopwatch.StartNew();

			LoggerFactory = LogFactory.Create(iTestOutputHelper);
		}

		protected static TestConfiguration LoadConfiguration(string jsonFilePath)
		{
			var location = typeof(TestWithOutput).GetTypeInfo().Assembly.Location;
			var dirPath = Path.Combine(Path.GetDirectoryName(location), "../../..");

			TestConfiguration configuration;
			var configurationRoot = new ConfigurationBuilder()
				.SetBasePath(dirPath)
				.AddJsonFile(jsonFilePath, false, false)
				.Build();
			var services = new ServiceCollection();
			services.AddOptions();
			services.Configure<TestConfiguration>(configurationRoot);
			using (var sp = services.BuildServiceProvider())
			{
				var options = sp.GetService<IOptions<TestConfiguration>>();
				configuration = options.Value;
			}

			return configuration;
		}

		protected ITestOutputHelper ITestOutputHelper { get; }

		private Stopwatch Stopwatch { get; }

		public ILoggerFactory LoggerFactory { get; }

		protected long StartEpoch { get; }

		protected long EndEpoch { get; }

		protected DatamartClient DatamartClient { get; }

		protected void AssertIsFast(int durationSeconds) => Assert.InRange(Stopwatch.ElapsedMilliseconds, 0, durationSeconds * 1000);
	}
}