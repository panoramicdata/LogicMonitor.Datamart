using LogicMonitor.Api.LogicModules;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LogicMonitor.Datamart.Test;

public class DimensionTests(ITestOutputHelper iTestOutputHelper) : TestWithOutput(iTestOutputHelper)
{
	[Fact]
	public void Arrays_ReverseAsExpected()
	{
		var values = new double?[] { 1, 2, null, 4 };
		var reversedValues = values.Reverse().ToList();
		reversedValues.Should().BeEquivalentTo(new double?[] { 4, null, 2, 1 });
	}

	[Fact]
	public async Task GetDimensions_All_RunsSuccessfully()
	{
		await new DimensionSync(
				DatamartClient,
				Configuration,
				LoggerFactory,
				default)
			.ExecuteAsync(default)
			.ConfigureAwait(true);
	}

	[Fact]
	public async Task GetDimensions_Device_RunsSuccessfully()
	{
		await new DimensionSync(
				DatamartClient,
				Configuration,
				[
					nameof(Resource),
				],
				LoggerFactory,
				TestNotificationReceiver)
			.ExecuteAsync(default)
			.ConfigureAwait(true);
	}

	[Fact]
	public async Task GetDimensions_DeviceAndDeviceDataSourceInstance_RunsSuccessfully()
	{
		await new DimensionSync(
				DatamartClient,
				Configuration,
				[
					nameof(Resource),
					nameof(ResourceDataSourceInstance),
				],
				LoggerFactory,
				TestNotificationReceiver)
			.ExecuteAsync(default)
			.ConfigureAwait(true);
	}

	[Fact]
	[Trait("Long Running Test", "")]
	public async Task GetDimensions_LogicModuleUpdates_RunsSuccessfully()
	{
		await new DimensionSync(
				DatamartClient,
				Configuration,
				[
					nameof(LogicModuleUpdate),
				],
				LoggerFactory,
				TestNotificationReceiver)
			.ExecuteAsync(default)
			.ConfigureAwait(true);
	}

	[Fact]
	public async Task GetDimensions_DataSources_RunsSuccessfully()
	{
		await new DimensionSync(
				DatamartClient,
				Configuration,
				[
					nameof(DataSource)
				],
				LoggerFactory,
				TestNotificationReceiver)
			.ExecuteAsync(default)
			.ConfigureAwait(true);
	}
}
