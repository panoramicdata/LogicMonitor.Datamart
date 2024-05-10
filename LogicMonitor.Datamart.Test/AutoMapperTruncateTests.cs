using LogicMonitor.Datamart.Mapping;

namespace LogicMonitor.Datamart.Test;

public class AutoMapperTruncateTests(ITestOutputHelper iTestOutputHelper) : TestWithOutput(iTestOutputHelper)
{
	[Fact]
	public void Test()
	{

		var config = new MapperConfiguration(cfg =>
		{
			cfg.AddProfile<AlertProfile>();
			cfg.AddProfile<AlertRuleProfile>();
			cfg.AddProfile<CollectorGroupProfile>();
			cfg.AddProfile<CollectorProfile>();
			cfg.AddProfile<ConfigSourceProfile>();
			cfg.AddProfile<DataSourceProfile>();
			cfg.AddProfile<DeviceDataSourceInstanceProfile>();
			cfg.AddProfile<DeviceDataSourceProfile>();
			cfg.AddProfile<DeviceGroupProfile>();
			cfg.AddProfile<DeviceProfile>();
			cfg.AddProfile<EscalationChainProfile>();
			cfg.AddProfile<EventSourceProfile>();
			cfg.AddProfile<LogProfile>();
			cfg.AddProfile<WebsiteGroupProfile>();
			cfg.AddProfile<WebsiteProfile>();
		});
		config.AssertConfigurationIsValid();
	}

	[Fact]
	public void ResolveAndTruncate_LongValue_TruncatedValue()
	{
		var source = new Alert()
		{
			Id = "111111111122222222223333333333444444444455555555556666666666",
			AckedBy = "111111111122222222223333333333444444444455555555556666666666",
			MonitorObjectId = "111111",
		};
		var destination = DatamartClient.MapperInstance.Map<Alert, AlertStoreItem>(source);
		Assert.Equal("11111111112222222222333333333344444444445555555555", destination.AckedBy);
		Assert.Equal("11111111112222222222", destination.LogicMonitorId);
	}
}
