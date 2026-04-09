namespace LogicMonitor.Datamart.Models;

/// <summary>
/// Represents a DataSource assignment to a resource stored in the datamart.
/// </summary>
public class ResourceDataSourceStoreItem : ResourceLogicModuleStoreItem
{
	/// <summary>
	/// Navigation property to the DataSource instances on this resource.
	/// </summary>
	public virtual ICollection<ResourceDataSourceInstanceStoreItem>? DeviceDataSourceInstances { get; set; }

	/// <summary>
	/// Navigation property to the parent DataSource definition.
	/// </summary>
	public DataSourceStoreItem? DataSource { get; set; }

	/// <summary>
	/// The datamart identifier of the DataSource.
	/// </summary>
	public Guid DataSourceId { get; set; }
}