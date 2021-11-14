using LogicMonitor.Datamart.Service.Config;
using Serilog.Debugging;

namespace LogicMonitor.Datamart.Service;

public class Program
{
	public async static Task Main(string[] args)
	{
		SelfLog.Enable(msg => Console.Error.WriteLine(msg));

		try
		{
			IHost host = Host.CreateDefaultBuilder(args)
				.ConfigureServices((context, services) =>
				{
					services
						.AddHostedService<Worker>()
						.AddOptions()
						.Configure<AppConfiguration>(context.Configuration.GetSection("Configuration"));
				})
				.Build();
			await host.RunAsync();
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine(ex.ToString());
			var dumpPath = Path.GetTempPath();
			var dumpFile = $"{ThisAssembly.AssemblyName}-Error-{Guid.NewGuid()}.txt";
			string dumpFileFullPath = Path.Combine(dumpPath, dumpFile);
			File.WriteAllText(dumpFileFullPath, ex.ToString());
			Console.WriteLine($"Exception written to file: {dumpFileFullPath}");
		}
	}
}