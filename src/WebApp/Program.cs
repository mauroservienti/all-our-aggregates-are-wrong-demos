using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NServiceBus;

namespace WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, loggingBuilder) =>
                {
                    loggingBuilder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                })
                .UseNServiceBus(ctx =>
                {
                    var endpointConfiguration = new EndpointConfiguration("WebApp");
                    var connectionString = ctx.Configuration["NServiceBus:WebAppDatabase"];
                    if (!string.IsNullOrEmpty(connectionString))
                    {
                        endpointConfiguration.ApplyCommonConfigurationWithPersistence(connectionString, "WebApp");
                    }
                    else
                    {
                        endpointConfiguration.ApplyCommonConfiguration();
                    }

                    return endpointConfiguration;
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}
