namespace LogicMonitor.Datamart.Models;

public class MonitorObjectGroupStoreItem : StoreItem
{
	// Navigation properties
	public virtual ICollection<AlertStoreItem>? AlertsFromGroup0 { get; set; }
	public virtual ICollection<AlertStoreItem>? AlertsFromGroup1 { get; set; }
	public virtual ICollection<AlertStoreItem>? AlertsFromGroup2 { get; set; }
	public virtual ICollection<AlertStoreItem>? AlertsFromGroup3 { get; set; }
	public virtual ICollection<AlertStoreItem>? AlertsFromGroup4 { get; set; }
	public virtual ICollection<AlertStoreItem>? AlertsFromGroup5 { get; set; }
	public virtual ICollection<AlertStoreItem>? AlertsFromGroup6 { get; set; }
	public virtual ICollection<AlertStoreItem>? AlertsFromGroup7 { get; set; }
	public virtual ICollection<AlertStoreItem>? AlertsFromGroup8 { get; set; }
	public virtual ICollection<AlertStoreItem>? AlertsFromGroup9 { get; set; }

	// Database fields
	[MaxLength(200)]
	public string FullPath { get; set; } = string.Empty;

	public MonitoredObjectType MonitoredObjectType { get; set; }
}
