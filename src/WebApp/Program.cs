using NServiceBus;

namespace WebApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebAppConfig.CreateBuilder(args);
        builder.Host.UseNServiceBus(WebAppConfig.CreateNServiceBusConfiguration);
        var application = builder.Build();
        WebAppConfig.ConfigurePipeline(application);
        application.Run();
    }
}
