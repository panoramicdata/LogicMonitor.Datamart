using AutoMapper;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace LogicMonitor.Datamart.Mapping
{
	internal class TruncateMappingAction<TSource, TDestination> : IMappingAction<TSource, TDestination>
	{
		public void Process(TSource source, TDestination destination, ResolutionContext context)
		{
			// Loop over all the destination properties and truncate any strings if required
			var destType = typeof(TDestination);
			var sourceType = typeof(TSource);

			foreach (var property in destType.GetProperties())
			{
				// Truncate if we have a string, and a MaxLength set, and the length is more than this
				if (
					property.GetValue(destination) is string currentValue
					&& property.GetCustomAttributes().FirstOrDefault(a => a.GetType() == typeof(MaxLengthAttribute)) is MaxLengthAttribute maxLengthAttribute
					&& currentValue.Length > maxLengthAttribute.Length)
				{
					property.SetValue(destination, currentValue[..maxLengthAttribute.Length]);
				}
			}
		}
	}
}