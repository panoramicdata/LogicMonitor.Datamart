using System.ComponentModel.DataAnnotations;

namespace LogicMonitor.Datamart.Config
{
	/// <summary>
	/// Credentials used to access LogicMonitor
	/// </summary>
	public class LogicMonitorCredential
	{
		/// <summary>
		/// The LogicMonitor account name
		/// </summary>
		[Required]
		public string Subdomain { get; set; }

		/// <summary>
		/// The LogicMonitor AccessId
		/// </summary>
		[Required]
		public string AccessId { get; set; }

		/// <summary>
		/// The LogicMonitor AccessKey
		/// </summary>
		[Required]
		public string AccessKey { get; set; }
	}
}