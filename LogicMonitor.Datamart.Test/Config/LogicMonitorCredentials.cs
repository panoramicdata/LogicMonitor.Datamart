namespace LogicMonitor.Datamart.Test.Config;

public class LogicMonitorCredentials
{
	/// <summary>
	/// The LogicMonitor account
	/// For example, if your URL is https://example.logicmonitor.com/
	/// ... set to "example"
	/// </summary>
	public string Account { get; set; }

	/// <summary>
	/// The access Id.
	/// See https://www.logicmonitor.com/support/settings/users-and-roles/api-tokens/
	/// </summary>
	public string AccessId { get; set; }

	/// <summary>
	/// The access key.
	/// See https://www.logicmonitor.com/support/settings/users-and-roles/api-tokens/
	/// </summary>
	public string AccessKey { get; set; }
}
