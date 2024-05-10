namespace LogicMonitor.Datamart;

internal class DataAging(
	DatamartClient datamartClient,
	int countAggregationDaysToRetain,
	ILoggerFactory loggerFactory) : LoopInterval(nameof(DataAging), loggerFactory)
{

	public override async Task ExecuteAsync(CancellationToken cancellationToken)
		=> await datamartClient.AgeAggregationTablesAsync(countAggregationDaysToRetain);
}
