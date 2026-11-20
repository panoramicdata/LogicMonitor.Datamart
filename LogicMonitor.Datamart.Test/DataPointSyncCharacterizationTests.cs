using LogicMonitor.Api.LogicModules;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace LogicMonitor.Datamart.Test;

/// <summary>
/// Offline characterization tests of the DataSource and DataPoint dimension updates against an in-memory database.
/// </summary>
public class DataPointSyncCharacterizationTests
{
	private static DataSource ApiDataSource(int id, string name, params (int Id, string Name, string Alert, string Description)[] dataPoints)
		=> new()
		{
			Id = id,
			Name = name,
			DataSourceDataPoints =
			[
				.. dataPoints.Select(dp => new DataPoint
				{
					Id = dp.Id,
					Name = dp.Name,
					AlertExpression = dp.Alert,
					Description = dp.Description,
				})
			],
		};

	private static DataSourceDataPointStoreItem[] StoredDataPoints(DatamartClient client)
	{
		using var context = client.GetContext();
		return [.. context.DataSourceDataPoints.AsNoTracking().OrderBy(dp => dp.Name)];
	}

	private static async Task SeedDataSourceAsync(DatamartClient client, params DataSource[] dataSources)
	{
		using var context = client.GetContext();
		foreach (var dataSource in dataSources)
		{
			var storeItem = DatamartClient.MapperInstance.Map<DataSource, DataSourceStoreItem>(dataSource);
			storeItem.Id = Guid.NewGuid();
			context.DataSources.Add(storeItem);
		}

		await context.SaveChangesAsync(TestContext.Current.CancellationToken);
	}

	private static async Task UpdateAsync(DatamartClient client, params DataSource[] apiDataSources)
	{
		using var context = client.GetContext();
		await client.UpdateDataPointsAsync(context, [.. apiDataSources], TestContext.Current.CancellationToken);
	}

	private static DatamartClient ClientFor(params DataSourceConfigurationItem[] dataSources)
		=> InMemoryDatamart.CreateClient(configuration => configuration.DataSources = [.. dataSources]);

	private static DataSourceConfigurationItem ConfigFor(string name, params DataPointConfigurationItem[] dataPoints)
		=> new() { Name = name, DataPoints = [.. dataPoints] };

	/// <summary>
	/// A configured DataPoint missing from the database is added, taking the description and alert expression from the API.
	/// </summary>
	[Fact]
	public async Task UpdateDataPoints_NewDataPoint_IsAddedWithApiValuesAndConfigOnlyFields()
	{
		var client = ClientFor(ConfigFor("WinCPU", new DataPointConfigurationItem
		{
			Name = "CPUBusyPercent",
			MeasurementUnit = "%",
			Tags = "t",
			Property1 = "p1",
			Property10 = "p10",
			PercentageAvailabilityCalculation = "PercentUpTime",
			ResyncTimeSeriesData = true,
		}));
		var api = ApiDataSource(1, "WinCPU", (7, "CPUBusyPercent", "> 90", "API description"));
		await SeedDataSourceAsync(client, api);

		await UpdateAsync(client, api);

		var stored = StoredDataPoints(client).Should().ContainSingle().Subject;
		stored.Name.Should().Be("CPUBusyPercent");
		stored.LogicMonitorId.Should().Be(7);
		stored.Description.Should().Be("API description");
		stored.GlobalAlertExpression.Should().Be("> 90");
		stored.MeasurementUnit.Should().Be("%");
		stored.Tags.Should().Be("t");
		stored.Property1.Should().Be("p1");
		stored.Property10.Should().Be("p10");
		stored.PercentageAvailabilityCalculation.Should().Be("PercentUpTime");
		stored.ResyncTimeSeriesData.Should().BeTrue();
		stored.Calculation.Should().BeEmpty();
	}

	/// <summary>
	/// A description and alert expression in the configuration win over the API values.
	/// </summary>
	[Fact]
	public async Task UpdateDataPoints_ConfigDescriptionAndAlertExpression_OverrideApi()
	{
		var client = ClientFor(ConfigFor("WinCPU", new DataPointConfigurationItem
		{
			Name = "CPUBusyPercent",
			MeasurementUnit = "u",
			Description = "Config description",
			GlobalAlertExpression = "< 5",
		}));
		var api = ApiDataSource(1, "WinCPU", (7, "CPUBusyPercent", "> 90", "API description"));
		await SeedDataSourceAsync(client, api);

		await UpdateAsync(client, api);

		var stored = StoredDataPoints(client).Should().ContainSingle().Subject;
		stored.Description.Should().Be("Config description");
		stored.GlobalAlertExpression.Should().Be("< 5");
	}

	/// <summary>
	/// A second update changes the existing row in place instead of adding another.
	/// </summary>
	[Fact]
	public async Task UpdateDataPoints_ExistingDataPoint_IsUpdatedInPlace()
	{
		var dataPoint = new DataPointConfigurationItem { Name = "CPUBusyPercent", MeasurementUnit = "%", Property2 = "old" };
		var client = ClientFor(ConfigFor("WinCPU", dataPoint));
		var api = ApiDataSource(1, "WinCPU", (7, "CPUBusyPercent", "> 90", "API description"));
		await SeedDataSourceAsync(client, api);
		await UpdateAsync(client, api);

		dataPoint.MeasurementUnit = "pct";
		dataPoint.Property2 = "new";
		await UpdateAsync(client, ApiDataSource(1, "WinCPU", (7, "CPUBusyPercent", "> 80", "New API description")));

		var stored = StoredDataPoints(client).Should().ContainSingle().Subject;
		stored.MeasurementUnit.Should().Be("pct");
		stored.Property2.Should().Be("new");
		stored.GlobalAlertExpression.Should().Be("> 80");
		stored.Description.Should().Be("New API description");
		stored.LogicMonitorId.Should().Be(7);
	}

