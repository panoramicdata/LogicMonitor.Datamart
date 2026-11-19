using LogicMonitor.Api.Data;
using PanoramicData.NCalcExtensions;

namespace LogicMonitor.Datamart;

/// <summary>
/// Builds a <see cref="TimeSeriesDataAggregationStoreItem"/> from the graph data of one period.
/// </summary>
internal static class TimeSeriesAggregationBuilder
{
	private static readonly int[] _centileLevels = [5, 10, 25, 50, 75, 90, 95];

	private sealed record ValueStats(
		int DataCount,
		int NoDataCount,
		double Sum,
		double SumSquared,
		double? Max,
		double? Min,
		double? First,
		double? Last,
		double? FirstWithData,
		double? LastWithData,
		double?[] Centiles);

	internal static List<(long timestamp, double? value)> BuildPairedData(
		GraphData graphData,
		ResourceStoreItem device,
		ResourceDataSourceInstanceDataPointStoreItem instanceDataPoint,
		string dataPointName,
		DataSourceDataPointStoreItem dataPoint)
	{
		TrimToDeviceLifetime(graphData, device);

		var calculation = instanceDataPoint.DataSourceDataPoint!.Calculation;
		return string.IsNullOrWhiteSpace(calculation)
			? BuildDirectPairs(graphData, instanceDataPoint, dataPointName, dataPoint)
			: BuildCalculatedPairs(graphData, calculation);
	}

	internal static TimeSeriesDataAggregationStoreItem Build(
		TimeSeriesAggregationRequest request,
		List<(long timestamp, double? value)> pairedData,
		List<(long StartTimestampMs, long EndTimestampMs)>? sdtPeriods)
	{
		var dataPoint = request.DataPoint;
		var timeSeriesDataPoints = pairedData
			.Select(pair => new TimeSeriesDataPoint
			{
				IsInSdt = sdtPeriods?.Any(sdt => pair.timestamp >= sdt.StartTimestampMs && pair.timestamp <= sdt.EndTimestampMs) ?? false,
				Timestamp = pair.timestamp,
				Value = pair.value
			})
			.ToList();

		var allValues = timeSeriesDataPoints.Select(dp => dp.Value).ToList();

		var item = new TimeSeriesDataAggregationStoreItem
		{
			Id = Guid.NewGuid(),
			DeviceDataSourceInstanceDataPointId = request.InstanceDataPoint.Id,
			PeriodStart = request.StartDateTimeUtc.UtcDateTime,
			PeriodEnd = request.EndDateTimeUtc.UtcDateTime,
		};

		ApplyMainColumns(item, allValues, dataPoint);

		if (sdtPeriods != null)
		{
			ApplySdtCounts(item, timeSeriesDataPoints, dataPoint.GlobalAlertExpression);
			ApplyExcludingSdtColumns(item, timeSeriesDataPoints, dataPoint);
		}

		// IMPORTANT! This must be calculated last as this process can reverse the array.
		item.AvailabilityPercent = LowResolutionDataSync.CalculatePercentageAvailability(
			allValues,
			dataPoint.PercentageAvailabilityCalculation);

		return item;
	}

	private static void TrimToDeviceLifetime(GraphData graphData, ResourceStoreItem device)
	{
		var createdOnMs = device.CreatedOnSeconds * 1000;
		foreach (var graphDataLine in graphData.Lines)
		{
			graphDataLine.Data = [.. graphDataLine.Data.Where((_, index) => graphData.TimeStamps[index] >= createdOnMs)];
		}

		graphData.TimeStamps = [.. graphData.TimeStamps.Where(ts => ts >= createdOnMs)];
	}

	private static List<(long timestamp, double? value)> BuildDirectPairs(
		GraphData graphData,
		ResourceDataSourceInstanceDataPointStoreItem instanceDataPoint,
		string dataPointName,
		DataSourceDataPointStoreItem dataPoint)
	{
		var line = graphData.Lines.SingleOrDefault(dp => dp.Legend == dataPointName);

		if (line is null || dataPoint is null)
		{
			throw new FormatException($"Could not find DataPoint '{dataPointName}' for DataSource '{instanceDataPoint.DataSourceDataPoint!.DataSource.Name}'");
		}

		return [.. graphData.TimeStamps.Select((ts, index) => (ts, line.Data[index]))];
	}

	private static List<(long timestamp, double? value)> BuildCalculatedPairs(GraphData graphData, string calculation)
	{
		var expression = new ExtendedExpression(calculation);

		return [.. graphData.TimeStamps.Select((ts, index) =>
		{
			expression.Parameters.Clear();
			foreach (var line in graphData.Lines)
			{
				expression.Parameters.Add(line.Legend, line.Data[index]);
			}

			var calculatedValue = expression.Evaluate() as double?;

			// MS-22318 Prevent infinite and NaN values from being included
			if (calculatedValue.HasValue && (double.IsInfinity(calculatedValue.Value) || double.IsNaN(calculatedValue.Value)))
			{
				calculatedValue = null;
			}

			return (ts, calculatedValue);
		})];
	}

