namespace LogicMonitor.Datamart.Extensions;

public static class DbSetExtension
{
	public static async Task AddOrUpdateIdentifiedItem<TApi, TStore>(
			this DbSet<TStore> dbSet,
			Context context,
			TApi apiItem,
			DateTimeOffset lastObservedUtc,
			ILogger logger,
			CancellationToken cancellationToken
		)
		where TApi : IdentifiedItem
		where TStore : IdentifiedStoreItem
	{
		if (dbSet == null)
		{
			throw new ArgumentNullException(nameof(dbSet));
		}

		if (apiItem == null)
		{
			throw new ArgumentNullException(nameof(apiItem));
		}

		// Do we have it already?
		var storeItem = dbSet
			.FirstOrDefault(si => si.LogicMonitorId == apiItem.Id);
		if (storeItem is not null)
		{
			// Yes.  Update it
			if (logger?.IsEnabled(LogLevel.Trace) == true)
			{
				logger.LogTrace("Updating existing {TypeName} with id {StoreItemId} ({StoreItemDatamartId})",
					typeof(TStore).Name,
					storeItem.LogicMonitorId,
					storeItem.Id);
			}
			// Map from data onto the existing storeItem which EF internal tracker will work out whether anything changed
			storeItem = DatamartClient.MapperInstance.Map(apiItem, storeItem);

			// Update foreign keys
			await UpdateForeignKeysAsync(
				context,
				apiItem,
				storeItem,
				cancellationToken
				).ConfigureAwait(false);

			storeItem.DatamartLastObserved = lastObservedUtc.UtcDateTime;
			return;
		}
		// No, this is new

		if (logger?.IsEnabled(LogLevel.Trace) == true)
		{
			logger.LogTrace("Adding new {TypeName} with id {DataId}",
				typeof(TStore).Name,
				apiItem.Id
				);
		}

		// Add a new entry
		storeItem = DatamartClient.MapperInstance.Map<TApi, TStore>(apiItem);

		// Update foreign keys
		await UpdateForeignKeysAsync(
			context,
			apiItem,
			storeItem,
			cancellationToken
			).ConfigureAwait(false);

		storeItem.DatamartLastObserved = lastObservedUtc.UtcDateTime;
		dbSet.Add(storeItem);
	}

	private static async Task UpdateForeignKeysAsync<TApi, TStore>(
		Context dbContext,
		TApi apiItem,
		TStore storeItem,
		CancellationToken cancellationToken)
		where TApi : IdentifiedItem
		where TStore : IdentifiedStoreItem
	{
		switch (apiItem, storeItem)
		{
			case (AlertRule alertRule, AlertRuleStoreItem alertRuleStoreItem):
				{
					var escalationChainStoreItem = await dbContext
					.EscalationChains
						.FirstOrDefaultAsync(ar => ar.LogicMonitorId == alertRule.EscalationChainId, cancellationToken)
						.ConfigureAwait(false);

					alertRuleStoreItem.EscalationChainId = escalationChainStoreItem.Id;
					return;
				}
			case (Alert alert, AlertStoreItem alertStoreItem):
				{
					var alertRuleStoreItem = await dbContext
						.AlertRules
						.FirstOrDefaultAsync(ar => ar.LogicMonitorId == alert.AlertRuleId, cancellationToken)
						.ConfigureAwait(false);

					alertStoreItem.AlertRuleId = alertRuleStoreItem.Id;
					return;
				}
			case (Collector collector, CollectorStoreItem collectorStoreItem):
				{
					var collectorGroupStoreItem = await dbContext
						.CollectorGroups
						.FirstOrDefaultAsync(ar => ar.LogicMonitorId == collector.GroupId, cancellationToken)
						.ConfigureAwait(false);

					collectorStoreItem.CollectorGroupId = collectorGroupStoreItem.Id;
					return;
				}
			case (Device device, DeviceStoreItem deviceStoreItem):
				{
					var collectorStoreItem = await dbContext
					.Collectors
						.FirstOrDefaultAsync(ar => ar.LogicMonitorId == device.PreferredCollectorId, cancellationToken)
					.ConfigureAwait(false);

					deviceStoreItem.PreferredCollectorId = collectorStoreItem?.Id;
					return;
				}
			case (Website website, WebsiteStoreItem websiteStoreItem):
				{
					var websiteGroupStoreItem = await dbContext
					.WebsiteGroups
						.FirstOrDefaultAsync(ar => ar.LogicMonitorId == website.GroupId, cancellationToken)
					.ConfigureAwait(false);

					websiteStoreItem.WebsiteGroupId = websiteGroupStoreItem.Id;
					return;
				}
			case (CollectorGroup collectorGroup, CollectorGroupStoreItem collectorGroupStoreItem2):
			case (ConfigSource configSource, ConfigSourceStoreItem configSourceStoreItem):
			case (DataSource dataSource, DataSourceStoreItem dataSourceStoreItem):
			case (DeviceGroup deviceGroup, DeviceGroupStoreItem deviceGroupStoreItem):
			case (EscalationChain escalationChain, EscalationChainStoreItem escalationChainStoreItem):
			case (EventSource eventSource, EventSourceStoreItem eventSourceStoreItem):
			case (WebsiteGroup websiteGroup, WebsiteGroupStoreItem websiteGroupStoreItem2):
				return;
			default:
				throw new NotSupportedException($"Unsupported TApi/TStore combination: {typeof(TApi).Name}/{typeof(TStore).Name}");
		}
	}

