using System.Collections.Generic;

namespace LogicMonitor.Datamart.Models
{
	public class CollectorGroupStoreItem : IdentifiedStoreItem
	{
#pragma warning disable CA2227 // Collection properties should be read only
		// Navigation properties
		public List<CollectorStoreItem> Collectors { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

		// Database properties
		public string Name { get; set; }

		public string Description { get; set; }

		public long CreatedOnTimeStampSeconds { get; set; }

		public int CollectorCount { get; set; }
	}
}
