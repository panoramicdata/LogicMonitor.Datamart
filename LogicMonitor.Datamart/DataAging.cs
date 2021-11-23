namespace LogicMonitor.Datamart;

internal class DataAging : LoopInterval
{
	private readonly DatamartClient _datamartClient;
	private readonly int _countAggregationDaysToRetain;

	public DataAging(
		DatamartClient datamartClient,
		int countAggregationDaysToRetain,
		ILoggerFactory loggerFactory)
		: base(nameof(DataAging), loggerFactory)
	{
		_datamartClient = datamartClient;
		_countAggregationDaysToRetain = countAggregationDaysToRetain;
	}

	public override async Task ExecuteAsync(CancellationToken cancellationToken)
		=> await _datamartClient.AgeAggregationTablesAsync(_countAggregationDaysToRetain);
}
