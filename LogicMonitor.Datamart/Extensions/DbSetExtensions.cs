namespace LogicMonitor.Datamart.Extensions;

public static class DbSetExtension
{
	public static async Task AddOrUpdateIdentifiedItemAsync<TApi, TStore>(
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
		ArgumentNullException.ThrowIfNull(dbSet);
		ArgumentNullException.ThrowIfNull(apiItem);

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

	public static void AddOrUpdateLogicModuleUpdate(
			this DbSet<LogicModuleUpdateStoreItem> dbSet,
			LogicModuleUpdate apiItem,
			DateTimeOffset lastObservedUtc,
			ILogger logger
		)
	{
		ArgumentNullException.ThrowIfNull(dbSet);
		ArgumentNullException.ThrowIfNull(apiItem);

		// Do we have it already?
		var storeItem = dbSet
			.FirstOrDefault(si => si.CurrentUuid == apiItem.CurrentUuid);
		if (storeItem is not null)
		{
			// Yes.  Update it
			if (logger?.IsEnabled(LogLevel.Trace) == true)
			{
				logger.LogTrace("Updating existing {TypeName} with id {StoreItemId} ({StoreItemDatamartId})",
					nameof(LogicModuleUpdateStoreItem),
					storeItem.CurrentUuid,
					storeItem.Id);
			}
			// Map from data onto the existing storeItem which EF internal tracker will work out whether anything changed
			storeItem = DatamartClient.MapperInstance.Map(apiItem, storeItem);

			// TODO - Update foreign keys

			storeItem.DatamartLastObserved = lastObservedUtc.UtcDateTime;
			return;
		}
		// No, this is new

		if (logger?.IsEnabled(LogLevel.Trace) == true)
		{
			logger.LogTrace("Adding new {TypeName} with id {DataId}",
				nameof(LogicModuleUpdateStoreItem),
				apiItem.CurrentUuid
				);
		}

		// Add a new entry
		storeItem = DatamartClient.MapperInstance.Map<LogicModuleUpdate, LogicModuleUpdateStoreItem>(apiItem);

		// TODO - Update foreign keys

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
						.ConfigureAwait(false)
						?? throw new InvalidOperationException("EscalationChain not found");

					alertRuleStoreItem.EscalationChainId = escalationChainStoreItem.Id;
					return;
				}
			case (Alert alert, AlertStoreItem alertStoreItem):
				{
					var alertRuleStoreItem = await dbContext
						.AlertRules
						.FirstOrDefaultAsync(ar => ar.LogicMonitorId == alert.AlertRuleId, cancellationToken)
						.ConfigureAwait(false)
						?? throw new InvalidOperationException("AlertRule not found");

					alertStoreItem.AlertRuleId = alertRuleStoreItem.Id;
					return;
				}
			case (Collector collector, CollectorStoreItem collectorStoreItem):
				{
					var collectorGroupStoreItem = await dbContext
						.CollectorGroups
						.FirstOrDefaultAsync(ar => ar.LogicMonitorId == collector.GroupId, cancellationToken)
						.ConfigureAwait(false)
					?? throw new InvalidOperationException("CollectorGroup not found");


					collectorStoreItem.CollectorGroupId = collectorGroupStoreItem.Id;
					return;
				}
			case (Resource device, DeviceStoreItem deviceStoreItem):
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
					.ConfigureAwait(false)
					?? throw new InvalidOperationException("WebsiteGroup not found");


					websiteStoreItem.WebsiteGroupId = websiteGroupStoreItem.Id;
					return;
				}
			case (CollectorGroup collectorGroup, CollectorGroupStoreItem collectorGroupStoreItem2):
			case (ConfigSource configSource, ConfigSourceStoreItem configSourceStoreItem):
			case (DataSource dataSource, DataSourceStoreItem dataSourceStoreItem):
			case (ResourceGroup deviceGroup, DeviceGroupStoreItem deviceGroupStoreItem):
			case (EscalationChain escalationChain, EscalationChainStoreItem escalationChainStoreItem):
			case (EventSource eventSource, EventSourceStoreItem eventSourceStoreItem):
			case (WebsiteGroup websiteGroup, WebsiteGroupStoreItem websiteGroupStoreItem2):
				return;
			default:
				throw new NotSupportedException($"Unsupported TApi/TStore combination: {typeof(TApi).Name}/{typeof(TStore).Name}");
		}
	}
}