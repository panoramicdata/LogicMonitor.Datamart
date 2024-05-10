namespace LogicMonitor.Datamart.Notifications;
public class Notification
{
	public required string CurrentStage { get; set; }

	public required string CurrentItem { get; set; }

	public required int Item { get; set; }

	public required int ItemCount { get; set; }

	public required string? Message { get; set; }

	public required int Stage { get; set; }

	public required int StageCount { get; set; }

	public override string ToString() => Message is null
		? $"{CurrentStage} Stage {Stage}/{StageCount}, Item {Item}/{ItemCount}: {Message}"
		: $"{CurrentStage} Stage {Stage}/{StageCount}, Item {Item}/{ItemCount}";
}
