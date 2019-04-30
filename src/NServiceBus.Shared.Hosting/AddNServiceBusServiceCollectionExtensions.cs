using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using System;

namespace NServiceBus.Shared.Hosting
{
    public static class AddNServiceBusServiceCollectionExtensions
    {
        public static IServiceCollection AddNServiceBus(this IServiceCollection services, string endpointName, Action<EndpointConfiguration> configuration)
        {
            var endpointConfiguration = new EndpointConfiguration(endpointName);
            configuration(endpointConfiguration);
            return services.AddNServiceBus(endpointConfiguration);
        }

        public static IServiceCollection AddNServiceBus(this IServiceCollection services, EndpointConfiguration configuration)
        {
            var management = new SessionAndConfigurationHolder(configuration);
            services.AddSingleton<IMessageSession>(provider => management.Session);
            services.AddSingleton(management);
            services.AddHostedService<EndpointManagement>();
            return services;
        }
    }
}
