using LogicMonitor.Datamart.Interfaces;

namespace LogicMonitor.Datamart.Test;
internal sealed partial class TestNotificationReceiver(ILogger logger) : INotificationReceiver
{
	private int _itemCount;
	private int _itemIndex;
	private string _stageName = string.Empty;

	[LoggerMessage(Level = LogLevel.Information, Message = "SetItemCountAsync: {ItemCount}")]
	private static partial void LogSetItemCount(ILogger logger, int itemCount);

	[LoggerMessage(Level = LogLevel.Information, Message = "SetItemIndexAsync: {ItemIndex}")]
	private static partial void LogSetItemIndex(ILogger logger, int itemIndex);

	[LoggerMessage(Level = LogLevel.Information, Message = "SetStageNameAsync: {StageName}")]
	private static partial void LogSetStageName(ILogger logger, string stageName);

	public Task SetItemCountAsync(int itemCount, CancellationToken cancellationToken)
	{
		_itemCount = itemCount;
		LogSetItemCount(logger, itemCount);
		return Task.CompletedTask;
	}

	public Task SetItemIndexAsync(int itemIndex, CancellationToken cancellationToken)
	{
		_itemIndex = itemIndex;
		LogSetItemIndex(logger, itemIndex);
		return Task.CompletedTask;
	}

	public Task SetStageNameAsync(string stageName, CancellationToken cancellationToken)
	{
		_stageName = stageName;
		_itemIndex = 1;
		_itemCount = 1;
		LogSetStageName(logger, stageName);
		return Task.CompletedTask;
	}
}