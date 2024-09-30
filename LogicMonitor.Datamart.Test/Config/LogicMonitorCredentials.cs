namespace LogicMonitor.Datamart.Test.Config;

public class LogicMonitorCredentials
{
	/// <summary>
	/// The LogicMonitor account
	/// For example, if your URL is https://example.logicmonitor.com/
	/// ... set to "example"
	/// </summary>
	public required string Account { get; set; }

	/// <summary>
	/// The access Id.
	/// See https://www.logicmonitor.com/support/settings/users-and-roles/api-tokens/
	/// </summary>
	public required string AccessId { get; set; }

	/// <summary>
	/// The access key.
	/// See https://www.logicmonitor.com/support/settings/users-and-roles/api-tokens/
	/// </summary>
	public required string AccessKey { get; set; }
}
