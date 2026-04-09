namespace LogicMonitor.Datamart.Interfaces;

/// <summary>
/// Provides an abstraction over the system clock for testability.
/// </summary>
public interface ITimeProviderService
{
	/// <summary>
	/// Sets an override for the current UTC time, or null to use the real clock.
	/// </summary>
	/// <param name="dateTime">The fake time, or null to reset.</param>
	public void SetDateTimeNow(DateTime? dateTime);

	/// <summary>
	/// Gets the current UTC date and time.
	/// </summary>
	public DateTime UtcNow { get; }

	/// <summary>
	/// Gets the current UTC date and time as a DateTimeOffset.
	/// </summary>
	public DateTimeOffset UtcOffsetNow { get; }
}