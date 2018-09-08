namespace NServiceBus.Shared
{
    public static class CommonEndpointSettings
    {
        public static void ApplyCommonConfiguration(this EndpointConfiguration config)
        {
            config.UseSerialization<NewtonsoftSerializer>();
            config.UseTransport<LearningTransport>();

            config.AuditProcessedMessagesTo("audit");
            config.SendFailedMessagesTo("error");

            var messageConventions = config.Conventions();
            messageConventions.DefiningMessagesAs(t => t.Namespace != null && t.Namespace.EndsWith(".Messages"));
            messageConventions.DefiningEventsAs(t => t.Namespace != null && t.Namespace.EndsWith(".Messages.Events"));
            messageConventions.DefiningCommandsAs(t => t.Namespace != null && t.Namespace.EndsWith(".Messages.Commands"));
        }
    }
}
