using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace LogicMonitor.Datamart
{
	internal class DataAging : LoopInterval
	{
		private readonly DatamartClient _datamartClient;

		public DataAging(DatamartClient datamartClient, ILoggerFactory loggerFactory)
			: base(nameof(DataAging), loggerFactory)
			=> _datamartClient = datamartClient;

		public override async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			// Get the existing table list

			await _datamartClient.AgeAggregationTablesAsync();
		}
	}
}
