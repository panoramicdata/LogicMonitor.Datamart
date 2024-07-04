using LogicMonitor.Datamart.Interfaces;

namespace LogicMonitor.Datamart.Services;

internal class TimeProviderService : ITimeProviderService
{
	private DateTime? _configuredDateTime;

	private DateTimeOffset? _configuredDateTimeOffset;

	public DateTime UtcNow => _configuredDateTime ?? DateTime.UtcNow;

	public DateTimeOffset UtcOffsetNow => _configuredDateTimeOffset ?? DateTimeOffset.UtcNow;

	public void SetDateTimeNow(DateTime? dateTime)
	{
		if (dateTime is not null && dateTime.Value.Kind != DateTimeKind.Utc)
		{
			throw new ArgumentException("Date Time Kind is not UTC");
		}

		_configuredDateTime = dateTime;
		_configuredDateTimeOffset = dateTime != null ? new DateTimeOffset(dateTime.Value) : null;
	}
}
