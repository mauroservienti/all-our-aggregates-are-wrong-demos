using System;
using NServiceBus;

namespace Sales.Service
{
    static class SalesServiceConfig
    {
        public static EndpointConfiguration Create()
        {
            var connectionString = Environment.GetEnvironmentVariable("POSTGRES_SALES_CONNECTION_STRING")
                ?? "Host=localhost;Port=7432;Username=db_user;Password=P@ssw0rd;Database=sales_database";

            var config = new EndpointConfiguration("Sales.Service");
            config.ApplyCommonConfigurationWithPersistence(connectionString);
            //config.AuditSagaStateChanges(serviceControlQueue: "Particular.ServiceControl");

            return config;
        }
    }
}
