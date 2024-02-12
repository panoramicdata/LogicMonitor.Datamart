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
	/// Property 1
	/// </summary>
	public string Property1 { get; set; } = string.Empty;

	/// <summary>
	/// Property 2
	/// </summary>
	public string Property2 { get; set; } = string.Empty;

	/// <summary>
	/// Property 3
	/// </summary>
	public string Property3 { get; set; } = string.Empty;

	/// <summary>
	/// Property 4
	/// </summary>
	public string Property4 { get; set; } = string.Empty;

	/// <summary>
	/// Property 5
	/// </summary>
	public string Property5 { get; set; } = string.Empty;

	/// <summary>
	/// Property 6
	/// </summary>
	public string Property6 { get; set; } = string.Empty;

	/// <summary>
	/// Property 7
	/// </summary>
	public string Property7 { get; set; } = string.Empty;

	/// <summary>
	/// Property 8
	/// </summary>
	public string Property8 { get; set; } = string.Empty;

	/// <summary>
	/// Property 9
	/// </summary>
	public string Property9 { get; set; } = string.Empty;

	/// <summary>
	/// Property 10
	/// </summary>
	public string Property10 { get; set; } = string.Empty;

	/// <summary>
	/// InstanceDatapointProperty 1
	/// </summary>
	public string InstanceDatapointProperty1 { get; set; } = string.Empty;

	/// <summary>
	/// InstanceDatapointProperty 2
	/// </summary>
	public string InstanceDatapointProperty2 { get; set; } = string.Empty;

	/// <summary>
	/// InstanceDatapointProperty 3
	/// </summary>
	public string InstanceDatapointProperty3 { get; set; } = string.Empty;

	/// <summary>
	/// InstanceDatapointProperty 4
	/// </summary>
	public string InstanceDatapointProperty4 { get; set; } = string.Empty;

	/// <summary>
	/// InstanceDatapointProperty 5
	/// </summary>
	public string InstanceDatapointProperty5 { get; set; } = string.Empty;

	/// <summary>
	/// InstanceDatapointProperty 6
	/// </summary>
	public string InstanceDatapointProperty6 { get; set; } = string.Empty;

	/// <summary>
	/// InstanceDatapointProperty 7
	/// </summary>
	public string InstanceDatapointProperty7 { get; set; } = string.Empty;

	/// <summary>
	/// InstanceDatapointProperty 8
	/// </summary>
	public string InstanceDatapointProperty8 { get; set; } = string.Empty;

	/// <summary>
	/// InstanceDatapointProperty 9
	/// </summary>
	public string InstanceDatapointProperty9 { get; set; } = string.Empty;

	/// <summary>
	/// InstanceDatapointProperty 10
	/// </summary>
	public string InstanceDatapointProperty10 { get; set; } = string.Empty;

	/// <summary>
	/// If true, the DataPoint will be resynced even if it is not out of sync
	/// This will remove all old data and replace it with new data
	/// </summary>
	public bool ResyncTimeSeriesData { get; set; }

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
