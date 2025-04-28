using LogicMonitor.Datamart.Interfaces;
using LogicMonitor.Datamart.Services;
using PanoramicData.NCalcExtensions;

namespace LogicMonitor.Datamart;

public class DatamartClient : LogicMonitorClient
{
	internal DbContextOptions<Context> DbContextOptions { get; }

	internal DatabaseType DatabaseType => _configuration.DatabaseType;

	internal bool LimitAlertSyncToDataSourceAppliesTo => _configuration.LimitAlertSyncToDataSourceAppliesTo ?? false;

	internal const string LogicMonitorCredentialNullMessage = "Either the configuration or some aspect of the LogicMonitorCredential is null";

	private const string ConnectionStringApplicationName = "LogicMonitor.Datamart";

	private readonly ILoggerFactory _loggerFactory;

	private readonly ILogger _logger;

	private readonly Configuration _configuration;

	private static readonly MapperConfiguration _mapperConfig = new(cfg => cfg.AddMaps(typeof(DatamartClient).Assembly));

	internal static IMapper MapperInstance = new Mapper(_mapperConfig);

	private readonly TimeProviderService _timeProviderService = new();

	public DatamartClient(
		Configuration configuration,
		ILoggerFactory loggerFactory
		) : base((configuration ?? throw new ArgumentNullException(nameof(configuration))).LogicMonitorClientOptions)
	{
		// Store and validate configuration
		_configuration = configuration;
		_configuration.Validate();

		_timeProviderService.SetDateTimeNow(configuration.FakeExecutionTime);

		// Set up the AutoMapper CustomPropertyFetcher
		CustomPropertyHandler.Configure(_configuration.DeviceProperties);

		// Check AutoMapper
		_mapperConfig.AssertConfigurationIsValid();

		var dbContextOptionsBuilder = new DbContextOptionsBuilder<Context>();
		switch (configuration.DatabaseType)
		{
			case DatabaseType.SqlServer:
				dbContextOptionsBuilder
					.UseSqlServer(new DbConnectionStringBuilder
					{
						ConnectionString = new SqlConnectionStringBuilder
						{
							TrustServerCertificate = true,
							Encrypt = true,
							ConnectTimeout = 30,
							DataSource = $"{configuration.DatabaseServerName},{configuration.DatabaseServerPort ?? 1433}",
							InitialCatalog = configuration.DatabaseName,
							UserID = configuration.DatabaseUsername,
							Password = configuration.DatabasePassword,
							ApplicationName = ConnectionStringApplicationName,
							Authentication = configuration.SqlServerAuthenticationMethod ?? SqlAuthenticationMethod.NotSpecified
						}.ToString()
					}.ConnectionString,
					options => options
						.CommandTimeout(configuration.SqlCommandTimeoutSeconds)
						.EnableRetryOnFailure(configuration.DatabaseRetryOnFailureCount ?? 5)
					);
				break;
			case DatabaseType.Postgres:
				dbContextOptionsBuilder
					.UseNpgsql(
						$"Server={configuration.DatabaseServerName};" +
						$"Port={configuration.DatabaseServerPort ?? 5432};" +
						$"Database={configuration.DatabaseName};" +
						$"Uid={configuration.DatabaseUsername};" +
						$"Pwd={configuration.DatabasePassword};" +
						$"Timezone=UTC",
						options => options
							.EnableRetryOnFailure(configuration.DatabaseRetryOnFailureCount ?? 5)
					);
				break;
			case DatabaseType.InMemory:
				dbContextOptionsBuilder
					.UseInMemoryDatabase(databaseName: configuration.DatabaseName);
				break;
			case DatabaseType.None:
				break;
			default:
				throw new NotSupportedException($"Database type {configuration.DatabaseType} not supported");
		}

		if (configuration.EnableSensitiveDatabaseLogging)
		{
			dbContextOptionsBuilder.EnableSensitiveDataLogging();
		}

		DbContextOptions = dbContextOptionsBuilder.Options;

		_loggerFactory = loggerFactory;
		_logger = loggerFactory.CreateLogger<DatamartClient>();
	}

	public async Task<bool> IsDatabaseCreatedAsync(CancellationToken cancellationToken)
	{
		using var context = GetContext();
		return await context
			.Database
			.GetService<IRelationalDatabaseCreator>()
			.ExistsAsync(cancellationToken)
			.ConfigureAwait(false);
	}

	internal Context GetContext() => DatabaseType switch
	{
		DatabaseType.SqlServer => new SqlServerContext(DbContextOptions),
		DatabaseType.Postgres => new NpgsqlContext(DbContextOptions),
		DatabaseType.InMemory => new InMemoryContext(DbContextOptions),
		_ => throw new NotSupportedException($"DatabaseType {DatabaseType} is not supported"),
	};

	public async Task<bool> IsDatabaseSchemaUpToDateAsync(CancellationToken cancellationToken)
	{
		using var context = GetContext();
		var exists = await context
			.Database
			.GetService<IRelationalDatabaseCreator>()
			.ExistsAsync(cancellationToken)
			.ConfigureAwait(false);
		if (!exists)
		{
			return false;
		}

		var pendingMigrations = await context
			.Database
			.GetPendingMigrationsAsync(cancellationToken)
			.ConfigureAwait(false);

		return !pendingMigrations.Any();
	}

	public async Task EnsureDatabaseCreatedAndSchemaUpdatedAsync(CancellationToken cancellationToken)
	{
		using var migrationsContext = GetContext();

		_logger.LogInformation("Applying migrations to database...");
		await migrationsContext
			.Database
			.MigrateAsync(cancellationToken)
			.ConfigureAwait(false);

		_logger.LogInformation("Migrations up to date.");
	}

	public async Task EnsureDatabaseDeletedAsync(CancellationToken cancellationToken)
	{
		using var context = GetContext();
		using var dbConnection = context.Database.GetDbConnection();
		_logger.LogInformation(
			"Deleting database {DbConnectionDatabase} on {DbConnectionDataSource}...",
			dbConnection.Database,
			dbConnection.DataSource);
		await context
			.Database
			.EnsureDeletedAsync(cancellationToken)
			.ConfigureAwait(false);
		_logger.LogInformation(
			"Deleted database {DbConnectionDatabase} on {DbConnectionDataSource}...",
			dbConnection.Database,
			dbConnection.DataSource);
	}

	private DbSet<TStore> GetDbSet<TStore>(Context context) where TStore : class, new()
	{
		var result = typeof(TStore).Name switch
		{
			nameof(AlertStoreItem) => context.Alerts as DbSet<TStore>,
			nameof(AlertRuleStoreItem) => context.AlertRules as DbSet<TStore>,
			nameof(CollectorStoreItem) => context.Collectors as DbSet<TStore>,
			nameof(CollectorGroupStoreItem) => context.CollectorGroups as DbSet<TStore>,
			nameof(ResourceDataSourceStoreItem) => context.DeviceDataSources as DbSet<TStore>,
			nameof(ResourceStoreItem) => context.Devices as DbSet<TStore>,
			nameof(ResourceGroupStoreItem) => context.DeviceGroups as DbSet<TStore>,
			nameof(ConfigSourceStoreItem) => context.ConfigSources as DbSet<TStore>,
			nameof(DataSourceStoreItem) => context.DataSources as DbSet<TStore>,
			nameof(EscalationChainStoreItem) => context.EscalationChains as DbSet<TStore>,
			nameof(EventSourceStoreItem) => context.EventSources as DbSet<TStore>,
			nameof(WebsiteGroupStoreItem) => context.WebsiteGroups as DbSet<TStore>,
			nameof(WebsiteStoreItem) => context.Websites as DbSet<TStore>,
			_ => throw new NotSupportedException(),
		};

		return result ?? throw new NotSupportedException($"Type {typeof(TStore).Name} is not supported");
	}

	public async Task<List<T>> SqlListQuery<T>(string sql) where T : class, IHasEndpoint, new()
	{
		using var context = GetContext();
		return !context.Database.IsSqlServer()
			? throw new NotSupportedException("Only SQL Server types support SQL queries.")
			: await Task.FromResult(GetDbSet<T>(context)
				.FromSqlRaw(sql)
				.ToList()).ConfigureAwait(false);
	}

	public async Task<TApi> GetCachedAsync<TApi>(int id, CancellationToken cancellationToken)
		where TApi : IdentifiedItem
	{
		using var context = GetContext();
		var typeName = typeof(TApi).Name;
		switch (typeName)
		{
			case nameof(Resource):
				var deviceStoreItem = await context
					.Devices
					.SingleOrDefaultAsync(i => i.LogicMonitorId == id, cancellationToken)
					.ConfigureAwait(false);
				var result = deviceStoreItem == null
					? throw new KeyNotFoundException($"Device with id {id} not found")
					: MapperInstance.Map<ResourceStoreItem, Resource>(deviceStoreItem) as TApi;

				return result ?? throw new InvalidOperationException($"Could not convert {nameof(ResourceStoreItem)} to {nameof(Resource)}");
			default:
				throw new NotSupportedException($"Getting cached {typeName} is not supported");
		}
	}

	public async Task<List<TApi>> GetAllCachedAsync<TApi>(CancellationToken cancellationToken)
		where TApi : class, IHasEndpoint, new()
	{
		using var context = GetContext();
		var className = typeof(TApi).Name;
		switch (className)
		{
			case nameof(Alert):
				var alertStoreItems = await context
					.Alerts
					.ToListAsync(cancellationToken)
					.ConfigureAwait(false);
				return alertStoreItems
					.ConvertAll(a => MapperInstance.Map<AlertStoreItem, Alert>(a) as TApi ?? throw new InvalidOperationException($"Could not convert {nameof(AlertStoreItem)} to {nameof(Alert)}"));
			case nameof(CollectorGroup):
				var collectorGroupStoreItems = await context
					.CollectorGroups
					.ToListAsync(cancellationToken)
					.ConfigureAwait(false);
				return collectorGroupStoreItems
					.ConvertAll(cg => MapperInstance.Map<CollectorGroupStoreItem, CollectorGroup>(cg) as TApi ?? throw new InvalidOperationException($"Could not convert {nameof(CollectorGroupStoreItem)} to {nameof(CollectorGroup)}"));
			case nameof(ResourceGroup):
				var deviceGroupStoreItems = await context
					.DeviceGroups
					.ToListAsync(cancellationToken)
					.ConfigureAwait(false);
				return deviceGroupStoreItems
					.ConvertAll(dg => MapperInstance.Map<ResourceGroupStoreItem, ResourceGroup>(dg) as TApi ?? throw new InvalidOperationException($"Could not convert {nameof(ResourceGroupStoreItem)} to {nameof(ResourceGroup)}"));
			case nameof(WebsiteGroup):
				var websiteGroupStoreItems = await context
					.WebsiteGroups
					.ToListAsync(cancellationToken)
					.ConfigureAwait(false);
				return websiteGroupStoreItems
					.ConvertAll(wg => MapperInstance.Map<WebsiteGroupStoreItem, WebsiteGroup>(wg) as TApi ?? throw new InvalidOperationException($"Could not convert {nameof(WebsiteGroupStoreItem)} to {nameof(WebsiteGroup)}"));
			default:
				throw new NotSupportedException($"{className} not supported.  Add it to GetAllCachedAsync<T>().");
		}
	}

