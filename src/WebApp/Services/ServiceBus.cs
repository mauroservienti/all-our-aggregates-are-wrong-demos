using Microsoft.Extensions.DependencyInjection;
using NServiceBus;

namespace WebApp.Services
{
    static class ServiceBus
    {
        public static void AddNServiceBus(this IServiceCollection services)
        {
            var config = new EndpointConfiguration("WebApp");

            config.ApplyCommonConfiguration(asSendOnly: true);

            var instance = Endpoint.Start(config).GetAwaiter().GetResult();
            services.AddSingleton<IMessageSession>(instance);
        }
    }
}
