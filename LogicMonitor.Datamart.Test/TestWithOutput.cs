using LogicMonitor.Api;
using LogicMonitor.Datamart.Interfaces;

namespace LogicMonitor.Datamart.Test;

public abstract class TestWithOutput
{
	protected static DateTimeOffset TwelveHoursAgo = DateTimeOffset.UtcNow.AddHours(-12);

	protected INotificationReceiver TestNotificationReceiver { get; }

	protected static Configuration Configuration = new()
	{
		Name = "Test",
		AggregationDurationMinutes = 15,
		LateArrivingDataWindowHours = 2,
		StartDateTimeUtc =
		new DateTimeOffset(
			TwelveHoursAgo.Year,
			1,
			1,
			0,
			0,
			0,
			TimeSpan.Zero)
		.UtcDateTime,

		DeviceProperties = ["fix.url", "ReportMagic", "location", "customer.code"],

		DataSources =
			[
				new DataSourceConfigurationItem{
					Name = "WinCPU",
					DataPoints =
					[
						new DataPointConfigurationItem
						{
							Name = "CPUBusyPercent",
							MeasurementUnit = "%",
							ResyncTimeSeriesData = true
						},
						new DataPointConfigurationItem
						{
							Name= "ProcessorQueueLength",
							MeasurementUnit = "count"
						}
					]
				},
				//new DataSourceConfigurationItem{
				//	Name = "SNMP_Network_Interfaces",
				//	InstanceInclusionExpression = "Description != ''",
				//	DataPoints = new List<DataPointConfigurationItem>
				//	{
				//		new DataPointConfigurationItem
				//			{
				//				Name = "InMbps",
				//				MeasurementUnit = "MBps",
				//			},
				//		new DataPointConfigurationItem
				//			{
				//				Name = "OutMbps",
				//				MeasurementUnit = "MBps",
				//			},
				//		new DataPointConfigurationItem
				//			{
				//				Name = "OutGbps",
				//				Calculation = "OutMbps / 1024",
				//				GlobalAlertExpression = "> 1 2 3",
				//				MeasurementUnit = "GBps",
				//				Tags = "Calculated",
				//				Property1 = "Value1",
				//				Property2 = "Value2",
				//				Property3 = "Value3",
				//				Property4 = "Value4",
				//				Property5 = "Value5",
				//				Property6 = "Value6",
				//				Property7 = "Value7",
				//				Property8 = "Value8",
				//				Property9 = "Value9",
				//				Property10 = "Value10",
				//				Description = "We calculate the Outbound bandwidth in Gbps as: OutMbps / 1024",
				//			}
				//	},
				//},
				new DataSourceConfigurationItem{
					Name = "Non_Existent",
					DataPoints =
					[
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
					]
				},
				new DataSourceConfigurationItem{
					Name = "HostStatus",
					DataPoints =
					[
						new DataPointConfigurationItem
						{
							//Condition = "isSet('xxx')",
							Name = "idleInterval",
							MeasurementUnit = "Seconds",
						}
					]
				},
				new DataSourceConfigurationItem{
					Name = "SNMP_Host_Uptime",
					DataPoints =
					[
						new DataPointConfigurationItem
						{
							Name = "Uptime",
							MeasurementUnit = "Seconds",
							PercentageAvailabilityCalculation = "PercentUpTime"
						}
					]
				},
				new DataSourceConfigurationItem{
					Name = "WinSystemUptime",
					DataPoints =
					[
						new DataPointConfigurationItem
						{
							Name = "UptimeSeconds",
							MeasurementUnit = "Seconds",
							PercentageAvailabilityCalculation = "PercentUpTime"
						}
					]
				}
			]
	};

	protected TestWithOutput(ITestOutputHelper iTestOutputHelper)
	{
		ITestOutputHelper = iTestOutputHelper;
		var nowUtc = DateTimeOffset.UtcNow;
		StartEpoch = nowUtc.AddDays(-30).ToUnixTimeSeconds();
		EndEpoch = nowUtc.ToUnixTimeSeconds();
		var configuration = LoadConfiguration("appsettings.json");
		var logicMonitorCredentials = configuration.LogicMonitorCredentials;
		var loggerFactory = iTestOutputHelper.BuildLoggerFactory();

		Configuration.LogicMonitorClientOptions = new LogicMonitorClientOptions
		{
			Account = logicMonitorCredentials.Account,
			AccessId = logicMonitorCredentials.AccessId,
			AccessKey = logicMonitorCredentials.AccessKey,
			Logger = loggerFactory.CreateLogger<LogicMonitorClient>(),
			HttpClientTimeoutSeconds = 3600
		};

		Configuration.DatabaseType = configuration.DatabaseType;
		Configuration.DatabaseServerName = configuration.DatabaseServer;
		Configuration.DatabaseServerPort = configuration.DatabaseServerPort;
		Configuration.DatabaseName = configuration.DatabaseName;
		Configuration.DatabaseUsername = configuration.DatabaseUsername;
		Configuration.DatabasePassword = configuration.DatabasePassword;
		Configuration.DatabaseRetryOnFailureCount = configuration.DatabaseRetryOnFailureCount;
		DatamartClient = new DatamartClient(
			Configuration,
			loggerFactory);

		DatamartClient
			.EnsureDatabaseCreatedAndSchemaUpdatedAsync(default)
			.GetAwaiter()
			.GetResult();

		Stopwatch = Stopwatch.StartNew();

		LoggerFactory = LogFactory.Create(iTestOutputHelper);

		TestNotificationReceiver = new TestNotificationReceiver(LoggerFactory.CreateLogger<TestWithOutput>());
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
