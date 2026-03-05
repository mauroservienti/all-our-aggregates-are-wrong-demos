using System;
using NServiceBus;

namespace Warehouse.Service
{
    static class WarehouseServiceConfig
    {
        public static EndpointConfiguration Create()
        {
            var connectionString = Environment.GetEnvironmentVariable("POSTGRES_WAREHOUSE_CONNECTION_STRING")
                ?? "Host=localhost;Port=9432;Username=db_user;Password=P@ssw0rd;Database=warehouse_database";

            var config = new EndpointConfiguration("Warehouse.Service");
            config.ApplyCommonConfigurationWithPersistence(connectionString);

            return config;
        }
    }
}
