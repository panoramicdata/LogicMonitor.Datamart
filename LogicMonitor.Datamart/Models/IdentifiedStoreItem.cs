namespace LogicMonitor.Datamart.Models
{
	public abstract class IdentifiedStoreItem : StoreItem
	{
		/// <summary>
		/// The LogicMonitor Id
		/// </summary>
		public int Id { get; set; }
	}
}