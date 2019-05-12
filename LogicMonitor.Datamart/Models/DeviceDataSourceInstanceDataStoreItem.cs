using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogicMonitor.Datamart.Models
{
	public class DeviceDataSourceInstanceDataStoreItem
	{
		// Navigation properties
		public DeviceDataSourceInstanceStoreItem DeviceDataSourceInstance { get; set; }

		// Database properties
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public DateTime DateTime { get; set; }

		public int DeviceDataSourceInstanceId { get; set; }

		public string DataPointName { get; set; }

		public double? Value { get; set; }
	}
}