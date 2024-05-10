using Humanizer;
using Humanizer.Localisation;

namespace LogicMonitor.Datamart;

internal abstract class LoopInterval(string name, ILoggerFactory loggerFactory)
{
	public ILogger Logger { get; } = loggerFactory.CreateLogger<LoopInterval>();

	public abstract Task ExecuteAsync(CancellationToken cancellationToken);

	/// <summary>
	/// Loops attempting to keep a minimum interval between the start of each execution.
	/// Exits when complete or cancelled.
	/// </summary>
	/// <param name="intervalMinutes">The minutes, or LoopIntervals.Immediate and LoopIntervals.ExecuteOnce</param>
	/// <param name="cancellationToken">CancellationToken</param>
	public async Task LoopAsync(int intervalMinutes, CancellationToken cancellationToken)
	{
		var syncInterval = TimeSpan.FromMinutes(intervalMinutes);

		// Create a Stopwatch to monitor how long the sync takes
		var stopwatch = Stopwatch.StartNew();

		while (!cancellationToken.IsCancellationRequested)
		{
			stopwatch.Restart();

			Logger.LogInformation("Starting {Name}...", name);

			try
			{
				await ExecuteAsync(cancellationToken).ConfigureAwait(false);
			}
			catch (Exception ex) when (ex is OperationCanceledException || ex is TaskCanceledException)
			{
				Logger.LogInformation(ex, "LoopAsync {Name} cancelled.", name);
			}
			catch (Exception ex)
			{
				// This shouldn't generally happen so if it does, dump the entire exception ex which will include inner exceptions
				Logger.LogError(
					ex,
					"An unexpected error occurred during the LoopInterval: '{Message}'.  Stack trace: {StackTrace}",
					ex.Message,
					ex.StackTrace);
			}

			stopwatch.Stop();
			Logger.LogInformation(
				"Finished {Name} in {StopwatchHumanized}.",
				name,
				stopwatch.Elapsed.Humanize(7, minUnit: TimeUnit.Second));

			// Are we repeating?
			if (intervalMinutes == LoopIntervals.ExecuteOnce)
			{
				// NO
				Logger.LogInformation(
					"{Name} configured to run once, finished.",
					name);
				break;
			}

			// YES - determine the interval
			var remainingTimeInInterval = syncInterval.Subtract(stopwatch.Elapsed);
			if (remainingTimeInInterval.TotalSeconds > 0)
			{
				Logger.LogInformation(
					"Next {Name} will start in {RemainingTimeInInterval} at {RemainingTimeInInterval}.",
					name,
					remainingTimeInInterval.Humanize(7, minUnit: TimeUnit.Second),
					DateTime.UtcNow.Add(remainingTimeInInterval)
					);
				await Task.Delay(remainingTimeInInterval, cancellationToken).ConfigureAwait(false);
			}
			else
			{
				Logger.LogWarning(
					"Next {Name} will start immediately as it took longer than the configured {IntervalMinutes} minutes.",
					name,
					intervalMinutes);
			}
		}
	}
}
