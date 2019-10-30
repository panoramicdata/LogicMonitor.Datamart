using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogicMonitor.Datamart.Models
{
	/// <summary>
	/// DeviceDataSourceInstance/DataPoint Aggregation store item
	/// </summary>
	public class DeviceDataSourceInstanceAggregatedDataStoreItem
	{
		/// <summary>
		/// Navigation property to the DeviceDataSourceInstanceStoreItem table
		/// </summary>
		public DeviceDataSourceInstanceStoreItem DeviceDataSourceInstance { get; set; }

		/// <summary>
		/// Datamart primary key
		/// </summary>
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		/// <summary>
		/// Aggregation period start DateTime UTC
		/// </summary>
		public DateTime Hour { get; set; }

		/// <summary>
		/// The LogicMonitor DeviceDataSourceInstance id
		/// </summary>
		public int DeviceDataSourceInstanceId { get; set; }

		/// <summary>
		/// Navigation property to the DataPoint table
		/// </summary>
		public DataSourceDataPointStoreItem DataPoint { get; set; }

		/// <summary>
		/// The LogicMonitor DataPoint ID
		/// </summary>
		public int DataPointId { get; set; }

		/// <summary>
		/// The minimum (or null if no values or all values are "No Data")
		/// </summary>
		public double? Min { get; set; }

		/// <summary>
		/// The maximum (or null if no values or all values are "No Data")
		/// </summary>
		public double? Max { get; set; }

		/// <summary>
		/// The sum (or null if no values or all values are "No Data")
		/// </summary>
		public double? Sum { get; set; }

		/// <summary>
		/// The sum of each "value squared" for non-null values (or null if no values or all values are "No Data")
		/// </summary>
		public double? SumSquared { get; set; }

		/// <summary>
		/// The count of values that are not "No Data"
		/// </summary>
		public int DataCount { get; set; }

		/// <summary>
		/// The count of values that are "No Data"
		/// </summary>
		public int NoDataCount { get; set; }
	}
}
