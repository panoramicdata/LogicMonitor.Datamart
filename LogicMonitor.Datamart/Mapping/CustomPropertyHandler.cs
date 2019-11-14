using LogicMonitor.Api.Devices;
using LogicMonitor.Datamart.Config;
using LogicMonitor.Datamart.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace LogicMonitor.Datamart.Mapping
{
	public static class CustomPropertyHandler
	{
		private const int MaxPropertyCount = 5;
		private static List<string> _propertyNames;

		internal static void Configure(List<string> propertyNames)
		{
			if (propertyNames == null)
			{
				throw new System.ArgumentNullException(nameof(propertyNames));
			}
			if (propertyNames.Count > MaxPropertyCount)
			{
				throw new ConfigurationException($"Configure between 0 and {MaxPropertyCount} {nameof(Configuration.DeviceProperties)}");
			}
			_propertyNames = propertyNames;
		}

		internal static string Get(Device device, int propertyNumber)
		{
			// If the propertyNumber requested was out of range then return null
			// This is quite likely when less than 5 DeviceProperties were configured
			if (propertyNumber > _propertyNames.Count)
			{
				return null;
			}

			var result =
				device.CustomProperties.SingleOrDefault(p => p.Name == _propertyNames[propertyNumber - 1])
			?? device.InheritedProperties.SingleOrDefault(p => p.Name == _propertyNames[propertyNumber - 1]);
			return result?.Value;
		}
	}
}
