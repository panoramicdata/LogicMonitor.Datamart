namespace LogicMonitor.Datamart.Models;

/// <summary>
/// Represents a monitor object group (resource group path segment) used for alert grouping in the datamart.
/// </summary>
public class MonitorObjectGroupStoreItem : StoreItem
{
	/// <summary>
	/// Alerts where this group is at hierarchy index 0.
	/// </summary>
	public virtual ICollection<AlertStoreItem>? AlertsFromGroup0 { get; set; }

	/// <summary>
	/// Alerts where this group is at hierarchy index 1.
	/// </summary>
	public virtual ICollection<AlertStoreItem>? AlertsFromGroup1 { get; set; }

	/// <summary>
	/// Alerts where this group is at hierarchy index 2.
	/// </summary>
	public virtual ICollection<AlertStoreItem>? AlertsFromGroup2 { get; set; }

	/// <summary>
	/// Alerts where this group is at hierarchy index 3.
	/// </summary>
	public virtual ICollection<AlertStoreItem>? AlertsFromGroup3 { get; set; }

	/// <summary>
	/// Alerts where this group is at hierarchy index 4.
	/// </summary>
	public virtual ICollection<AlertStoreItem>? AlertsFromGroup4 { get; set; }

	/// <summary>
	/// Alerts where this group is at hierarchy index 5.
	/// </summary>
	public virtual ICollection<AlertStoreItem>? AlertsFromGroup5 { get; set; }

	/// <summary>
	/// Alerts where this group is at hierarchy index 6.
	/// </summary>
	public virtual ICollection<AlertStoreItem>? AlertsFromGroup6 { get; set; }

	/// <summary>
	/// Alerts where this group is at hierarchy index 7.
	/// </summary>
	public virtual ICollection<AlertStoreItem>? AlertsFromGroup7 { get; set; }

	/// <summary>
	/// Alerts where this group is at hierarchy index 8.
	/// </summary>
	public virtual ICollection<AlertStoreItem>? AlertsFromGroup8 { get; set; }

	/// <summary>
	/// Alerts where this group is at hierarchy index 9.
	/// </summary>
	public virtual ICollection<AlertStoreItem>? AlertsFromGroup9 { get; set; }

	/// <summary>
	/// The full path of the monitor object group in the hierarchy.
	/// </summary>
	[MaxLength(200)]
	public string FullPath { get; set; } = string.Empty;

	/// <summary>
	/// The type of monitored object this group applies to.
	/// </summary>
	public MonitoredObjectType MonitoredObjectType { get; set; }
}
