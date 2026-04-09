using LogicMonitor.Datamart.Interfaces;

namespace LogicMonitor.Datamart.Notifications;

/// <summary>
/// A no-op implementation of <see cref="INotificationReceiver"/> that discards all notifications.
/// </summary>
public class NullNotificationReceiver : INotificationReceiver
{
	/// <inheritdoc />
	public Task SetStageNameAsync(string stageName, CancellationToken cancellationToken) => Task.CompletedTask;

	/// <inheritdoc />
	public Task SetItemCountAsync(int itemCount, CancellationToken cancellationToken) => Task.CompletedTask;

	/// <inheritdoc />
	public Task SetItemIndexAsync(int itemIndex, CancellationToken cancellationToken) => Task.CompletedTask;
}
