using LogicMonitor.Api;

namespace LogicMonitor.Datamart.Test;

public abstract class TestWithOutput
{
	protected static DateTimeOffset TwelveHoursAgo = DateTimeOffset.UtcNow.AddHours(-12);

	protected static Configuration Configuration = new()
	{
		Name = "Test",
		AggregationDurationMinutes = 15,
		LateArrivingDataWindowHours = 2,
		StartDateTimeUtc = new DateTimeOffset(
			TwelveHoursAgo.Year,
			1,
			1,
			0,
			0,
			0,
			TimeSpan.Zero).UtcDateTime,

		DeviceProperties = new List<string> { "fix.url", "ReportMagic", "location", "customer.code" },

		DataSources = new List<DataSourceConfigurationItem>
			{
				new DataSourceConfigurationItem{
					Name = "WinCPU",
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
					},
				},
				new DataSourceConfigurationItem{
					Name = "SNMP_Network_Interfaces",
					DataPoints = new List<DataPointConfigurationItem>
					{
						new DataPointConfigurationItem
							{
								Name = "InMbps",
								MeasurementUnit = "MBps",
							},
						new DataPointConfigurationItem
							{
								Name = "OutMbps",
								MeasurementUnit = "MBps",
							},
						new DataPointConfigurationItem
							{
								Name = "OutGbps",
								Calculation = "OutMbps / 1024",
								MeasurementUnit = "GBps",
								Tags = "Calculated",
								Description = "We calculate the Outbound bandwidth in Gbps as: OutMbps / 1024",
							}
					},
				},
				new DataSourceConfigurationItem{
					Name = "Non_Existent",
					DataPoints = new List<DataPointConfigurationItem>
					{
						new DataPointConfigurationItem
							{
								Name = "Blah",
								MeasurementUnit = "Grains",
							},
						new DataPointConfigurationItem
							{
								Name = "Foo",
								MeasurementUnit = "Chains",
							}
					},
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

		Configuration.LogicMonitorClientOptions = new LogicMonitorClientOptions
		{
			Account = logicMonitorCredentials.Account,
			AccessId = logicMonitorCredentials.AccessId,
			AccessKey = logicMonitorCredentials.AccessKey,
			Logger = loggerFactory.CreateLogger<LogicMonitorClient>()
		};

		Configuration.DatabaseType = configuration.DatabaseType;
		Configuration.DatabaseServerName = configuration.DatabaseServer;
		Configuration.DatabaseServerPort = configuration.DatabaseServerPort;
		Configuration.DatabaseName = configuration.DatabaseName;
		Configuration.DatabaseUsername = configuration.DatabaseUsername;
		Configuration.DatabasePassword = configuration.DatabasePassword;
		DatamartClient = new DatamartClient(
			Configuration,
			loggerFactory);

		DatamartClient
			.EnsureDatabaseCreatedAndSchemaUpdatedAsync(default)
			.GetAwaiter()
			.GetResult();

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
