using System;
using NServiceBus;

namespace Marketing.Service
{
    internal static class MarketingServiceConfig
    {
        public static EndpointConfiguration Create()
        {
            var connectionString = Environment.GetEnvironmentVariable("POSTGRES_MARKETING_CONNECTION_STRING")
                ?? "Host=localhost;Port=6432;Username=db_user;Password=P@ssw0rd;Database=marketing_database";

            var config = new EndpointConfiguration("Marketing.Service");
            config.ApplyCommonConfigurationWithPersistence(connectionString);

            return config;
        }
    }
}
