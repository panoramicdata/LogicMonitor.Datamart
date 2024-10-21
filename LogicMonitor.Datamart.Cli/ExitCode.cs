namespace LogicMonitor.Datamart.Cli;

internal enum ExitCode
{
	UnexpectedException = -1,
	Ok = 0,
	ConfigurationException = 1,
	RunCancelled = 3
}