	public Task SyncDimensionsAsync(
		int desiredMaxIntervalMinutes,
		INotificationReceiver? notificationReceiver,
		CancellationToken cancellationToken)
	{
		var sync = new DimensionSync(
			this,
			_configuration,
			_loggerFactory,
			notificationReceiver);
		return sync.LoopAsync(desiredMaxIntervalMinutes, cancellationToken);
	}

	public Task SyncDimensionsAsync(
		int desiredMaxIntervalMinutes,
		List<string> types,
		INotificationReceiver? notificationReceiver,
		CancellationToken cancellationToken)
	{
		var sync = new DimensionSync(
			this,
			_configuration,
			types,
			_loggerFactory,
			notificationReceiver);
		return sync.LoopAsync(desiredMaxIntervalMinutes, cancellationToken);
	}

	public Task SyncLowResolutionDataAsync(
		int desiredMaxIntervalMinutes,
		INotificationReceiver? notificationReceiver,
		CancellationToken cancellationToken)
	{
		var sync = new LowResolutionDataSync(
			this,
			_configuration,
			_loggerFactory,
			notificationReceiver,
			_timeProviderService);
		return sync.LoopAsync(desiredMaxIntervalMinutes, cancellationToken);
	}

	public Task SyncAlertsAsync(
		int desiredMaxIntervalMinutes,
		DateTimeOffset startDateTimeUtc,
		CancellationToken cancellationToken)
	{
		var sync = new AlertSync(this, startDateTimeUtc, _loggerFactory);
		return sync.LoopAsync(desiredMaxIntervalMinutes, cancellationToken);
	}

	public Task SyncAuditLogAsync(
		int desiredMaxIntervalMinutes,
		DateTimeOffset startDateTimeUtc,
		CancellationToken cancellationToken)
	{
		var sync = new LogSync(
			this,
			startDateTimeUtc,
			_loggerFactory);
		return sync.LoopAsync(desiredMaxIntervalMinutes, cancellationToken);
	}

	public Task PerformDataAgeingAsync(
	int desiredMaxIntervalMinutes,
	int CountAggregationDaysToRetain,
	CancellationToken cancellationToken)
	{
		var sync = new DataAgeing(
			this,
			CountAggregationDaysToRetain,
			_loggerFactory);
		return sync.LoopAsync(desiredMaxIntervalMinutes, cancellationToken);
	}

