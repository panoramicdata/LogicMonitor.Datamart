namespace LogicMonitor.Datamart.Test;

/// <summary>
/// Validates that all AutoMapper profiles in the Datamart assembly are internally consistent.
/// </summary>
public class AutomapperTests
{
	private static readonly MapperConfiguration _mapperConfig = new(cfg => cfg.AddMaps(typeof(DatamartClient).Assembly));

	/// <summary>
	/// Asserts that AutoMapper profile configuration loads without mapping conflicts.
	/// </summary>
	[Fact]
	public void ValidateAutoMapperProfiles()
		=> _mapperConfig.AssertConfigurationIsValid();
}
