namespace LogicMonitor.Datamart.Test;

/// <summary>
/// Verifies truncation behavior for mapped string fields.
/// </summary>
/// <param name="iTestOutputHelper">xUnit output helper used for test logging.</param>
public class AutoMapperTruncateTests(ITestOutputHelper iTestOutputHelper) : TestWithOutput(iTestOutputHelper)
{
	/// <summary>
	/// Verifies over-length source fields are truncated to destination schema limits during mapping.
	/// </summary>
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
		destination.AckedBy.Should().Be("11111111112222222222333333333344444444445555555555");
		destination.LogicMonitorId.Should().Be("11111111112222222222");
	}
}