	/// <summary>
	/// Add or Update the Database using the items already retrieved from the LogicMonitor API
	/// </summary>
	/// <typeparam name="TApi">The LogicMonitor API type</typeparam>
	/// <typeparam name="TStore">The Database StoreItem type</typeparam>
	/// <param name="action"></param>
	/// <param name="cancellationToken"></param>
	internal async Task AddOrUpdate<TApi, TStore>(
		Func<Context, DbSet<TStore>> action,
		bool haltOnError,
		ILogger logger,
		INotificationReceiver notificationReceiver,
		CancellationToken cancellationToken)
		where TApi : IdentifiedItem, IHasEndpoint, new()
		where TStore : IdentifiedStoreItem
	{
		try
		{
			await notificationReceiver
				.SetStageNameAsync($"Dimension {typeof(TApi).Name}", cancellationToken)
				.ConfigureAwait(false);
			logger.LogInformation("Syncing {Type}s...", typeof(TApi).Name);

			using var context = GetContext();
			// Get the right DbSet from the context
			var dbSet = action(context);

			// Fetch the items from the LogicMonitor API
			var lastObservedUtc = _timeProviderService.UtcOffsetNow;

			var apiItems = await GetAllAsync<TApi>(cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			await notificationReceiver
				.SetItemCountAsync(apiItems.Count, cancellationToken)
				.ConfigureAwait(false);

			// Add/update all the items
			var itemIndex = 0;
			var stopwatch = Stopwatch.StartNew();
			foreach (var item in apiItems)
			{
				itemIndex++;

				if (stopwatch.ElapsedMilliseconds > 10_000)
				{
					logger.LogInformation(
						"Syncing {Type}s: {ItemIndex}/{ItemCount}",
						typeof(TApi).Name,
						itemIndex,
						apiItems.Count);
					await notificationReceiver
						.SetItemIndexAsync(itemIndex, cancellationToken)
						.ConfigureAwait(false);
					stopwatch.Restart();
				}

				await dbSet.AddOrUpdateIdentifiedItemAsync(
					context,
					item,
					lastObservedUtc,
					logger,
					cancellationToken);
			}

			await notificationReceiver
				.SetItemIndexAsync(itemIndex, cancellationToken)
				.ConfigureAwait(false);

			// Calculate and log the stats
			var added = context.ChangeTracker.Entries().Count(e => e.State == EntityState.Added);
			var modified = context.ChangeTracker.Entries().Count(e => e.State == EntityState.Modified);
			var total = context.ChangeTracker.Entries().Count();
			var affectedRowCount = await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
			logger.LogInformation(
				"{TypeName}: Total {Total}; Added {Added}; Modified {Modified}.",
				typeof(TApi).Name,
				total,
				added,
				modified);

			// For DataPoints, the information from LogicMonitor is present on the DataSources.
			// So, after fetching the DataSources, we should also update the DataPoints in the database
			switch (typeof(TStore).Name)
			{
				case nameof(DataSourceStoreItem):
					await UpdateGraphsAsync(context, [.. apiItems.Cast<DataSource>()], cancellationToken)
						.ConfigureAwait(false);

					await UpdateDataPointsAsync(context, [.. apiItems.Cast<DataSource>()], cancellationToken)
						.ConfigureAwait(false);
					break;
			}

			logger.LogInformation(
				"Syncing {Type}s complete.",
				typeof(TApi).Name);
		}
		catch (Exception e)
		{
			logger.LogError(
				e,
				"Could not sync {Type}s due to {Message}",
				typeof(TApi).Name,
				e.Message
			);
			if (haltOnError)
			{
				throw;
			}
		}
	}

	/// <summary>
	/// Add or Update the Database using the items already retrieved from the LogicMonitor API
	/// </summary>
	/// <typeparam name="TApi">The LogicMonitor API type</typeparam>
	/// <typeparam name="TStore">The Database StoreItem type</typeparam>
	/// <param name="action"></param>
	/// <param name="cancellationToken"></param>
	internal async Task AddOrUpdateLogicModuleUpdates(
		Func<Context, DbSet<LogicModuleUpdateStoreItem>> action,
		bool haltOnError,
		ILogger logger,
		INotificationReceiver notificationReceiver,
		CancellationToken cancellationToken)
	{
		try
		{
			logger.LogInformation("Syncing {Type}s...", nameof(LogicModuleUpdate));
			await notificationReceiver
				.SetStageNameAsync($"Syncing {nameof(LogicModuleUpdate)}s...", cancellationToken)
				.ConfigureAwait(false);

			using var context = GetContext();
			// Get the right DbSet from the context
			logger.LogDebug("{TypeName}: Loading entries...", nameof(LogicModuleUpdateStoreItem));
			var dbSet = action(context);

			// Fetch the items from the LogicMonitor API
			var lastObservedUtc = _timeProviderService.UtcOffsetNow;
			var apiItems = await GetLogicModuleUpdatesAsync(LogicModuleType.All, cancellationToken: cancellationToken)
				.ConfigureAwait(false);
			logger.LogDebug(
				"{TypeName}: Loaded {ApiItemsCount} items.",
				nameof(LogicModuleUpdateStoreItem),
				apiItems.Items.Count);

			await notificationReceiver
				.SetItemCountAsync(apiItems.Items.Count, cancellationToken)
				.ConfigureAwait(false);

			var itemIndex = 0;
			var stopwatch = Stopwatch.StartNew();
			// Add/update all the items
			foreach (var item in apiItems.Items)
			{
				itemIndex++;

				if (stopwatch.ElapsedMilliseconds > 10_000)
				{
					logger.LogInformation(
						"Syncing {Type}s: {ItemIndex}/{ItemCount}",
						nameof(LogicModuleUpdate),
						itemIndex,
						apiItems.Items.Count);
					await notificationReceiver
						.SetItemIndexAsync(itemIndex, cancellationToken)
						.ConfigureAwait(false);
					stopwatch.Restart();
				}

				dbSet.AddOrUpdateLogicModuleUpdate(
					item,
					lastObservedUtc,
					logger);
			}

			// Send a final notification
			await notificationReceiver
				.SetItemIndexAsync(itemIndex, cancellationToken)
				.ConfigureAwait(false);

			// Calculate and log the stats
			var added = context.ChangeTracker.Entries().Count(e => e.State == EntityState.Added);
			var modified = context.ChangeTracker.Entries().Count(e => e.State == EntityState.Modified);
			var total = context.ChangeTracker.Entries().Count();
			var affectedRowCount = await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
			logger.LogInformation(
				"{TypeName}: Total {Total}; Added {Added}; Modified {Modified}.",
				nameof(LogicModuleUpdateStoreItem),
				total,
				added,
				modified);

			logger.LogInformation($"Syncing {nameof(LogicModuleUpdate)}s complete.");
		}
		catch (Exception e)
		{
			logger.LogError(
				e,
				$"Could not sync {nameof(LogicModuleUpdate)}s due to {{Message}}", e.Message
			);
			if (haltOnError)
			{
				throw;
			}
		}
	}
	/// <summary>
	/// Update Graphs, given a list of DataSources just retrieved from the LogicMonitor API
	/// </summary>
	/// <param name="context">The database context</param>
	/// <param name="apiDataSources">The list of API DataSources</param>
	private async Task UpdateGraphsAsync(Context context, List<DataSource> apiDataSources, CancellationToken cancellationToken)
	{
		_logger.LogInformation("Updating DataSource Graphs...");
		var graphsStopwatch = Stopwatch.StartNew();
		var dataSourceCount = apiDataSources.Count;
		var dataSourceIndex = 0;
		foreach (var apiDataSource in apiDataSources)
		{
			++dataSourceIndex;
			if (graphsStopwatch.Elapsed > TimeSpan.FromSeconds(10))
			{
				_logger.LogInformation(
					"UpdateGraphsAsync: DataSource {DataSourceIndex}/{DataSourceCount}",
					dataSourceIndex,
					dataSourceCount);
				graphsStopwatch.Restart();
			}

			var apiGraphs = await GetDataSourceGraphsAsync(apiDataSource.Id, cancellationToken).ConfigureAwait(false);
			var apiOverviewGraphs = (await GetDataSourceOverviewGraphsPageAsync(apiDataSource.Id, null, cancellationToken).ConfigureAwait(false)).Items;

			var databaseDataSource = await context
				.DataSources
				.SingleOrDefaultAsync(ds => ds.Name == apiDataSource.Name, cancellationToken)
				.ConfigureAwait(false);

			if (databaseDataSource is null)
			{
				_logger.LogError(
					"For LogicMonitor instance {LogicMonitorAccount}, expected to find Database DataSource called '{DataSourceName}', but it was missing.",
					_configuration.LogicMonitorClientOptions.Account,
					apiDataSource.Name);
				continue;
			}

			var databaseGraphs = await context
				.DataSourceGraphs
				.Where(g => g.DataSource!.LogicMonitorId == apiDataSource.Id)
				.ToListAsync(cancellationToken)
				.ConfigureAwait(false);

			UpdateGraphs(context, apiGraphs, databaseDataSource.Id, databaseGraphs, false);
			UpdateGraphs(context, apiOverviewGraphs, databaseDataSource.Id, databaseGraphs, true);

			await context
				.SaveChangesAsync(cancellationToken)
				.ConfigureAwait(false);
		}

		_logger.LogInformation("Updating DataSource Graphs done.");
	}

	private static void UpdateGraphs(
		Context context,
		List<DataSourceGraph> apiGraphs,
		Guid databaseDataSourceId,
		List<DataSourceGraphStoreItem> databaseGraphs,
		bool areOverview)
	{
		// Graphs to add = API - Database
		var graphsToAdd = apiGraphs
			.Where(g => !databaseGraphs.Any(dg => dg.Name == g.Name && dg.IsOverview == areOverview))
			.Select(g =>
			{
				var databaseDataSourceGraph = MapperInstance.Map<DataSourceGraphStoreItem>(g);
				databaseDataSourceGraph.DataSourceId = databaseDataSourceId;
				databaseDataSourceGraph.IsOverview = areOverview;
				return databaseDataSourceGraph;
			}
			)
			.ToList();

		var graphsToRemove = databaseGraphs
			.Where(dg => !apiGraphs.Any(g => g.Name == dg.Name && dg.IsOverview == areOverview))
			.ToList();

		var graphsToUpdate = databaseGraphs
			.Where(dg => apiGraphs.Any(g => g.Name == dg.Name && dg.IsOverview == areOverview))
			.ToList();

		// Add, remove and update
		context.DataSourceGraphs.AddRange(graphsToAdd);
		context.DataSourceGraphs.RemoveRange(graphsToRemove);
		foreach (var graphToUpdate in graphsToUpdate)
		{
			var databaseGraph = databaseGraphs.SingleOrDefault(dg => dg.Name == graphToUpdate.Name && dg.IsOverview == areOverview);
			if (databaseGraph is null)
			{
				continue;
			}

			MapperInstance.Map(graphToUpdate, databaseGraph);
			databaseGraph.IsOverview = areOverview;
		}
	}

	/// <summary>
	/// Update DataPoints, given a list of DataSources just retrieved from the LogicMonitor API
	/// </summary>
	/// <param name="context">The database context</param>
	/// <param name="apiDataSources">The list of API DataSources</param>
	private async Task UpdateDataPointsAsync(Context context, List<DataSource> apiDataSources, CancellationToken cancellationToken)
	{
		_logger.LogInformation("Updating DataSource DataPoints...");
		var dataPointsStopwatch = Stopwatch.StartNew();
		var dataSourceCount = apiDataSources.Count;
		var dataSourceIndex = 0;
		// Update the nominated DataSources' DataPoints only for those reference in the config
		foreach (var configDataSourceSpecification in _configuration.DataSources)
		{
			++dataSourceIndex;
			if (dataPointsStopwatch.Elapsed > TimeSpan.FromSeconds(10))
			{
				_logger.LogInformation(
					"UpdateDataPointsAsync: DataSource {DataSourceIndex}/{DataSourceCount}",
					dataSourceIndex,
					dataSourceCount);
				dataPointsStopwatch.Restart();
			}

			// The DataSource name from the config
			var dataSourceName = configDataSourceSpecification.Name;

			// The DataSource from the API
			var apiDataSource = apiDataSources
				.SingleOrDefault(ds => ds.Name == dataSourceName);
			if (apiDataSource is null)
			{
				// May not happen if the config references a non-existent DataSource
				_logger.LogWarning(
					"For LogicMonitor instance {LogicMonitorAccount}, expected to find LogicMonitor API DataSource called '{DataSourceName}', but it was missing.",
					_configuration.LogicMonitorClientOptions.Account,
					dataSourceName);
				continue;
			}

			// The DataSource from the database
			var databaseDataSource = await context
				.DataSources
				.SingleOrDefaultAsync(ds => ds.Name == dataSourceName, cancellationToken)
				.ConfigureAwait(false);
			if (databaseDataSource is null)
			{
				// Should not happen, as we have only just updated the database with DataSources
				_logger.LogError(
					"For LogicMonitor instance {LogicMonitorAccount}, expected to find Database DataSource called '{DataSourceName}', but it was missing.",
					_configuration.LogicMonitorClientOptions.Account,
					dataSourceName);
				continue;
			}
			// We have a matching DataSource from both the API and the database.

			// Consider each DataPoint in the config
			foreach (var configDataPoint in configDataSourceSpecification.DataPoints)
			{
				// Is it present in the API DataSource?
				var apiDataPoint = apiDataSource
					.DataSourceDataPoints
					.SingleOrDefault(dp => dp.Name == configDataPoint.Name);
				if (apiDataPoint == null && string.IsNullOrWhiteSpace(configDataPoint.Calculation))
				{
					_logger.LogWarning(
						"For LogicMonitor instance '{LogicMonitorAccount}', DataSource '{DataSourceName}': could not find configured DataPoint '{ConfigDataPointName}' when not using calculations. Either specify a calculation, or use one of the following available DataPoints: {DataPoints}",
						_configuration.LogicMonitorClientOptions.Account,
						dataSourceName,
						configDataPoint.Name,
						string.Join(", ", apiDataSource.DataSourceDataPoints.Select(dp => dp.Name).OrderBy(dp => dp))
						);
					continue;
				}

				var globalAlertExpression = string.IsNullOrWhiteSpace(configDataPoint.GlobalAlertExpression)
					? apiDataPoint?.AlertExpression ?? string.Empty
					: configDataPoint.GlobalAlertExpression;

				var description = string.IsNullOrWhiteSpace(configDataPoint.Description)
					? apiDataPoint?.Description ?? string.Empty
					: configDataPoint.Description;

				// Is it in the database?
				var databaseDataSourceDataPointModel = await context
					.DataSourceDataPoints
					.SingleOrDefaultAsync(dsdp => dsdp.DataSource.Name == apiDataSource.Name && dsdp.Name == configDataPoint.Name, cancellationToken)
					.ConfigureAwait(false);

				if (databaseDataSourceDataPointModel is null)
				{
					// No. Add it to the database
					_logger.LogInformation(
						"For LogicMonitor instance {LogicMonitorAccount}, for {DataSourceName}, added datapoint {ConfigDataPointName} to database.",
						_configuration.LogicMonitorClientOptions.Account,
						dataSourceName,
						configDataPoint.Name);

					var dataSourceDataPointStoreItem = context.DataSourceDataPoints.Add(new DataSourceDataPointStoreItem
					{
						DataSource = databaseDataSource,
						Name = configDataPoint.Name,
						LogicMonitorId = apiDataPoint?.Id ?? 0,

						// API/Config
						Description = description,
						GlobalAlertExpression = globalAlertExpression,

						// Config only
						MeasurementUnit = configDataPoint.MeasurementUnit,
						Calculation = configDataPoint.Calculation,
						PercentageAvailabilityCalculation = configDataPoint.PercentageAvailabilityCalculation,
						Tags = configDataPoint.Tags,
						Property1 = configDataPoint.Property1,
						Property2 = configDataPoint.Property2,
						Property3 = configDataPoint.Property3,
						Property4 = configDataPoint.Property4,
						Property5 = configDataPoint.Property5,
						Property6 = configDataPoint.Property6,
						Property7 = configDataPoint.Property7,
						Property8 = configDataPoint.Property8,
						Property9 = configDataPoint.Property9,
						Property10 = configDataPoint.Property10,
						ResyncTimeSeriesData = configDataPoint.ResyncTimeSeriesData,
					});
				}
				else
				{
					// Update everything, even if it hasn't changed

					// API/Config
					databaseDataSourceDataPointModel.Description = description;
					databaseDataSourceDataPointModel.GlobalAlertExpression = globalAlertExpression;

					// Config only
					databaseDataSourceDataPointModel.MeasurementUnit = configDataPoint.MeasurementUnit;
					databaseDataSourceDataPointModel.Calculation = configDataPoint.Calculation;
					databaseDataSourceDataPointModel.PercentageAvailabilityCalculation = configDataPoint.PercentageAvailabilityCalculation;
					databaseDataSourceDataPointModel.Tags = configDataPoint.Tags;
					databaseDataSourceDataPointModel.Property1 = configDataPoint.Property1;
					databaseDataSourceDataPointModel.Property2 = configDataPoint.Property2;
					databaseDataSourceDataPointModel.Property3 = configDataPoint.Property3;
					databaseDataSourceDataPointModel.Property4 = configDataPoint.Property4;
					databaseDataSourceDataPointModel.Property5 = configDataPoint.Property5;
					databaseDataSourceDataPointModel.Property6 = configDataPoint.Property6;
					databaseDataSourceDataPointModel.Property7 = configDataPoint.Property7;
					databaseDataSourceDataPointModel.Property8 = configDataPoint.Property8;
					databaseDataSourceDataPointModel.Property9 = configDataPoint.Property9;
					databaseDataSourceDataPointModel.Property10 = configDataPoint.Property10;
					databaseDataSourceDataPointModel.ResyncTimeSeriesData = configDataPoint.ResyncTimeSeriesData;
				}

				await context
					.SaveChangesAsync(cancellationToken)
					.ConfigureAwait(false);
			}

			// TODO - remove old DataPoints and associated data.
			// This will cascade delete, so may take a long time.
			// Consider adjusting the command timeout.
		}

		_logger.LogInformation("Updating DataSource DataPoints done.");
	}

	public async Task<List<Alert>> GetCachedAlertsAsync(
		long startSeconds,
		long endSeconds,
		bool includeInactive,
		int? skip,
		int? take,
		string? id,
		AckFilter ackFilter,
		ICollection<string> monitorObjectGroups,
		string? monitorObjectName,
		int? monitorObjectId,
		ICollection<AlertType>? alertTypes,
		string? resourceTemplateDisplayName,
		int? resourceTemplateId,
		string? instanceName,
		string? dataPointName,
		ICollection<AlertLevel>? alertLevels,
		string? orderBy,
		OrderDirection? orderDirection,
		SdtFilter sdtFilter,
		string? problemSignature,
		bool isPercentageAvailability = false
		)
	{
		using var context = GetContext();
		var queryable = context.Alerts.AsQueryable();

		if (id != null)
		{
			queryable = queryable.Where(a => a.LogicMonitorId == id);
		}

		if (problemSignature != null)
		{
			queryable = queryable.Where(a => a.InternalId == problemSignature);
		}

		if (monitorObjectId != null)
		{
			queryable = queryable.Where(a => a.MonitorObjectId == monitorObjectId);
		}

		if (monitorObjectName != null)
		{
			queryable = queryable.Where(a => a.MonitorObjectName == monitorObjectName);
		}

		if (resourceTemplateId != null)
		{
			queryable = queryable.Where(a => a.ResourceTemplateId == resourceTemplateId);
		}

		if (resourceTemplateDisplayName != null)
		{
			queryable = queryable.Where(a => a.ResourceTemplateName == resourceTemplateDisplayName);
		}

		if (instanceName != null)
		{
			queryable = queryable.Where(a => a.InstanceName == instanceName);
		}

		if (dataPointName != null)
		{
			queryable = queryable.Where(a => a.DataPointName == dataPointName);
		}

		if (alertLevels != null)
		{
			queryable = queryable.Where(a => alertLevels.Select(al => (int)al).Contains(a.Severity));
		}

		if (alertTypes != null)
		{
			queryable = queryable.Where(a => alertTypes.Contains(a.AlertType));
		}

		if (!includeInactive)
		{
			queryable = queryable.Where(a => !a.IsCleared);
		}

		// Only one is ever passed
		var monitorObjectGroup = monitorObjectGroups?.First();
		if (monitorObjectGroup != null)
		{
			var likeString = $"{monitorObjectGroup.TrimEnd('*')}%";
			queryable = queryable
				.Where(a =>
				   (a.MonitorObjectGroup0 != null && a.MonitorObjectGroup0.MonitoredObjectType == a.MonitorObjectType && EF.Functions.Like(a.MonitorObjectGroup0.FullPath, likeString))
				|| (a.MonitorObjectGroup1 != null && a.MonitorObjectGroup1.MonitoredObjectType == a.MonitorObjectType && EF.Functions.Like(a.MonitorObjectGroup1.FullPath, likeString))
				|| (a.MonitorObjectGroup2 != null && a.MonitorObjectGroup2.MonitoredObjectType == a.MonitorObjectType && EF.Functions.Like(a.MonitorObjectGroup2.FullPath, likeString))
				|| (a.MonitorObjectGroup3 != null && a.MonitorObjectGroup3.MonitoredObjectType == a.MonitorObjectType && EF.Functions.Like(a.MonitorObjectGroup3.FullPath, likeString))
				|| (a.MonitorObjectGroup4 != null && a.MonitorObjectGroup4.MonitoredObjectType == a.MonitorObjectType && EF.Functions.Like(a.MonitorObjectGroup4.FullPath, likeString))
				|| (a.MonitorObjectGroup5 != null && a.MonitorObjectGroup5.MonitoredObjectType == a.MonitorObjectType && EF.Functions.Like(a.MonitorObjectGroup5.FullPath, likeString))
				|| (a.MonitorObjectGroup6 != null && a.MonitorObjectGroup6.MonitoredObjectType == a.MonitorObjectType && EF.Functions.Like(a.MonitorObjectGroup6.FullPath, likeString))
				|| (a.MonitorObjectGroup7 != null && a.MonitorObjectGroup7.MonitoredObjectType == a.MonitorObjectType && EF.Functions.Like(a.MonitorObjectGroup7.FullPath, likeString))
				|| (a.MonitorObjectGroup8 != null && a.MonitorObjectGroup8.MonitoredObjectType == a.MonitorObjectType && EF.Functions.Like(a.MonitorObjectGroup8.FullPath, likeString))
				|| (a.MonitorObjectGroup9 != null && a.MonitorObjectGroup9.MonitoredObjectType == a.MonitorObjectType && EF.Functions.Like(a.MonitorObjectGroup9.FullPath, likeString))
				);
		}

		switch (sdtFilter)
		{
			case SdtFilter.All:
				// No constraint
				break;
			case SdtFilter.Sdt:
				queryable = queryable.Where(a => a.InScheduledDownTime);
				break;
			case SdtFilter.NonSdt:
				queryable = queryable.Where(a => !a.InScheduledDownTime);
				break;
			default:
				throw new NotSupportedException($"Unsupported SDT filter {sdtFilter}");
		}

		switch (ackFilter)
		{
			case AckFilter.All:
				// No constraint
				break;
			case AckFilter.Acked:
				queryable = queryable.Where(a => a.Acked);
				break;
			case AckFilter.Nonacked:
				queryable = queryable.Where(a => !a.Acked);
				break;
			default:
				throw new NotSupportedException($"Unsupported SDT filter {sdtFilter}");
		}

		queryable = queryable.Where(a =>
			// Alert is not cleared and it started before the end of the period
			(a.StartOnSeconds <= endSeconds && !a.IsCleared)
			// Or it starts within the period
			|| (a.StartOnSeconds > startSeconds && a.StartOnSeconds <= endSeconds)
			// Or it ends within the period
			|| (a.EndOnSeconds > startSeconds && a.EndOnSeconds <= endSeconds)
			// Or it covers the entire period
			|| (a.StartOnSeconds <= startSeconds && a.EndOnSeconds > endSeconds)
		);

		if (orderBy != null)
		{
			queryable = $"{orderBy}/{orderDirection}" switch
			{
				"StartOnSeconds/Asc" => queryable.OrderBy(a => a.StartOnSeconds),
				"StartOnSeconds/Desc" => queryable.OrderByDescending(a => a.StartOnSeconds),
				_ => throw new NotSupportedException("Only orderBy=StartOnSeconds currently supported."),
			};
		}

		if (skip.HasValue)
		{
			queryable = queryable.Skip(skip.Value);
		}

		if (take.HasValue)
		{
			queryable = queryable.Take(take.Value);
		}

		// Use only the needed fields
		if (isPercentageAvailability)
		{
			queryable = queryable.Select(a => new AlertStoreItem
			{
				Id = a.Id,
				StartOnSeconds = a.StartOnSeconds,
				EndOnSeconds = a.EndOnSeconds,
				IsCleared = a.IsCleared,
				ClearValue = a.ClearValue,
				Severity = a.Severity,
				MonitorObjectId = a.MonitorObjectId,
				InstanceId = a.InstanceId,
				InstanceName = a.InstanceName,
				ResourceTemplateName = a.ResourceTemplateName
			});
		}

		return (await queryable.ToListAsync().ConfigureAwait(false))
			.ConvertAll(MapperInstance.Map<AlertStoreItem, Alert>);
	}

	public static async Task<List<int>> GetAllCachedCollectorGroupIdsAsync(DbSet<CollectorGroupStoreItem> collectorGroups, string groupName)
		=> await collectorGroups
			.Where(cg => cg.Name == groupName)
			.Select(cg => cg.LogicMonitorId)
			.ToListAsync()
			.ConfigureAwait(false);

	public static async Task<List<int>> GetAllCachedDeviceGroupIdsAsync(DbSet<ResourceGroupStoreItem> deviceGroups, string groupName)
		=> (groupName ?? throw new ArgumentNullException(nameof(groupName))).EndsWith('*')
			? await deviceGroups
				.Where(dg => dg.FullPath.StartsWith(groupName.TrimEnd('*'), StringComparison.Ordinal))
				.Select(dg => dg.LogicMonitorId)
				.ToListAsync()
				.ConfigureAwait(false)
			: await deviceGroups
				.Where(dg => dg.FullPath == groupName)
				.Select(dg => dg.LogicMonitorId)
				.ToListAsync()
				.ConfigureAwait(false);

	public static async Task<List<int>> GetAllCachedWebsiteGroupIdsAsync(DbSet<WebsiteGroupStoreItem> websiteGroups, string groupName)
		=> (groupName ?? throw new ArgumentNullException(nameof(groupName))).EndsWith('*')
			? await websiteGroups
				.Where(wg => wg.FullPath.StartsWith(groupName.TrimEnd('*'), StringComparison.Ordinal))
				.Select(wg => wg.LogicMonitorId)
				.ToListAsync()
				.ConfigureAwait(false)
			: await websiteGroups
				.Where(wg => wg.FullPath == groupName)
				.Select(wg => wg.LogicMonitorId)
				.ToListAsync()
				.ConfigureAwait(false);

	internal Task<List<string>> GetAggregationTablesAsync()
			 => AggregationWriter.GetTablesAsync(DbContextOptions);

	internal Task DropAggregationTableAsync(DateTimeOffset testAggregationPeriod)
		=> AggregationWriter.DropTableAsync(DbContextOptions, testAggregationPeriod, _logger);

	internal Task AgeAggregationTablesAsync(int countAggregationDaysToRetain)
		=> AggregationWriter.PerformAgingAsync(DbContextOptions, countAggregationDaysToRetain, _logger);

	internal async Task<string> EnsureTableExistsAsync(DateTimeOffset testAggregationPeriod)
	{
		using var context = GetContext();
		using var dbConnection = context.Database.GetDbConnection();
		using var sqlConnection = new SqlConnection(dbConnection.ConnectionString);
		await sqlConnection
			.OpenAsync()
			.ConfigureAwait(false);
		var tableName = await AggregationWriter
			.EnsureTableExistsAsync(sqlConnection, testAggregationPeriod)
			.ConfigureAwait(false);
		await sqlConnection
			.CloseAsync()
			.ConfigureAwait(false);
		return tableName;
	}

	public async Task SyncDeviceLogicModuleSourcesAndInstancesAsync(
		LogicModuleConfigurationItem logicModuleConfigurationItem,
		int logicModuleIndex,
		int logicModuleCount,
		INotificationReceiver notificationReceiver,
		ILogger logger,
		CancellationToken cancellationToken)
	{
		try
		{
			using var context = GetContext();
			int logicModuleId;
			string databaseAppliesTo;
			Guid databaseLogicModuleId;
			List<DataSourceDataPointStoreItem> dataSourceDataPoints = [];
			var logicModuleType = logicModuleConfigurationItem switch
			{
				DataSourceConfigurationItem => nameof(DataSource),
				ConfigSourceConfigurationItem => nameof(ConfigSource),
				_ => throw new NotSupportedException($"LogicModuleType {logicModuleConfigurationItem.GetType().Name} is not supported."),
			};

			logger.LogInformation(
				$"Syncing {nameof(ResourceDataSourceInstance)}s for {{LogicModuleType}} {{LogicModuleName}} ({{LogicModuleIndex}}/{{LogicModuleCount}})...",
				logicModuleType,
				logicModuleConfigurationItem.Name,
				logicModuleIndex,
				logicModuleCount);

			switch (logicModuleConfigurationItem)
			{
				case DataSourceConfigurationItem dataSourceConfigurationItem:
					var dataSourceStoreItem = await context
						.DataSources
						.SingleOrDefaultAsync(ds => ds.Name == dataSourceConfigurationItem.Name, cancellationToken)
						.ConfigureAwait(false);

					if (dataSourceStoreItem is null)
					{
						logger.LogError(
							"For LogicMonitor instance {LogicMonitorAccount}, expected to find Database DataSource called '{DataSourceName}', but it was missing.",
							_configuration.LogicMonitorClientOptions.Account,
							dataSourceConfigurationItem.Name);
						return;
					}

					databaseLogicModuleId = dataSourceStoreItem.Id;
					databaseAppliesTo = dataSourceStoreItem.AppliesTo;
					logicModuleId = dataSourceStoreItem.LogicMonitorId;

					dataSourceDataPoints = await SyncDataSourceDataPointsAsync(
						dataSourceStoreItem,
						context,
						dataSourceConfigurationItem.DataPoints,
						cancellationToken
						)
						.ConfigureAwait(false);
					break;
				case ConfigSourceConfigurationItem configSourceConfigurationItem:
					var configSourceStoreItem = await context
							.ConfigSources
							.Where(cs => cs.Name == configSourceConfigurationItem.Name)
							.FirstOrDefaultAsync(cancellationToken)
							.ConfigureAwait(false);

					if (configSourceStoreItem is null)
					{
						logger.LogError(
								"For LogicMonitor instance {LogicMonitorAccount}, expected to find Database ConfigSource called '{ConfigSourceName}', but it was missing.",
								_configuration.LogicMonitorClientOptions.Account,
								configSourceConfigurationItem.Name);
						return;
					}

					databaseLogicModuleId = configSourceStoreItem.Id;
					databaseAppliesTo = configSourceStoreItem.AppliesTo;
					logicModuleId = configSourceStoreItem.LogicMonitorId;

					break;
				default:
					throw new NotSupportedException($"LogicModuleType {logicModuleConfigurationItem.GetType().Name} is not supported.");
			}

			// Get the Devices that match the appliesTo function on the DataSource
			var appliesToMatches = await GetAppliesToAsync(databaseAppliesTo, cancellationToken)
				.ConfigureAwait(false);

			// Further constrain the appliesToMatches if requested
			if (!string.IsNullOrWhiteSpace(logicModuleConfigurationItem.AppliesTo))
			{
				var requestedAppliesToMatches = await GetAppliesToAsync(logicModuleConfigurationItem.AppliesTo, cancellationToken)
					.ConfigureAwait(false);

				appliesToMatches = [.. appliesToMatches.Where(a => requestedAppliesToMatches.Any(r => r.Id == a.Id))];
			}

			logger.LogDebug(
				"Syncing {DataSourceName} instances for {AppliesToMatchesCount} devices",
				logicModuleConfigurationItem.Name,
				appliesToMatches.Count);

			var instanceProperties = typeof(ResourceDataSourceInstance)
				.GetProperties();

			var markedMissing = 0;

			// Not all of these will have instances
			var appliesToMatchIndex = 0;
			var appliesToMatchCount = appliesToMatches.Count;
			foreach (var appliesToMatch in appliesToMatches)
			{
				appliesToMatchIndex++;
				await notificationReceiver.SetStageNameAsync(
					$"Syncing {logicModuleType} {logicModuleConfigurationItem.Name} ({logicModuleIndex}/{logicModuleCount}) instances for Resource {appliesToMatchIndex}/{appliesToMatchCount})",
					cancellationToken)
					.ConfigureAwait(false);

				// Get the resource
				var resource = await GetAsync<Resource>(appliesToMatch.Id, cancellationToken)
					.ConfigureAwait(false);

				// Get the ResourceDataSource
				var resourceDataSource = await GetResourceDataSourceByResourceIdAndDataSourceIdAsync(
						resource.Id,
						logicModuleId,
						cancellationToken
					)
					.ConfigureAwait(false);
				if (resourceDataSource is null)
				{
					continue;
				}
				// We have a ResourceDataSource

				markedMissing = logicModuleConfigurationItem switch
				{
					DataSourceConfigurationItem dataSourceConfigurationItem
						=> await ProcessResourceDataSourceAsync(
							dataSourceConfigurationItem,
							logger,
							context,
							databaseLogicModuleId,
							dataSourceDataPoints,
							instanceProperties,
							markedMissing,
							resource,
							resourceDataSource,
							cancellationToken)
								.ConfigureAwait(false),
					ConfigSourceConfigurationItem configSourceConfigurationItem
						=> await ProcessResourceConfigSourceAsync(
							configSourceConfigurationItem,
							logger,
							context,
							databaseLogicModuleId,
							instanceProperties,
							markedMissing,
							resource,
							resourceDataSource,
							cancellationToken)
								.ConfigureAwait(false),
					_ => throw new NotSupportedException($"LogicModuleType {logicModuleConfigurationItem.GetType().Name} is not supported."),
				};

				// Check for any that were NOT in the entries that came back from the API
			}

			var added = context.ChangeTracker.Entries().Count(e => e.State == EntityState.Added);
			var modified = context.ChangeTracker.Entries().Count(e => e.State == EntityState.Modified);
			var total = context.ChangeTracker.Entries().Count();
			var rowsAffected = await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

			logger.LogInformation(
				"Sync completed for {LogicModuleName}; Total {Total}; Added {Added}; Modified {Modified} ({MarkedMissing:N0} MarkedMissing).",
				logicModuleConfigurationItem.Name,
				total,
				added,
				modified,
				markedMissing);

		}
		catch (Exception e) when (e is OperationCanceledException or TaskCanceledException)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				// We're done, don't loop any more
				return;
			}
			// If it was anything else then re-throw
			throw;
		}
		catch (Exception ex)
		{
			logger.LogWarning(
				ex,
				$"Error while syncing {nameof(ResourceDataSourceInstance)}s for LogicModule '{{DataSource}}' : {{Message}}\n {{StackTrace}}",
				logicModuleConfigurationItem.Name,
				ex.Message,
				ex.StackTrace);
		}
	}

	private async Task<int> ProcessResourceDataSourceAsync(
		DataSourceConfigurationItem dataSourceConfigurationItem,
		ILogger logger,
		Context context,
		Guid databaseLogicModuleId,
		List<DataSourceDataPointStoreItem> dataSourceDataPoints,
		PropertyInfo[] instanceProperties,
		int markedMissing,
		Resource device,
		ResourceDataSource deviceDataSource,
		CancellationToken cancellationToken)
	{

		// Ensure that this DeviceDataSource exists in the database
		var deviceDataSourceStoreItem = await context
			.DeviceDataSources
			.Include(dds => dds.DataSource)
			.Include(dds => dds.Device)
			.SingleOrDefaultAsync(dds =>
					dds.Device!.LogicMonitorId == deviceDataSource.ResourceId
					&& dds.DataSource!.LogicMonitorId == deviceDataSource.DataSourceId,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var deviceStoreItem = await context
			.Devices
			.SingleOrDefaultAsync(d => d.LogicMonitorId == deviceDataSource.ResourceId, cancellationToken)
			.ConfigureAwait(false);

		if (deviceStoreItem is null)
		{
			logger.LogError(
				"For LogicMonitor instance {LogicMonitorAccount}, expected to find Database Device called '{DeviceName}', but it was missing.",
				_configuration.LogicMonitorClientOptions.Account,
				device.Name);
			return markedMissing;
		}

		var dataSourceStoreItem = await context
			.DataSources
			.SingleOrDefaultAsync(d => d.LogicMonitorId == deviceDataSource.DataSourceId, cancellationToken)
			.ConfigureAwait(false);

		if (dataSourceStoreItem is null)
		{
			logger.LogError(
					"For LogicMonitor instance {LogicMonitorAccount}, expected to find Database DataSource called '{DataSourceName}', but it was missing.",
					_configuration.LogicMonitorClientOptions.Account,
					dataSourceConfigurationItem.Name);
			return markedMissing;
		}

		if (deviceDataSourceStoreItem == null)
		{
			// Add it to the database
			deviceDataSourceStoreItem = MapperInstance.Map<ResourceDataSourceStoreItem>(deviceDataSource);
			context.DeviceDataSources.Add(deviceDataSourceStoreItem);
		}
		else
		{
			// Update the existing entry
			deviceDataSourceStoreItem = MapperInstance.Map(deviceDataSource, deviceDataSourceStoreItem);
		}

		deviceDataSourceStoreItem.DeviceId = deviceStoreItem!.Id;
		deviceDataSourceStoreItem.DataSourceId = dataSourceStoreItem.Id;
		// It is now in the database context

		// Fetch the DeviceDataSourceInstances from the API
		var apiDeviceDataSourceInstances =
			await GetAllResourceDataSourceInstancesAsync(
				device.Id,
				deviceDataSource.Id,
				new(),
				cancellationToken)
			.ConfigureAwait(false);

		var instanceObservedDateTimeUtc = _timeProviderService.UtcOffsetNow;

		// Update the DatamartLastObserved BEFORE we remove instances that do not match
		foreach (var instance in apiDeviceDataSourceInstances)
		{
			if (await context
				.DeviceDataSourceInstances
				.SingleOrDefaultAsync(dddsi => dddsi.LogicMonitorId == instance.Id, cancellationToken: cancellationToken)
				.ConfigureAwait(false) is ResourceDataSourceInstanceStoreItem instanceStoreItem)
			{
				// RM-16087 Update "DatamartLastObserved" to the date the sync noticed them, even if nothing was changed
				instanceStoreItem.DatamartLastObserved = instanceObservedDateTimeUtc;
			}

			// Those device data source instances NOT in the database will be added further on and "DatamartLastObserved" will be set there
		}

		await context
			.SaveChangesAsync(cancellationToken)
			.ConfigureAwait(false);

		// Remove any instances that do not match the DataSourceConfigurationItem's InstanceInclusionExpression
		if (!string.IsNullOrWhiteSpace(dataSourceConfigurationItem.InstanceInclusionExpression) && dataSourceConfigurationItem.InstanceInclusionExpression != "true")
		{
			var instanceInclusionExpression = new ExtendedExpression(dataSourceConfigurationItem.InstanceInclusionExpression);

			apiDeviceDataSourceInstances = [.. apiDeviceDataSourceInstances
						.Where(i =>
						{
							try
							{
								foreach (var instanceProperty in instanceProperties)
								{
									instanceInclusionExpression.Parameters[instanceProperty.Name] = instanceProperty.GetValue(i) ?? string.Empty;
								}
							}
							catch (Exception e)
							{
								logger.LogError(e, "Error setting InstanceInclusionExpression parameters for {DeviceName} instance {InstanceName}", device.Name, i.Name);
							}

							try
							{
								foreach (var instanceCustomProperty in i.CustomProperties)
								{
									instanceInclusionExpression.Parameters[instanceCustomProperty.Name] = instanceCustomProperty.Value ?? string.Empty;
								}
							}
							catch (Exception e)
							{
								logger.LogError(e, "Error setting InstanceInclusionExpression parameters for {DeviceName} instance {InstanceName} custom properties", device.Name, i.Name);
							}

							try
							{
								return instanceInclusionExpression.Evaluate() as bool? ?? true;
							}
							catch (Exception e)
							{
								logger.LogError(
									e,
									"Error evaluating InstanceInclusionExpression '{InstanceInclusionExpression}' for {DeviceName} instance {InstanceName}",
									dataSourceConfigurationItem.InstanceInclusionExpression,
									device.Name,
									i.Name);

								// Default to true
								return true;
							}
						})];
		}

		foreach (var apiDeviceDataSourceInstance in apiDeviceDataSourceInstances)
		{
			// Ensure that this DeviceDataSourceInstance exists in the database
			var databaseDeviceDataSourceInstance = await context
				.DeviceDataSourceInstances
				.SingleOrDefaultAsync(dddsi => dddsi.LogicMonitorId == apiDeviceDataSourceInstance.Id, cancellationToken: cancellationToken)
				.ConfigureAwait(false);
			if (databaseDeviceDataSourceInstance == null)
			{
				// Add it to the database
				databaseDeviceDataSourceInstance = MapperInstance.Map<ResourceDataSourceInstanceStoreItem>(apiDeviceDataSourceInstance);
				databaseDeviceDataSourceInstance.DeviceDataSourceId = deviceDataSourceStoreItem.Id;
				context.DeviceDataSourceInstances.Add(databaseDeviceDataSourceInstance);
			}
			else
			{
				// Update - including clearing the LastWentMissingUtc field
				// Update the existing entry using AutoMapper
				MapperInstance.Map(apiDeviceDataSourceInstance, databaseDeviceDataSourceInstance);
				databaseDeviceDataSourceInstance.DeviceDataSourceId = deviceDataSourceStoreItem.Id;
				databaseDeviceDataSourceInstance.LastWentMissing = null;
			}
			// It is now in the database context

			// RM-16087 Update "DatamartLastObserved" to the date the sync noticed them
			databaseDeviceDataSourceInstance.DatamartLastObserved = instanceObservedDateTimeUtc;

			// Set the properties by using NCalc
			databaseDeviceDataSourceInstance.InstanceProperty1 = EvaluateProperty(dataSourceConfigurationItem.InstanceProperty1, device, apiDeviceDataSourceInstance, logger);
			databaseDeviceDataSourceInstance.InstanceProperty2 = EvaluateProperty(dataSourceConfigurationItem.InstanceProperty2, device, apiDeviceDataSourceInstance, logger);
			databaseDeviceDataSourceInstance.InstanceProperty3 = EvaluateProperty(dataSourceConfigurationItem.InstanceProperty3, device, apiDeviceDataSourceInstance, logger);
			databaseDeviceDataSourceInstance.InstanceProperty4 = EvaluateProperty(dataSourceConfigurationItem.InstanceProperty4, device, apiDeviceDataSourceInstance, logger);
			databaseDeviceDataSourceInstance.InstanceProperty5 = EvaluateProperty(dataSourceConfigurationItem.InstanceProperty5, device, apiDeviceDataSourceInstance, logger);
			databaseDeviceDataSourceInstance.InstanceProperty6 = EvaluateProperty(dataSourceConfigurationItem.InstanceProperty6, device, apiDeviceDataSourceInstance, logger);
			databaseDeviceDataSourceInstance.InstanceProperty7 = EvaluateProperty(dataSourceConfigurationItem.InstanceProperty7, device, apiDeviceDataSourceInstance, logger);
			databaseDeviceDataSourceInstance.InstanceProperty8 = EvaluateProperty(dataSourceConfigurationItem.InstanceProperty8, device, apiDeviceDataSourceInstance, logger);
			databaseDeviceDataSourceInstance.InstanceProperty9 = EvaluateProperty(dataSourceConfigurationItem.InstanceProperty9, device, apiDeviceDataSourceInstance, logger);
			databaseDeviceDataSourceInstance.InstanceProperty10 = EvaluateProperty(dataSourceConfigurationItem.InstanceProperty10, device, apiDeviceDataSourceInstance, logger);

			var deviceDataSourceInstanceDataPoints = await context
				.DeviceDataSourceInstanceDataPoints
				.Where(ddsidp => ddsidp.DeviceDataSourceInstanceId == databaseDeviceDataSourceInstance.Id)
				.ToListAsync(cancellationToken);

			var index = 0;
			foreach (var dataSourceDataPointId in dataSourceDataPoints.Select(dsdp => dsdp.Id))
			{
				if (!deviceDataSourceInstanceDataPoints.Any(ddsidp => ddsidp.DeviceDataSourceInstanceId == databaseDeviceDataSourceInstance.Id && ddsidp.DataSourceDataPointId == dataSourceDataPointId))
				{
					// Add to the database
					context
					.DeviceDataSourceInstanceDataPoints
					.Add(new ResourceDataSourceInstanceDataPointStoreItem
					{
						DeviceDataSourceInstanceId = databaseDeviceDataSourceInstance.Id,
						DataSourceDataPointId = dataSourceDataPointId,
						InstanceDatapointProperty1 = EvaluateProperty(dataSourceConfigurationItem.DataPoints[index].InstanceDatapointProperty1, device, apiDeviceDataSourceInstance, logger),
						InstanceDatapointProperty2 = EvaluateProperty(dataSourceConfigurationItem.DataPoints[index].InstanceDatapointProperty2, device, apiDeviceDataSourceInstance, logger),
						InstanceDatapointProperty3 = EvaluateProperty(dataSourceConfigurationItem.DataPoints[index].InstanceDatapointProperty3, device, apiDeviceDataSourceInstance, logger),
						InstanceDatapointProperty4 = EvaluateProperty(dataSourceConfigurationItem.DataPoints[index].InstanceDatapointProperty4, device, apiDeviceDataSourceInstance, logger),
						InstanceDatapointProperty5 = EvaluateProperty(dataSourceConfigurationItem.DataPoints[index].InstanceDatapointProperty5, device, apiDeviceDataSourceInstance, logger),
						InstanceDatapointProperty6 = EvaluateProperty(dataSourceConfigurationItem.DataPoints[index].InstanceDatapointProperty6, device, apiDeviceDataSourceInstance, logger),
						InstanceDatapointProperty7 = EvaluateProperty(dataSourceConfigurationItem.DataPoints[index].InstanceDatapointProperty7, device, apiDeviceDataSourceInstance, logger),
						InstanceDatapointProperty8 = EvaluateProperty(dataSourceConfigurationItem.DataPoints[index].InstanceDatapointProperty8, device, apiDeviceDataSourceInstance, logger),
						InstanceDatapointProperty9 = EvaluateProperty(dataSourceConfigurationItem.DataPoints[index].InstanceDatapointProperty9, device, apiDeviceDataSourceInstance, logger),
						InstanceDatapointProperty10 = EvaluateProperty(dataSourceConfigurationItem.DataPoints[index].InstanceDatapointProperty10, device, apiDeviceDataSourceInstance, logger)
					});
				}
				else
				{
					// If (through error), there is more than one, each one should be updated.
					foreach (var ddsipsi in deviceDataSourceInstanceDataPoints.Where(ddsidp => ddsidp.DeviceDataSourceInstanceId == databaseDeviceDataSourceInstance.Id && ddsidp.DataSourceDataPointId == dataSourceDataPointId))
					{
						ddsipsi.InstanceDatapointProperty1 = EvaluateProperty(dataSourceConfigurationItem.DataPoints[index].InstanceDatapointProperty1, device, apiDeviceDataSourceInstance, logger);
						ddsipsi.InstanceDatapointProperty2 = EvaluateProperty(dataSourceConfigurationItem.DataPoints[index].InstanceDatapointProperty2, device, apiDeviceDataSourceInstance, logger);
						ddsipsi.InstanceDatapointProperty3 = EvaluateProperty(dataSourceConfigurationItem.DataPoints[index].InstanceDatapointProperty3, device, apiDeviceDataSourceInstance, logger);
						ddsipsi.InstanceDatapointProperty4 = EvaluateProperty(dataSourceConfigurationItem.DataPoints[index].InstanceDatapointProperty4, device, apiDeviceDataSourceInstance, logger);
						ddsipsi.InstanceDatapointProperty5 = EvaluateProperty(dataSourceConfigurationItem.DataPoints[index].InstanceDatapointProperty5, device, apiDeviceDataSourceInstance, logger);
						ddsipsi.InstanceDatapointProperty6 = EvaluateProperty(dataSourceConfigurationItem.DataPoints[index].InstanceDatapointProperty6, device, apiDeviceDataSourceInstance, logger);
						ddsipsi.InstanceDatapointProperty7 = EvaluateProperty(dataSourceConfigurationItem.DataPoints[index].InstanceDatapointProperty7, device, apiDeviceDataSourceInstance, logger);
						ddsipsi.InstanceDatapointProperty8 = EvaluateProperty(dataSourceConfigurationItem.DataPoints[index].InstanceDatapointProperty8, device, apiDeviceDataSourceInstance, logger);
						ddsipsi.InstanceDatapointProperty9 = EvaluateProperty(dataSourceConfigurationItem.DataPoints[index].InstanceDatapointProperty9, device, apiDeviceDataSourceInstance, logger);
						ddsipsi.InstanceDatapointProperty10 = EvaluateProperty(dataSourceConfigurationItem.DataPoints[index].InstanceDatapointProperty10, device, apiDeviceDataSourceInstance, logger);
					}
				}

				index++;
			}

		}

		// It's possible that there are entries in the database that are no longer brought back from the API, due to instances being deleted by Active Discovery/manual deletion
		// Get all database instances where the
		var databaseDeviceDataSourceInstanceIdsThatShouldHaveComeBackFromApi = new HashSet<int>(await context
				.DeviceDataSourceInstances
				.Include(ddsi => ddsi.DeviceDataSource!.DataSource)
				.Include(ddsi => ddsi.DeviceDataSource!.Device)
				.Where(ddsi => ddsi.DeviceDataSource!.Device!.LogicMonitorId == device.Id && ddsi.DeviceDataSource!.DataSourceId == databaseLogicModuleId && ddsi.LastWentMissing == null)
				.Select(ddsi => ddsi.LogicMonitorId)
				.ToListAsync(cancellationToken: cancellationToken)
				.ConfigureAwait(false));

		var apiDeviceDataSourceInstanceIds = new HashSet<int>(apiDeviceDataSourceInstances.Select(ddsi => ddsi.Id));

		var deviceDatasourceInstanceIdsToMarkMissing = databaseDeviceDataSourceInstanceIdsThatShouldHaveComeBackFromApi
			.Except(apiDeviceDataSourceInstanceIds)
			.ToList();
		if (deviceDatasourceInstanceIdsToMarkMissing.Count > 0)
		{
			foreach (var deviceDatasourceInstanceIdToMarkMissing in deviceDatasourceInstanceIdsToMarkMissing)
			{
				// Get the entry to modify from the context and update it
				var databaseDeviceDataSourceInstance = await context
					.DeviceDataSourceInstances
					.SingleOrDefaultAsync(dddsi => dddsi.LogicMonitorId == deviceDatasourceInstanceIdToMarkMissing, cancellationToken: cancellationToken)
					.ConfigureAwait(false);

				if (databaseDeviceDataSourceInstance is null)
				{
					continue;
				}

				databaseDeviceDataSourceInstance.LastWentMissing = _timeProviderService.UtcOffsetNow;
			}

			markedMissing += deviceDatasourceInstanceIdsToMarkMissing.Count;
		}

		return markedMissing;
	}

	private async Task<int> ProcessResourceConfigSourceAsync(
		ConfigSourceConfigurationItem configSourceConfigurationItem,
		ILogger logger,
		Context context,
		Guid databaseLogicModuleId,
		PropertyInfo[] instanceProperties,
		int markedMissing,
		Resource device,
		ResourceDataSource deviceConfigSource,
		CancellationToken cancellationToken)
	{
		var oldestUtc = configSourceConfigurationItem.MaxAgeDays == 0
			? (DateTimeOffset?)null
			: DateTimeOffset.UtcNow.AddDays(-configSourceConfigurationItem.MaxAgeDays);

		// Ensure that this ConfigSource exists in the database
		var deviceConfigSourceStoreItem = await context
			.DeviceConfigSources
			.Include(dds => dds.ConfigSource)
			.Include(dds => dds.Device)
			.SingleOrDefaultAsync(dds =>
					dds.Device!.LogicMonitorId == deviceConfigSource.ResourceId
					&& dds.ConfigSource!.LogicMonitorId == deviceConfigSource.DataSourceId,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var deviceStoreItem = await context
			.Devices
			.SingleOrDefaultAsync(d => d.LogicMonitorId == deviceConfigSource.ResourceId, cancellationToken)
			.ConfigureAwait(false);

		if (deviceStoreItem is null)
		{
			logger.LogError(
				"For LogicMonitor instance {LogicMonitorAccount}, expected to find Database Device called '{DeviceName}', but it was missing.",
				_configuration.LogicMonitorClientOptions.Account,
				device.Name);
			return markedMissing;
		}

		var configSourceStoreItem = await context
			.ConfigSources
			.SingleOrDefaultAsync(d => d.LogicMonitorId == deviceConfigSource.DataSourceId, cancellationToken)
			.ConfigureAwait(false);

		if (configSourceStoreItem is null)
		{
			logger.LogError(
					"For LogicMonitor instance {LogicMonitorAccount}, expected to find Database ConfigSource called '{ConfigSourceName}', but it was missing.",
					_configuration.LogicMonitorClientOptions.Account,
					configSourceConfigurationItem.Name);
			return markedMissing;
		}

		if (deviceConfigSourceStoreItem == null)
		{
			// Add it to the database
			deviceConfigSourceStoreItem = MapperInstance.Map<ResourceConfigSourceStoreItem>(deviceConfigSource);
			context.DeviceConfigSources.Add(deviceConfigSourceStoreItem);
		}
		else
		{
			// Update the existing entry
			deviceConfigSourceStoreItem = MapperInstance.Map(deviceConfigSource, deviceConfigSourceStoreItem);
		}

		deviceConfigSourceStoreItem.DeviceId = deviceStoreItem!.Id;
		deviceConfigSourceStoreItem.ConfigSourceId = configSourceStoreItem.Id;
		// It is now in the database context

		// Fetch the DeviceDataSourceInstances from the API
		var apiDeviceConfigSourceInstances =
			await GetAllResourceDataSourceInstancesAsync(
				device.Id,
				deviceConfigSource.Id,
				new(),
				cancellationToken)
			.ConfigureAwait(false);

		var instanceObservedDateTimeUtc = _timeProviderService.UtcOffsetNow;

		// Update the DatamartLastObserved BEFORE we remove instances that do not match
		foreach (var instance in apiDeviceConfigSourceInstances)
		{
			if (await context
				.DeviceConfigSourceInstances
				.SingleOrDefaultAsync(dddsi => dddsi.LogicMonitorId == instance.Id, cancellationToken: cancellationToken)
				.ConfigureAwait(false) is ResourceConfigSourceInstanceStoreItem instanceStoreItem)
			{
				// RM-16087 Update "DatamartLastObserved" to the date the sync noticed them, even if nothing was changed
				instanceStoreItem.DatamartLastObserved = instanceObservedDateTimeUtc;
			}

			// Those device config source instances NOT in the database will be added further on and "DatamartLastObserved" will be set there
		}

		await context
			.SaveChangesAsync(cancellationToken)
			.ConfigureAwait(false);

		// Remove any instances that do not match the ConfigSourceConfigurationItem's InstanceInclusionExpression
		if (!string.IsNullOrWhiteSpace(configSourceConfigurationItem.InstanceInclusionExpression) && configSourceConfigurationItem.InstanceInclusionExpression != "true")
		{
			var instanceInclusionExpression = new ExtendedExpression(configSourceConfigurationItem.InstanceInclusionExpression);

			apiDeviceConfigSourceInstances = [.. apiDeviceConfigSourceInstances
						.Where(i =>
						{
							try
							{
								foreach (var instanceProperty in instanceProperties)
								{
									instanceInclusionExpression.Parameters[instanceProperty.Name] = instanceProperty.GetValue(i) ?? string.Empty;
								}
							}
							catch (Exception e)
							{
								logger.LogError(e, "Error setting InstanceInclusionExpression parameters for {DeviceName} instance {InstanceName}", device.Name, i.Name);
							}

							try
							{
								foreach (var instanceCustomProperty in i.CustomProperties)
								{
									instanceInclusionExpression.Parameters[instanceCustomProperty.Name] = instanceCustomProperty.Value ?? string.Empty;
								}
							}
							catch (Exception e)
							{
								logger.LogError(e, "Error setting InstanceInclusionExpression parameters for {DeviceName} instance {InstanceName} custom properties", device.Name, i.Name);
							}

							try
							{
								return instanceInclusionExpression.Evaluate() as bool? ?? true;
							}
							catch (Exception e)
							{
								logger.LogError(
									e,
									"Error evaluating InstanceInclusionExpression '{InstanceInclusionExpression}' for {DeviceName} instance {InstanceName}",
									configSourceConfigurationItem.InstanceInclusionExpression,
									device.Name,
									i.Name);

								// Default to true
								return true;
							}
						})];
		}

		foreach (var apiDeviceConfigSourceInstance in apiDeviceConfigSourceInstances)
		{
			// Ensure that this DeviceConfigSourceInstance exists in the database
			var databaseDeviceConfigSourceInstance = await context
				.DeviceConfigSourceInstances
				.SingleOrDefaultAsync(dddsi => dddsi.LogicMonitorId == apiDeviceConfigSourceInstance.Id, cancellationToken: cancellationToken)
				.ConfigureAwait(false);
			if (databaseDeviceConfigSourceInstance == null)
			{
				// Add it to the database
				databaseDeviceConfigSourceInstance = MapperInstance.Map<ResourceConfigSourceInstanceStoreItem>(apiDeviceConfigSourceInstance);
				databaseDeviceConfigSourceInstance.DeviceConfigSourceId = deviceConfigSourceStoreItem.Id;
				context.DeviceConfigSourceInstances.Add(databaseDeviceConfigSourceInstance);
			}
			else
			{
				// Update - including clearing the LastWentMissingUtc field
				// Update the existing entry using AutoMapper
				MapperInstance.Map(apiDeviceConfigSourceInstance, databaseDeviceConfigSourceInstance);
				databaseDeviceConfigSourceInstance.DeviceConfigSourceId = deviceConfigSourceStoreItem.Id;
				databaseDeviceConfigSourceInstance.LastWentMissing = null;
			}
			// It is now in the database context

			// RM-16087 Update "DatamartLastObserved" to the date the sync noticed them
			databaseDeviceConfigSourceInstance.DatamartLastObserved = instanceObservedDateTimeUtc;

			// Set the properties by using NCalc
			databaseDeviceConfigSourceInstance.InstanceProperty1 = EvaluateProperty(configSourceConfigurationItem.InstanceProperty1, device, apiDeviceConfigSourceInstance, logger);
			databaseDeviceConfigSourceInstance.InstanceProperty2 = EvaluateProperty(configSourceConfigurationItem.InstanceProperty2, device, apiDeviceConfigSourceInstance, logger);
			databaseDeviceConfigSourceInstance.InstanceProperty3 = EvaluateProperty(configSourceConfigurationItem.InstanceProperty3, device, apiDeviceConfigSourceInstance, logger);
			databaseDeviceConfigSourceInstance.InstanceProperty4 = EvaluateProperty(configSourceConfigurationItem.InstanceProperty4, device, apiDeviceConfigSourceInstance, logger);
			databaseDeviceConfigSourceInstance.InstanceProperty5 = EvaluateProperty(configSourceConfigurationItem.InstanceProperty5, device, apiDeviceConfigSourceInstance, logger);
			databaseDeviceConfigSourceInstance.InstanceProperty6 = EvaluateProperty(configSourceConfigurationItem.InstanceProperty6, device, apiDeviceConfigSourceInstance, logger);
			databaseDeviceConfigSourceInstance.InstanceProperty7 = EvaluateProperty(configSourceConfigurationItem.InstanceProperty7, device, apiDeviceConfigSourceInstance, logger);
			databaseDeviceConfigSourceInstance.InstanceProperty8 = EvaluateProperty(configSourceConfigurationItem.InstanceProperty8, device, apiDeviceConfigSourceInstance, logger);
			databaseDeviceConfigSourceInstance.InstanceProperty9 = EvaluateProperty(configSourceConfigurationItem.InstanceProperty9, device, apiDeviceConfigSourceInstance, logger);
			databaseDeviceConfigSourceInstance.InstanceProperty10 = EvaluateProperty(configSourceConfigurationItem.InstanceProperty10, device, apiDeviceConfigSourceInstance, logger);

			await ProcessConfigsAsync(
				context,
				databaseDeviceConfigSourceInstance,
				apiDeviceConfigSourceInstance,
				oldestUtc,
				logger,
				cancellationToken)
				.ConfigureAwait(false);
		}

		// It's possible that there are entries in the database that are no longer brought back from the API, due to instances being deleted by Active Discovery/manual deletion
		// Get all database instances where the
		var databaseDeviceConfigSourceInstanceIdsThatShouldHaveComeBackFromApi = new HashSet<int>(await context
				.DeviceConfigSourceInstances
				.Include(ddsi => ddsi.DeviceConfigSource!.ConfigSource)
				.Include(ddsi => ddsi.DeviceConfigSource!.Device)
				.Where(ddsi => ddsi.DeviceConfigSource!.Device!.LogicMonitorId == device.Id && ddsi.DeviceConfigSource!.ConfigSourceId == databaseLogicModuleId && ddsi.LastWentMissing == null)
				.Select(ddsi => ddsi.LogicMonitorId)
				.ToListAsync(cancellationToken: cancellationToken)
				.ConfigureAwait(false));

		var apiDeviceConfigSourceInstanceIds = new HashSet<int>(apiDeviceConfigSourceInstances.Select(ddsi => ddsi.Id));

		var deviceConfigSourceInstanceIdsToMarkMissing = databaseDeviceConfigSourceInstanceIdsThatShouldHaveComeBackFromApi
			.Except(apiDeviceConfigSourceInstanceIds)
			.ToList();
		if (deviceConfigSourceInstanceIdsToMarkMissing.Count > 0)
		{
			foreach (var deviceConfigSourceInstanceIdToMarkMissing in deviceConfigSourceInstanceIdsToMarkMissing)
			{
				// Get the entry to modify from the context and update it
				var databaseConfigSourceInstance = await context
					.DeviceConfigSourceInstances
					.SingleOrDefaultAsync(dddsi => dddsi.LogicMonitorId == deviceConfigSourceInstanceIdToMarkMissing, cancellationToken: cancellationToken)
					.ConfigureAwait(false);

				if (databaseConfigSourceInstance is null)
				{
					continue;
				}

				databaseConfigSourceInstance.LastWentMissing = _timeProviderService.UtcOffsetNow;
			}

			markedMissing += deviceConfigSourceInstanceIdsToMarkMissing.Count;
		}

		return markedMissing;
	}

	private async Task ProcessConfigsAsync(
		Context context,
		ResourceConfigSourceInstanceStoreItem databaseDeviceConfigSourceInstance,
		ResourceDataSourceInstance apiDeviceConfigSourceInstance,
		DateTimeOffset? oldestUtc,
		ILogger logger,
		CancellationToken cancellationToken)
	{
		// Always get the most recent

		var mostRecentFilter = new Filter<ResourceDataSourceInstanceConfig>
		{
			Take = 1,
			Order = new Order<ResourceDataSourceInstanceConfig>()
			{
				Property = nameof(ResourceDataSourceInstanceConfig.PollTimestampUtc),
				Direction = OrderDirection.Desc
			}
		};

		var configs = await GetAllResourceDataSourceInstanceConfigsAsync(
				apiDeviceConfigSourceInstance.ResourceId,
				apiDeviceConfigSourceInstance.ResourceDataSourceId,
				apiDeviceConfigSourceInstance.Id,
				mostRecentFilter,
				cancellationToken)
			.ConfigureAwait(false);

		// If there are none - return
		ResourceDataSourceInstanceConfig mostRecentResourceDataSourceInstanceConfig;
		switch (configs.Count)
		{
			case 0:
				// No configs - nothing to do
				return;
			case 1:
				// One config - use it
				mostRecentResourceDataSourceInstanceConfig = configs[0];
				break;
			default:
				// Our "take 1" didn't work.  Error
				logger.LogError(
					"More than one config was returned for {DeviceName} instance {InstanceName} for ConfigSource {ConfigSourceName} ({DataSourceId})",
					apiDeviceConfigSourceInstance.ResourceDisplayName,
					apiDeviceConfigSourceInstance.DisplayName,
					apiDeviceConfigSourceInstance.DataSourceName,
					apiDeviceConfigSourceInstance.DataSourceId);
				return;
		}

		// Only get the other configs if the settings' maxAgeDays is greater than 0
		var otherConfigs = oldestUtc is null
			? []
			: await GetAllResourceDataSourceInstanceConfigsAsync(
				apiDeviceConfigSourceInstance.ResourceId,
				apiDeviceConfigSourceInstance.ResourceDataSourceId,
				apiDeviceConfigSourceInstance.Id,
				new()
				{
					FilterItems =
					[
						// We want to get the configs that are older than the one we just got
						new Lt<ResourceDataSourceInstanceConfig>(nameof(ResourceDataSourceInstanceConfig.PollTimestampUtc), mostRecentResourceDataSourceInstanceConfig.PollTimestampUtc!),
						// More recent than the oldest permitted
						new Gt<ResourceDataSourceInstanceConfig>(nameof(ResourceDataSourceInstanceConfig.PollTimestampUtc), oldestUtc.Value.ToUnixTimeSeconds())
					]
				},
				cancellationToken)
			.ConfigureAwait(false);

		// Make sure that they are all present in the database
		// Create a union of the single and the others
		var allConfigs = new List<ResourceDataSourceInstanceConfig> { mostRecentResourceDataSourceInstanceConfig };
		allConfigs.AddRange(otherConfigs);

		foreach (var config in allConfigs)
		{
			// Ensure that this ConfigSourceInstanceConfig exists in the database
			var databaseConfigSourceInstanceConfig = await context
				.DeviceConfigSourceInstanceConfigs
				.SingleOrDefaultAsync(ddsic => ddsic.LogicMonitorStringId == config.Id, cancellationToken: cancellationToken)
				.ConfigureAwait(false);
			if (databaseConfigSourceInstanceConfig is null)
			{
				// Add it to the database
				databaseConfigSourceInstanceConfig = MapperInstance.Map<ResourceConfigSourceInstanceConfigStoreItem>(config);
				databaseConfigSourceInstanceConfig.DeviceConfigSourceInstanceId = databaseDeviceConfigSourceInstance.Id;
				context.DeviceConfigSourceInstanceConfigs.Add(databaseConfigSourceInstanceConfig);
			}
			else
			{
				// Update the existing entry using AutoMapper
				databaseConfigSourceInstanceConfig = MapperInstance.Map(config, databaseConfigSourceInstanceConfig);
			}

			databaseConfigSourceInstanceConfig.DeviceConfigSourceInstanceId = databaseDeviceConfigSourceInstance.Id;
			databaseConfigSourceInstanceConfig.DatamartLastObserved = _timeProviderService.UtcOffsetNow;
			databaseConfigSourceInstanceConfig.PollUtc = config.PollUtc is null ? DateTimeOffset.MinValue : new(config.PollUtc.Value);
			databaseConfigSourceInstanceConfig.LogicMonitorStringId = config.Id;
		}

		// Remove old ones
		switch (oldestUtc)
		{
			case null:
				// Remove all but the mostRecentResourceDataSourceInstanceConfig
				await context
					.DeviceConfigSourceInstanceConfigs
					.Where(ddsic =>
						// Must be for this instance
						ddsic.DeviceConfigSourceInstanceId == databaseDeviceConfigSourceInstance.Id
						// Don't delete the most recent one
						&& ddsic.LogicMonitorStringId != mostRecentResourceDataSourceInstanceConfig.Id)
					.ExecuteDeleteAsync(cancellationToken: cancellationToken)
					.ConfigureAwait(false);
				break;
			case DateTimeOffset oldestDateTimeOffset:
				// Remove all configs older than the oldestDateTimeOffset
				var configIdsToDelete = await context
					.DeviceConfigSourceInstanceConfigs
					.Where(ddsic =>
						// Must be for this instance
						ddsic.DeviceConfigSourceInstanceId == databaseDeviceConfigSourceInstance.Id
						// Don't delete the most recent one
						&& ddsic.LogicMonitorStringId != mostRecentResourceDataSourceInstanceConfig.Id
						// The older ones
						&& ddsic.PollUtc < oldestDateTimeOffset)
					.ExecuteDeleteAsync(cancellationToken: cancellationToken)
					.ConfigureAwait(false);
				break;
		}
	}

	private static string EvaluateProperty(string condition, Resource device, ResourceDataSourceInstance ddsi, ILogger logger)
	{
		if (string.Equals(condition, "true", StringComparison.OrdinalIgnoreCase) ||
			string.IsNullOrWhiteSpace(condition))
		{
			return string.Empty;
		}

		try
		{
			// Add all the properties on the DDSI into the NCalc expression
			var inclusionExpression = new ExtendedExpression(condition);

			// Device
			foreach (var property in device.AutoProperties)
			{
				inclusionExpression.Parameters[property.Name] = property.Value;
			}

			foreach (var property in device.CustomProperties)
			{
				inclusionExpression.Parameters[property.Name] = property.Value;
			}

			foreach (var property in device.SystemProperties)
			{
				inclusionExpression.Parameters[property.Name] = property.Value;
			}

			// Device Data Source Instance
			foreach (var property in ddsi.AutoProperties)
			{
				inclusionExpression.Parameters[property.Name] = property.Value;
			}

			foreach (var property in ddsi.CustomProperties)
			{
				inclusionExpression.Parameters[property.Name] = property.Value;
			}

			foreach (var property in ddsi.SystemProperties)
			{
				inclusionExpression.Parameters[property.Name] = property.Value;
			}

			return inclusionExpression.Evaluate()?.ToString() ?? string.Empty;
		}
		catch (Exception e)
		{
			logger.LogError(e, "Error evaluating DeviceDataSourceInstance Condition '{Condition}' for {DeviceName} instance {InstanceName} due to {Message}",
				condition,
				ddsi.ResourceDisplayName,
				ddsi.DisplayName,
				e.Message);

			// Default to true
			return string.Empty;
		}
	}

	internal static async Task<List<DataSourceDataPointStoreItem>> SyncDataSourceDataPointsAsync(
		DataSourceStoreItem dataSource,
		Context context,
		List<DataPointConfigurationItem> configDataSourceDataPoints,
		CancellationToken cancellationToken
	)
	{
		var dataSourceDataPointStoreItems = new List<DataSourceDataPointStoreItem>();

		// Make sure that they are present in the database
		foreach (var configDataSourceDataPoint in configDataSourceDataPoints)
		{
			// Ensure that this DataPoint exists in the database
			var databaseDataPoint = await context
				.DataSourceDataPoints
				.SingleOrDefaultAsync(
					dp => dp.DataSource.Name == dataSource.Name
						  && dp.Name == configDataSourceDataPoint.Name,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			if (databaseDataPoint is null)
			{
				// Add it to the database
				databaseDataPoint = MapperInstance.Map<DataSourceDataPointStoreItem>(configDataSourceDataPoint);
				databaseDataPoint.DataSourceId = dataSource.Id;
				context.DataSourceDataPoints.Add(databaseDataPoint);
			}

			databaseDataPoint.ResyncTimeSeriesData = configDataSourceDataPoint.ResyncTimeSeriesData;
			databaseDataPoint.MeasurementUnit = configDataSourceDataPoint.MeasurementUnit;
			databaseDataPoint.Calculation = configDataSourceDataPoint.Calculation;
			databaseDataPoint.PercentageAvailabilityCalculation = configDataSourceDataPoint.PercentageAvailabilityCalculation;
			databaseDataPoint.Property1 = configDataSourceDataPoint.Property1;
			databaseDataPoint.Property2 = configDataSourceDataPoint.Property2;
			databaseDataPoint.Property3 = configDataSourceDataPoint.Property3;
			databaseDataPoint.Property4 = configDataSourceDataPoint.Property4;
			databaseDataPoint.Property5 = configDataSourceDataPoint.Property5;
			databaseDataPoint.Property6 = configDataSourceDataPoint.Property6;
			databaseDataPoint.Property7 = configDataSourceDataPoint.Property7;
			databaseDataPoint.Property8 = configDataSourceDataPoint.Property8;
			databaseDataPoint.Property9 = configDataSourceDataPoint.Property9;
			databaseDataPoint.Property10 = configDataSourceDataPoint.Property10;
			databaseDataPoint.Tags = configDataSourceDataPoint.Tags;

			// Only update the description if it is not null or white-space
			if (!string.IsNullOrWhiteSpace(configDataSourceDataPoint.Description))
			{
				databaseDataPoint.Description = configDataSourceDataPoint.Description;
			}

			dataSourceDataPointStoreItems.Add(databaseDataPoint);
		}

		await context
			.SaveChangesAsync(cancellationToken)
			.ConfigureAwait(false);

		return dataSourceDataPointStoreItems;
	}

	internal async Task<List<int>> GetOrderedDeviceIdsForDataSourceAppliesTos(CancellationToken cancellationToken)
	{
		var databaseDeviceIds = new List<int>();

		using var context = GetContext();

		// Loop over the DataSources
		foreach (var configDataSource in _configuration.DataSources)
		{
			// This is the AppliesTo we have in the database (or LogicMonitor)
			var dsAppliesTo = string.Empty;

			// Find the DataSource in the database
			if (await context
				.DataSources
				.SingleOrDefaultAsync(ds => ds.Name == configDataSource.Name, cancellationToken)
				.ConfigureAwait(false)
				is DataSourceStoreItem databaseDataSource)
			{
				dsAppliesTo = databaseDataSource.AppliesTo;
			}
			else
			{
				// Fetch the DataSource via LogicMonitor instead
				if (await GetDataSourceByUniqueNameAsync(configDataSource.Name, cancellationToken).ConfigureAwait(false)
					is DataSource lmDataSource)
				{
					dsAppliesTo = lmDataSource.AppliesTo;
				}
				else
				{
					// Not in the database and not in LogicMonior
					throw new InvalidOperationException($"Could not find DataSource '{configDataSource.Name}' in database or LogicMonitor.");
				}
			}

			// Get the Devices that match the ACTUAL AppliesTo on the DataSource
			var appliesToMatches = await GetAppliesToAsync(dsAppliesTo, cancellationToken).ConfigureAwait(false);

			if (!string.IsNullOrWhiteSpace(configDataSource.AppliesTo))
			{
				// Constrain the matches based on the AppliesTo in ** THE CONFIGURATION **

				var constrainedAppliesToMatches =
					await GetAppliesToAsync(
						configDataSource.AppliesTo,
						cancellationToken)
					.ConfigureAwait(false);

				databaseDeviceIds.AddRange(
					[.. appliesToMatches
						.Where(match => constrainedAppliesToMatches.Any(constrained => constrained.Id == match.Id))
						.Select(match => match.Id)
					]);

				// Go to the next one...
				continue;
			}

			// We use the original set of matches (because the DataSource configuration didn't specify one)
			databaseDeviceIds.AddRange(appliesToMatches.Select(match => match.Id));
		}

		return [.. databaseDeviceIds.Distinct().OrderBy(id => id)];
	}
}