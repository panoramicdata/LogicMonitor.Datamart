using LogicMonitor.Api.LogicModules;
using LogicMonitor.Api.Settings;
using LogicMonitor.Api.Websites;
using Microsoft.EntityFrameworkCore;

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
	public Task GetDimensions_All_RunsSuccessfully()
		=> new DimensionSync(
				DatamartClient,
				Configuration,
				LoggerFactory,
				default)
			.ExecuteAsync(default);

	[Fact]
	public Task GetDimensions_Device_RunsSuccessfully()
		=> new DimensionSync(
			DatamartClient,
			Configuration,
			[
				nameof(Resource),
			],
			LoggerFactory,
			TestNotificationReceiver)
		.ExecuteAsync(default);

	[Fact]
	public Task GetDimensions_Integration_RunsSuccessfully()
		=> new DimensionSync(
			DatamartClient,
			Configuration,
			[
				nameof(Integration),
			],
			LoggerFactory,
			TestNotificationReceiver)
		.ExecuteAsync(default);

	[Fact]
	public Task GetDimensions_EscalationChain_RunsSuccessfully()
		=> new DimensionSync(
			DatamartClient,
			Configuration,
			[
				nameof(EscalationChain),
			],
			LoggerFactory,
			TestNotificationReceiver)
		.ExecuteAsync(default);

	[Fact]
	public Task GetDimensions_Website_RunsSuccessfully()
		=> new DimensionSync(
				DatamartClient,
				Configuration,
				[
					nameof(Website),
				],
				LoggerFactory,
				TestNotificationReceiver)
			.ExecuteAsync(default);

	[Fact]
	public Task GetDimensions_DeviceAndDeviceDataSourceInstance_RunsSuccessfully()
		=> new DimensionSync(
				DatamartClient,
				Configuration,
				[
					nameof(Resource),
					nameof(ResourceDataSourceInstance),
				],
				LoggerFactory,
				TestNotificationReceiver)
			.ExecuteAsync(default);

	[Fact]
	[Trait("Long Running Test", "")]
	public Task GetDimensions_LogicModuleUpdates_RunsSuccessfully()
		=> new DimensionSync(
				DatamartClient,
				Configuration,
				[
					nameof(LogicModuleUpdate),
				],
				LoggerFactory,
				TestNotificationReceiver)
			.ExecuteAsync(default);

	[Fact]
	public Task GetDimensions_DataSources_RunsSuccessfully()
		=> new DimensionSync(
				DatamartClient,
				Configuration,
				[
					nameof(DataSource)
				],
				LoggerFactory,
				TestNotificationReceiver)
			.ExecuteAsync(default);
}
