namespace LogicMonitor.Datamart.Models;

/// <summary>
/// Represents a LogicMonitor ConfigSource stored in the datamart.
/// </summary>
public class ConfigSourceStoreItem : LogicModuleStoreItem
{

	/// <summary>
	/// The display name shown in the LogicMonitor UI.
	/// </summary>
	public required string DisplayName { get; set; }
    
	/// <summary>
	/// The data collection method (e.g. SNMP, Script, WMI).
	/// </summary>
	public required string CollectionMethod { get; set; }
}
