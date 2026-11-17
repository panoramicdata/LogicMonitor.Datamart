using LogicMonitor.Api;
using LogicMonitor.Datamart.Exceptions;

namespace LogicMonitor.Datamart.Test;

/// <summary>
/// Offline tests of <see cref="Configuration.Validate"/> and the configuration item validators.
/// </summary>
public class ConfigurationValidationTests
{
	/// <summary>
	/// InvalidConfigurations.
	/// </summary>
	public static TheoryData<string, Action<Configuration>, string> InvalidConfigurations => new()
	{
		{ "NoName", c => c.Name = " ", "Name not set" },
		{ "OffHourStart", c => c.StartDateTimeUtc = c.StartDateTimeUtc.AddMinutes(1), "hour boundary" },
		{ "FutureStart", c => c.StartDateTimeUtc = DateTimeOffset.UtcNow.Date.AddDays(2), "should not be in the future" },
		{ "NoClientOptions", c => c.LogicMonitorClientOptions = null!, "LogicMonitorClientOptions not set" },
		{ "UnknownDatabase", c => c.DatabaseType = DatabaseType.Unknown, "DatabaseType not set" },
		{ "NoServer", c => c.DatabaseServerName = string.Empty, "DatabaseServerName not set" },
		{ "NoPort", c => c.DatabaseServerPort = 0, "DatabaseServerPort not set" },
		{ "NoDatabaseName", c => c.DatabaseName = string.Empty, "DatabaseName not set" },
		{ "BatchTooSmall", c => c.DeviceDataSourceInstanceBatchSize = 0, "1..100" },
		{ "BatchTooLarge", c => c.DeviceDataSourceInstanceBatchSize = 101, "1..100" },
		{ "OffsetTooLow", c => c.MinutesOffset = -781, "-780..780" },
		{ "OffsetTooHigh", c => c.MinutesOffset = 781, "-780..780" },
		{ "ChunkTooSmall", c => c.AutoChunkSizeDays = 0, "1..31" },
		{ "ChunkTooLarge", c => c.AutoChunkSizeDays = 32, "1..31" },
		{ "DataSourceWithoutDataPoints", c => c.DataSources.Add(new DataSourceConfigurationItem { Name = "WinCPU" }), "DataPoints missing" },
		{ "DataSourceWithoutName", c => c.DataSources.Add(new DataSourceConfigurationItem { DataPoints = [new DataPointConfigurationItem { Name = "a", MeasurementUnit = "%" }] }), "Either Name or LogicMonitorId" },
		{ "DataPointWithoutUnit", c => c.DataSources.Add(new DataSourceConfigurationItem { Name = "WinCPU", DataPoints = [new DataPointConfigurationItem { Name = "a" }] }), "MeasurementUnit not set" },
	};

	/// <summary>
	/// Validate - ValidConfiguration - DoesNotThrow.
	/// </summary>
	[Fact]
	public void Validate_ValidConfiguration_DoesNotThrow()
		=> CreateValid().Validate();

	/// <summary>
	/// Validate - InvalidConfiguration - ThrowsConfigurationException.
	/// </summary>
	[Theory]
	[MemberData(nameof(InvalidConfigurations))]
	public void Validate_InvalidConfiguration_ThrowsConfigurationException(
		string scenario,
		Action<Configuration> mutate,
		string expectedMessageFragment)
	{
		var configuration = CreateValid();
		mutate(configuration);

		var exception = Assert.Throws<ConfigurationException>(configuration.Validate);

		exception.Message.Should().Contain(expectedMessageFragment, "scenario {0}", scenario);
	}

	/// <summary>
	/// Validate - BatchSizeBoundary - Accepted.
	/// </summary>
	[Theory]
	[InlineData(1)]
	[InlineData(100)]
	public void Validate_BatchSizeBoundary_Accepted(int batchSize)
	{
		var configuration = CreateValid();
		configuration.DeviceDataSourceInstanceBatchSize = batchSize;

		configuration.Validate();
	}

	/// <summary>
	/// Validate - MinutesOffsetBoundary - Accepted.
	/// </summary>
	[Theory]
	[InlineData(-780)]
	[InlineData(780)]
	public void Validate_MinutesOffsetBoundary_Accepted(int minutesOffset)
	{
		var configuration = CreateValid();
		configuration.MinutesOffset = minutesOffset;

		configuration.Validate();
	}

	/// <summary>
	/// ToString - HidesPassword.
	/// </summary>
	[Fact]
	public void ToString_HidesPassword()
	{
		var text = CreateValid().ToString();

		text.Should().Contain("db.example:5432/Datamart");
		text.Should().NotContain("hunter2");
		text.Should().Contain(new string('*', "hunter2".Length));
	}

	/// <summary>
	/// DataPointValidate - InvalidCalculation - Throws.
	/// </summary>
	[Fact]
	public void DataPointValidate_InvalidCalculation_Throws()
	{
		var dataPoint = new DataPointConfigurationItem { Name = "a", MeasurementUnit = "%", Calculation = "1 +* (" };

		var exception = Assert.Throws<ConfigurationException>(dataPoint.Validate);

		exception.Message.Should().Contain("invalid NCalc expression");
	}

	/// <summary>
	/// DataPointValidate - ValidCalculation - DoesNotThrow.
	/// </summary>
	[Fact]
	public void DataPointValidate_ValidCalculation_DoesNotThrow()
		=> new DataPointConfigurationItem { Name = "a", MeasurementUnit = "%", Calculation = "Value / 1024" }.Validate();

	/// <summary>
	/// ConfigSourceValidate - NegativeRetention - Throws.
	/// </summary>
	[Fact]
	public void ConfigSourceValidate_NegativeRetention_Throws()
	{
		var configSource = new ConfigSourceConfigurationItem { Name = "Running config", MaxAgeDays = -1 };

		var exception = Assert.Throws<ConfigurationException>(configSource.Validate);

		exception.Message.Should().Contain("Running config");
	}

	/// <summary>
	/// ConfigSourceValidate - ZeroRetention - DoesNotThrow.
	/// </summary>
	[Fact]
	public void ConfigSourceValidate_ZeroRetention_DoesNotThrow()
		=> new ConfigSourceConfigurationItem { Name = "Running config", MaxAgeDays = 0 }.Validate();

	/// <summary>
	/// DataSourceValidate - DataPointFailure - NamesTheDataPointInTheMessage.
	/// </summary>
	[Fact]
	public void DataSourceValidate_DataPointFailure_NamesTheDataPointInTheMessage()
	{
		var dataSource = new DataSourceConfigurationItem
		{
			Name = "WinCPU",
			DataPoints = [new DataPointConfigurationItem { Name = "CPUBusyPercent" }]
		};

		var exception = Assert.Throws<ConfigurationException>(dataSource.Validate);

		exception.Message.Should().Contain("MeasurementUnit not set");
		exception.InnerException.Should().BeOfType<ConfigurationException>();
	}

	private static Configuration CreateValid()
		=> new()
		{
			Name = "Contoso",
			StartDateTimeUtc = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
			DatabaseType = DatabaseType.Postgres,
			DatabaseServerName = "db.example",
			DatabaseServerPort = 5432,
			DatabaseName = "Datamart",
			DatabaseUsername = "user",
			DatabasePassword = "hunter2",
			LogicMonitorClientOptions = new LogicMonitorClientOptions
			{
				Account = "contoso",
				AccessId = "id",
				AccessKey = "key",
			},
		};
}
