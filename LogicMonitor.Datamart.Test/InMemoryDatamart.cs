using LogicMonitor.Api;
using Microsoft.Extensions.Logging.Abstractions;

namespace LogicMonitor.Datamart.Test;

/// <summary>
/// Creates a <see cref="DatamartClient"/> backed by a private EF Core in-memory database, with no network access.
/// </summary>
internal static class InMemoryDatamart
{
	public static DatamartClient CreateClient(Action<Configuration>? configure = null)
	{
		var configuration = new Configuration
		{
			Name = "InMemory",
			StartDateTimeUtc = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero).UtcDateTime,
			DatabaseType = DatabaseType.InMemory,
			DatabaseServerName = "localhost",
			DatabaseServerPort = 1,
			DatabaseName = Guid.NewGuid().ToString(),
			LogicMonitorClientOptions = new LogicMonitorClientOptions
			{
				Account = "offline-test",
				AccessId = "id",
				AccessKey = "key",
				Logger = NullLogger.Instance,
			},
			DataSources =
			[
				new DataSourceConfigurationItem
				{
					Name = "WinCPU",
					DataPoints = [new DataPointConfigurationItem { Name = "CPUBusyPercent", MeasurementUnit = "%" }],
				},
			],
		};
		configure?.Invoke(configuration);

		return new DatamartClient(configuration, NullLoggerFactory.Instance);
	}
}
