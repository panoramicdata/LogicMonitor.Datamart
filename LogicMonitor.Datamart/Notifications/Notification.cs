namespace LogicMonitor.Datamart.Notifications;

/// <summary>
/// Represents a progress notification from the datamart sync process.
/// </summary>
public class Notification
{
	/// <summary>
	/// The name of the current processing stage.
	/// </summary>
	public required string CurrentStage { get; set; }

	/// <summary>
	/// The name or description of the current item being processed.
	/// </summary>
	public required string CurrentItem { get; set; }

	/// <summary>
	/// The zero-based index of the current item within the stage.
	/// </summary>
	public required int Item { get; set; }

	/// <summary>
	/// The total number of items to process in the current stage.
	/// </summary>
	public required int ItemCount { get; set; }

	/// <summary>
	/// An optional message providing additional details about the notification.
	/// </summary>
	public required string? Message { get; set; }

	/// <summary>
	/// The one-based index of the current stage.
	/// </summary>
	public required int Stage { get; set; }

	/// <summary>
	/// The total number of stages in the sync process.
	/// </summary>
	public required int StageCount { get; set; }

	/// <inheritdoc />
	public override string ToString() => Message is null
		? $"{CurrentStage} Stage {Stage}/{StageCount}, Item {Item}/{ItemCount}: {Message}"
		: $"{CurrentStage} Stage {Stage}/{StageCount}, Item {Item}/{ItemCount}";
}
