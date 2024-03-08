namespace LogicMonitor.Datamart.Config;

public class DataSourceConfigurationItem
{
	/// <summary>
	/// The DataSource name
	/// </summary>
	public string Name { get; set; } = string.Empty;

	///// <summary>
	///// The aggregation duration override in minutes
	///// Can be overridden at the lower level
	///// </summary>
	//public int? AggregationDurationMinutes { get; set; }

	/// <summary>
	/// The NCalc to execute when determining whether to include a DeviceDataSourceInstance
	/// All properties of the DeviceDataSourceInstance are available for the calculation
	/// </summary>
	public string InstanceInclusionExpression { get; set; } = "true";

	/// <summary>
	/// The list of DataPoints which we are interested in for this DataSource
	/// </summary>
	public List<DataPointConfigurationItem> DataPoints { get; set; } = [];

	public string AppliesTo { get; set; } = string.Empty;

	public string InstanceProperty1 { get; set; } = string.Empty;

	public string InstanceProperty2 { get; set; } = string.Empty;

	public string InstanceProperty3 { get; set; } = string.Empty;

	public string InstanceProperty4 { get; set; } = string.Empty;

	public string InstanceProperty5 { get; set; } = string.Empty;

	public string InstanceProperty6 { get; set; } = string.Empty;

	public string InstanceProperty7 { get; set; } = string.Empty;

	public string InstanceProperty8 { get; set; } = string.Empty;

	public string InstanceProperty9 { get; set; } = string.Empty;

	public string InstanceProperty10 { get; set; } = string.Empty;

	public void Validate()
	{
		if (string.IsNullOrWhiteSpace(Name))
		{
			throw new ConfigurationException("Name missing on configured DataSource.");
		}

		//if (AggregationDurationMinutes != null)
		//{
		//	Configuration.ValidateAggegationDuration(AggregationDurationMinutes.Value, "dataSource", Name);
		//}

		if (DataPoints == null || DataPoints.Count == 0)
		{
			throw new ConfigurationException($"DataPoints missing for DataSource {Name}");
		}

		foreach (var dataPointConfigurationItem in DataPoints)
		{
			try
			{
				dataPointConfigurationItem.Validate();
			}
			catch (ConfigurationException exception)
			{
				throw new ConfigurationException($"Issue in config for DataSource {dataPointConfigurationItem}: {exception.Message}", exception);
			}
		}
	}
}
