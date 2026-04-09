namespace LogicMonitor.Datamart.Models;

/// <summary>
/// Represents a DataSource instance on a resource stored in the datamart.
/// </summary>
public class ResourceDataSourceInstanceStoreItem : ResourceLogicModuleInstanceStoreItem
{
	/// <summary>
	/// Navigation property to the parent resource-DataSource assignment.
	/// </summary>
	public ResourceDataSourceStoreItem? DeviceDataSource { get; set; }

	/// <summary>
	/// The datamart identifier of the parent resource-DataSource assignment.
	/// </summary>
	public Guid DeviceDataSourceId { get; set; }

	/// <summary>
	/// Navigation property to the DataPoint values collected by this instance.
	/// </summary>
	public virtual ICollection<ResourceDataSourceInstanceDataPointStoreItem> DeviceDataSourceInstanceDataPoints { get; set; } = null!;
}