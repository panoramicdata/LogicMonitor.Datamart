namespace LogicMonitor.Datamart.Config;

public class LogicModuleConfigurationItem
{
	/// <summary>
	/// The LogicModule unique name
	/// </summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// The NCalc to execute when determining whether to include a DeviceDataSourceInstance
	/// All properties of the DeviceDataSourceInstance are available for the calculation
	/// </summary>
	public string InstanceInclusionExpression { get; set; } = "true";

	/// <summary>
	/// Beyond the ConfigSource's own Applies to, the AppliesTo that must ALSO be true for the ConfigSource to be included
	/// </summary>
	public string AppliesTo { get; set; } = string.Empty;

	/// <summary>
	/// Instance property 1
	/// </summary>
	public string InstanceProperty1 { get; set; } = string.Empty;

	/// <summary>
	/// Instance property 2
	/// </summary>
	public string InstanceProperty2 { get; set; } = string.Empty;

	/// <summary>
	/// Instance property 3
	/// </summary>
	public string InstanceProperty3 { get; set; } = string.Empty;

	/// <summary>
	/// Instance property 4
	/// </summary>
	public string InstanceProperty4 { get; set; } = string.Empty;

	/// <summary>
	/// Instance property 5
	/// </summary>
	public string InstanceProperty5 { get; set; } = string.Empty;

	/// <summary>
	/// Instance property 6
	/// </summary>
	public string InstanceProperty6 { get; set; } = string.Empty;

	/// <summary>
	/// Instance property 7
	/// </summary>
	public string InstanceProperty7 { get; set; } = string.Empty;

	/// <summary>
	/// Instance property 8
	/// </summary>
	public string InstanceProperty8 { get; set; } = string.Empty;

	/// <summary>
	/// Instance property 9
	/// </summary>
	public string InstanceProperty9 { get; set; } = string.Empty;

	/// <summary>
	/// Instance property 10
	/// </summary>
	public string InstanceProperty10 { get; set; } = string.Empty;

	/// <summary>
	/// Validates the base properties
	/// </summary>
	/// <exception cref="ConfigurationException"></exception>
	protected virtual void ValidateBase()
	{
		if (string.IsNullOrWhiteSpace(Name))
		{
			throw new ConfigurationException("Name missing on configured DataSource.");
		}
	}
}
