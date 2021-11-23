namespace LogicMonitor.Datamart;

// This is internal, we don't want to expose the EfContext outside the client
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

	public DbSet<AlertStoreItem> Alerts { get; set; }
	public DbSet<AlertRuleStoreItem> AlertRules { get; set; }
	public DbSet<CollectorStoreItem> Collectors { get; set; }
	public DbSet<CollectorGroupStoreItem> CollectorGroups { get; set; }
	public DbSet<ConfigSourceStoreItem> ConfigSources { get; set; }
	public DbSet<DataSourceStoreItem> DataSources { get; set; }
	public DbSet<DataSourceDataPointStoreItem> DataSourceDataPoints { get; set; }
	public DbSet<DeviceDataSourceStoreItem> DeviceDataSources { get; set; }
	public DbSet<DeviceDataSourceInstanceStoreItem> DeviceDataSourceInstances { get; set; }
	public DbSet<EscalationChainStoreItem> EscalationChains { get; set; }
	public DbSet<EventSourceStoreItem> EventSources { get; set; }
	public DbSet<DeviceStoreItem> Devices { get; set; }
	public DbSet<DeviceGroupStoreItem> DeviceGroups { get; set; }
	public DbSet<LogStoreItem> LogItems { get; set; }
	public DbSet<WebsiteStoreItem> Websites { get; set; }
	public DbSet<WebsiteGroupStoreItem> WebsiteGroups { get; set; }
	public DbSet<MonitorObjectGroupStoreItem> MonitorObjectGroups { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ForNpgsqlUseIdentityByDefaultColumns();
		modelBuilder.ForSqlServerUseIdentityColumns();

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
			alertIndexBuilder.ForNpgsqlInclude(new[]
			{
					nameof(AlertStoreItem.Id),
					nameof(AlertStoreItem.Severity),
					nameof(AlertStoreItem.ClearValue),
					nameof(AlertStoreItem.MonitorObjectId),
					nameof(AlertStoreItem.ResourceTemplateName),
					nameof(AlertStoreItem.InstanceId),
					nameof(AlertStoreItem.InstanceName)
				})
			.HasName($"IX_{nameof(Alerts)}_FasterPercentageAvailability");
		}
		if (Database.IsSqlServer())
		{
			alertIndexBuilder.ForSqlServerInclude(new[]
			{
					nameof(AlertStoreItem.Id),
					nameof(AlertStoreItem.Severity),
					nameof(AlertStoreItem.ClearValue),
					nameof(AlertStoreItem.MonitorObjectId),
					nameof(AlertStoreItem.ResourceTemplateName),
					nameof(AlertStoreItem.InstanceId),
					nameof(AlertStoreItem.InstanceName)
				})
			.HasName($"IX_{nameof(Alerts)}_FasterPercentageAvailability")
			;
		}

		// DeviceDataSourceInstance indexes
		modelBuilder.Entity<DeviceDataSourceInstanceStoreItem>()
			.HasIndex(ddsi => ddsi.LastWentMissingUtc);

		// Relational stuff
		modelBuilder.Entity<DeviceDataSourceInstanceStoreItem>()
			.HasOne(ddsi => ddsi.DeviceDataSource)
			.WithMany(ds => ds.DeviceDataSourceInstances)
			.HasForeignKey(ddsi => ddsi.DeviceDataSourceId)
			.HasPrincipalKey(ds => ds.Id)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<DeviceDataSourceInstanceStoreItem>()
			.HasOne(ddsi => ddsi.Device)
			.WithMany(d => d.DeviceDataSourceInstances)
			.HasForeignKey(ddsi => ddsi.DeviceId)
			.HasPrincipalKey(d => d.Id)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<DeviceDataSourceStoreItem>()
			.HasOne(dds => dds.DataSource)
			.WithMany(d => d.DeviceDataSources)
			.HasForeignKey(dds => dds.DataSourceId)
			.HasPrincipalKey(d => d.Id)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<DeviceDataSourceStoreItem>()
			.HasOne(dds => dds.Device)
			.WithMany(d => d.DeviceDataSources)
			.HasForeignKey(dds => dds.DeviceId)
			.HasPrincipalKey(d => d.Id)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<AlertStoreItem>()
			.HasOne(a => a.AlertRule)
			.WithMany(ar => ar.Alerts)
			.HasForeignKey(a => a.AlertRuleId)
			.HasPrincipalKey(ar => ar.Id)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<AlertStoreItem>()
			.HasOne(a => a.MonitorObjectGroup0).WithMany(mog => mog.AlertsFromGroup0)
			.OnDelete(DeleteBehavior.Restrict);
		modelBuilder.Entity<AlertStoreItem>()
			.HasOne(a => a.MonitorObjectGroup1).WithMany(mog => mog.AlertsFromGroup1)
			.OnDelete(DeleteBehavior.Restrict);
		modelBuilder.Entity<AlertStoreItem>()
			.HasOne(a => a.MonitorObjectGroup2).WithMany(mog => mog.AlertsFromGroup2)
			.OnDelete(DeleteBehavior.Restrict);
		modelBuilder.Entity<AlertStoreItem>()
			.HasOne(a => a.MonitorObjectGroup3).WithMany(mog => mog.AlertsFromGroup3)
			.OnDelete(DeleteBehavior.Restrict);
		modelBuilder.Entity<AlertStoreItem>()
			.HasOne(a => a.MonitorObjectGroup4).WithMany(mog => mog.AlertsFromGroup4)
			.OnDelete(DeleteBehavior.Restrict);
		modelBuilder.Entity<AlertStoreItem>()
			.HasOne(a => a.MonitorObjectGroup5).WithMany(mog => mog.AlertsFromGroup5)
			.OnDelete(DeleteBehavior.Restrict);
		modelBuilder.Entity<AlertStoreItem>()
			.HasOne(a => a.MonitorObjectGroup6).WithMany(mog => mog.AlertsFromGroup6)
			.OnDelete(DeleteBehavior.Restrict);
		modelBuilder.Entity<AlertStoreItem>()
			.HasOne(a => a.MonitorObjectGroup7).WithMany(mog => mog.AlertsFromGroup7)
			.OnDelete(DeleteBehavior.Restrict);
		modelBuilder.Entity<AlertStoreItem>()
			.HasOne(a => a.MonitorObjectGroup8).WithMany(mog => mog.AlertsFromGroup8)
			.OnDelete(DeleteBehavior.Restrict);
		modelBuilder.Entity<AlertStoreItem>()
			.HasOne(a => a.MonitorObjectGroup9).WithMany(mog => mog.AlertsFromGroup9)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<CollectorStoreItem>()
			.HasOne(c => c.CollectorGroup)
			.WithMany(cg => cg.Collectors)
			.HasForeignKey(c => c.GroupId)
			.HasPrincipalKey(cg => cg.Id)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<WebsiteStoreItem>()
			.HasOne(ws => ws.WebsiteGroup)
			.WithMany(wsg => wsg.Websites)
			.HasForeignKey(ws => ws.WebsiteGroupId)
			.HasPrincipalKey(wsg => wsg.Id)
			.OnDelete(DeleteBehavior.Restrict);
	}

	public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
	{
		UpdateTimestamps();
		return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
	}

	private void UpdateTimestamps()
	{
		var utcNow = DateTime.UtcNow;
		foreach (var changedEntity in ChangeTracker.Entries())
		{
			if (changedEntity.Entity is StoreItem entity)
			{
				switch (changedEntity.State)
				{
					case EntityState.Added:
						entity.DatamartCreatedUtc = utcNow;
						entity.DatamartLastModifiedUtc = utcNow;
						break;

					case EntityState.Modified:
						entity.DatamartLastModifiedUtc = utcNow;
						break;
				}
			}
		}
	}
}
