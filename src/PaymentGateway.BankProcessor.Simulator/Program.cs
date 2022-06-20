namespace PaymentGateway.BankProcessor.Simulator
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			IWebHost host = new WebHostBuilder()
			   .UseKestrel()
			   .ConfigureKestrel(options => options.AddServerHeader = false)
			   .UseContentRoot(Directory.GetCurrentDirectory())
			   .ConfigureAppConfiguration((hostingContext, config) =>
			   {
				   config
					   .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
					   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
					   .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true)
					   .AddJsonFile("appsettings.development.json", optional: true);
			   })
			   .ConfigureLogging((hostingContext, logging) =>
			   {
				   logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
				   logging.AddConsole();
				   logging.AddDebug();
				   logging.AddEventSourceLogger();
			   })
			   .UseStartup<Startup>()
			   .Build();

			await host.RunAsync();
		}
	}
}