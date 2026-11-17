using System.Linq.Expressions;

namespace LogicMonitor.Datamart.Mapping;

internal static class MappingExpressionExtensions
{
	/// <summary>
	/// Ignores each of the given destination members.
	/// </summary>
	internal static IMappingExpression<TSource, TDestination> IgnoreMembers<TSource, TDestination>(
		this IMappingExpression<TSource, TDestination> expression,
		params Expression<Func<TDestination, object?>>[] members)
	{
		foreach (var member in members)
		{
			expression.ForMember(member, opts => opts.Ignore());
		}

		return expression;
	}
}