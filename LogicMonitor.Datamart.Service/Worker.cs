using LogicMonitor.Datamart.Logging;
using LogicMonitor.Datamart.Service.Config;
using Microsoft.Extensions.Options;

namespace LogicMonitor.Datamart.Service;

public class Worker : BackgroundService
{
	private readonly ILogger<Worker> _logger;
	private readonly ILoggerFactory _loggerFactory;
	private readonly AppConfiguration _config;
	private readonly List<DatamartClient> _datamartClients = new();
	private readonly List<Task> _dataAgingTasks = new();
	private readonly List<Task> _logSyncTasks = new();
	private readonly List<Task> _dimensionSyncTasks = new();
	private readonly List<Task> _alertSyncTasks = new();
	private readonly List<Task> _dataSyncTasks = new();

	public Worker(
		ILogger<Worker> logger,
		IOptions<AppConfiguration> options,
		ILoggerFactory loggerFactory)
	{
		_logger = logger;
		_loggerFactory = loggerFactory;
		_config = options.Value;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_logger.LogInformation($"Starting LogicMonitor Datamart v{ThisAssembly.AssemblyFileVersion}...");

		stoppingToken.Register(() => _logger.LogWarning("Application shutdown was requested. Cancelling any tasks..."));

		// Create tasks
		await CreateTasksAsync(stoppingToken).ConfigureAwait(false);

		// Wait for stopping
		while (!stoppingToken.IsCancellationRequested)
		{
			await Task.Delay(100, CancellationToken.None);
		}

		// Destroy tasks
		await WaitToShutDownAsync();
	}

	private async Task CreateTasksAsync(CancellationToken stoppingToken)
	{
		// Set up tasks
		foreach (var accountConfiguration in _config.AccountConfigurations)
		{
			try
			{
				// Create a portal client
				var datamartClient = new DatamartClient(
					accountConfiguration,
					new PrefixLoggerFactory(accountConfiguration.Name, _loggerFactory)
				);
				_datamartClients.Add(datamartClient);

				// Check whether database needs creating
				if (!await datamartClient.IsDatabaseCreatedAsync(stoppingToken).ConfigureAwait(false))
				{
					_logger.LogError("Database does not exist or there is a permissions issue.");

					_logger.LogInformation("Creating missing database...");
					await datamartClient
						.EnsureDatabaseCreatedAndSchemaUpdatedAsync(stoppingToken)
						.ConfigureAwait(false);
				}

				_logger.LogInformation("Updating database...");
				await datamartClient
					.EnsureDatabaseCreatedAndSchemaUpdatedAsync(stoppingToken)
					.ConfigureAwait(false);

				// TODO - VALIDATE CONFIGURATION INCLUDING EXITING IF NO SYNCS ARE SET

				var dataSourceSpecifications = accountConfiguration.DataSources.ToDictionary(v => v.Name, v => v.DataPoints);

				// TODO ENSURE THAT SYNCDATA IS ONLY USING DATASOURCEINSTANCES FROM THE DATABASE
				// TODO In SyncDimensions, make it possible to 'disable' a datasourceinstance as it is no longer retrieved from LogicMonitor

				_logSyncTasks.Add(accountConfiguration.SyncLogs
					? datamartClient.SyncAuditLogAsync(
						accountConfiguration.LogSyncDesiredMaxIntervalMinutes,
						accountConfiguration.StartDateTimeUtc,
						stoppingToken)
					: Task.CompletedTask);

				_dimensionSyncTasks.Add(accountConfiguration.SyncDimensions
					? datamartClient.SyncDimensionsAsync(
						accountConfiguration.DimensionSyncDesiredMaxIntervalMinutes,
						stoppingToken)
					: Task.CompletedTask);

				_alertSyncTasks.Add(accountConfiguration.SyncAlerts
					? datamartClient.SyncAlertsAsync(
						accountConfiguration.AlertSyncDesiredMaxIntervalMinutes,
						accountConfiguration.StartDateTimeUtc,
						stoppingToken)
					: Task.CompletedTask);

				if (accountConfiguration.SyncData)
				{
					_dataSyncTasks.Add(accountConfiguration.SyncData
						? datamartClient.SyncDataAsync(
							accountConfiguration.DataSyncDesiredMaxIntervalMinutes,
							stoppingToken)
						: Task.CompletedTask);

					_dataAgingTasks.Add(datamartClient.PerformDataAgingAsync(
						accountConfiguration.DataAgingDesiredMaxIntervalMinutes,
						accountConfiguration.CountAggregationDaysToRetain,
						stoppingToken));
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred starting account {accountConfigurationName}: {exMessage}", accountConfiguration.Name, ex.Message);
				throw;
			}
		}
	}

	public async Task WaitToShutDownAsync()
	{
		// Wait for all tasks to complete
		// They were probably cancelled and so will throw an exception, but we try and wait for them in case they're
		// involved in a long running operation that can't be cancelled, such as a bulk write.

		_logger.LogDebug("Setting up wait tasks.");

		// Use Task.WhenAll as it completes when all the tasks have completed and is async so unwraps the aggregate exception if they've been cancelled
		var logSyncTasksComplete = Task.WhenAll(_logSyncTasks.ToArray());
		var dimensionSyncTasksComplete = Task.WhenAll(_dimensionSyncTasks.ToArray());
		var alertSyncTasksComplete = Task.WhenAll(_alertSyncTasks.ToArray());
		var dataSyncTasksComplete = Task.WhenAll(_dataSyncTasks.ToArray());
		var dataAgingTasksComplete = Task.WhenAll(_dataAgingTasks.ToArray());

		try
		{
			_logger.LogInformation("Waiting for audit log sync to complete");
			await logSyncTasksComplete.ConfigureAwait(false);
		}
		catch (Exception ex) when (ex is OperationCanceledException || ex is TaskCanceledException)
		{
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error occurred while waiting for audit log sync to complete: {message}", ex.Message);
		}
		_logger.LogInformation("Finished waiting for audit log sync to complete");

		try
		{
			_logger.LogInformation("Waiting for dimension sync to complete");
			await dimensionSyncTasksComplete.ConfigureAwait(false);
		}
		catch (Exception ex) when (ex is OperationCanceledException || ex is TaskCanceledException)
		{
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error occurred while waiting for dimension sync to complete: {message}", ex.Message);
		}
		_logger.LogInformation("Finished waiting for dimension sync to complete");

		try
		{
			_logger.LogInformation("Waiting for alert sync to complete");
			await alertSyncTasksComplete.ConfigureAwait(false);
		}
		catch (Exception ex) when (ex is OperationCanceledException || ex is TaskCanceledException)
		{
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error occurred while waiting for alert sync to complete: {message}", ex.Message);
		}
		_logger.LogInformation("Finished waiting for alert sync to complete");

		try
		{
			_logger.LogInformation("Waiting for data sync to complete");
			await dataSyncTasksComplete.ConfigureAwait(false);
		}
		catch (Exception ex) when (ex is OperationCanceledException || ex is TaskCanceledException)
		{
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error occurred while waiting for data sync to complete: {message}", ex.Message);
		}
		_logger.LogInformation("Finished waiting for data sync to complete");

		try
		{
			_logger.LogInformation("Waiting for data aging to complete");
			await dataAgingTasksComplete.ConfigureAwait(false);
		}
		catch (Exception ex) when (ex is OperationCanceledException || ex is TaskCanceledException)
		{
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error occurred while waiting for data aging to complete: {message}", ex.Message);
		}
		_logger.LogInformation("Finished waiting for data aging to complete");

		_logger.LogInformation("Application has completed shut-down tasks...");
	}
}