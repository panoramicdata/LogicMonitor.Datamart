using LogicMonitor.Datamart.Interfaces;
using System.Threading;

namespace LogicMonitor.Datamart.Test;
internal class TestNotificationReceiver(ILogger logger) : INotificationReceiver
{
	private int _itemCount;
	private int _itemIndex;
	private string _stageName = string.Empty;

	public Task SetItemCountAsync(int itemCount, CancellationToken cancellationToken)
	{
		_itemCount = itemCount;
		logger.LogInformation("SetItemCountAsync: {ItemCount}", _itemCount);
		return Task.CompletedTask;
	}

	public Task SetItemIndexAsync(int itemIndex, CancellationToken cancellationToken)
	{
		_itemIndex = itemIndex;
		logger.LogInformation("SetItemIndexAsync: {ItemIndex}", _itemIndex);
		return Task.CompletedTask;
	}

	public Task SetStageNameAsync(string stageName, CancellationToken cancellationToken)
	{
		_stageName = stageName;
		_itemIndex = 1;
		_itemCount = 1;
		logger.LogInformation("SetStageNameAsync: {StageName}", _stageName);
		return Task.CompletedTask;
	}
}