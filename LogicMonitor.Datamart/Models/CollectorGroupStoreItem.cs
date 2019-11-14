using System.Collections.Generic;

namespace LogicMonitor.Datamart.Models
{
	public class CollectorGroupStoreItem : IdentifiedStoreItem
	{
		// Navigation properties
		public List<CollectorStoreItem> Collectors { get; set; }

		// Database properties
		public string Name { get; set; }

		public string Description { get; set; }

		public long CreatedOnTimeStampSeconds { get; set; }

		public int CollectorCount { get; set; }
	}
}
