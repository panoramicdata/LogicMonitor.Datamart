using System.Linq.Expressions;
using LogicMonitor.Api.Alerts;
using LogicMonitor.Api.Filters;

namespace LogicMonitor.Datamart;

/// <summary>
/// The filters, ordering and paging applied to the cached alert store.
/// </summary>
internal sealed class CachedAlertQuery
{
	public required long StartSeconds { get; init; }

	public required long EndSeconds { get; init; }

	public required bool IncludeInactive { get; init; }

	public required int? Skip { get; init; }

	public required int? Take { get; init; }

	public required string? Id { get; init; }

	public required AckFilter AckFilter { get; init; }

	public required ICollection<string>? MonitorObjectGroups { get; init; }

	public required string? MonitorObjectName { get; init; }

	public required int? MonitorObjectId { get; init; }

	public required ICollection<AlertType>? AlertTypes { get; init; }

	public required string? ResourceTemplateDisplayName { get; init; }

	public required int? ResourceTemplateId { get; init; }

	public required string? InstanceName { get; init; }

	public required string? DataPointName { get; init; }

	public required ICollection<AlertLevel>? AlertLevels { get; init; }

	public required string? OrderBy { get; init; }

	public required OrderDirection? OrderDirection { get; init; }

	public required SdtFilter SdtFilter { get; init; }

	public required string? ProblemSignature { get; init; }

	public required bool IsPercentageAvailability { get; init; }

	internal IQueryable<AlertStoreItem> Apply(IQueryable<AlertStoreItem> queryable)
	{
		queryable = ApplyEqualityFilters(queryable);
		queryable = ApplyLevelAndTypeFilters(queryable);
		queryable = ApplyGroupFilter(queryable);
		queryable = ApplySdtFilter(queryable);
		queryable = ApplyAckFilter(queryable);
		queryable = ApplyPeriod(queryable);
		queryable = ApplyOrder(queryable);
		queryable = ApplyPaging(queryable);

		return IsPercentageAvailability ? ApplyNarrowProjection(queryable) : queryable;
	}

	private IQueryable<AlertStoreItem> ApplyEqualityFilters(IQueryable<AlertStoreItem> queryable)
		=> ApplyDetailFilters(ApplyIdentityFilters(queryable));

	private IQueryable<AlertStoreItem> ApplyIdentityFilters(IQueryable<AlertStoreItem> queryable)
	{
		var id = Id;
		var problemSignature = ProblemSignature;
		var monitorObjectId = MonitorObjectId;
		var monitorObjectName = MonitorObjectName;

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

		return monitorObjectName != null
			? queryable.Where(a => a.MonitorObjectName == monitorObjectName)
			: queryable;
	}

	private IQueryable<AlertStoreItem> ApplyDetailFilters(IQueryable<AlertStoreItem> queryable)
	{
		var resourceTemplateId = ResourceTemplateId;
		var resourceTemplateDisplayName = ResourceTemplateDisplayName;
		var instanceName = InstanceName;
		var dataPointName = DataPointName;

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

		return dataPointName != null
			? queryable.Where(a => a.DataPointName == dataPointName)
			: queryable;
	}
	private IQueryable<AlertStoreItem> ApplyLevelAndTypeFilters(IQueryable<AlertStoreItem> queryable)
	{
		var alertLevels = AlertLevels;
		var alertTypes = AlertTypes;

		if (alertLevels != null)
		{
			queryable = queryable.Where(a => alertLevels.Select(al => (int)al).Contains(a.Severity));
		}

		if (alertTypes != null)
		{
			queryable = queryable.Where(a => alertTypes.Contains(a.AlertType));
		}

		return IncludeInactive ? queryable : queryable.Where(a => !a.IsCleared);
	}

	private sealed class LikeParameter
	{
		public required string Pattern { get; init; }
	}

