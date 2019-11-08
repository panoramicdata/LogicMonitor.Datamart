using AutoMapper;
using LogicMonitor.Api.Alerts;
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
		public void Test() => Mapper.Configuration.AssertConfigurationIsValid();

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
