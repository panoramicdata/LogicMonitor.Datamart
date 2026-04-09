namespace LogicMonitor.Datamart;

/// <summary>
/// Constants for controlling loop execution intervals.
/// </summary>
public static class LoopIntervals
{
	/// <summary>
	/// Indicates the loop should execute exactly once and then stop.
	/// </summary>
	public const int ExecuteOnce = -1;

	/// <summary>
	/// Indicates the loop should execute immediately without delay.
	/// </summary>
	public const int Immediately = 0;
}
