using Microsoft.Extensions.DependencyInjection;
using System;

namespace NServiceBus.Shared.Hosting
{
    public static class AddNServiceBusServiceCollectionExtensions
    {
        static void AddRequiredInfrastructure(this IServiceCollection services, EndpointConfiguration configuration)
        {
            var holder = new SessionAndConfigurationHolder(configuration);
            services.AddSingleton(provider => holder.Session);
            services.AddSingleton(holder);
            services.AddHostedService<EndpointManagement>();
        }

        public static IServiceProvider AddNServiceBus(this IServiceCollection services, string endpointName, Func<EndpointConfiguration, IServiceProvider> configuration)
        {
            var endpointConfiguration = new EndpointConfiguration(endpointName);
            services.AddRequiredInfrastructure(endpointConfiguration);

            return configuration(endpointConfiguration);
        }

        public static void AddNServiceBus(this IServiceCollection services, string endpointName, Action<EndpointConfiguration> configuration)
        {
            var endpointConfiguration = new EndpointConfiguration(endpointName);
            services.AddRequiredInfrastructure(endpointConfiguration);

            configuration(endpointConfiguration);
        }

        public static void AddNServiceBus(this IServiceCollection services, EndpointConfiguration endpointConfiguration)
        {
            services.AddRequiredInfrastructure(endpointConfiguration);
        }
    }
}