	/// <summary>
	/// A DataPoint absent from the API is skipped unless a calculation is configured.
	/// </summary>
	[Fact]
	public async Task UpdateDataPoints_DataPointMissingFromApi_IsAddedOnlyWithCalculation()
	{
		var client = ClientFor(ConfigFor(
			"WinCPU",
			new DataPointConfigurationItem { Name = "Missing", MeasurementUnit = "u" },
			new DataPointConfigurationItem { Name = "Calculated", MeasurementUnit = "u", Calculation = "a + b" }));
		var api = ApiDataSource(1, "WinCPU", (7, "CPUBusyPercent", "> 90", "API description"));
		await SeedDataSourceAsync(client, api);

		await UpdateAsync(client, api);

		var stored = StoredDataPoints(client).Should().ContainSingle().Subject;
		stored.Name.Should().Be("Calculated");
		stored.LogicMonitorId.Should().Be(0);
		stored.Calculation.Should().Be("a + b");
		stored.Description.Should().BeEmpty();
		stored.GlobalAlertExpression.Should().BeEmpty();
	}

	/// <summary>
	/// A configured DataSource missing from the API, or from the database, adds nothing.
	/// </summary>
	[Fact]
	public async Task UpdateDataPoints_DataSourceMissingFromApiOrDatabase_AddsNothing()
	{
		var client = ClientFor(
			ConfigFor("NotInApi", new DataPointConfigurationItem { Name = "CPUBusyPercent", MeasurementUnit = "u" }),
			ConfigFor("NotInDatabase", new DataPointConfigurationItem { Name = "CPUBusyPercent", MeasurementUnit = "u" }));
		var inApiOnly = ApiDataSource(2, "NotInDatabase", (7, "CPUBusyPercent", "> 90", "d"));
		await UpdateAsync(client, inApiOnly);

		StoredDataPoints(client).Should().BeEmpty();
	}

	/// <summary>
	/// A configured LogicMonitor ID selects the API DataSource by ID, and a name mismatch in the database adds nothing.
	/// </summary>
	[Fact]
	public async Task UpdateDataPoints_ConfiguredById_ResolvesByIdAndRejectsNameMismatch()
	{
		var matching = new DataSourceConfigurationItem
		{
			LogicMonitorId = 1,
			Name = "WinCPU",
			DataPoints = [new DataPointConfigurationItem { Name = "CPUBusyPercent", MeasurementUnit = "u" }],
		};
		var mismatching = new DataSourceConfigurationItem
		{
			LogicMonitorId = 2,
			Name = "Other",
			DataPoints = [new DataPointConfigurationItem { Name = "Disk", MeasurementUnit = "u" }],
		};
		var client = ClientFor(matching, mismatching);
		var cpu = ApiDataSource(1, "WinCPU", (7, "CPUBusyPercent", "> 90", "d"));
		var disk = ApiDataSource(2, "WinDisk", (8, "Disk", "> 1", "d"));
		await SeedDataSourceAsync(client, cpu, disk);

		await UpdateAsync(client, cpu, disk);

		StoredDataPoints(client).Select(dp => dp.Name).Should().Equal("CPUBusyPercent");
	}

	/// <summary>
	/// The DataSource lookup resolves by ID, by name (taking the first of duplicates), and returns null when absent or inconsistent.
	/// </summary>
	[Fact]
	public async Task ResolveDataSourceStoreItem_CoversIdNameAndMismatchCases()
	{
		var client = ClientFor();
		await SeedDataSourceAsync(client, ApiDataSource(1, "WinCPU"), ApiDataSource(2, "Dup"), ApiDataSource(3, "Dup"));
		using var context = client.GetContext();
		var logger = NullLogger.Instance;
		var token = TestContext.Current.CancellationToken;

		(await client.ResolveDataSourceStoreItemAsync(context, new DataSourceConfigurationItem { LogicMonitorId = 1 }, logger, token))!
			.Name.Should().Be("WinCPU");
		(await client.ResolveDataSourceStoreItemAsync(context, new DataSourceConfigurationItem { LogicMonitorId = 1, Name = "WinCPU" }, logger, token))
			.Should().NotBeNull();
		(await client.ResolveDataSourceStoreItemAsync(context, new DataSourceConfigurationItem { LogicMonitorId = 1, Name = "Wrong" }, logger, token))
			.Should().BeNull();
		(await client.ResolveDataSourceStoreItemAsync(context, new DataSourceConfigurationItem { LogicMonitorId = 99 }, logger, token))
			.Should().BeNull();
		(await client.ResolveDataSourceStoreItemAsync(context, new DataSourceConfigurationItem { Name = "WinCPU" }, logger, token))!
			.LogicMonitorId.Should().Be(1);
		(await client.ResolveDataSourceStoreItemAsync(context, new DataSourceConfigurationItem { Name = "Dup" }, logger, token))
			.Should().NotBeNull();
		(await client.ResolveDataSourceStoreItemAsync(context, new DataSourceConfigurationItem { Name = "Nope" }, logger, token))
			.Should().BeNull();
	}
}
