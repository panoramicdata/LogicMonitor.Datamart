using EFCore.BulkExtensions;

namespace LogicMonitor.Datamart;

/// <summary>
/// The Entity Framework database context for the LogicMonitor datamart.
/// </summary>
public class Context : DbContext
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Context"/> class. Required for adding migrations.
	/// </summary>
	public Context()
	{
		// Required for adding migrations
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Context"/> class with the specified options.
	/// </summary>
	/// <param name="options">The database context options.</param>
	public Context(DbContextOptions<Context> options) : base(options)
	{
	}

	/// <summary>
	/// Gets or sets the alerts DbSet.
	/// </summary>
	public DbSet<AlertStoreItem> Alerts { get; set; } = null!;

	/// <summary>
	/// Gets or sets the alert rules DbSet.
	/// </summary>
	public DbSet<AlertRuleStoreItem> AlertRules { get; set; } = null!;

	/// <summary>
	/// Gets or sets the audit events DbSet.
	/// </summary>
	public DbSet<AuditEventStoreItem> AuditEvents { get; set; } = null!;

	/// <summary>
	/// Gets or sets the collectors DbSet.
	/// </summary>
	public DbSet<CollectorStoreItem> Collectors { get; set; } = null!;

	/// <summary>
	/// Gets or sets the collector groups DbSet.
	/// </summary>
	public DbSet<CollectorGroupStoreItem> CollectorGroups { get; set; } = null!;

	/// <summary>
	/// Gets or sets the ConfigSources DbSet.
	/// </summary>
	public DbSet<ConfigSourceStoreItem> ConfigSources { get; set; } = null!;

	/// <summary>
	/// Gets or sets the DataSource graphs DbSet.
	/// </summary>
	public DbSet<DataSourceGraphStoreItem> DataSourceGraphs { get; set; } = null!;

	/// <summary>
	/// Gets or sets the DataSources DbSet.
	/// </summary>
	public DbSet<DataSourceStoreItem> DataSources { get; set; } = null!;

	/// <summary>
	/// Gets or sets the DataSource DataPoints DbSet.
	/// </summary>
	public DbSet<DataSourceDataPointStoreItem> DataSourceDataPoints { get; set; } = null!;

	/// <summary>
	/// Gets or sets the integrations DbSet.
	/// </summary>
	public DbSet<IntegrationStoreItem> Integrations { get; set; } = null!;

	/// <summary>
	/// Gets or sets the resource-ConfigSource assignments DbSet.
	/// </summary>
	public DbSet<ResourceConfigSourceStoreItem> DeviceConfigSources { get; set; } = null!;

	/// <summary>
	/// Gets or sets the resource-ConfigSource instances DbSet.
	/// </summary>
	public DbSet<ResourceConfigSourceInstanceStoreItem> DeviceConfigSourceInstances { get; set; } = null!;

	/// <summary>
	/// Gets or sets the resource-ConfigSource instance configuration snapshots DbSet.
	/// </summary>
	public DbSet<ResourceConfigSourceInstanceConfigStoreItem> DeviceConfigSourceInstanceConfigs { get; set; } = null!;

	/// <summary>
	/// Gets or sets the resource-DataSource assignments DbSet.
	/// </summary>
	public DbSet<ResourceDataSourceStoreItem> DeviceDataSources { get; set; } = null!;

	/// <summary>
	/// Gets or sets the resource-DataSource instances DbSet.
	/// </summary>
	public DbSet<ResourceDataSourceInstanceStoreItem> DeviceDataSourceInstances { get; set; } = null!;

	/// <summary>
	/// Gets or sets the escalation chains DbSet.
	/// </summary>
	public DbSet<EscalationChainStoreItem> EscalationChains { get; set; } = null!;

	/// <summary>
	/// Gets or sets the EventSources DbSet.
	/// </summary>
	public DbSet<EventSourceStoreItem> EventSources { get; set; } = null!;

	/// <summary>
	/// Gets or sets the LogicModule updates DbSet.
	/// </summary>
	public DbSet<LogicModuleUpdateStoreItem> LogicModuleUpdates { get; set; } = null!;

	/// <summary>
	/// Gets or sets the log items DbSet.
	/// </summary>
	public DbSet<LogStoreItem> LogItems { get; set; } = null!;

	/// <summary>
	/// Gets or sets the websites DbSet.
	/// </summary>
	public DbSet<WebsiteStoreItem> Websites { get; set; } = null!;

	/// <summary>
	/// Gets or sets the resource-DataSource instance DataPoints DbSet.
	/// </summary>
	public DbSet<ResourceDataSourceInstanceDataPointStoreItem> DeviceDataSourceInstanceDataPoints { get; set; } = null!;

	/// <summary>
	/// Gets or sets the resources (devices) DbSet.
	/// </summary>
	public DbSet<ResourceStoreItem> Devices { get; set; } = null!;

	/// <summary>
	/// Gets or sets the resource groups DbSet.
	/// </summary>
	public DbSet<ResourceGroupStoreItem> DeviceGroups { get; set; } = null!;

	/// <summary>
	/// Gets or sets the website groups DbSet.
	/// </summary>
	public DbSet<WebsiteGroupStoreItem> WebsiteGroups { get; set; } = null!;

	/// <summary>
	/// Gets or sets the monitor object groups DbSet.
	/// </summary>
	public DbSet<MonitorObjectGroupStoreItem> MonitorObjectGroups { get; set; } = null!;

	/// <summary>
	/// Gets or sets the time series data aggregations DbSet.
	/// </summary>
	public DbSet<TimeSeriesDataAggregationStoreItem> TimeSeriesDataAggregations { get; set; } = null!;

	/// <inheritdoc />
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		// MonitorObjectGroup indexes
		var monitorObjectGroups = modelBuilder.Entity<MonitorObjectGroupStoreItem>();
		monitorObjectGroups.HasIndex(g => new { g.FullPath, g.MonitoredObjectType });

		// Alert indexes
		var alerts = modelBuilder.Entity<AlertStoreItem>();
		alerts.HasIndex(a => a.InternalId);
		alerts.HasIndex(a => a.Id);
		alerts.HasIndex(a => a.MonitorObjectId);
		alerts.HasIndex(a => a.MonitorObjectName);
		alerts.HasIndex(a => a.MonitorObjectType);
		alerts.HasIndex(a => a.MonitorObjectGroup0Id);
		alerts.HasIndex(a => a.MonitorObjectGroup1Id);
		alerts.HasIndex(a => a.MonitorObjectGroup2Id);
		alerts.HasIndex(a => a.MonitorObjectGroup3Id);
		alerts.HasIndex(a => a.MonitorObjectGroup4Id);
		alerts.HasIndex(a => a.MonitorObjectGroup5Id);
		alerts.HasIndex(a => a.MonitorObjectGroup6Id);
		alerts.HasIndex(a => a.MonitorObjectGroup7Id);
		alerts.HasIndex(a => a.MonitorObjectGroup8Id);
		alerts.HasIndex(a => a.MonitorObjectGroup9Id);
		alerts.HasIndex(a => a.DataPointId);
		alerts.HasIndex(a => a.DataPointName);
		alerts.HasIndex(a => a.InstanceId);
		alerts.HasIndex(a => a.InstanceName);
		alerts.HasIndex(a => a.Severity);
		alerts.HasIndex(a => a.IsCleared);
		alerts.HasIndex(a => a.ResourceId);
		alerts.HasIndex(a => a.ResourceTemplateId);
		alerts.HasIndex(a => a.ResourceTemplateName);
		alerts.HasIndex(a => a.ResourceTemplateType);
		alerts.HasIndex(a => a.StartOnSeconds);
		alerts.HasIndex(a => a.EndOnSeconds);

		var alertIndexBuilder = alerts
			.HasIndex(a => new
			{
				a.StartOnSeconds,
				a.EndOnSeconds,
				a.IsCleared,
				a.InScheduledDownTime,
				a.MonitorObjectGroup0Id,
				a.MonitorObjectGroup1Id,
				a.MonitorObjectGroup2Id,
				a.MonitorObjectGroup3Id,
				a.MonitorObjectGroup4Id,
				a.MonitorObjectGroup5Id,
				a.MonitorObjectGroup6Id,
				a.MonitorObjectGroup7Id,
				a.MonitorObjectGroup8Id,
				a.MonitorObjectGroup9Id,
			});

		if (Database.IsNpgsql())
		{
			NpgsqlIndexBuilderExtensions.IncludeProperties(alertIndexBuilder,
					nameof(AlertStoreItem.Id),
					nameof(AlertStoreItem.Severity),
					nameof(AlertStoreItem.ClearValue),
					nameof(AlertStoreItem.MonitorObjectId),
					nameof(AlertStoreItem.ResourceTemplateName),
					nameof(AlertStoreItem.InstanceId),
					nameof(AlertStoreItem.InstanceName)
				)
			.HasDatabaseName($"IX_{nameof(Alerts)}_FasterPercentageAvailability");
		}

		if (Database.IsSqlServer())
		{
			SqlServerIndexBuilderExtensions.IncludeProperties(alertIndexBuilder,
					nameof(AlertStoreItem.Id),
					nameof(AlertStoreItem.Severity),
					nameof(AlertStoreItem.ClearValue),
					nameof(AlertStoreItem.MonitorObjectId),
					nameof(AlertStoreItem.ResourceTemplateName),
					nameof(AlertStoreItem.InstanceId),
					nameof(AlertStoreItem.InstanceName)
				)
			.HasDatabaseName($"IX_{nameof(Alerts)}_FasterPercentageAvailability");
		}

		modelBuilder.Entity<AlertStoreItem>()
			.HasOne(a => a.AlertRule)
			.WithMany(ar => ar.AlertStoreItems)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<ResourceConfigSourceInstanceConfigStoreItem>()
			.HasIndex(rds => rds.LogicMonitorStringId);

		modelBuilder.Entity<ResourceConfigSourceInstanceConfigStoreItem>()
			.HasOne(rds => rds.DeviceConfigSourceInstance)
			.WithMany(rds => rds.DeviceConfigSourceInstanceConfigs)
			.OnDelete(DeleteBehavior.Restrict);

		// DeviceDataSourceInstance indexes
		modelBuilder.Entity<ResourceDataSourceInstanceStoreItem>()
			.HasIndex(ddsi => ddsi.LastWentMissing);

		// Relational stuff
		modelBuilder.Entity<ResourceDataSourceInstanceStoreItem>()
			.HasOne(ddsi => ddsi.DeviceDataSource)
			.WithMany(ds => ds.DeviceDataSourceInstances)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<ResourceDataSourceInstanceStoreItem>()
			.HasOne(ddsi => ddsi.DeviceDataSource)
			.WithMany(d => d.DeviceDataSourceInstances)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<ResourceDataSourceStoreItem>()
			.HasOne(dds => dds.DataSource)
			.WithMany(d => d.DeviceDataSources)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<ResourceDataSourceStoreItem>()
			.HasOne(dds => dds.Device)
			.WithMany(d => d.DeviceDataSources)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<AlertStoreItem>()
			.HasOne(a => a.MonitorObjectGroup0)
			.WithMany(mog => mog.AlertsFromGroup0)
			.OnDelete(DeleteBehavior.Restrict);
		modelBuilder.Entity<AlertStoreItem>()
			.HasOne(a => a.MonitorObjectGroup1)
			.WithMany(mog => mog.AlertsFromGroup1)
			.OnDelete(DeleteBehavior.Restrict);
		modelBuilder.Entity<AlertStoreItem>()
			.HasOne(a => a.MonitorObjectGroup2)
			.WithMany(mog => mog.AlertsFromGroup2)
			.OnDelete(DeleteBehavior.Restrict);
		modelBuilder.Entity<AlertStoreItem>()
			.HasOne(a => a.MonitorObjectGroup3)
			.WithMany(mog => mog.AlertsFromGroup3)
			.OnDelete(DeleteBehavior.Restrict);
		modelBuilder.Entity<AlertStoreItem>()
			.HasOne(a => a.MonitorObjectGroup4)
			.WithMany(mog => mog.AlertsFromGroup4)
			.OnDelete(DeleteBehavior.Restrict);
		modelBuilder.Entity<AlertStoreItem>()
			.HasOne(a => a.MonitorObjectGroup5)
			.WithMany(mog => mog.AlertsFromGroup5)
			.OnDelete(DeleteBehavior.Restrict);
		modelBuilder.Entity<AlertStoreItem>()
			.HasOne(a => a.MonitorObjectGroup6)
			.WithMany(mog => mog.AlertsFromGroup6)
			.OnDelete(DeleteBehavior.Restrict);
		modelBuilder.Entity<AlertStoreItem>()
			.HasOne(a => a.MonitorObjectGroup7)
			.WithMany(mog => mog.AlertsFromGroup7)
			.OnDelete(DeleteBehavior.Restrict);
		modelBuilder.Entity<AlertStoreItem>()
			.HasOne(a => a.MonitorObjectGroup8)
			.WithMany(mog => mog.AlertsFromGroup8)
			.OnDelete(DeleteBehavior.Restrict);
		modelBuilder.Entity<AlertStoreItem>()
			.HasOne(a => a.MonitorObjectGroup9)
			.WithMany(mog => mog.AlertsFromGroup9)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<CollectorStoreItem>()
			.HasOne(c => c.CollectorGroup)
			.WithMany(cg => cg.Collectors)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<WebsiteStoreItem>()
			.HasOne(ws => ws.WebsiteGroup)
			.WithMany(wsg => wsg.Websites)
			.OnDelete(DeleteBehavior.Restrict);
	}

	/// <summary>
	/// Performs a bulk insert of items into the database, using native bulk insert for PostgreSQL and SQL Server, or batched AddRange for other providers.
	/// </summary>
	/// <typeparam name="T">The entity type to insert.</typeparam>
	/// <param name="items">The items to insert.</param>
	/// <param name="databaseType">The target database type.</param>
	/// <param name="logger">The logger instance.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	public async Task BulkInsertAsync<T>(
		List<T> items,
		DatabaseType databaseType,
		ILogger logger,
		CancellationToken cancellationToken) where T : class
	{
		try
		{
			// Add and save in chunks to avoid over usage of memory. This can be done using proper bulk insert once the ef core libraries can be updated.
			const int BatchSize = 10000;
			var typeName = typeof(T).Name;
			var itemCount = items.Count;

			switch (databaseType)
			{
				case DatabaseType.Postgres:
				case DatabaseType.SqlServer:
					// Bulk insert
					await this.BulkInsertAsync(
						items,
						new BulkConfig
						{
							BulkCopyTimeout = 0,
							BatchSize = BatchSize,
						},
						n => logger.LogDebug(
							"Bulk inserted {ItemType} {ItemNumber}/{AlertStoreItemCount}",
							typeName,
							(int)(n * itemCount),
							itemCount),
						cancellationToken: cancellationToken
						).ConfigureAwait(false);
					break;
				default:
					// Disable change tracking for better insert performance
					ChangeTracker.AutoDetectChangesEnabled = false;

					for (var batch = 0; batch * BatchSize < itemCount; batch++)
					{
						logger.LogDebug(
							"Bulk inserting batch {BatchNumber} of up to {BatchSize} alerts...",
							batch + 1,
							BatchSize);

						AddRange(items.Skip(batch * BatchSize).Take(BatchSize));
						await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
					}

					// Re-enable change tracking
					ChangeTracker.AutoDetectChangesEnabled = true;

					break;
			}
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "An error occurred during bulk insert: {Message}", ex.Message);
			throw;
		}
	}

	/// <inheritdoc />
	public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken)
	{
		UpdateTimestamps();
		return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
	}

	private void UpdateTimestamps()
	{
		var utcNow = DateTimeOffset.UtcNow;
		foreach (var changedEntity in ChangeTracker.Entries())
		{
			if (changedEntity.Entity is StoreItem entity)
			{
				switch (changedEntity.State)
				{
					case EntityState.Added:
						entity.DatamartCreated = utcNow;
						entity.DatamartLastModified = utcNow;
						break;

					case EntityState.Modified:
						entity.DatamartLastModified = utcNow;
						break;
				}
			}
		}
	}
}
