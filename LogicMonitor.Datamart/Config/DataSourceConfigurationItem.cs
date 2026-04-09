namespace LogicMonitor.Datamart.Config;

/// <summary>
/// Configuration item for a DataSource to sync into the datamart.
/// </summary>
public class DataSourceConfigurationItem : LogicModuleConfigurationItem
{
	/// <summary>
	/// The list of DataPoints which we are interested in for this DataSource
	/// </summary>
	public List<DataPointConfigurationItem> DataPoints { get; set; } = [];

	/// <summary>
	/// Validates the DataSource configuration item and all its DataPoints.
	/// </summary>
	public void Validate()
	{
		ValidateBase();

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
