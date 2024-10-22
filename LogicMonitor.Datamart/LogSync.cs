using LogicMonitor.Api.Extensions;

namespace LogicMonitor.Datamart;

internal class LogSync(
	DatamartClient datamartClient,
	DateTimeOffset startDateTimeUtc,
	ILoggerFactory loggerFactory) : LoopInterval(nameof(LogSync), loggerFactory)
{
	public override async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		Logger.LogInformation("Loading LogItems...");

		var nowSecondsSinceEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

		long timeCursor;
		const int pageSize = 300;

		var stopwatch = new Stopwatch();

		long mostRecentTimestamp;

		// Get the latest timestamp from the database
		using (var context = new Context(datamartClient.DbContextOptions))
		{
			mostRecentTimestamp = (await context
				.LogItems
				.OrderByDescending(l => l.HappenedOnTimeStampUtc)
				.FirstOrDefaultAsync(cancellationToken: cancellationToken)
				.ConfigureAwait(false))
				?.HappenedOnTimeStampUtc
				?? 0;
		}

		// Only go as far back as the configured StartDateTimeUtc
		timeCursor = Math.Max(mostRecentTimestamp, startDateTimeUtc.ToUnixTimeSeconds());
		var initialTimeCursor = timeCursor;
		var timeCursorLastTime = timeCursor;
		var totalLogEntriesStored = 0;
		while (timeCursor <= nowSecondsSinceEpoch)
		{
			using var context = new Context(datamartClient.DbContextOptions);
			var filter = new Filter<LogItem>
			{
				FilterItems =
							[
								new Ge<LogItem>(nameof(LogItem.HappenedOnTimeStampUtc), timeCursor),
							],
				Order = new Order<LogItem>
				{
					Property = nameof(LogItem.HappenedOnTimeStampUtc),
					Direction = OrderDirection.Asc
				},
				Take = pageSize
			};
			var apiEntriesThisTime = await datamartClient
				.GetAllAsync(filter, cancellationToken)
				.ConfigureAwait(false);

			if (apiEntriesThisTime.Count == 0)
			{
				// Nothing to do
				break;
			}

			// Check the ones on the time boundary to see whether we already have them
			var existingOnBoundary = await context
				.LogItems
				.Where(l => l.HappenedOnTimeStampUtc == timeCursor)
				.Select(l => l.LogicMonitorId)
				.ToListAsync(cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			// Remove those that came in on the boundary that we already have
			apiEntriesThisTime = apiEntriesThisTime
				.Where(e => !existingOnBoundary.Contains(e.Id))
				.ToList();

			if (apiEntriesThisTime.Count == 0)
			{
				// Nothing to do after filtering, we must be done?
				break;
			}

			totalLogEntriesStored += apiEntriesThisTime.Count;

			Logger.LogDebug(
				"Processing log items for {DatamartClientAccountName}",
				datamartClient.AccountName);
			var dataProcessingStopwatch = Stopwatch.StartNew();
			var sqlSave = new Stopwatch();

			// The raw record
			context.LogItems.AddRange(apiEntriesThisTime.Select(DatamartClient.MapperInstance.Map<LogItem, LogStoreItem>));

			context.AuditEvents.AddRange(apiEntriesThisTime
				.Select(x => x.ToAuditEvent())
				.Select(DatamartClient.MapperInstance.Map<AuditEvent, AuditEventStoreItem>));

			await context
				.SaveChangesAsync(cancellationToken)
				.ConfigureAwait(false);

			timeCursor = apiEntriesThisTime.Max(e => e.HappenedOnTimeStampUtc);

			Logger.LogDebug("Processed {ApiEntriesThisTime} log items ending {TimeCursor} for {DatamartClientAccountName}",
				apiEntriesThisTime,
				timeCursor,
				datamartClient.AccountName);
		}

		Logger.LogInformation(
			"Finished storing {TotalLogEntriesStored} Log entries for {LogicMonitorAccountName}",
			totalLogEntriesStored,
			datamartClient.AccountName);
	}
}
