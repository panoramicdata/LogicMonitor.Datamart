namespace LogicMonitor.Datamart.Models;

public class MonitorObjectGroupStoreItem : StoreItem
{
	// Navigation properties
	public ICollection<AlertStoreItem> AlertsFromGroup0 { get; set; }
	public ICollection<AlertStoreItem> AlertsFromGroup1 { get; set; }
	public ICollection<AlertStoreItem> AlertsFromGroup2 { get; set; }
	public ICollection<AlertStoreItem> AlertsFromGroup3 { get; set; }
	public ICollection<AlertStoreItem> AlertsFromGroup4 { get; set; }
	public ICollection<AlertStoreItem> AlertsFromGroup5 { get; set; }
	public ICollection<AlertStoreItem> AlertsFromGroup6 { get; set; }
	public ICollection<AlertStoreItem> AlertsFromGroup7 { get; set; }
	public ICollection<AlertStoreItem> AlertsFromGroup8 { get; set; }
	public ICollection<AlertStoreItem> AlertsFromGroup9 { get; set; }

	// Database fields
	[MaxLength(200)]
	public string FullPath { get; set; }

	public MonitoredObjectType MonitoredObjectType { get; set; }
}