	// An empty collection means the same as no collection: no device group filter.
	private IQueryable<AlertStoreItem> ApplyGroupFilter(IQueryable<AlertStoreItem> queryable)
	{
		var monitorObjectGroup = MonitorObjectGroups?.FirstOrDefault();
		if (monitorObjectGroup == null)
		{
			return queryable;
		}

		var like = new LikeParameter { Pattern = $"{monitorObjectGroup.TrimEnd('*')}%" };
		var alert = Expression.Parameter(typeof(AlertStoreItem), "a");
		var anySlotMatches = Enumerable
			.Range(0, 10)
			.Select(slot => GroupSlotMatches(alert, slot, like))
			.Aggregate(Expression.OrElse);

		return queryable.Where(Expression.Lambda<Func<AlertStoreItem, bool>>(anySlotMatches, alert));
	}

	private static BinaryExpression GroupSlotMatches(ParameterExpression alert, int slot, LikeParameter like)
	{
		var group = Expression.Property(alert, $"MonitorObjectGroup{slot}");
		var likeCall = Expression.Call(
			typeof(DbFunctionsExtensions),
			nameof(DbFunctionsExtensions.Like),
			Type.EmptyTypes,
			Expression.Property(null, typeof(EF), nameof(EF.Functions)),
			Expression.Property(group, nameof(MonitorObjectGroupStoreItem.FullPath)),
			Expression.Property(Expression.Constant(like), nameof(LikeParameter.Pattern)));

		return Expression.AndAlso(
			Expression.AndAlso(
				Expression.NotEqual(group, Expression.Constant(null, typeof(MonitorObjectGroupStoreItem))),
				Expression.Equal(
					Expression.Property(group, nameof(MonitorObjectGroupStoreItem.MonitoredObjectType)),
					Expression.Property(alert, nameof(AlertStoreItem.MonitorObjectType)))),
			likeCall);
	}
	private IQueryable<AlertStoreItem> ApplySdtFilter(IQueryable<AlertStoreItem> queryable)
		=> SdtFilter switch
		{
			SdtFilter.All => queryable,
			SdtFilter.Sdt => queryable.Where(a => a.InScheduledDownTime),
			SdtFilter.NonSdt => queryable.Where(a => !a.InScheduledDownTime),
			_ => throw new NotSupportedException($"Unsupported SDT filter {SdtFilter}"),
		};

	private IQueryable<AlertStoreItem> ApplyAckFilter(IQueryable<AlertStoreItem> queryable)
		=> AckFilter switch
		{
			AckFilter.All => queryable,
			AckFilter.Acked => queryable.Where(a => a.Acked),
			AckFilter.Nonacked => queryable.Where(a => !a.Acked),
			_ => throw new NotSupportedException($"Unsupported SDT filter {SdtFilter}"),
		};

	private IQueryable<AlertStoreItem> ApplyPeriod(IQueryable<AlertStoreItem> queryable)
	{
		var startSeconds = StartSeconds;
		var endSeconds = EndSeconds;

		return queryable.Where(a =>
			// Alert is not cleared and it started before the end of the period
			(a.StartOnSeconds <= endSeconds && !a.IsCleared)
			// Or it starts within the period
			|| (a.StartOnSeconds > startSeconds && a.StartOnSeconds <= endSeconds)
			// Or it ends within the period
			|| (a.EndOnSeconds > startSeconds && a.EndOnSeconds <= endSeconds)
			// Or it covers the entire period
			|| (a.StartOnSeconds <= startSeconds && a.EndOnSeconds > endSeconds)
		);
	}

	private IQueryable<AlertStoreItem> ApplyOrder(IQueryable<AlertStoreItem> queryable)
		=> OrderBy == null
			? queryable
			: $"{OrderBy}/{OrderDirection}" switch
			{
				"StartOnSeconds/Asc" => queryable.OrderBy(a => a.StartOnSeconds),
				"StartOnSeconds/Desc" => queryable.OrderByDescending(a => a.StartOnSeconds),
				_ => throw new NotSupportedException("Only orderBy=StartOnSeconds currently supported."),
			};

	private IQueryable<AlertStoreItem> ApplyPaging(IQueryable<AlertStoreItem> queryable)
	{
		if (Skip.HasValue)
		{
			queryable = queryable.Skip(Skip.Value);
		}

		return Take.HasValue ? queryable.Take(Take.Value) : queryable;
	}

	private static IQueryable<AlertStoreItem> ApplyNarrowProjection(IQueryable<AlertStoreItem> queryable)
		=> queryable.Select(a => new AlertStoreItem
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
