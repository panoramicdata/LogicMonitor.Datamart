namespace LogicMonitor.Datamart.Models;

public class MonitorObjectGroupStoreItem : StoreItem
{
	// Navigation properties
	public List<AlertStoreItem> AlertsFromGroup0 { get; set; }
	public List<AlertStoreItem> AlertsFromGroup1 { get; set; }
	public List<AlertStoreItem> AlertsFromGroup2 { get; set; }
	public List<AlertStoreItem> AlertsFromGroup3 { get; set; }
	public List<AlertStoreItem> AlertsFromGroup4 { get; set; }
	public List<AlertStoreItem> AlertsFromGroup5 { get; set; }
	public List<AlertStoreItem> AlertsFromGroup6 { get; set; }
	public List<AlertStoreItem> AlertsFromGroup7 { get; set; }
	public List<AlertStoreItem> AlertsFromGroup8 { get; set; }
	public List<AlertStoreItem> AlertsFromGroup9 { get; set; }

	// Database fields
	[MaxLength(200)]
	public string FullPath { get; set; }

	public MonitoredObjectType MonitoredObjectType { get; set; }
}
