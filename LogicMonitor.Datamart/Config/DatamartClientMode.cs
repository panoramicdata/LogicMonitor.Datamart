namespace LogicMonitor.Datamart.Config;

/// <summary>
/// Specifies the operating mode of the datamart client.
/// </summary>
public enum DatamartClientMode
{
	/// <summary>
	/// High-resolution mode: collects detailed time series data.
	/// </summary>
	HighResolution = 0,

	/// <summary>
	/// Low-resolution mode: collects aggregated time series data.
	/// </summary>
	LowResolution = 1,
}