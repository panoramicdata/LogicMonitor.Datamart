﻿namespace LogicMonitor.Datamart.Config;

public class Configuration
{
	/// <summary>
	/// The configuration name
	/// </summary>
	public string Name { get; set; } = null!;

	/// <summary>
	/// The DataSources
	/// </summary>
	public List<DataSourceConfigurationItem> DataSources { get; set; } = [];

	/// <summary>
	/// The ConfigSources
	/// </summary>
	public List<ConfigSourceConfigurationItem> ConfigSources { get; set; } = [];

	/// <summary>
	/// Whether to reset the aggregations
	/// Default false
	/// If set to true, as part of the LowResolutionDataSync, the aggregation data will be reset by:
	/// - truncating the TimeSeriesDataAggregations table
	/// - setting DeviceDataSourceInstanceDataPointStoreItem.DataCompleteTo to null for all DeviceDataSourceInstanceDataPointStoreItems
	/// </summary>
	public bool? AggregationReset { get; set; }

	/// <summary>
	/// Syncs only alerts for Devices relevant to each of the appliesTo's
	/// </summary>
	public bool? LimitAlertSyncToDataSourceAppliesTo { get; set; }

	/// <summary>
	/// Don't fetch any data before this date
	/// </summary>
	public DateTimeOffset StartDateTimeUtc { get; set; }

	/// <summary>
	/// The LogicMonitor client options
	/// </summary>
	public LogicMonitorClientOptions LogicMonitorClientOptions { get; set; } = null!;

	/// <summary>
	/// The Database type
	/// </summary>
	public DatabaseType DatabaseType { get; set; }

	/// <summary>
	/// The Database server name
	/// </summary>
	public string DatabaseServerName { get; set; } = null!;

	/// <summary>
	/// The Database server name
	/// </summary>
	public int? DatabaseServerPort { get; set; }

	/// <summary>
	/// The Database retry on failure count
	/// </summary>
	public int? DatabaseRetryOnFailureCount { get; set; }

	/// <summary>
	/// The non-standard SQL Server authentication method
	/// </summary>
	public SqlAuthenticationMethod? SqlServerAuthenticationMethod { get; set; }

	/// <summary>
	/// The Database name
	/// </summary>
	public string DatabaseName { get; set; } = null!;

	/// <summary>
	/// Optional database username for providers that utilize it
	/// </summary>
	public string DatabaseUsername { get; set; } = null!;

	/// <summary>
	/// Optional database password for providers that utilize it
	/// </summary>
	public string DatabasePassword { get; set; } = null!;

	/// <summary>
	/// The number of seconds to wait for SqlCommands to timeout
	/// </summary>
	public int SqlCommandTimeoutSeconds { get; set; } = 600;

	/// <summary>
	/// The number of seconds to wait for BulkCopy operations to timeout
	/// </summary>
	public int SqlBulkCopyTimeoutSeconds { get; set; } = 600;

	/// <summary>
	/// Whether to enable sensitive data logging.
	/// </summary>
	public bool EnableSensitiveDatabaseLogging { get; set; }

	public int DeviceDataSourceInstanceBatchSize { get; set; } = 100;

	public List<string> DeviceProperties { get; set; } = [];

	public DatamartClientMode Mode { get; set; } = DatamartClientMode.HighResolution;

	public bool DimensionSyncHaltOnError { get; set; } = true;

	// RM-16049 Ability to set a static UTC offset to apply to the reporting time period. Between -13 and 13 hours
	public int MinutesOffset { get; set; }

	public DateTime? FakeExecutionTime { get; set; }

	public void Validate()
	{
		if (string.IsNullOrWhiteSpace(Name))
		{
			throw new ConfigurationException($"Configuration {nameof(Name)} not set.");
		}

		if (StartDateTimeUtc.Minute != 0 || StartDateTimeUtc.Second != 0 || StartDateTimeUtc.Millisecond != 0)
		{
			throw new ConfigurationException($"{nameof(StartDateTimeUtc)} should always be on a UTC DateTime hour boundary.");
		}

		if (DateTimeOffset.UtcNow < StartDateTimeUtc)
		{
			throw new ConfigurationException($"{nameof(StartDateTimeUtc)} should not be in the future.");
		}

		if (LogicMonitorClientOptions == null)
		{
			throw new ConfigurationException($"{nameof(LogicMonitorClientOptions)} not set.");
		}

		if (DatabaseType == DatabaseType.Unknown)
		{
			throw new ConfigurationException($"{nameof(DatabaseType)} not set.");
		}

		if (string.IsNullOrWhiteSpace(DatabaseServerName))
		{
			throw new ConfigurationException($"{nameof(DatabaseServerName)} not set.");
		}

		if (DatabaseServerPort <= 0)
		{
			throw new ConfigurationException($"{nameof(DatabaseServerPort)} not set.");
		}

		if (string.IsNullOrWhiteSpace(DatabaseName))
		{
			throw new ConfigurationException($"{nameof(DatabaseName)} not set.");
		}

		if (DeviceDataSourceInstanceBatchSize is < 1 or > 100)
		{
			// Do not exceed 100 for BatchSize as limited by the LogicMonitor DataFetch endpoint
			throw new ConfigurationException("BatchSize should be in the range 1..100.");
		}

		if (MinutesOffset is < (-780) or > 780)
		{
			// RM-16049 states the permitted range of the UTC offset
			throw new ConfigurationException($"{nameof(MinutesOffset)} should be in the range -780..780 (i.e. -13..13 hours).");
		}

		foreach (var dataSource in DataSources)
		{
			dataSource.Validate();
		}
	}

	public override string ToString()
		=> $"{Name} ({DatabaseType} {DatabaseServerName}:{DatabaseServerPort}/{DatabaseName} password: {new string('*', DatabasePassword.Length)})";
}
