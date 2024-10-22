namespace LogicMonitor.Datamart.Test;
public class AutomapperTests
{
	private static readonly MapperConfiguration _mapperConfig = new(cfg => cfg.AddMaps(typeof(DatamartClient).Assembly));

	[Fact]
	public void ValidateAutoMapperProfiles()
		=> _mapperConfig.AssertConfigurationIsValid();
}
