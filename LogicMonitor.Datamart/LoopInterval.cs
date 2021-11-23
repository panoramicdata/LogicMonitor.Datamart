using Humanizer;
using Humanizer.Localisation;

namespace LogicMonitor.Datamart;

internal abstract class LoopInterval
{
	private readonly string _name;
	public ILogger Logger { get; }

	protected LoopInterval(string name, ILoggerFactory loggerFactory)
	{
		_name = name;
		Logger = new PrefixLogger(name, loggerFactory);
	}

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

			Logger.LogInformation($"Starting {_name}...");

			try
			{
				await ExecuteAsync(cancellationToken).ConfigureAwait(false);
			}
			catch (Exception ex) when (ex is OperationCanceledException || ex is TaskCanceledException)
			{
				Logger.LogInformation(ex, $"Loopsync {_name} cancelled.");
			}
			catch (Exception ex)
			{
				// This shouldn't generally happen so if it does, dump the entire exception ex which will include inner exceptions
				Logger.LogError(ex, $"An unexpected error occurred during the LoopInterval: {ex}");
			}

			stopwatch.Stop();
			Logger.LogInformation($"Finished {_name} in {stopwatch.Elapsed.Humanize(7, minUnit: TimeUnit.Second)}.");

			// Are we repeating?
			if (intervalMinutes == LoopIntervals.ExecuteOnce)
			{
				// NO
				Logger.LogInformation($"{_name} configured to run once, finished.");
				break;
			}

			// YES - determine the interval
			var remainingTimeInInterval = syncInterval.Subtract(stopwatch.Elapsed);
			if (remainingTimeInInterval.TotalSeconds > 0)
			{
				Logger.LogInformation($"Next {_name} will start in {remainingTimeInInterval.Humanize(7, minUnit: TimeUnit.Second)} at {DateTime.UtcNow.Add(remainingTimeInInterval)}.");
				await Task.Delay(remainingTimeInInterval, cancellationToken).ConfigureAwait(false);
			}
			else
			{
				Logger.LogWarning($"Next {_name} will start immediately as it took longer than the configured {intervalMinutes} minutes.");
			}
		}
	}
}
