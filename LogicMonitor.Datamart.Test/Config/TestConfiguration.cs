﻿namespace LogicMonitor.Datamart.Test.Config
{
#pragma warning disable CA1812 // Avoid uninstantiated internal classes - This is instantiated through DI
	/// <summary>
	/// Application configuration, loaded from an appsettings.json file upon execution
	/// You can modify/extend this class and provide your own settings
	/// </summary>
	public class TestConfiguration
#pragma warning restore CA1812 // Avoid uninstantiated internal classes - End
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
	}
}