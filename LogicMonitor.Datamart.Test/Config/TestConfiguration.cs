namespace LogicMonitor.Datamart.Test.Config;

/// <summary>
/// Application configuration, loaded from an appsettings.json file upon execution
/// You can modify/extend this class and provide your own settings
/// </summary>
public class TestConfiguration
{
	/// <summary>
	/// LogicMonitor credentials
	/// </summary>
	public LogicMonitorCredentials LogicMonitorCredentials { get; set; }

	/// <summary>
	/// ServerName
	/// </summary>
	public string DatabaseServer { get; set; }

	/// <summary>
	/// DatabaseName
	/// </summary>
	public string DatabaseName { get; set; }

	public DatabaseType DatabaseType { get; set; }

	public string DatabaseUsername { get; set; }

	public string DatabasePassword { get; set; }
}
