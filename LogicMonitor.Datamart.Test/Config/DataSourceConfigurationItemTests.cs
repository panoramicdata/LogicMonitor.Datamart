using LogicMonitor.Datamart.Exceptions;

namespace LogicMonitor.Datamart.Test.Config;

/// <summary>
/// Unit tests for <see cref="DataSourceConfigurationItem"/> validation logic.
/// No live data or network access required.
/// </summary>
public class DataSourceConfigurationItemTests
{
    private static List<DataPointConfigurationItem> OneDataPoint =>
    [
        new DataPointConfigurationItem { Name = "dp1", MeasurementUnit = "ms" }
    ];

    /// <summary>Name-only config (backward compatible) passes validation.</summary>
    [Fact]
    public void WhenNameOnly_Validate_Succeeds()
    {
        var item = new DataSourceConfigurationItem { Name = "MyDS", DataPoints = OneDataPoint };
        var act = () => item.Validate();
        act.Should().NotThrow();
    }

    /// <summary>LogicMonitorId-only config (no name) passes validation.</summary>
    [Fact]
    public void WhenLogicMonitorIdOnly_Validate_Succeeds()
    {
        var item = new DataSourceConfigurationItem { LogicMonitorId = 42, DataPoints = OneDataPoint };
        var act = () => item.Validate();
        act.Should().NotThrow();
    }

    /// <summary>Config with both Name and LogicMonitorId passes validation.</summary>
    [Fact]
    public void WhenBothNameAndLogicMonitorId_Validate_Succeeds()
    {
        var item = new DataSourceConfigurationItem { Name = "MyDS", LogicMonitorId = 42, DataPoints = OneDataPoint };
        var act = () => item.Validate();
        act.Should().NotThrow();
    }

    /// <summary>Config with neither Name nor LogicMonitorId throws.</summary>
    [Fact]
    public void WhenNeitherNameNorLogicMonitorId_Validate_Throws()
    {
        var item = new DataSourceConfigurationItem { DataPoints = OneDataPoint };
        var act = () => item.Validate();
        act.Should().Throw<ConfigurationException>();
    }

    /// <summary>Config with a name but no DataPoints throws.</summary>
    [Fact]
    public void WhenNoDataPoints_Validate_Throws()
    {
        var item = new DataSourceConfigurationItem { Name = "MyDS", DataPoints = [] };
        var act = () => item.Validate();
        act.Should().Throw<ConfigurationException>();
    }

    /// <summary>Config with LogicMonitorId but no DataPoints throws.</summary>
    [Fact]
    public void WhenLogicMonitorIdOnlyAndNoDataPoints_Validate_Throws()
    {
        var item = new DataSourceConfigurationItem { LogicMonitorId = 42, DataPoints = [] };
        var act = () => item.Validate();
        act.Should().Throw<ConfigurationException>();
    }
}
