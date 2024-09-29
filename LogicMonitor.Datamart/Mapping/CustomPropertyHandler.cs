namespace LogicMonitor.Datamart.Mapping;

public static class CustomPropertyHandler
{
	private const int MaxPropertyCount = 20;
	private static List<string>? _propertyNames;

	internal static void Configure(List<string> propertyNames)
	{
		_propertyNames = propertyNames
			?? throw new ArgumentNullException(nameof(propertyNames));

		if (propertyNames.Count > MaxPropertyCount)
		{
			throw new ConfigurationException($"Configure between 0 and {MaxPropertyCount} {nameof(Configuration.DeviceProperties)}");
		}
	}

	internal static string? Get(Resource device, int propertyNumber)
	{
		if (_propertyNames is null)
		{
			throw new InvalidOperationException("Call Configure() before Get()");
		}

		// If the propertyNumber requested was out of range then return null
		// This is quite likely when less than 5 DeviceProperties were configured
		if (propertyNumber > _propertyNames.Count)
		{
			return null;
		}

		var result =
			device.CustomProperties.SingleOrDefault(p => p.Name == _propertyNames[propertyNumber - 1])
			?? device.AutoProperties.SingleOrDefault(p => p.Name == _propertyNames[propertyNumber - 1])
			?? device.SystemProperties.SingleOrDefault(p => p.Name == _propertyNames[propertyNumber - 1])
			?? device.InheritedProperties.SingleOrDefault(p => p.Name == _propertyNames[propertyNumber - 1]);
		return result?.Value;
	}
}
