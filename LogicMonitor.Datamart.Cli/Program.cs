using LogicMonitor.Datamart.Config;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using PanoramicData.HealthChecks.Core;
using Serilog;
using Serilog.Debugging;
using System.Diagnostics;

namespace LogicMonitor.Datamart.Cli;

public static class Program
{
	private const string ConfigFileEnvironmentVariable = "CONFIG_FILE";

	public static async Task<int> Main(string[] args)
	{
		var fileVersion = FileVersionInfo.GetVersionInfo(typeof(Program).Assembly.Location);
		Console.Title = $"LogicMonitor.Datamart.Cli {fileVersion.FileMajorPart}.{fileVersion.FileMinorPart}.{fileVersion.FileBuildPart}";

		try
		{
			SelfLog.Enable(msg =>
			{
				Debug.WriteLine(msg);
				Console.Error.WriteLine(msg);
			});

			var builder = WebApplication.CreateBuilder(args);

			var configFileFromEnvironmentVariables = Environment.GetEnvironmentVariable(ConfigFileEnvironmentVariable);

			if (!string.IsNullOrWhiteSpace(configFileFromEnvironmentVariables))
			{
				builder.Configuration.AddJsonFile(configFileFromEnvironmentVariables);
			}

			builder.Configuration
#if DEBUG
				.AddJsonFile("../../../appsettings.json", true)
#else
				.AddJsonFile("appsettings.json", true)
#endif
				.AddEnvironmentVariables()
				.AddCommandLine(args)
				.AddUserSecrets<Configuration>();

			builder.Services
				.Configure<Configuration>(builder.Configuration.GetSection("Configuration"));

			_ = builder
				.Host
				.UseSerilog((hostingContext, _, configuration) => configuration
					.ReadFrom
					.Configuration(hostingContext.Configuration)
				);

			// Add the desired health checks
			_ = builder
				.Services
				.AddHealthChecks()
				.AddVersions(
					"versions",
					HealthStatus.Unhealthy,
					["application", "versions"],
					TimeSpan.FromSeconds(5))
				.AddSystemMemory(
					1000,
					name: "System Memory",
					HealthStatus.Unhealthy,
					["system", "memory"],
					timeout: TimeSpan.FromSeconds(5)
				)
				.AddAssemblies(
					"assemblies",
					HealthStatus.Unhealthy,
					["application", "assemblies"],
					TimeSpan.FromSeconds(5)
				)
				;

			builder.Services
				.AddSingleton<Application>()
				.AddSingleton<IHostLifetime, Application>();

			var app = builder.Build();
			_ = app.MapHealthChecks("/health", new HealthCheckOptions
			{
				ResponseWriter = ResponseWriters.JsonResponseWriter
			}
			);

			await app.RunAsync();

			return (int)ExitCode.Ok;
		}
		catch (OperationCanceledException)
		{
			// This is normal for using CTRL+C to exit the run
			await Console
				.Out
				.WriteLineAsync("** Execution run cancelled - exiting **")
				.ConfigureAwait(false);
			return (int)ExitCode.RunCancelled;
		}
		catch (Exception ex)
		{
			await Console
				.Error
				.WriteLineAsync(ex.ToString())
				.ConfigureAwait(false);
			var dumpFilePath = Path.Combine(Path.GetTempPath(), $"{ThisAssembly.AssemblyName}-Error-{Guid.NewGuid()}.txt");
			await File.WriteAllTextAsync(dumpFilePath, ex.ToString(), default)
				.ConfigureAwait(false);
			await Console
				.Error
				.WriteLineAsync($"Error written to {dumpFilePath}")
				.ConfigureAwait(false);
			return (int)ExitCode.UnexpectedException;
		}
	}
}
