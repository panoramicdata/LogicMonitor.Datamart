using PanoramicData.NCalcExtensions;

namespace LogicMonitor.Datamart.Config;

/// <summary>
/// The configuration specification for a DataPoint
/// </summary>
public class DataPointConfigurationItem
{
	/// <summary>
	/// The LogicMonitor unique DataSource name (or the name if it's a calculation)
	/// </summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// A description of the DataPoint for the benefit of the developer
	/// </summary>
	public string Description { get; set; } = string.Empty;

	/// <summary>
	/// The measurement unit to report to downstream systems
	/// </summary>
	public string MeasurementUnit { get; set; } = string.Empty;

	/// <summary>
	/// The means of calculating the percentage availability
	/// </summary>
	public string PercentageAvailabilityCalculation { get; set; } = string.Empty;

	/// <summary>
	/// The
	/// </summary>
	public string GlobalAlertExpression { get; set; } = string.Empty;

	/// <summary>
	/// The
	/// </summary>
	public string Calculation { get; set; } = string.Empty;

	/// <summary>
	/// The measurement unit to report to downstream systems
	/// </summary>
	public string Tags { get; set; } = string.Empty;

	/// <summary>
	/// Validate the DataPoint
	/// </summary>
	public void Validate()
	{
		if (string.IsNullOrWhiteSpace(Name))
		{
			throw new ConfigurationException("Name not set on DataPoint.");
		}

		if (string.IsNullOrWhiteSpace(MeasurementUnit))
		{
			throw new ConfigurationException($"MeasurementUnit not set on DataPoint '{Name}'.");
		}

		if (!string.IsNullOrWhiteSpace(Calculation))
		{
			var expression = new ExtendedExpression(Calculation);
			if (expression.HasErrors())
			{
				throw new ConfigurationException($"Calculation '{Calculation}' is an invalid NCalc expression on DataPoint '{Name}'.");
			}
		}
	}
}
