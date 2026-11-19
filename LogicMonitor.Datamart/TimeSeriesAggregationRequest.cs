using LogicMonitor.Api.ScheduledDownTimes;

namespace LogicMonitor.Datamart;

/// <summary>
/// The inputs needed to aggregate one period of graph data for one DataPoint.
/// </summary>
internal sealed class TimeSeriesAggregationRequest
{
	public required DatamartClient DatamartClient { get; init; }

	public required ResourceStoreItem Device { get; init; }

	public required ResourceDataSourceInstanceDataPointStoreItem InstanceDataPoint { get; init; }

	public required string DataPointName { get; init; }

	public required DataSourceDataPointStoreItem DataPoint { get; init; }

	public required DateTimeOffset StartDateTimeUtc { get; init; }

	public required DateTimeOffset EndDateTimeUtc { get; init; }

	public required bool ExcludeSdtPeriods { get; init; }

	public required Dictionary<string, List<ScheduledDownTimeHistory>> SdtCache { get; init; }

	public required ILogger Logger { get; init; }
}
