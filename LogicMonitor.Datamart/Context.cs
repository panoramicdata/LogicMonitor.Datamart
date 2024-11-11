namespace LogicMonitor.Datamart;

public class Context : DbContext
{
	public Context()
	{
		// Required for adding migrations
	}

	public Context(DbContextOptions<Context> options) : base(options)
	{
		//Database.SetCommandTimeout(TimeSpan.FromHours(1));
	}

	public DbSet<AlertStoreItem> Alerts { get; set; } = null!;
	public DbSet<AlertRuleStoreItem> AlertRules { get; set; } = null!;
	public DbSet<AuditEventStoreItem> AuditEvents { get; set; } = null!;
	public DbSet<CollectorStoreItem> Collectors { get; set; } = null!;
	public DbSet<CollectorGroupStoreItem> CollectorGroups { get; set; } = null!;
	public DbSet<ConfigSourceStoreItem> ConfigSources { get; set; } = null!;
	public DbSet<DataSourceGraphStoreItem> DataSourceGraphs { get; set; } = null!;
	public DbSet<DataSourceStoreItem> DataSources { get; set; } = null!;
	public DbSet<DataSourceDataPointStoreItem> DataSourceDataPoints { get; set; } = null!;
	//public DbSet<IntegrationStoreItem> Integrations { get; set; } = null!;
	public DbSet<ResourceDataSourceStoreItem> DeviceDataSources { get; set; } = null!;
	public DbSet<ResourceDataSourceInstanceStoreItem> DeviceDataSourceInstances { get; set; } = null!;
	public DbSet<EscalationChainStoreItem> EscalationChains { get; set; } = null!;
	public DbSet<EventSourceStoreItem> EventSources { get; set; } = null!;
	public DbSet<LogicModuleUpdateStoreItem> LogicModuleUpdates { get; set; } = null!;
	public DbSet<LogStoreItem> LogItems { get; set; } = null!;
	public DbSet<WebsiteStoreItem> Websites { get; set; } = null!;
	public DbSet<ResourceDataSourceInstanceDataPointStoreItem> DeviceDataSourceInstanceDataPoints { get; set; } = null!;
	public DbSet<ResourceStoreItem> Devices { get; set; } = null!;
	public DbSet<ResourceGroupStoreItem> DeviceGroups { get; set; } = null!;
	public DbSet<WebsiteGroupStoreItem> WebsiteGroups { get; set; } = null!;
	public DbSet<MonitorObjectGroupStoreItem> MonitorObjectGroups { get; set; } = null!;
	public DbSet<TimeSeriesDataAggregationStoreItem> TimeSeriesDataAggregations { get; set; } = null!;

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
