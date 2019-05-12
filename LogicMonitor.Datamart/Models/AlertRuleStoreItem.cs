using System.Collections.Generic;

namespace LogicMonitor.Datamart.Models
{
	public class AlertRuleStoreItem : IdentifiedStoreItem
	{
#pragma warning disable CA2227 // Collection properties should be read only
		// Navigation properties
		public List<AlertStoreItem> Alerts { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

		// DataStore properties
		public string Name { get; set; }

		public string Description { get; set; }

		public int Priority { get; set; }

		public string LevelString { get; set; }

		public string DataSourceName { get; set; }

		public string DataSourceInstanceName { get; set; }

		public string DataPoint { get; set; }

		public int EscalationChainIntervalMinutes { get; set; }

		public int EscalationChainId { get; set; }

		public bool SuppressAlertClear { get; set; }

		public bool SuppressAlertAckSdt { get; set; }
	}
}
