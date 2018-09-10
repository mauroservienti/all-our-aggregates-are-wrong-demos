using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using NServiceBus.Shared;

namespace Sales.Api.Services
{
    static class ServiceBus
    {
        public static void AddNServiceBus(this IServiceCollection services)
        {
            var config = new EndpointConfiguration("Sales.Api");

            config.SendOnly();
            config.ApplyCommonConfiguration();

            var instance = Endpoint.Start(config).GetAwaiter().GetResult();
            services.AddSingleton<IMessageSession>(instance);
        }
    }
}
