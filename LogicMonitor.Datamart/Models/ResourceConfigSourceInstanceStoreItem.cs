﻿namespace LogicMonitor.Datamart.Models;

public class ResourceConfigSourceInstanceStoreItem : ResourceLogicModuleInstanceStoreItem
{
	// Navigation properties
	public ResourceConfigSourceStoreItem? DeviceConfigSource { get; set; }

	public Guid DeviceConfigSourceId { get; set; }

	public virtual ICollection<ResourceConfigSourceInstanceConfigStoreItem>? DeviceConfigSourceInstanceConfigs { get; set; }

}
