namespace LogicMonitor.Datamart.Extensions;

/// <summary>
/// Extension methods for Entity Framework <see cref="DbSet{TEntity}"/> operations in the datamart.
/// </summary>
public static class DbSetExtension
{
	/// <summary>
	/// Adds a new identified item or updates an existing one in the DbSet, mapping from the API model.
	/// </summary>
	/// <typeparam name="TApi">The LogicMonitor API item type.</typeparam>
	/// <typeparam name="TStore">The datamart store item type.</typeparam>
	/// <param name="dbSet">The DbSet to add to or update in.</param>
	/// <param name="context">The datamart database context.</param>
	/// <param name="apiItem">The API item to map from.</param>
	/// <param name="lastObservedUtc">The UTC timestamp when this item was last observed in the API.</param>
	/// <param name="logger">The logger instance.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
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

	/// <summary>
	/// Adds a new Exchange LogicModule or updates an existing one in the DbSet, matching by ExchangeId.
	/// </summary>
	/// <param name="dbSet">The DbSet to add to or update in.</param>
	/// <param name="apiItem">The API Exchange LogicModule item.</param>
	/// <param name="lastObservedUtc">The UTC timestamp when this item was last observed in the API.</param>
	/// <param name="logger">The logger instance.</param>
	public static void AddOrUpdateLogicModuleUpdate(
			this DbSet<LogicModuleUpdateStoreItem> dbSet,
			ExchangeLogicModule apiItem,
			DateTimeOffset lastObservedUtc,
			ILogger logger
		)
	{
		ArgumentNullException.ThrowIfNull(dbSet);
		ArgumentNullException.ThrowIfNull(apiItem);

		// Do we have it already?
		var storeItem = dbSet
			.FirstOrDefault(si => si.ExchangeId == apiItem.Id);
		if (storeItem is not null)
		{
			// Yes.  Update it
			if (logger?.IsEnabled(LogLevel.Trace) == true)
			{
				logger.LogTrace("Updating existing {TypeName} with id {StoreItemId} ({StoreItemDatamartId})",
					nameof(LogicModuleUpdateStoreItem),
					storeItem.ExchangeId,
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
				apiItem.Id
				);
		}

		// Add a new entry
		storeItem = DatamartClient.MapperInstance.Map<ExchangeLogicModule, LogicModuleUpdateStoreItem>(apiItem);

		// TODO - Update foreign keys

		storeItem.DatamartLastObserved = lastObservedUtc.UtcDateTime;
		dbSet.Add(storeItem);
	}

	private static readonly (Type Api, Type Store)[] _itemsWithoutForeignKeys =
	[
		(typeof(CollectorGroup), typeof(CollectorGroupStoreItem)),
		(typeof(ConfigSource), typeof(ConfigSourceStoreItem)),
		(typeof(DataSource), typeof(DataSourceStoreItem)),
		(typeof(ResourceGroup), typeof(ResourceGroupStoreItem)),
		(typeof(EscalationChain), typeof(EscalationChainStoreItem)),
		(typeof(EventSource), typeof(EventSourceStoreItem)),
		(typeof(WebsiteGroup), typeof(WebsiteGroupStoreItem)),
		(typeof(Integration), typeof(IntegrationStoreItem)),
	];

	private static bool HasNoForeignKeys(object apiItem, object storeItem)
		=> _itemsWithoutForeignKeys.Any(pair => pair.Api.IsInstanceOfType(apiItem) && pair.Store.IsInstanceOfType(storeItem));

	private static async Task UpdateForeignKeysAsync<TApi, TStore>(
		Context dbContext,
		TApi apiItem,
		TStore storeItem,
		CancellationToken cancellationToken)
		where TApi : IdentifiedItem
		where TStore : IdentifiedStoreItem
	{
		if (HasNoForeignKeys(apiItem, storeItem))
		{
			return;
		}

		switch (apiItem, storeItem)
		{
			case (AlertRule alertRule, AlertRuleStoreItem alertRuleStoreItem):
				await UpdateAlertRuleAsync(dbContext, alertRule, alertRuleStoreItem, cancellationToken).ConfigureAwait(false);
				return;
			case (Alert alert, AlertStoreItem alertStoreItem):
				await UpdateAlertAsync(dbContext, alert, alertStoreItem, cancellationToken).ConfigureAwait(false);
				return;
			case (Collector collector, CollectorStoreItem collectorStoreItem):
				await UpdateCollectorAsync(dbContext, collector, collectorStoreItem, cancellationToken).ConfigureAwait(false);
				return;
			case (Resource device, ResourceStoreItem deviceStoreItem):
				await UpdateResourceAsync(dbContext, device, deviceStoreItem, cancellationToken).ConfigureAwait(false);
				return;
			case (Website website, WebsiteStoreItem websiteStoreItem):
				await UpdateWebsiteAsync(dbContext, website, websiteStoreItem, cancellationToken).ConfigureAwait(false);
				return;
			default:
				throw new NotSupportedException($"Unsupported TApi/TStore combination: {typeof(TApi).Name}/{typeof(TStore).Name}");
		}
	}

	private static async Task UpdateAlertRuleAsync(Context dbContext, AlertRule alertRule, AlertRuleStoreItem alertRuleStoreItem, CancellationToken cancellationToken)
	{
		var escalationChainStoreItem = await dbContext
			.EscalationChains
			.FirstOrDefaultAsync(ar => ar.LogicMonitorId == alertRule.EscalationChainId, cancellationToken)
			.ConfigureAwait(false)
			?? throw new InvalidOperationException("EscalationChain not found");
		alertRuleStoreItem.EscalationChainId = escalationChainStoreItem.Id;
	}

	private static async Task UpdateAlertAsync(Context dbContext, Alert alert, AlertStoreItem alertStoreItem, CancellationToken cancellationToken)
	{
		var alertRuleStoreItem = await dbContext
			.AlertRules
			.FirstOrDefaultAsync(ar => ar.LogicMonitorId == alert.AlertRuleId, cancellationToken)
			.ConfigureAwait(false)
			?? throw new InvalidOperationException("AlertRule not found");
		alertStoreItem.AlertRuleId = alertRuleStoreItem.Id;
	}

	private static async Task UpdateCollectorAsync(Context dbContext, Collector collector, CollectorStoreItem collectorStoreItem, CancellationToken cancellationToken)
	{
		var collectorGroupStoreItem = await dbContext
			.CollectorGroups
			.FirstOrDefaultAsync(ar => ar.LogicMonitorId == collector.GroupId, cancellationToken)
			.ConfigureAwait(false)
			?? throw new InvalidOperationException("CollectorGroup not found");
		collectorStoreItem.CollectorGroupId = collectorGroupStoreItem.Id;
	}

	private static async Task UpdateResourceAsync(Context dbContext, Resource device, ResourceStoreItem deviceStoreItem, CancellationToken cancellationToken)
	{
		var preferredCollector = await dbContext
			.Collectors
			.FirstOrDefaultAsync(ar => ar.LogicMonitorId == device.PreferredCollectorId, cancellationToken)
			.ConfigureAwait(false);
		deviceStoreItem.PreferredCollectorId = preferredCollector?.Id;
	}

	private static async Task UpdateWebsiteAsync(Context dbContext, Website website, WebsiteStoreItem websiteStoreItem, CancellationToken cancellationToken)
	{
		var websiteGroupStoreItem = await dbContext
			.WebsiteGroups
			.FirstOrDefaultAsync(ar => ar.LogicMonitorId == website.GroupId, cancellationToken)
			.ConfigureAwait(false)
			?? throw new InvalidOperationException("WebsiteGroup not found");
		websiteStoreItem.WebsiteGroupId = websiteGroupStoreItem.Id;
	}
}