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
						npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(50)
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
		string typeName = typeof(TApi).Name;
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
			await dbSet.AddOrUpdateIdentifiedItem(
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
				_logger.LogError(
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
				// Is it present in the API DataSource
				var apiDataPoint = apiDataSource
					.DataSourceDataPoints
					.SingleOrDefault(dp => dp.Name == configDataPoint.Name);
				if (apiDataPoint == null)
				{
					_logger.LogError(
						"For LogicMonitor instance '{LogicMonitorAccount}', DataSource '{DataSourceName}': could not find configured DataPoint '{ConfigDataPointName}'. Available DataPoints: {DataPoints}",
						_configuration.LogicMonitorClientOptions.Account,
						dataSourceName,
						configDataPoint.Name,
						string.Join(", ", apiDataSource.DataSourceDataPoints.Select(dp => dp.Name).OrderBy(dp => dp))
						);
					continue;
				}

				// Is it in the database?
				var databaseDataSourceDataPointModel = await context
					.DataSourceDataPoints
					.SingleOrDefaultAsync(dsdp => dsdp.DataSource.Name == apiDataSource.Name && dsdp.Name == configDataPoint.Name)
					.ConfigureAwait(false);
				if (databaseDataSourceDataPointModel == null)
				{
					// No. Add it to the database
					var dataSourceDataPointStoreItem = context.DataSourceDataPoints.Add(new DataSourceDataPointStoreItem
					{
						DataSource = databaseDataSource,
						Name = apiDataPoint.Name,
						Description = string.IsNullOrWhiteSpace(configDataPoint.Description)
							? configDataPoint.Description
							: apiDataPoint.Description,
						LogicMonitorId = apiDataPoint.Id,
						MeasurementUnit = configDataPoint.MeasurementUnit,
						GlobalAlertExpression = apiDataPoint.AlertExpression
					});

					_logger.LogInformation(
						"For LogicMonitor instance {LogicMonitorAccount}, for {DataSourceName}, added datapoint {ConfigDataPointName} to database.",
						_configuration.LogicMonitorClientOptions.Account,
						dataSourceName,
						configDataPoint.Name);
					await context
						.SaveChangesAsync()
						.ConfigureAwait(false);
				}
				// Update the measurement unit or Global Alert Expression?
				else if (
					databaseDataSourceDataPointModel.MeasurementUnit != configDataPoint.MeasurementUnit
					|| databaseDataSourceDataPointModel.GlobalAlertExpression != apiDataPoint.AlertExpression)
				{
					// Yes
					databaseDataSourceDataPointModel.MeasurementUnit = configDataPoint.MeasurementUnit;
					databaseDataSourceDataPointModel.GlobalAlertExpression = apiDataPoint.AlertExpression;
					await context
						.SaveChangesAsync()
						.ConfigureAwait(false);
				}
			}
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
		=> (groupName ?? throw new ArgumentNullException(nameof(groupName))).EndsWith("*", StringComparison.Ordinal)
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
		=> (groupName ?? throw new ArgumentNullException(nameof(groupName))).EndsWith("*", StringComparison.Ordinal)
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
				logger,
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

			// Fetch the DeviceDataSourceInstances
			var instanceFetchDateTimeUtc = DateTimeOffset.UtcNow;
			var apiDeviceDataSourceInstances = await GetAllDeviceDataSourceInstancesAsync(
					device.Id,
					deviceDataSource.Id,
					new(),
					cancellationToken
				)
				.ConfigureAwait(false);

			await context
				.SaveChangesAsync(cancellationToken)
				.ConfigureAwait(false);

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

				// Get the DeviceDataSourceInstanceDataPoints
				var deviceDataSourceInstanceDataPoints = await context
					.DeviceDataSourceInstanceDataPoints
					.Where(ddsidp => ddsidp.DeviceDataSourceInstanceId == databaseDeviceDataSourceInstance.Id)
					.ToListAsync(cancellationToken);

				foreach (var dataSourceDataPoint in dataSourceDataPoints.Where(dsdp => !deviceDataSourceInstanceDataPoints.Any(ddsidp => ddsidp.DeviceDataSourceInstanceId == databaseDeviceDataSourceInstance.Id && ddsidp.DataSourceDataPointId == dsdp.Id)))
				{
					context
					.DeviceDataSourceInstanceDataPoints
					.Add(new DeviceDataSourceInstanceDataPointStoreItem
					{
						DeviceDataSourceInstanceId = databaseDeviceDataSourceInstance.Id,
						DataSourceDataPointId = dataSourceDataPoint.Id
					});
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

	internal async Task<List<DataSourceDataPointStoreItem>> SyncDataSourceDataPointsAsync(
		DataSourceStoreItem dataSource,
		Context context,
		DataSourceConfigurationItem dataSourceSpecification,
		ILogger logger,
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
				.SingleOrDefaultAsync(dp => dp.Name == configDataSourceDataPoint.Name, cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			if (databaseDataPoint is null)
			{
				// Add it to the database
				databaseDataPoint = MapperInstance.Map<DataSourceDataPointStoreItem>(configDataSourceDataPoint);
				context.DataSourceDataPoints.Add(databaseDataPoint);
			}

			dataSourceDataPointStoreItems.Add(databaseDataPoint);
		}

		await context
			.SaveChangesAsync(cancellationToken)
			.ConfigureAwait(false);

		return dataSourceDataPointStoreItems;
	}
}