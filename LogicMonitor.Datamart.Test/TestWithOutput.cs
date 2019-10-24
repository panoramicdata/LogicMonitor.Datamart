using Divergic.Logging.Xunit;
using LogicMonitor.Datamart.Logging;
using LogicMonitor.Datamart.Models;
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
	public class TestWithOutput
	{
		protected static Dictionary<string, List<DataSourceDataPointModel>> DataSourceSpecifications = new Dictionary<string, List<DataSourceDataPointModel>>
		{
			{ "WinCPU",
				new List<DataSourceDataPointModel>
				{
					new DataSourceDataPointModel
						{
							Name = "CPUBusyPercent",
							MeasurementUnit = "%"
						},
					new DataSourceDataPointModel
						{
							Name= "ProcessorQueueLength",
							MeasurementUnit = "count"
						}
				}
			}
		};

		protected TestWithOutput(ITestOutputHelper iTestOutputHelper)
		{
			ITestOutputHelper = iTestOutputHelper;
			Logger = iTestOutputHelper.BuildLogger();
			var nowUtc = DateTimeOffset.UtcNow;
			StartEpoch = nowUtc.AddDays(-30).ToUnixTimeSeconds();
			EndEpoch = nowUtc.ToUnixTimeSeconds();
			var configuration = LoadConfiguration("appsettings.json");
			var logicMonitorCredentials = configuration.LogicMonitorCredentials;
			var loggerFactory = new PrefixLoggerFactory(logicMonitorCredentials.Account, LogFactory.Create(iTestOutputHelper));
			DatamartClient = new DatamartClient(
				logicMonitorCredentials.Account,
				logicMonitorCredentials.AccessId,
				logicMonitorCredentials.AccessKey,
				DatabaseType.SqlServer,
				configuration.DatabaseServer,
				configuration.DatabaseName,
				DataSourceSpecifications,
				loggerFactory);

			DatamartClient.EnsureDatabaseCreatedAndSchemaUpdatedAsync().GetAwaiter().GetResult();

			Stopwatch = Stopwatch.StartNew();
		}

		protected static Configuration LoadConfiguration(string jsonFilePath)
		{
			var location = typeof(TestWithOutput).GetTypeInfo().Assembly.Location;
			var dirPath = Path.Combine(Path.GetDirectoryName(location), "../../..");

			Configuration configuration;
			var configurationRoot = new ConfigurationBuilder()
				.SetBasePath(dirPath)
				.AddJsonFile(jsonFilePath, false, false)
				.Build();
			var services = new ServiceCollection();
			services.AddOptions();
			services.Configure<Configuration>(configurationRoot);
			using (var sp = services.BuildServiceProvider())
			{
				var options = sp.GetService<IOptions<Configuration>>();
				configuration = options.Value;
			}

			return configuration;
		}

		protected ILogger Logger { get; }

		protected ITestOutputHelper ITestOutputHelper { get; }

		private Stopwatch Stopwatch { get; }

		protected long StartEpoch { get; }
		protected long EndEpoch { get; }
		protected DatamartClient DatamartClient { get; }

		protected void AssertIsFast(int durationSeconds) => Assert.InRange(Stopwatch.ElapsedMilliseconds, 0, durationSeconds * 1000);
	}
}