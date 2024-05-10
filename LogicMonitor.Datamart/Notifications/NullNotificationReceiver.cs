using LogicMonitor.Datamart.Interfaces;

namespace LogicMonitor.Datamart.Notifications;

public class NullNotificationReceiver : INotificationReceiver
{
	public Task SetStageNameAsync(string stageName, CancellationToken cancellationToken) => Task.CompletedTask;

	public Task SetItemCountAsync(int itemCount, CancellationToken cancellationToken) => Task.CompletedTask;

	public Task SetItemIndexAsync(int itemIndex, CancellationToken cancellationToken) => Task.CompletedTask;
}
