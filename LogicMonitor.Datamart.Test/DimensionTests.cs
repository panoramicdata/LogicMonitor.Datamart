using LogicMonitor.Api.LogicModules;
using LogicMonitor.Api.Settings;
using LogicMonitor.Api.Websites;
using Microsoft.EntityFrameworkCore;

namespace LogicMonitor.Datamart.Test;

/// <summary>
/// Exercises DimensionSync for all supported entity types to verify each can be fetched and stored.
/// </summary>
/// <param name="iTestOutputHelper">xUnit output helper for test diagnostics.</param>
public class DimensionTests(ITestOutputHelper iTestOutputHelper) : TestWithOutput(iTestOutputHelper)
{
	/// <summary>
	/// Verifies that a reversed array has elements in reverse order.
	/// </summary>
	[Fact]
	public void Arrays_ReverseAsExpected()
	{
		var values = new double?[] { 1, 2, null, 4 };
		Array.Reverse(values);
		values.Should().BeEquivalentTo(new double?[] { 4, null, 2, 1 });
	}

	/// <summary>
	/// Syncs all dimension entity types and verifies the operation completes without error.
	/// </summary>
	[Fact]
	public Task GetDimensions_All_RunsSuccessfully()
		=> new DimensionSync(
				DatamartClient,
				Configuration,
				LoggerFactory,
				default)
			.ExecuteAsync(default);

	/// <summary>
	/// Syncs Resource entities and verifies the operation completes without error.
	/// </summary>
	[Fact]
	public Task GetDimensions_Resource_RunsSuccessfully()
		=> new DimensionSync(
			DatamartClient,
			Configuration,
			[
				nameof(Resource),
			],
			LoggerFactory,
			TestNotificationReceiver)
		.ExecuteAsync(default);

	/// <summary>
	/// Syncs Integration entities and verifies the operation completes without error.
	/// </summary>
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

	/// <summary>
	/// Syncs EscalationChain entities and verifies the operation completes without error.
	/// </summary>
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

	/// <summary>
	/// Syncs Website entities and verifies the operation completes without error.
	/// </summary>
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

	/// <summary>
	/// Syncs Resource and ResourceDataSourceInstance entities together and verifies the operation completes without error.
	/// </summary>
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

	/// <summary>
	/// Syncs LogicModuleUpdate entities and verifies the operation completes without error.
	/// </summary>
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

	/// <summary>
	/// Syncs DataSource entities and verifies the operation completes without error.
	/// </summary>
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

	/// <summary>
	/// Syncs Resource, ConfigSource, and ResourceDataSourceInstance entities together and verifies the operation completes without error.
	/// </summary>
	[Fact]
	public Task GetDimensions_ConfigSources_RunsSuccessfully()
		=> new DimensionSync(
				DatamartClient,
				Configuration,
				[
					nameof(Resource),
					nameof(ConfigSource),
					nameof(ResourceDataSourceInstance),
				],
				LoggerFactory,
				TestNotificationReceiver)
			.ExecuteAsync(default);
}
