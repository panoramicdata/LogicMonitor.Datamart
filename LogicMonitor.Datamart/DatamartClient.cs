using PanoramicData.NCalcExtensions;

namespace LogicMonitor.Datamart;

public class DatamartClient : LogicMonitorClient
{
	internal DbContextOptions<Context> DbContextOptions { get; }

	internal DatabaseType DatabaseType => _configuration.DatabaseType;

	internal const string LogicMonitorCredentialNullMessage = "Either the configuration or some aspect of the LogicMonitorCredential is null";

	private const string ConnectionStringApplicationName = "LogicMonitor.Datamart";

	private readonly ILoggerFactory _loggerFactory;
	private readonly ILogger _logger;

	private readonly Configuration _configuration;
	private static readonly MapperConfiguration _mapperConfig = new(cfg => cfg.AddMaps(typeof(DatamartClient).Assembly));

	internal static IMapper MapperInstance = new Mapper(_mapperConfig);

	public DatamartClient(
		Configuration configuration,
		ILoggerFactory loggerFactory
		) : base((configuration ?? throw new ArgumentNullException(nameof(configuration))).LogicMonitorClientOptions)
	{
		// Store and validate configuration
		_configuration = configuration;
		_configuration.Validate();

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
						ConnectionString =
						$"server={configuration.DatabaseServerName};" +
						$"port={(configuration.DatabaseServerPort ?? 1433)};" +
						$"database={configuration.DatabaseName};" +
						(
							string.IsNullOrWhiteSpace(configuration.DatabaseUsername)
								? "Trusted_Connection=True;"
								: $"User Id={configuration.DatabaseUsername};Password={configuration.DatabasePassword}"
						) +
						$"Application Name={ConnectionStringApplicationName};" +
						"TrustServerCertificate=True"
					}.ConnectionString,
					opts => opts.CommandTimeout(configuration.SqlCommandTimeoutSeconds)
					);
				break;
			case DatabaseType.Postgres:
				dbContextOptionsBuilder
					.UseNpgsql(
						$"Server={configuration.DatabaseServerName};" +
						$"Port={(configuration.DatabaseServerPort ?? 5432)};" +
						$"Database={configuration.DatabaseName};" +
						$"Uid={configuration.DatabaseUsername};" +
						$"Pwd={configuration.DatabasePassword};",
						npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(5)
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
		using var context = new Context(DbContextOptions);
		return await context
			.Database
			.GetService<IRelationalDatabaseCreator>()
			.ExistsAsync(cancellationToken)
			.ConfigureAwait(false);
	}

	public async Task<bool> IsDatabaseSchemaUpToDateAsync(CancellationToken cancellationToken)
	{
		using var context = new Context(DbContextOptions);
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
		using var migrationsContext = new Context(DbContextOptions);

		_logger.LogInformation("Applying migrations to database...");
		await migrationsContext
			.Database
			.MigrateAsync(cancellationToken)
			.ConfigureAwait(false);

		_logger.LogInformation("Migrations up to date.");
	}

	public async Task EnsureDatabaseDeletedAsync(CancellationToken cancellationToken)
	{
		using var context = new Context(DbContextOptions);
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
			nameof(DeviceDataSourceStoreItem) => context.DeviceDataSources as DbSet<TStore>,
			nameof(DeviceStoreItem) => context.Devices as DbSet<TStore>,
			nameof(DeviceGroupStoreItem) => context.DeviceGroups as DbSet<TStore>,
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
		using var context = new Context(DbContextOptions);
		return !context.Database.IsSqlServer()
			? throw new NotSupportedException("Only SQL Server types support SQL queries.")
			: await Task.FromResult(GetDbSet<T>(context)
				.FromSqlRaw(sql)
				.ToList()).ConfigureAwait(false);
	}

	public async Task<TApi> GetCachedAsync<TApi>(int id, CancellationToken cancellationToken)
		where TApi : IdentifiedItem
	{
		using var context = new Context(DbContextOptions);
		var typeName = typeof(TApi).Name;
		switch (typeName)
		{
			case nameof(Device):
				var deviceStoreItem = await context
					.Devices
					.SingleOrDefaultAsync(i => i.LogicMonitorId == id, cancellationToken)
					.ConfigureAwait(false);
				var result = deviceStoreItem == null
					? throw new KeyNotFoundException($"Device with id {id} not found")
					: MapperInstance.Map<DeviceStoreItem, Device>(deviceStoreItem) as TApi;

				return result ?? throw new InvalidOperationException($"Could not convert {nameof(DeviceStoreItem)} to {nameof(Device)}");
			default:
				throw new NotSupportedException($"Getting cached {typeName} is not supported");
		}
	}

	public async Task<List<TApi>> GetAllCachedAsync<TApi>(CancellationToken cancellationToken)
		where TApi : class, IHasEndpoint, new()
	{
		using var context = new Context(DbContextOptions);
		var className = typeof(TApi).Name;
		switch (className)
		{
			case nameof(Alert):
				var alertStoreItems = await context
					.Alerts
					.ToListAsync(cancellationToken)
					.ConfigureAwait(false);
				return alertStoreItems
					.ConvertAll(a => MapperInstance.Map<AlertStoreItem, Alert>(a) as TApi);
			case nameof(CollectorGroup):
				var collectorGroupStoreItems = await context
					.CollectorGroups
					.ToListAsync(cancellationToken)
					.ConfigureAwait(false);
				return collectorGroupStoreItems
					.ConvertAll(cg => MapperInstance.Map<CollectorGroupStoreItem, CollectorGroup>(cg) as TApi);
			case nameof(DeviceGroup):
				var deviceGroupStoreItems = await context
					.DeviceGroups
					.ToListAsync(cancellationToken)
					.ConfigureAwait(false);
				return deviceGroupStoreItems
					.ConvertAll(dg => MapperInstance.Map<DeviceGroupStoreItem, DeviceGroup>(dg) as TApi);
			case nameof(WebsiteGroup):
				var websiteGroupStoreItems = await context
					.WebsiteGroups
					.ToListAsync(cancellationToken)
					.ConfigureAwait(false);
				return websiteGroupStoreItems
					.ConvertAll(wg => MapperInstance.Map<WebsiteGroupStoreItem, WebsiteGroup>(wg) as TApi);
			default:
				throw new NotSupportedException($"{className} not supported.  Add it to GetAllCachedAsync<T>().");
		}
	}

	public Task SyncDimensionsAsync(
		int desiredMaxIntervalMinutes,
		CancellationToken cancellationToken)
	{
		var sync = new DimensionSync(
			this,
			_configuration,
			_loggerFactory);
		return sync.LoopAsync(desiredMaxIntervalMinutes, cancellationToken);
	}

	public Task SyncDimensionsAsync(
		int desiredMaxIntervalMinutes,
		List<string> types,
		CancellationToken cancellationToken)
	{
		var sync = new DimensionSync(
			this,
			_configuration,
			types,
			_loggerFactory);
		return sync.LoopAsync(desiredMaxIntervalMinutes, cancellationToken);
	}

	public Task SyncLowResolutionDataAsync(
		int desiredMaxIntervalMinutes,
		CancellationToken cancellationToken)
	{
		var sync = new LowResolutionDataSync(
			this,
			_configuration,
			_loggerFactory);
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

	public Task PerformDataAgingAsync(
	int desiredMaxIntervalMinutes,
	int CountAggregationDaysToRetain,
	CancellationToken cancellationToken)
	{
		var sync = new DataAging(
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
		ILogger logger,
		CancellationToken cancellationToken)
		where TApi : IdentifiedItem, IHasEndpoint, new()
		where TStore : IdentifiedStoreItem
	{
		logger.LogDebug(
			"{TypeName}: Loading entries...",
			typeof(TApi).Name);

		using var context = new Context(DbContextOptions);
		// Get the right DbSet from the context
		var dbSet = action(context);

		// Fetch the items from the LogicMonitor API
		var lastObservedUtc = DateTimeOffset.UtcNow;
		var apiItems = await GetAllAsync<TApi>(cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		logger.LogDebug(
			"{TypeName}: Loaded {ApiItemsCount} items.",
			typeof(TApi).Name,
			apiItems.Count);

		// Add/update all the items
		foreach (var item in apiItems)
		{
			await dbSet.AddOrUpdateIdentifiedItemAsync(
				context,
				item,
				lastObservedUtc,
				logger,
				cancellationToken);
		}

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
		if (typeof(TStore) == typeof(DataSourceStoreItem))
		{
			await UpdateDataPointsAsync(context, apiItems.Cast<DataSource>().ToList())
				.ConfigureAwait(false);
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
		ILogger logger,
		CancellationToken cancellationToken)
	{
		logger.LogDebug("{TypeName}: Loading entries...", nameof(LogicModuleUpdateStoreItem));

		using var context = new Context(DbContextOptions);
		// Get the right DbSet from the context
		var dbSet = action(context);

		// Fetch the items from the LogicMonitor API
		var lastObservedUtc = DateTimeOffset.UtcNow;
		var apiItems = await GetLogicModuleUpdatesAsync(LogicModuleType.All, cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		logger.LogDebug(
			"{TypeName}: Loaded {ApiItemsCount} items.",
			nameof(LogicModuleUpdateStoreItem),
			apiItems.Items.Count);

		// Add/update all the items
		foreach (var item in apiItems.Items)
		{
			dbSet.AddOrUpdateLogicModuleUpdate(
				item,
				lastObservedUtc,
				logger);
		}

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
	}

	/// <summary>
	/// Update DataPoints, given a list of DataSources just retrieved from the LogicMonitor API
	/// </summary>
	/// <param name="context">The database context</param>
	/// <param name="apiDataSources">The list of API DataSources</param>
	private async Task UpdateDataPointsAsync(Context context, List<DataSource> apiDataSources)
	{
		// Update the nominated DataSources' DataPoints only for those reference in the config
		foreach (var configDataSourceSpecification in _configuration.DataSources)
		{
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
				.SingleOrDefaultAsync(ds => ds.Name == dataSourceName)
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
					.SingleOrDefaultAsync(dsdp => dsdp.DataSource.Name == apiDataSource.Name && dsdp.Name == configDataPoint.Name)
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
					.SaveChangesAsync()
					.ConfigureAwait(false);
			}

			// TODO - remove old DataPoints and associated data.
			// This will cascade delete, so may take a long time.
			// Consider adjusting the command timeout.
		}
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
		using var context = new Context(DbContextOptions);
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
					(a.MonitorObjectGroup0.MonitoredObjectType == a.MonitorObjectType && EF.Functions.Like(a.MonitorObjectGroup0.FullPath, likeString))
				|| (a.MonitorObjectGroup1.MonitoredObjectType == a.MonitorObjectType && EF.Functions.Like(a.MonitorObjectGroup1.FullPath, likeString))
				|| (a.MonitorObjectGroup2.MonitoredObjectType == a.MonitorObjectType && EF.Functions.Like(a.MonitorObjectGroup2.FullPath, likeString))
				|| (a.MonitorObjectGroup3.MonitoredObjectType == a.MonitorObjectType && EF.Functions.Like(a.MonitorObjectGroup3.FullPath, likeString))
				|| (a.MonitorObjectGroup4.MonitoredObjectType == a.MonitorObjectType && EF.Functions.Like(a.MonitorObjectGroup4.FullPath, likeString))
				|| (a.MonitorObjectGroup5.MonitoredObjectType == a.MonitorObjectType && EF.Functions.Like(a.MonitorObjectGroup5.FullPath, likeString))
				|| (a.MonitorObjectGroup6.MonitoredObjectType == a.MonitorObjectType && EF.Functions.Like(a.MonitorObjectGroup6.FullPath, likeString))
				|| (a.MonitorObjectGroup7.MonitoredObjectType == a.MonitorObjectType && EF.Functions.Like(a.MonitorObjectGroup7.FullPath, likeString))
				|| (a.MonitorObjectGroup8.MonitoredObjectType == a.MonitorObjectType && EF.Functions.Like(a.MonitorObjectGroup8.FullPath, likeString))
				|| (a.MonitorObjectGroup9.MonitoredObjectType == a.MonitorObjectType && EF.Functions.Like(a.MonitorObjectGroup9.FullPath, likeString))
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

	public static async Task<List<int>> GetAllCachedDeviceGroupIdsAsync(DbSet<DeviceGroupStoreItem> deviceGroups, string groupName)
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
		using var context = new Context(DbContextOptions);
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

	public async Task SyncDeviceDataSourcesAndInstancesAsync(
		DataSourceConfigurationItem dataSourceSpecification,
		ILogger logger,
		CancellationToken cancellationToken
	)
	{
		var dataSourceName = dataSourceSpecification.Name;

		using var context = new Context(DbContextOptions);

		var databaseDataSource = await context
			.DataSources
			.SingleOrDefaultAsync(ds => ds.Name == dataSourceName, cancellationToken)
			.ConfigureAwait(false) ?? throw new InvalidOperationException($"Could not find DataSource '{dataSourceName}' in database.");

		var dataSourceDataPoints = await SyncDataSourceDataPointsAsync(
				databaseDataSource,
				context,
				dataSourceSpecification,
				cancellationToken
			)
			.ConfigureAwait(false);

		// Get the Devices that match the appliesTo function on the DataSource
		var appliesToMatches = await GetAppliesToAsync(databaseDataSource.AppliesTo, cancellationToken)
			.ConfigureAwait(false);

		// Further constrain the appliesToMatches if requested
		if (!string.IsNullOrWhiteSpace(dataSourceSpecification.AppliesTo))
		{
			var requestedAppliesToMatches = await GetAppliesToAsync(dataSourceSpecification.AppliesTo, cancellationToken)
				.ConfigureAwait(false);

			appliesToMatches = appliesToMatches
				.Where(a => requestedAppliesToMatches.Any(r => r.Id == a.Id))
				.ToList();
		}

		logger.LogDebug(
			"Syncing {DataSourceName} instances for {AppliesToMatchesCount} devices",
			dataSourceName,
			appliesToMatches.Count);

		var instanceProperties = typeof(DeviceDataSourceInstance)
			.GetProperties();

		var markedMissing = 0;

		// Not all of these will have instances
		foreach (var appliesToMatch in appliesToMatches)
		{
			// Get the device
			var device = await GetAsync<Device>(appliesToMatch.Id, cancellationToken)
				.ConfigureAwait(false);

			// Get the DeviceDataSource
			var deviceDataSource = await GetDeviceDataSourceByDeviceIdAndDataSourceIdAsync(
					device.Id,
					databaseDataSource.LogicMonitorId,
					cancellationToken
				)
				.ConfigureAwait(false);
			if (deviceDataSource is null)
			{
				continue;
			}
			// We have a DeviceDataSource

			// Ensure that this DeviceDataSource exists in the database
			var deviceDataSourceStoreItem = await context
				.DeviceDataSources
				.Include(dds => dds.DataSource)
				.Include(dds => dds.Device)
				.SingleOrDefaultAsync(dds =>
						dds.Device!.LogicMonitorId == deviceDataSource.DeviceId
						&& dds.DataSource!.LogicMonitorId == deviceDataSource.DataSourceId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			var deviceStoreItem = await context
				.Devices
				.SingleOrDefaultAsync(d => d.LogicMonitorId == deviceDataSource.DeviceId, cancellationToken)
				.ConfigureAwait(false);

			var dataSourceStoreItem = await context
				.DataSources
				.SingleOrDefaultAsync(d => d.LogicMonitorId == deviceDataSource.DataSourceId, cancellationToken)
				.ConfigureAwait(false);

			if (deviceDataSourceStoreItem == null)
			{
				// Add it to the database
				deviceDataSourceStoreItem = MapperInstance.Map<DeviceDataSourceStoreItem>(deviceDataSource);
				context.DeviceDataSources.Add(deviceDataSourceStoreItem);
			}
			else
			{
				// Update the existing entry
				deviceDataSourceStoreItem = MapperInstance.Map(deviceDataSource, deviceDataSourceStoreItem);
			}

			deviceDataSourceStoreItem.DeviceId = deviceStoreItem!.Id;
			deviceDataSourceStoreItem.DataSourceId = dataSourceStoreItem!.Id;
			// It is now in the database context

			// Fetch the DeviceDataSourceInstances from the API
			var apiDeviceDataSourceInstances =
				await GetAllDeviceDataSourceInstancesAsync(
					device.Id,
					deviceDataSource.Id,
					new(),
					cancellationToken)
				.ConfigureAwait(false);

			var instanceObservedDateTimeUtc = DateTimeOffset.UtcNow;

			// Update the DatamartLastObserved BEFORE we remove instances that do not match
			foreach (var instance in apiDeviceDataSourceInstances)
			{
				if (await context
					.DeviceDataSourceInstances
					.SingleOrDefaultAsync(dddsi => dddsi.LogicMonitorId == instance.Id, cancellationToken: cancellationToken)
					.ConfigureAwait(false) is DeviceDataSourceInstanceStoreItem instanceStoreItem)
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
			if (!string.IsNullOrWhiteSpace(dataSourceSpecification.InstanceInclusionExpression) && dataSourceSpecification.InstanceInclusionExpression != "true")
			{
				var instanceInclusionExpression = new ExtendedExpression(dataSourceSpecification.InstanceInclusionExpression);

				apiDeviceDataSourceInstances = apiDeviceDataSourceInstances
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
							logger.LogError(e, "Error evaluating InstanceInclusionExpression '{InstanceInclusionExpression}' for {DeviceName} instance {InstanceName}", dataSourceSpecification.InstanceInclusionExpression, device.Name, i.Name);

							// Default to true
							return true;
						}
					})
					.ToList();
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
					databaseDeviceDataSourceInstance = MapperInstance.Map<DeviceDataSourceInstanceStoreItem>(apiDeviceDataSourceInstance);
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
				databaseDeviceDataSourceInstance.InstanceProperty1 = EvaluateProperty(dataSourceSpecification.InstanceProperty1, device, apiDeviceDataSourceInstance, logger);
				databaseDeviceDataSourceInstance.InstanceProperty2 = EvaluateProperty(dataSourceSpecification.InstanceProperty2, device, apiDeviceDataSourceInstance, logger);
				databaseDeviceDataSourceInstance.InstanceProperty3 = EvaluateProperty(dataSourceSpecification.InstanceProperty3, device, apiDeviceDataSourceInstance, logger);
				databaseDeviceDataSourceInstance.InstanceProperty4 = EvaluateProperty(dataSourceSpecification.InstanceProperty4, device, apiDeviceDataSourceInstance, logger);
				databaseDeviceDataSourceInstance.InstanceProperty5 = EvaluateProperty(dataSourceSpecification.InstanceProperty5, device, apiDeviceDataSourceInstance, logger);
				databaseDeviceDataSourceInstance.InstanceProperty6 = EvaluateProperty(dataSourceSpecification.InstanceProperty6, device, apiDeviceDataSourceInstance, logger);
				databaseDeviceDataSourceInstance.InstanceProperty7 = EvaluateProperty(dataSourceSpecification.InstanceProperty7, device, apiDeviceDataSourceInstance, logger);
				databaseDeviceDataSourceInstance.InstanceProperty8 = EvaluateProperty(dataSourceSpecification.InstanceProperty8, device, apiDeviceDataSourceInstance, logger);
				databaseDeviceDataSourceInstance.InstanceProperty9 = EvaluateProperty(dataSourceSpecification.InstanceProperty9, device, apiDeviceDataSourceInstance, logger);
				databaseDeviceDataSourceInstance.InstanceProperty10 = EvaluateProperty(dataSourceSpecification.InstanceProperty10, device, apiDeviceDataSourceInstance, logger);

				// Get the DeviceDataSourceInstanceDataPoints
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
						.Add(new DeviceDataSourceInstanceDataPointStoreItem
						{
							DeviceDataSourceInstanceId = databaseDeviceDataSourceInstance.Id,
							DataSourceDataPointId = dataSourceDataPointId,
							InstanceDatapointProperty1 = EvaluateProperty(dataSourceSpecification.DataPoints[index].InstanceDatapointProperty1, device, apiDeviceDataSourceInstance, logger),
							InstanceDatapointProperty2 = EvaluateProperty(dataSourceSpecification.DataPoints[index].InstanceDatapointProperty2, device, apiDeviceDataSourceInstance, logger),
							InstanceDatapointProperty3 = EvaluateProperty(dataSourceSpecification.DataPoints[index].InstanceDatapointProperty3, device, apiDeviceDataSourceInstance, logger),
							InstanceDatapointProperty4 = EvaluateProperty(dataSourceSpecification.DataPoints[index].InstanceDatapointProperty4, device, apiDeviceDataSourceInstance, logger),
							InstanceDatapointProperty5 = EvaluateProperty(dataSourceSpecification.DataPoints[index].InstanceDatapointProperty5, device, apiDeviceDataSourceInstance, logger),
							InstanceDatapointProperty6 = EvaluateProperty(dataSourceSpecification.DataPoints[index].InstanceDatapointProperty6, device, apiDeviceDataSourceInstance, logger),
							InstanceDatapointProperty7 = EvaluateProperty(dataSourceSpecification.DataPoints[index].InstanceDatapointProperty7, device, apiDeviceDataSourceInstance, logger),
							InstanceDatapointProperty8 = EvaluateProperty(dataSourceSpecification.DataPoints[index].InstanceDatapointProperty8, device, apiDeviceDataSourceInstance, logger),
							InstanceDatapointProperty9 = EvaluateProperty(dataSourceSpecification.DataPoints[index].InstanceDatapointProperty9, device, apiDeviceDataSourceInstance, logger),
							InstanceDatapointProperty10 = EvaluateProperty(dataSourceSpecification.DataPoints[index].InstanceDatapointProperty10, device, apiDeviceDataSourceInstance, logger)
						});
					}
					else
					{
						if (deviceDataSourceInstanceDataPoints.SingleOrDefault(
							ddsidp => ddsidp.DeviceDataSourceInstanceId == databaseDeviceDataSourceInstance.Id && ddsidp.DataSourceDataPointId == dataSourceDataPointId)
							is DeviceDataSourceInstanceDataPointStoreItem ddsipsi)
						{
							ddsipsi.InstanceDatapointProperty1 = EvaluateProperty(dataSourceSpecification.DataPoints[index].InstanceDatapointProperty1, device, apiDeviceDataSourceInstance, logger);
							ddsipsi.InstanceDatapointProperty2 = EvaluateProperty(dataSourceSpecification.DataPoints[index].InstanceDatapointProperty2, device, apiDeviceDataSourceInstance, logger);
							ddsipsi.InstanceDatapointProperty3 = EvaluateProperty(dataSourceSpecification.DataPoints[index].InstanceDatapointProperty3, device, apiDeviceDataSourceInstance, logger);
							ddsipsi.InstanceDatapointProperty4 = EvaluateProperty(dataSourceSpecification.DataPoints[index].InstanceDatapointProperty4, device, apiDeviceDataSourceInstance, logger);
							ddsipsi.InstanceDatapointProperty5 = EvaluateProperty(dataSourceSpecification.DataPoints[index].InstanceDatapointProperty5, device, apiDeviceDataSourceInstance, logger);
							ddsipsi.InstanceDatapointProperty6 = EvaluateProperty(dataSourceSpecification.DataPoints[index].InstanceDatapointProperty6, device, apiDeviceDataSourceInstance, logger);
							ddsipsi.InstanceDatapointProperty7 = EvaluateProperty(dataSourceSpecification.DataPoints[index].InstanceDatapointProperty7, device, apiDeviceDataSourceInstance, logger);
							ddsipsi.InstanceDatapointProperty8 = EvaluateProperty(dataSourceSpecification.DataPoints[index].InstanceDatapointProperty8, device, apiDeviceDataSourceInstance, logger);
							ddsipsi.InstanceDatapointProperty9 = EvaluateProperty(dataSourceSpecification.DataPoints[index].InstanceDatapointProperty9, device, apiDeviceDataSourceInstance, logger);
							ddsipsi.InstanceDatapointProperty10 = EvaluateProperty(dataSourceSpecification.DataPoints[index].InstanceDatapointProperty10, device, apiDeviceDataSourceInstance, logger);
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
					.Where(ddsi => ddsi.DeviceDataSource!.Device!.LogicMonitorId == device.Id && ddsi.DeviceDataSource!.DataSourceId == databaseDataSource.Id && ddsi.LastWentMissing == null)
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

					databaseDeviceDataSourceInstance.LastWentMissing = DateTimeOffset.UtcNow;
				}

				markedMissing += deviceDatasourceInstanceIdsToMarkMissing.Count;
			}

			// Check for any that were NOT in the entries that came back from the API
		}

		var added = context.ChangeTracker.Entries().Count(e => e.State == EntityState.Added);
		var modified = context.ChangeTracker.Entries().Count(e => e.State == EntityState.Modified);
		var total = context.ChangeTracker.Entries().Count();
		var rowsAffected = await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		logger.LogInformation(
			"Sync completed for {DataSourceName}; Total {Total}; Added {Added}; Modified {Modified} ({MarkedMissing:N0} MarkedMissing).",
			dataSourceName,
			total,
			added,
			modified,
			markedMissing);
	}

	private static string EvaluateProperty(string condition, Device device, DeviceDataSourceInstance ddsi, ILogger logger)
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
				ddsi.DeviceDisplayName,
				ddsi.DisplayName,
				e.Message);

			// Default to true
			return string.Empty;
		}
	}

	internal async Task<List<DataSourceDataPointStoreItem>> SyncDataSourceDataPointsAsync(
		DataSourceStoreItem dataSource,
		Context context,
		DataSourceConfigurationItem dataSourceSpecification,
		CancellationToken cancellationToken
	)
	{
		var configDataSourceDataPoints = dataSourceSpecification.DataPoints;

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

			// Only update the description if it is not null or whitespace
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
}