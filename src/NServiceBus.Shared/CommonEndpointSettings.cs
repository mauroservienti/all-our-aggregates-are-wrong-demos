using System;
using Npgsql;
using NpgsqlTypes;

namespace NServiceBus
{
    public static class CommonEndpointSettings
    {
        public static void ApplyCommonConfiguration(this EndpointConfiguration config) 
        {
            config.AuditProcessedMessagesTo("audit");
            config.SendFailedMessagesTo("error");

            config.UseSerialization<NewtonsoftJsonSerializer>();
            var transportConfig = config.UseTransport<LearningTransport>();
            transportConfig.Transactions(TransportTransactionMode.ReceiveOnly);

            var messageConventions = config.Conventions();
            messageConventions.DefiningMessagesAs(t => t.Namespace != null && t.Namespace.EndsWith(".Messages"));
            messageConventions.DefiningEventsAs(t => t.Namespace != null && t.Namespace.EndsWith(".Messages.Events"));
            messageConventions.DefiningCommandsAs(t => t.Namespace != null && t.Namespace.EndsWith(".Messages.Commands"));

            config.SendHeartbeatTo(
                serviceControlQueue: "Particular.ServiceControl",
                frequency: TimeSpan.FromSeconds(10),
                timeToLive: TimeSpan.FromSeconds(5));

            var metrics = config.EnableMetrics();
            metrics.SendMetricDataToServiceControl(
                serviceControlMetricsAddress: "Particular.Monitoring",
                interval: TimeSpan.FromSeconds(5));
        }

        public static void ApplyCommonConfigurationWithPersistence(this EndpointConfiguration config, string sqlPersistenceConnectionString, string tablePrefix = null)
        {
            ApplyCommonConfiguration(config);

            config.EnableInstallers();

            var persistence = config.UsePersistence<SqlPersistence>();
            var dialect = persistence.SqlDialect<SqlDialect.PostgreSql>();
            if (!string.IsNullOrWhiteSpace(tablePrefix))
            {
                persistence.TablePrefix(tablePrefix);
            }

            dialect.JsonBParameterModifier(
                modifier: parameter =>
                {
                    var npgsqlParameter = (NpgsqlParameter)parameter;
                    npgsqlParameter.NpgsqlDbType = NpgsqlDbType.Jsonb;
                });
            persistence.ConnectionBuilder(
                connectionBuilder: () => new NpgsqlConnection(sqlPersistenceConnectionString));

            config.EnableOutbox();
        }
    }
}
