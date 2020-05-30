using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Silgred.Shared.Services;

namespace Silgred.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .CaptureStartupErrors(true)
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();

                    if (bool.TryParse(hostingContext.Configuration["ApplicationOptions:EnableWindowsEventLog"],
                        out var enableEventLog))
                        if (OSUtils.IsWindows && enableEventLog)
                            logging.AddEventLog();
                });
        }
    }
}