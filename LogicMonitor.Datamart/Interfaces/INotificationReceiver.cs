namespace LogicMonitor.Datamart.Interfaces;

/// <summary>
/// Defines a receiver for datamart sync progress notifications.
/// </summary>
public interface INotificationReceiver
{
	/// <summary>
	/// Sets the name of the current processing stage.
	/// </summary>
	/// <param name="stageName">The name of the stage.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	public Task SetStageNameAsync(string stageName, CancellationToken cancellationToken);

	/// <summary>
	/// Sets the total number of items to process in the current stage.
	/// </summary>
	/// <param name="itemCount">The total item count.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	public Task SetItemCountAsync(int itemCount, CancellationToken cancellationToken);

	/// <summary>
	/// Sets the current item index being processed.
	/// </summary>
	/// <param name="itemIndex">The zero-based item index.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	public Task SetItemIndexAsync(int itemIndex, CancellationToken cancellationToken);

	/// <summary>
	/// Reports a problem the sync found and carried on past, such as a DataSource configuration it could not use, so that the caller
	/// can show it; the default ignores it.
	/// </summary>
	/// <param name="message">The problem, written for the person who owns the configuration.</param>
	/// <param name="cancellationToken">A cancellation token.</param>
	public Task ReportWarningAsync(string message, CancellationToken cancellationToken) => Task.CompletedTask;
}
