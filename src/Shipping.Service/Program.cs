using Microsoft.Data.SqlClient;
using NServiceBus;
using System;
using System.Threading.Tasks;

namespace Shipping.Service
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceName = typeof(Program).Namespace;
            Console.Title = serviceName;

            var config = new EndpointConfiguration(serviceName);
            config.ApplyCommonConfigurationWithPersistence(@"Data Source=(localdb)\all-our-aggregates-are-wrong;Initial Catalog=Shipping;Integrated Security=True");

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

            var endpointInstance = await Endpoint.Start(config);

            Console.WriteLine($"{serviceName} sarted. Press any key to stop.");
            Console.ReadLine();

            await endpointInstance.Stop();
        }
    }
}
