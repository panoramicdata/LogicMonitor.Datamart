using LogicMonitor.Datamart.Exceptions;

namespace LogicMonitor.Datamart.Config
{
	/// <summary>
	/// The configuration specification for a DataPoint
	/// </summary>
	public class DataPointConfigurationItem
	{
		/// <summary>
		/// The LogicMonitor unique DataSource name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The measurement unit to report to downstream systems
		/// </summary>
		public string MeasurementUnit { get; set; }

		/// <summary>
		/// Validate the DataPoint
		/// </summary>
		public void Validate()
		{
			if (string.IsNullOrWhiteSpace(Name))
			{
				throw new ConfigurationException($"Name not set on DataPoint.");
			}

			if (string.IsNullOrWhiteSpace(MeasurementUnit))
			{
				throw new ConfigurationException($"MeasurementUnit not set on DataPoint '{Name}'.");
			}
		}
	}
}
