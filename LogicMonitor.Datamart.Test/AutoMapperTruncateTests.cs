using AutoMapper;
using LogicMonitor.Api.Alerts;
using LogicMonitor.Datamart.Mapping;
using LogicMonitor.Datamart.Models;
using Xunit;
using Xunit.Abstractions;

namespace LogicMonitor.Datamart.Test
{
	public class AutoMapperTruncateTests : TestWithOutput
	{
		public AutoMapperTruncateTests(ITestOutputHelper iTestOutputHelper)
		 : base(iTestOutputHelper)
		{
		}

		[Fact]
		public void Test()
		{
			Mapper.Initialize(x =>
			{
				x.AddProfile<AlertProfile>();
				x.AddProfile<AlertRuleProfile>();
				x.AddProfile<CollectorGroupProfile>();
				x.AddProfile<CollectorProfile>();
				x.AddProfile<ConfigSourceProfile>();
				x.AddProfile<DataSourceProfile>();
				x.AddProfile<DeviceDataSourceInstanceProfile>();
				x.AddProfile<DeviceDataSourceProfile>();
				x.AddProfile<DeviceProfile>();
				x.AddProfile<EscalationChainProfile>();
				x.AddProfile<EventSourceProfile>();
				x.AddProfile<WebsiteGroupProfile>();
				x.AddProfile<WebsiteProfile>();
			});
			Mapper.Configuration.AssertConfigurationIsValid();
		}

		[Fact]
		public void ResolveAndTruncate_LongValue_TruncatedValue()
		{
			var source = new Alert()
			{
				Id = "111111111122222222223333333333444444444455555555556666666666",
				AckedBy = "111111111122222222223333333333444444444455555555556666666666"
			};
			var destination = DatamartClient.MapperInstance.Map<Alert, AlertStoreItem>(source);
			Assert.Equal("11111111112222222222333333333344444444445555555555", destination.AckedBy);
			Assert.Equal("11111111112222222222", destination.Id);
		}
	}
}
