using AutoMapper;
using LogicMonitor.Api;
using LogicMonitor.Api.Alerts;
using LogicMonitor.Api.Collectors;
using LogicMonitor.Api.Devices;
using LogicMonitor.Api.Filters;
using LogicMonitor.Api.LogicModules;
using LogicMonitor.Api.Websites;
using LogicMonitor.Datamart.Config;
using LogicMonitor.Datamart.Extensions;
using LogicMonitor.Datamart.Mapping;
using LogicMonitor.Datamart.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LogicMonitor.Datamart
{
	public class DatamartClient : LogicMonitorClient
	{
		internal DbContextOptions<Context> DbContextOptions { get; }

		internal DatabaseType DatabaseType => _configuration.DatabaseType;

		internal const string LogicMonitorCredentialNullMessage = "Either the configuration or some aspect of the LogicMonitorCredential is null";

		private const string ConnectionStringApplicationName = "LogicMonitor.Datamart";

		private readonly ILoggerFactory _loggerFactory;
		private readonly ILogger _logger;

		private readonly Configuration _configuration;
		private static readonly MapperConfiguration _mapperConfig = new MapperConfiguration(cfg => cfg.AddMaps(typeof(DatamartClient).Assembly));

		internal static IMapper MapperInstance = new Mapper(_mapperConfig);

		public DatamartClient(
			Configuration configuration,
			ILoggerFactory loggerFactory
			) : base(new LogicMonitorClientOptions
			{
				AccessId = configuration?.LogicMonitorCredential?.AccessId ?? throw new ArgumentNullException(nameof(configuration), LogicMonitorCredentialNullMessage),
				AccessKey = configuration?.LogicMonitorCredential?.AccessKey ?? throw new ArgumentNullException(nameof(configuration), LogicMonitorCredentialNullMessage),
				Account = configuration?.LogicMonitorCredential?.Subdomain ?? throw new ArgumentNullException(nameof(configuration), LogicMonitorCredentialNullMessage),
				Logger = loggerFactory.CreateLogger<LogicMonitorClient>()
			})
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
							$"database={configuration.DatabaseName};" +
							"Trusted_Connection=True;" +
							$"Application Name={ConnectionStringApplicationName}"
						}.ConnectionString,
						opts => opts.CommandTimeout(configuration.SqlCommandTimeoutSeconds)
						);
					break;
				case DatabaseType.Postgres:
					dbContextOptionsBuilder
						.UseNpgsql(
							$"Host={configuration.DatabaseServerName};" +
							$"Database={configuration.DatabaseName};" +
							$"Username={configuration.DatabaseUsername};" +
							$"Password={configuration.DatabasePassword};" +
							$"ApplicationName={ConnectionStringApplicationName}",
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

		public async Task<bool> IsDatabaseCreatedAsync(CancellationToken cancellationToken = default)
		{
			using (var context = new Context(DbContextOptions))
			{
				return await context
					.Database
					.GetService<IRelationalDatabaseCreator>()
					.ExistsAsync(cancellationToken)
					.ConfigureAwait(false);
			}
		}

		public async Task<bool> IsDatabaseSchemaUpToDateAsync(CancellationToken cancellationToken = default)
		{
			using (var context = new Context(DbContextOptions))
			{
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
		}

		public async Task EnsureDatabaseCreatedAndSchemaUpdatedAsync(CancellationToken cancellationToken = default)
		{
			using (var context = new Context(DbContextOptions))
			{
				_logger.LogInformation($"Applying migrations as appropriate to database...");
				await context
					.Database
					.MigrateAsync(cancellationToken)
					.ConfigureAwait(false);
				_logger.LogInformation("Migrations up to date.");
			}
		}

		public async Task EnsureDatabaseDeletedAsync(CancellationToken cancellationToken = default)
		{
			using (var context = new Context(DbContextOptions))
			{
				var dbConnection = context.Database.GetDbConnection();
				_logger.LogInformation($"Deleting database {dbConnection.Database} on {dbConnection.DataSource}...");
				await context
					.Database
					.EnsureDeletedAsync(cancellationToken)
					.ConfigureAwait(false);
				_logger.LogInformation($"Deleted database {dbConnection.Database} on {dbConnection.DataSource}...");
			}
		}

		private DbSet<TStore> GetDbSet<TStore>(Context context)
		where TStore : class, new()
		{
			switch (typeof(TStore).Name)
			{
				case nameof(AlertStoreItem):
					return context.Alerts as DbSet<TStore>;
				case nameof(AlertRuleStoreItem):
					return context.AlertRules as DbSet<TStore>;
				case nameof(CollectorStoreItem):
					return context.Collectors as DbSet<TStore>;
				case nameof(CollectorGroupStoreItem):
					return context.CollectorGroups as DbSet<TStore>;
				case nameof(DeviceDataSourceStoreItem):
					return context.DeviceDataSources as DbSet<TStore>;
				case nameof(DeviceStoreItem):
					return context.Devices as DbSet<TStore>;
				case nameof(DeviceGroupStoreItem):
					return context.DeviceGroups as DbSet<TStore>;
				case nameof(ConfigSourceStoreItem):
					return context.ConfigSources as DbSet<TStore>;
				case nameof(DataSourceStoreItem):
					return context.DataSources as DbSet<TStore>;
				case nameof(EscalationChainStoreItem):
					return context.EscalationChains as DbSet<TStore>;
				case nameof(EventSourceStoreItem):
					return context.EventSources as DbSet<TStore>;
				case nameof(WebsiteGroupStoreItem):
					return context.WebsiteGroups as DbSet<TStore>;
				case nameof(WebsiteStoreItem):
					return context.Websites as DbSet<TStore>;
				default:
					throw new NotSupportedException();
			}
		}

		public async Task<List<T>> SqlListQuery<T>(string sql) where T : class, IHasEndpoint, new()
		{
			using (var context = new Context(DbContextOptions))
			{
				if (!context.Database.IsSqlServer())
				{
					throw new NotSupportedException("Only SQL Server types support SQL queries.");
				}
				return await Task.FromResult(GetDbSet<T>(context)
					.FromSql(sql)
					.ToList()).ConfigureAwait(false);
			}
		}

		public async Task<TApi> GetCachedAsync<TApi>(int id, CancellationToken cancellationToken = default)
			where TApi : IdentifiedItem
		{
			using (var context = new Context(DbContextOptions))
			{
				switch (typeof(TApi).Name)
				{
					case nameof(Device):
						var deviceStoreItem = await context
							.Devices
							.SingleOrDefaultAsync(i => i.Id == id, cancellationToken)
							.ConfigureAwait(false);
						return deviceStoreItem == null
							? null
							: DatamartClient.MapperInstance.Map<DeviceStoreItem, Device>(deviceStoreItem) as TApi;
					default:
						throw new NotSupportedException();
				}
			}
		}

		public async Task<List<TApi>> GetAllCachedAsync<TApi>(CancellationToken cancellationToken = default)
			where TApi : class, IHasEndpoint, new()
		{
			using (var context = new Context(DbContextOptions))
			{
				var className = typeof(TApi).Name;
				switch (className)
				{
					case nameof(Alert):
						var alertStoreItems = await context
							.Alerts
							.ToListAsync(cancellationToken)
							.ConfigureAwait(false);
						return alertStoreItems
							.Select(a => DatamartClient.MapperInstance.Map<AlertStoreItem, Alert>(a) as TApi)
							.ToList();
					case nameof(CollectorGroup):
						var collectorGroupStoreItems = await context
							.CollectorGroups
							.ToListAsync(cancellationToken)
							.ConfigureAwait(false);
						return collectorGroupStoreItems
							.Select(cg => DatamartClient.MapperInstance.Map<CollectorGroupStoreItem, CollectorGroup>(cg) as TApi)
							.ToList();
					case nameof(DeviceGroup):
						var deviceGroupStoreItems = await context
							.DeviceGroups
							.ToListAsync(cancellationToken)
							.ConfigureAwait(false);
						return deviceGroupStoreItems
							.Select(dg => DatamartClient.MapperInstance.Map<DeviceGroupStoreItem, DeviceGroup>(dg) as TApi)
							.ToList();
					case nameof(WebsiteGroup):
						var websiteGroupStoreItems = await context
							.WebsiteGroups
							.ToListAsync(cancellationToken)
							.ConfigureAwait(false);
						return websiteGroupStoreItems
							.Select(wg => DatamartClient.MapperInstance.Map<WebsiteGroupStoreItem, WebsiteGroup>(wg) as TApi)
							.ToList();
					default:
						throw new NotSupportedException($"{className} not supported.  Add it to GetAllCachedAsync<T>().");
				}
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

		public Task SyncDataAsync(
			int desiredMaxIntervalMinutes,
			CancellationToken cancellationToken)
		{
			var sync = new DataSync(
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
		/// Add or Update the Database using the items already retreived from the LogicMonitor API
		/// </summary>
		/// <typeparam name="TApi">The LogicMonitor API type</typeparam>
		/// <typeparam name="TStore">The Database StoreItem type</typeparam>
		/// <param name="action"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		internal async Task AddOrUpdate<TApi, TStore>(
			Func<Context, DbSet<TStore>> action,
			ILogger logger,
			CancellationToken cancellationToken)
			where TApi : IdentifiedItem, IHasEndpoint, new()
			where TStore : IdentifiedStoreItem
		{
			using (var context = new Context(DbContextOptions))
			{
				// Get the right DbSet from the context
				var dbSet = action(context);

				// Fetch the items from the LogicMonitor API
				var lastObservedUtc = DateTime.UtcNow;
				var apiItems = await GetAllAsync<TApi>(cancellationToken: cancellationToken)
					.ConfigureAwait(false);
				logger.LogDebug($"{typeof(TApi).Name}: Loaded {apiItems.Count} items.");

				// Add/update all the items
				foreach (var item in apiItems)
				{
					dbSet.AddOrUpdateIdentifiedItem(item, lastObservedUtc, logger);
				}

				// Calculate and log the stats
				var added = context.ChangeTracker.Entries().Count(e => e.State == EntityState.Added);
				var modified = context.ChangeTracker.Entries().Count(e => e.State == EntityState.Modified);
				var total = context.ChangeTracker.Entries().Count();
				var affectedRowCount = await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
				logger.LogInformation($"{typeof(TApi).Name}: Total {total}; Added {added}; Modified {modified}.");

				// For DataPoints, the information from LogicMonitor is present on the DataSources.
				// So, after fetching the DataSources, we should also update the DataPoints in the database
				if (typeof(TStore) == typeof(DataSourceStoreItem))
				{
					await UpdateDataPointsAsync(context, apiItems.Cast<DataSource>().ToList());
				}
			}
		}

		/// <summary>
		/// Update Datapoints, given a list of DataSources just retrieved from the LogicMonitor API
		/// </summary>
		/// <param name="context">The database context</param>
		/// <param name="apiDataSources">The list of API DataSources</param>
		/// <returns></returns>
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
				if (apiDataSource == null)
				{
					// May not happen if the config references a non-existent DataSource
					_logger.LogError($"For LogicMonitor instance {_configuration.LogicMonitorCredential.Subdomain}, expected to find LogicMonitor API DataSource called '{dataSourceName}', but it was missing.");
					continue;
				}

				// The DataSource from the database
				var databaseDataSource = await context
					.DataSources
					.SingleOrDefaultAsync(ds => ds.Name == dataSourceName);
				if (apiDataSource == null)
				{
					// Should not happen, as we have only just updated the database with DataSources
					_logger.LogError($"For LogicMonitor instance {_configuration.LogicMonitorCredential.Subdomain}, expected to find Database DataSource called '{dataSourceName}', but it was missing.");
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
							$"For LogicMonitor instance '{_configuration.LogicMonitorCredential.Subdomain}', DataSource '{dataSourceName}': " +
							$"could not find configured DataPoint '{configDataPoint.Name}'. " +
							$"Available DataPoints: {string.Join(", ", apiDataSource.DataSourceDataPoints.Select(dp => dp.Name).OrderBy(dp => dp))}");
						continue;
					}

					// Is it in the database?
					var databaseDataSourceDataPointModel = await context
						.DataSourceDataPoints
						.SingleOrDefaultAsync(dsdp => dsdp.DataSource.Name == apiDataSource.Name && dsdp.Name == configDataPoint.Name);
					if (databaseDataSourceDataPointModel == null)
					{
						// No. Add it to the database
						var dataSourceDataPointStoreItem = context.DataSourceDataPoints.Add(new DataSourceDataPointStoreItem
						{
							DataSource = databaseDataSource,
							Name = apiDataPoint.Name,
							Description = apiDataPoint.Description,
							Id = apiDataPoint.Id,
							MeasurementUnit = configDataPoint.MeasurementUnit
						});

						_logger.LogInformation($"For LogicMonitor instance {_configuration.LogicMonitorCredential.Subdomain}, for {dataSourceName}, added datapoint {configDataPoint.Name} to database.");
						await context
							.SaveChangesAsync()
							.ConfigureAwait(false);
					}
					// Update the measurement unit?
					else if (databaseDataSourceDataPointModel.MeasurementUnit != configDataPoint.MeasurementUnit)
					{
						// Yes
						databaseDataSourceDataPointModel.MeasurementUnit = configDataPoint.MeasurementUnit;
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
			string id,
			AckFilter ackFilter,
			List<string> monitorObjectGroups,
			string monitorObjectName,
			int? monitorObjectId,
			List<AlertType> alertTypes,
			string resourceTemplateDisplayName,
			int? resourceTemplateId,
			string instanceName,
			string dataPointName,
			List<AlertLevel> alertLevels,
			string orderBy,
			OrderDirection orderDirection,
			SdtFilter sdtFilter,
			string problemSignature,
			bool isPercentageAvailability = false
			)
		{
			using (var context = new Context(DbContextOptions))
			{
				var queryable = context.Alerts.AsQueryable();

				if (id != null)
				{
					queryable = queryable.Where(a => a.Id == id);
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
				var monitorObjectGroup = monitorObjectGroups?[0];
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
					switch ($"{orderBy}/{orderDirection}")
					{
						case "StartOnSeconds/Asc":
							queryable = queryable.OrderBy(a => a.StartOnSeconds);
							break;
						case "StartOnSeconds/Desc":
							queryable = queryable.OrderByDescending(a => a.StartOnSeconds);
							break;
						default:
							throw new NotSupportedException("Only orderBy=StartOnSeconds currently supported.");
					}
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
					.Select(DatamartClient.MapperInstance.Map<AlertStoreItem, Alert>)
					.ToList();
			}
		}

		public static async Task<List<int>> GetAllCachedCollectorGroupIdsAsync(DbSet<CollectorGroupStoreItem> collectorGroups, string groupName)
			=> await collectorGroups
				.Where(cg => cg.Name == groupName)
				.Select(cg => cg.Id)
				.ToListAsync()
				.ConfigureAwait(false);

		public static async Task<List<int>> GetAllCachedDeviceGroupIdsAsync(DbSet<DeviceGroupStoreItem> deviceGroups, string groupName)
			=> groupName.EndsWith("*", StringComparison.Ordinal)
				? await deviceGroups
					.Where(dg => dg.FullPath.StartsWith(groupName.TrimEnd('*'), StringComparison.Ordinal))
					.Select(dg => dg.Id)
					.ToListAsync()
					.ConfigureAwait(false)
				: await deviceGroups
					.Where(dg => dg.FullPath == groupName)
					.Select(dg => dg.Id)
					.ToListAsync()
					.ConfigureAwait(false);

		public static async Task<List<int>> GetAllCachedWebsiteGroupIdsAsync(DbSet<WebsiteGroupStoreItem> websiteGroups, string groupName)
			=> groupName.EndsWith("*", StringComparison.Ordinal)
				? await websiteGroups
					.Where(wg => wg.FullPath.StartsWith(groupName.TrimEnd('*'), StringComparison.Ordinal))
					.Select(wg => wg.Id)
					.ToListAsync()
					.ConfigureAwait(false)
				: await websiteGroups
					.Where(wg => wg.FullPath == groupName)
					.Select(wg => wg.Id)
					.ToListAsync()
					.ConfigureAwait(false);

		internal Task<List<string>> GetAggregationTablesAsync()
				 => AggregationWriter.GetTablesAsync(DbContextOptions);

		internal Task DropAggregationTableAsync(DateTimeOffset testAggregationPeriod)
			=> AggregationWriter.DropTableAsync(DbContextOptions, testAggregationPeriod, _logger);

		internal Task AgeAggregationTablesAsync(int countAggregationDaysToRetain)
			=> AggregationWriter.PerformAgingAsync(DbContextOptions, _configuration.SqlCommandTimeoutSeconds, countAggregationDaysToRetain, _logger);

		internal async Task<string> EnsureTableExistsAsync(DateTimeOffset testAggregationPeriod)
		{
			using (var context = new Context(DbContextOptions))
			using (var sqlConnection = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
			{
				await sqlConnection.OpenAsync();
				var tableName = await AggregationWriter.EnsureTableExistsAsync(sqlConnection, _configuration.SqlCommandTimeoutSeconds, testAggregationPeriod);
				sqlConnection.Close();
				return tableName;
			}
		}
	}
}