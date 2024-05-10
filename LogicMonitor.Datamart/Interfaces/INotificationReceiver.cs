namespace LogicMonitor.Datamart.Interfaces;
public interface INotificationReceiver
{
	public Task SetStageNameAsync(string stageName, CancellationToken cancellationToken);

	public Task SetItemCountAsync(int itemCount, CancellationToken cancellationToken);

	public Task SetItemIndexAsync(int itemIndex, CancellationToken cancellationToken);
}
