namespace LogicMonitor.Datamart.Models
{
	public class EscalationChainStoreItem : IdentifiedStoreItem
	{
		public string Name { get; set; }

		public string Description { get; set; }

		public bool EnableThrottling { get; set; }

		public int ThrottlingPeriodMinutes { get; set; }

		public int ThrottlingAlertCount { get; set; }

		public bool InAlerting { get; set; }
	}
}
