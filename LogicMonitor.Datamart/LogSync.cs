namespace LogicMonitor.Datamart;

internal class LogSync : LoopInterval
{
	private readonly DatamartClient _datamartClient;
	private readonly DateTimeOffset _startDateTimeUtc;

	public LogSync(
		DatamartClient datamartClient,
		DateTimeOffset startDateTimeUtc,
		ILoggerFactory loggerFactory) : base(nameof(LogSync), loggerFactory)
	{
		_datamartClient = datamartClient;
		_startDateTimeUtc = startDateTimeUtc;
	}

	public override async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		Logger.LogInformation("Loading LogItems...");

		var nowSecondsSinceEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

		long timeCursor;
		const int pageSize = 300;

		var stopwatch = new Stopwatch();

		long mostRecentTimestamp;

		// Get the latest timestamp from the database
		using (var context = new Context(_datamartClient.DbContextOptions))
		{
			mostRecentTimestamp = (await context
				.LogItems
				.OrderByDescending(l => l.HappenedOnTimeStampUtc)
				.FirstOrDefaultAsync()
				.ConfigureAwait(false))
				?.HappenedOnTimeStampUtc
				?? 0;
		}

		// Only go as far back as the configured StartDateTimeUtc
		timeCursor = Math.Max(mostRecentTimestamp, _startDateTimeUtc.ToUnixTimeSeconds());
		var initialTimeCursor = timeCursor;
		var timeCursorLastTime = timeCursor;
		var totalLogEntriesStored = 0;
		while (timeCursor <= nowSecondsSinceEpoch)
		{
			using var context = new Context(_datamartClient.DbContextOptions);
			var filter = new Filter<LogItem>
			{
				FilterItems = new List<FilterItem<LogItem>>
							{
								new Ge<LogItem>(nameof(LogItem.HappenedOnTimeStampUtc), timeCursor),
							},
				Order = new Order<LogItem>
				{
					Property = nameof(LogItem.HappenedOnTimeStampUtc),
					Direction = OrderDirection.Asc
				},
				Take = pageSize
			};
			var apiEntriesThisTime = await _datamartClient.GetAllAsync(filter, cancellationToken).ConfigureAwait(false);

			if (apiEntriesThisTime.Count == 0)
			{
				// Nothing to do
				break;
			}

			// Check the ones on the time boundary to see whether we already have them
			var existingOnBoundary = await context
				.LogItems
				.Where(l => l.HappenedOnTimeStampUtc == timeCursor)
				.Select(l => l.Id)
				.ToListAsync()
				.ConfigureAwait(false);

			// Remove those that came in on the boundary that we already have
			apiEntriesThisTime = apiEntriesThisTime.Where(e => !existingOnBoundary.Contains(e.Id)).ToList();

			if (apiEntriesThisTime.Count == 0)
			{
				// Nothing to do after filtering, we must be done?
				break;
			}

			totalLogEntriesStored += apiEntriesThisTime.Count;

			Logger.LogDebug($"Processing log items for {_datamartClient.AccountName}");
			var dataProcessingStopwatch = Stopwatch.StartNew();
			var sqlSave = new Stopwatch();

			context.LogItems.AddRange(apiEntriesThisTime.Select(DatamartClient.MapperInstance.Map<LogItem, LogStoreItem>));

			await context
				.SaveChangesAsync()
				.ConfigureAwait(false);

			timeCursor = apiEntriesThisTime.Max(e => e.HappenedOnTimeStampUtc);

			Logger.LogDebug($"Processed {apiEntriesThisTime} log items ending {timeCursor} for {_datamartClient.AccountName}");
		}

		Logger.LogInformation($"Finished storing {totalLogEntriesStored} Log entries for {_datamartClient.AccountName}");
	}
}
