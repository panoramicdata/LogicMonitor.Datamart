using LogicMonitor.Datamart.Exceptions;
using System;
using System.Collections.Generic;

namespace LogicMonitor.Datamart.Config
{
	public class Configuration
	{
		/// <summary>
		/// The configuration name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The DataSources
		/// </summary>
		public List<DataSourceConfigurationItem> DataSources { get; set; }

		/// <summary>
		/// The aggregation duration override in minutes
		/// If not present, the duration from the parent DataSourceConfigurationItem is used.
		/// </summary>
		public int AggregationDurationMinutes { get; set; }

		/// <summary>
		/// Don't fetch any data before this date
		/// </summary>
		public DateTimeOffset StartDateTimeUtc { get; set; }

		/// <summary>
		/// The time to permit late arriving data to arrive
		/// </summary>
		public int LateArrivingDataWindowHours { get; set; }

		/// <summary>
		/// The LogicMonitor credential
		/// </summary>
		public LogicMonitorCredential LogicMonitorCredential { get; set; }

		/// <summary>
		/// The Database type
		/// </summary>
		public DatabaseType DatabaseType { get; set; }

		/// <summary>
		/// The Database server name
		/// </summary>
		public string DatabaseServerName { get; set; }

		/// <summary>
		/// The Database name
		/// </summary>
		public string DatabaseName { get; set; }

		/// <summary>
		/// Whether to enable sensitive data logging.
		/// </summary>
		public bool EnableSensitiveDatabaseLogging { get; set; }

		public int DeviceDataSourceInstanceBatchSize { get; set; } = 100;

		/// <summary>
		/// The number of aggregation day tables to retain prior to today
		/// </summary>
		public int CountAggregationDaysToRetain { get; set; } = 5;

		public void Validate()
		{
			if (string.IsNullOrWhiteSpace(Name))
			{
				throw new ConfigurationException("Configuration name not set.");
			}

			if (StartDateTimeUtc.Minute != 0 || StartDateTimeUtc.Second != 0 || StartDateTimeUtc.Millisecond != 0)
			{
				throw new ConfigurationException("StartDateTime should always be on a UTC DateTime hour boundary.");
			}

			if (DateTimeOffset.UtcNow < StartDateTimeUtc)
			{
				throw new ConfigurationException("StartDateTime should not be in the future.");
			}

			if (LateArrivingDataWindowHours <= 0)
			{
				throw new ConfigurationException("LateArrivingDataWindowHours should be a positive integer.");
			}

			if (LogicMonitorCredential == null)
			{
				throw new ConfigurationException("LogicMonitor credential not set.");
			}

			if (DatabaseType == DatabaseType.Unknown)
			{
				throw new ConfigurationException("DatabaseType not set.");
			}

			if (string.IsNullOrWhiteSpace(DatabaseServerName))
			{
				throw new ConfigurationException("DatabaseServerName not set.");
			}

			if (string.IsNullOrWhiteSpace(DatabaseName))
			{
				throw new ConfigurationException("DatabaseName not set.");
			}

			if (DeviceDataSourceInstanceBatchSize < 1 || DeviceDataSourceInstanceBatchSize > 100)
			{
				// Do not exceed 100 for BatchSize as limited by the LogicMonitor DataFetch endpoint
				throw new ConfigurationException("BatchSize should be in the range 1..100");
			}

			ValidateAggegationDuration(AggregationDurationMinutes, "configuration", Name);

			foreach (var dataSource in DataSources)
			{
				dataSource.Validate();
			}
		}

		public static void ValidateAggegationDuration(int aggregationDurationMinutes, string level, string name)
		{
			switch (aggregationDurationMinutes)
			{
				case 5:
				case 10:
				case 15:
				case 20:
				case 30:
				case 60:
					return;
				default:
					throw new ConfigurationException($"Invalid AggregationDurationMinutes {aggregationDurationMinutes} for {level} '{name}'.");
			}
		}
	}
}
