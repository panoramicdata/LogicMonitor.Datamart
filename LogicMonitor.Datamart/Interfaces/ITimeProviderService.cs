namespace LogicMonitor.Datamart.Interfaces;

public interface ITimeProviderService
{
	public void SetDateTimeNow(DateTime? dateTime);

	public DateTime UtcNow { get; }

	public DateTimeOffset UtcOffsetNow { get; }
}