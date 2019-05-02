namespace NServiceBus.Shared.Hosting
{
    class SessionAndConfigurationHolder
    {
        public SessionAndConfigurationHolder(EndpointConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IMessageSession Session { get; private set; }

        public EndpointConfiguration Configuration { get; }

        public void OnStart(IMessageSession session)
        {
            Session = session;
        }

        public void OnStop()
        {
            Session = null;
        }
    }
}