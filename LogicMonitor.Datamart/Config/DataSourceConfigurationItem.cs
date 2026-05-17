namespace LogicMonitor.Datamart.Config;

/// <summary>
/// Configuration item for a DataSource to sync into the datamart.
/// </summary>
public class DataSourceConfigurationItem : LogicModuleConfigurationItem
{
	/// <summary>
	/// The LogicMonitor DataSource ID.
	/// When provided, the sync resolves the DataSource by this ID rather than (or in addition to) <see cref="LogicModuleConfigurationItem.Name"/>.
	/// Supported modes:
	/// - Name only (backward compatible): resolves by name; warns if duplicates exist.
	/// - LogicMonitorId only: resolves by ID; Name is treated as a label only.
	/// - Both: resolves by ID and validates that the name matches; fails if they disagree.
	/// </summary>
	public int? LogicMonitorId { get; set; }

	/// <summary>
	/// The list of DataPoints which we are interested in for this DataSource
	/// </summary>
	public List<DataPointConfigurationItem> DataPoints { get; set; } = [];

	/// <inheritdoc/>
	protected override void ValidateBase()
	{
		if (LogicMonitorId is null && string.IsNullOrWhiteSpace(Name))
		{
			throw new ConfigurationException("Either Name or LogicMonitorId must be specified for a configured DataSource.");
		}
	}

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
