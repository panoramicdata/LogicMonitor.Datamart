using LogicMonitor.Datamart.Exceptions;
using System;
using System.Collections.Generic;

namespace LogicMonitor.Datamart.Models
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

			if (LateArrivingDataWindowHours <= 0)
			{
				throw new ConfigurationException("LateArrivingDataWindowHours should be a positive integer.");
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
