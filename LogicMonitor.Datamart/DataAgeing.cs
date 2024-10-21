namespace LogicMonitor.Datamart;

internal class DataAgeing(
	DatamartClient datamartClient,
	int countAggregationDaysToRetain,
	ILoggerFactory loggerFactory) : LoopInterval(nameof(DataAgeing), loggerFactory)
{

	public override async Task ExecuteAsync(CancellationToken cancellationToken)
		=> await datamartClient.AgeAggregationTablesAsync(countAggregationDaysToRetain);
}
