using Microsoft.Data.SqlClient;

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
	public required LogicMonitorCredentials LogicMonitorCredentials { get; set; }

	/// <summary>
	/// Server name
	/// </summary>
	public required string DatabaseServer { get; set; }

	/// <summary>
	/// Server port
	/// </summary>
	public int? DatabaseServerPort { get; set; }

	/// <summary>
	/// The Database server retry on failure count
	/// </summary>
	public int? DatabaseRetryOnFailureCount { get; set; }

	/// <summary>
	/// DatabaseName
	/// </summary>
	public required string DatabaseName { get; set; }

	public DatabaseType DatabaseType { get; set; }

	public required string DatabaseUsername { get; set; }

	public required string DatabasePassword { get; set; }

	/// <summary>
	/// The non-standard SQL Server authentication method
	/// </summary>
	public SqlAuthenticationMethod? SqlServerAuthenticationMethod { get; set; }
}