	private static ValueStats Summarise(List<double?> values)
	{
		var sortedNonNull = values
			.Where(v => v.HasValue)
			.Select(v => v!.Value)
			.OrderBy(v => v)
			.ToArray();

		return new ValueStats(
			values.Count(d => d.HasValue),
			values.Count(d => !d.HasValue),
			values.Sum(d => d ?? 0),
			values.Sum(d => d.HasValue ? d.Value * d.Value : 0),
			values.Where(d => d != null).DefaultIfEmpty(null).Max(),
			values.Where(d => d != null).DefaultIfEmpty(null).Min(),
			values.DefaultIfEmpty(null).First(),
			values.DefaultIfEmpty(null).Last(),
			values.Where(d => d != null).DefaultIfEmpty(null).First(),
			values.Where(d => d != null).DefaultIfEmpty(null).Last(),
			[.. _centileLevels.Select(level => LowResolutionDataSync.CalculatePercentile(sortedNonNull, level))]);
	}

	private static void ApplyMainColumns(
		TimeSeriesDataAggregationStoreItem item,
		List<double?> allValues,
		DataSourceDataPointStoreItem dataPoint)
	{
		var stats = Summarise(allValues);
		var expression = dataPoint.GlobalAlertExpression;

		item.DataCount = stats.DataCount;
		item.NoDataCount = stats.NoDataCount;
		item.Sum = stats.Sum;
		item.SumSquared = stats.SumSquared;
		item.Max = stats.Max;
		item.Min = stats.Min;
		item.First = stats.First;
		item.Last = stats.Last;
		item.FirstWithData = stats.FirstWithData;
		item.LastWithData = stats.LastWithData;
		item.Centile05 = stats.Centiles[0];
		item.Centile10 = stats.Centiles[1];
		item.Centile25 = stats.Centiles[2];
		item.Centile50 = stats.Centiles[3];
		item.Centile75 = stats.Centiles[4];
		item.Centile90 = stats.Centiles[5];
		item.Centile95 = stats.Centiles[6];
		item.NormalCount = AlertLevelCounter.Count(allValues, expression, CountAlertLevel.Normal);
		item.WarningCount = AlertLevelCounter.Count(allValues, expression, CountAlertLevel.Warning);
		item.ErrorCount = AlertLevelCounter.Count(allValues, expression, CountAlertLevel.Error);
		item.CriticalCount = AlertLevelCounter.Count(allValues, expression, CountAlertLevel.Critical);
		item.AvailabilityPercent2 = LowResolutionDataSync.CalculatePercentageAvailabilityNew(
			allValues,
			dataPoint.PercentageAvailabilityCalculation);
	}

	private static void ApplySdtCounts(
		TimeSeriesDataAggregationStoreItem item,
		List<TimeSeriesDataPoint> points,
		string expression)
	{
		item.NormalOrSdtCount = AlertLevelCounter.CountUseSdt(points, expression, CountAlertLevel.Normal);
		item.WarningSdtCount = AlertLevelCounter.CountUseSdt(points, expression, CountAlertLevel.Warning);
		item.ErrorSdtCount = AlertLevelCounter.CountUseSdt(points, expression, CountAlertLevel.Error);
		item.CriticalSdtCount = AlertLevelCounter.CountUseSdt(points, expression, CountAlertLevel.Critical);
	}

	private static void ApplyExcludingSdtColumns(
		TimeSeriesDataAggregationStoreItem item,
		List<TimeSeriesDataPoint> points,
		DataSourceDataPointStoreItem dataPoint)
	{
		var values = points.Where(dp => !dp.IsInSdt).Select(dp => dp.Value).ToList();
		var stats = Summarise(values);

		item.DataCountExcludingSdt = stats.DataCount;
		item.NoDataCountExcludingSdt = stats.NoDataCount;
		item.SumExcludingSdt = stats.Sum;
		item.SumSquaredExcludingSdt = stats.SumSquared;
		item.MaxExcludingSdt = stats.Max;
		item.MinExcludingSdt = stats.Min;
		item.FirstExcludingSdt = stats.First;
		item.LastExcludingSdt = stats.Last;
		item.FirstWithDataExcludingSdt = stats.FirstWithData;
		item.LastWithDataExcludingSdt = stats.LastWithData;
		item.Centile05ExcludingSdt = stats.Centiles[0];
		item.Centile10ExcludingSdt = stats.Centiles[1];
		item.Centile25ExcludingSdt = stats.Centiles[2];
		item.Centile50ExcludingSdt = stats.Centiles[3];
		item.Centile75ExcludingSdt = stats.Centiles[4];
		item.Centile90ExcludingSdt = stats.Centiles[5];
		item.Centile95ExcludingSdt = stats.Centiles[6];
		item.AvailabilityPercent2ExcludingSdt = LowResolutionDataSync.CalculatePercentageAvailabilityNew(
			values,
			dataPoint.PercentageAvailabilityCalculation);
		item.AvailabilityPercentExcludingSdt = LowResolutionDataSync.CalculatePercentageAvailability(
			values,
			dataPoint.PercentageAvailabilityCalculation);
	}
}
