using LogicMonitor.Api.LogicModules;
using System.Linq;

namespace LogicMonitor.Datamart.Test;

public class DimensionTests : TestWithOutput
{
	public DimensionTests(ITestOutputHelper iTestOutputHelper) : base(iTestOutputHelper)
	{
	}

	[Fact]
	public void Arrays_ReverseAsExpected()
	{
		var values = new double?[] { 1, 2, null, 4 };
		var reversedValues = values.Reverse().ToList();
		reversedValues.Should().BeEquivalentTo(new double?[] { 4, null, 2, 1 });
	}

	[Fact]
	public async Task GetDimensions_RunsSuccessFully()
	{
		await new DimensionSync(
				DatamartClient,
				Configuration,
				new List<string>
				{
					nameof(Device),
					nameof(DeviceDataSourceInstance),
				},
				LoggerFactory)
			.ExecuteAsync(default)
			.ConfigureAwait(false);
	}
}
