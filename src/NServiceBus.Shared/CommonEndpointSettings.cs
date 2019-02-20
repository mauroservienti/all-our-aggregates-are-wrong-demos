using System;

namespace NServiceBus
{
    public static class CommonEndpointSettings
    {
        public static void ApplyCommonConfiguration(this EndpointConfiguration config, bool asSendOnly = false)
        {
            config.UseSerialization<NewtonsoftSerializer>();
            config.UseTransport<LearningTransport>();
            config.UsePersistence<LearningPersistence>();

            config.AuditProcessedMessagesTo("audit");
            config.SendFailedMessagesTo("error");

            config.SendHeartbeatTo(
                serviceControlQueue: "Particular.ServiceControl",
                frequency: TimeSpan.FromSeconds(10),
                timeToLive: TimeSpan.FromSeconds(5));

            var messageConventions = config.Conventions();
            messageConventions.DefiningMessagesAs(t => t.Namespace != null && t.Namespace.EndsWith(".Messages"));
            messageConventions.DefiningEventsAs(t => t.Namespace != null && t.Namespace.EndsWith(".Messages.Events"));
            messageConventions.DefiningCommandsAs(t => t.Namespace != null && t.Namespace.EndsWith(".Messages.Commands"));

            if (asSendOnly)
            {
                config.SendOnly();
            }
            else
            {
                var metrics = config.EnableMetrics();
                metrics.SendMetricDataToServiceControl(
                    serviceControlMetricsAddress: "Particular.Monitoring",
                    interval: TimeSpan.FromSeconds(5));
            }
        }
    }
}