	public static async Task AddOrUpdateAlertRangeSavingChanges(this DbSet<AlertStoreItem> dbSet, ICollection<Alert> items)
	{
		foreach (var item in items ?? throw new ArgumentNullException(nameof(items)))
		{
			dbSet.AddOrUpdateAlert(item);

			await dbSet.GetContext()
				.SaveChangesAsync()
				.ConfigureAwait(false);
		}
	}

	public static void AddOrUpdateAlert(this DbSet<AlertStoreItem> dbSet, Alert data)
	{
		var context = (dbSet ?? throw new ArgumentNullException(nameof(dbSet))).GetContext();
		var storeItem = dbSet.AsQueryable().Where(si => si.LogicMonitorId == data.Id).FirstOrDefault();
		var mappedStoreItem = DatamartClient.MapperInstance.Map<Alert, AlertStoreItem>(data);

		var utcNow = DateTimeOffset.UtcNow;

		if (storeItem != null)
		{
			// Keep the existing Guid
			mappedStoreItem.Id = storeItem.Id;
			context.Entry(storeItem).CurrentValues.SetValues(mappedStoreItem);
			context.Entry(storeItem).State = EntityState.Modified;
			mappedStoreItem.DatamartLastModified = utcNow;
			return;
		}
		else
		{
			mappedStoreItem.DatamartCreated = utcNow;
			mappedStoreItem.DatamartLastModified = utcNow;
		}

		dbSet.Add(mappedStoreItem);
	}

	public static void AddOrUpdate<TApi, TStore>(this DbSet<TStore> dbSet, Expression<Func<TStore, object>> key, TApi data)
		where TApi : class
		where TStore : class
	{
		var context = dbSet.GetContext();
		var ids = context.Model.FindEntityType(typeof(TStore)).FindPrimaryKey().Properties.Select(x => x.Name);
		var t = typeof(TStore);
		var keyObject = key.Compile()(DatamartClient.MapperInstance.Map<TApi, TStore>(data));
		var keyFields = keyObject.GetType().GetProperties().Select(p => t.GetProperty(p.Name)).ToArray()
			?? throw new NotSupportedException($"{t.FullName} does not have a KeyAttribute field. Unable to exec AddOrUpdate call.");
		var keyVals = keyFields.Select(p => p.GetValue(data));
		var entities = dbSet.AsQueryable();
		var i = 0;
		foreach (var keyVal in keyVals)
		{
			entities = entities.Where(p => p.GetType().GetProperty(keyFields[i].Name).GetValue(p).Equals(keyVal));
			i++;
		}

		var dbVal = entities.FirstOrDefault();
		if (dbVal != null)
		{
			var keyAttrs =
				data.GetType().GetProperties().Where(p => ids.Contains(p.Name)).ToList();
			if (keyAttrs.Count > 0)
			{
				foreach (var keyAttr in keyAttrs)
				{
					keyAttr.SetValue(data,
						Array.Find(dbVal.GetType()
							.GetProperties(), p => p.Name == keyAttr.Name)
							.GetValue(dbVal));
				}

				context.Entry(dbVal).CurrentValues.SetValues(data);
				context.Entry(dbVal).State = EntityState.Modified;
				return;
			}
		}

		dbSet.Add(DatamartClient.MapperInstance.Map<TApi, TStore>(data));
	}
}

public static class HackyDbSetGetContextTrick
{
	public static DbContext GetContext<TEntity>(this DbSet<TEntity> dbSet)
		where TEntity : class => (DbContext)dbSet
			.GetType().GetTypeInfo()
			.GetField("_context", BindingFlags.NonPublic | BindingFlags.Instance)
			.GetValue(dbSet);
}
