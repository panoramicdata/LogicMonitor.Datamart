namespace LogicMonitor.Datamart.Models;

public class DevicePropertyStoreItem : StoreItem
{
	public DeviceStoreItem? Device { get; set; }

	public Guid DeviceId { get; set; }

	public string Name { get; set; }

	public string Value { get; set; }

	public PropertyType Type { get; set; }
}
