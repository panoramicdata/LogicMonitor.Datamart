namespace LogicMonitor.Datamart.Config;

public class ConfigSourceConfigurationItem : LogicModuleConfigurationItem
{
	/// <summary>
	/// The maximum age in days to keep historic configurations
	/// </summary>
	public int MaxAgeDays { get; set; }

	/// <summary>
	/// Validate
	/// </summary>
	public void Validate()
	{
		ValidateBase();
		if (MaxAgeDays < 0)
		{
			throw new ConfigurationException($"HistoricRetentionMaxAgeDays must be >= 0 for ConfigSource {Name}");
		}
	}
}