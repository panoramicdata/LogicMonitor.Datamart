using LogicMonitor.Datamart.Config;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace LogicMonitor.Datamart.Cli;
internal sealed class Application(
	IOptions<Configuration> configuration,
	ILoggerFactory loggerFactory)
	: IHostLifetime, IDisposable
{
	private readonly DatamartClient _datamartClient = new(configuration.Value, loggerFactory);
	private readonly CancellationTokenSource _cancellationTokenSource = new();
	private readonly ILogger _logger = loggerFactory.CreateLogger<Application>();

	private Task? _mainLoopTask;
	private bool _disposedValue;

	public Task WaitForStartAsync(CancellationToken cancellationToken)
	{
		_mainLoopTask = MainLoopAsync(_cancellationTokenSource.Token);
		return Task.CompletedTask;
	}

	private async Task MainLoopAsync(CancellationToken cancellationToken)
	{
		while (!cancellationToken.IsCancellationRequested)
		{
			try
			{
				var stopwatch = Stopwatch.StartNew();

				// Await a dimension sync
				await _datamartClient
					.SyncDimensionsAsync(
						LoopIntervals.ExecuteOnce,
						null,
						cancellationToken);

				// Sync the audit log
				await _datamartClient
					.SyncAuditLogAsync(
						LoopIntervals.ExecuteOnce,
						DateTimeOffset.UtcNow.AddDays(-60),
						cancellationToken);

				// Sync alerts
				await _datamartClient
					.SyncAlertsAsync(
						LoopIntervals.ExecuteOnce,
						DateTimeOffset.UtcNow.AddDays(-60),
						cancellationToken);

				// Sync time series data
				await _datamartClient
					.SyncLowResolutionDataAsync(
						LoopIntervals.ExecuteOnce,
						null,
						cancellationToken);

				// Wait for the remainder of 24 hours
				var elapsed = stopwatch.Elapsed;
				if (elapsed < TimeSpan.FromDays(1))
				{
					await Task.Delay(TimeSpan.FromDays(1) - elapsed, cancellationToken);
				}
			}
			catch (OperationCanceledException)
			{
				// Operation was cancelled
			}
			catch (Exception ex)
			{
				// Log the exception
				_logger.LogError(ex, "Exception in main loop: {Message}", ex.Message);
			}
		}
	}

	public async Task StopAsync(CancellationToken cancellationToken)
	{
		_cancellationTokenSource.Cancel();

		if (_mainLoopTask != null)
		{
			await _mainLoopTask
				.ConfigureAwait(false);
		}
	}

	private void Dispose(bool disposing)
	{
		if (!_disposedValue)
		{
			if (disposing)
			{
				_datamartClient?.Dispose();
				_cancellationTokenSource.Dispose();
			}

			_disposedValue = true;
		}
	}

	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
