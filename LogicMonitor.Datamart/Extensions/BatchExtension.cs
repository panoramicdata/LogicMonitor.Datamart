using System.Collections.Generic;
using System.Linq;

namespace LogicMonitor.Datamart.Extensions
{
	internal static class BatchExtension
	{
		public static IEnumerable<IGrouping<long, T>> Batch<T>(this IEnumerable<IGrouping<long, T>> items, int maxItems)
			=> items.Select((item, itemIndex) => (item, itemIndex))
				.GroupBy(x => x.itemIndex / maxItems)
				.SelectMany(g => g.Select(x => x.item));
	}
}
