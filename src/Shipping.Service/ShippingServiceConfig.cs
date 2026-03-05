using System;
using NServiceBus;

namespace Shipping.Service
{
    static class ShippingServiceConfig
    {
        public static EndpointConfiguration Create()
        {
            var connectionString = Environment.GetEnvironmentVariable("POSTGRES_SHIPPING_CONNECTION_STRING")
                ?? "Host=localhost;Port=8432;Username=db_user;Password=P@ssw0rd;Database=shipping_database";

            var config = new EndpointConfiguration("Shipping.Service");
            config.ApplyCommonConfigurationWithPersistence(connectionString);
            
            config.ReportCustomChecksTo(serviceControlQueue: "Particular.ServiceControl");
            var recoverabilityConfig = config.Recoverability();
            recoverabilityConfig.Immediate(immediate =>
            {
                immediate.NumberOfRetries(1);
            });
            recoverabilityConfig.Delayed(delayed =>
            {
                delayed.NumberOfRetries(1);
                delayed.TimeIncrease(TimeSpan.FromSeconds(5));
            });

            return config;
        }
    }
}
