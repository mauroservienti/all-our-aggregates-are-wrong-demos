using NServiceBus;
using System;
using System.Threading.Tasks;

namespace Sales.Service
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceName = typeof(Program).Namespace;
            Console.Title = serviceName;

            var config = new EndpointConfiguration(serviceName);
            config.ApplyCommonConfigurationWithPersistence(@"Data Source=(localdb)\all-our-aggregates-are-wrong;Initial Catalog=Sales;Integrated Security=True");
            config.AuditSagaStateChanges(serviceControlQueue: "Particular.ServiceControl");

            var endpointInstance = await Endpoint.Start(config);

            Console.WriteLine($"{serviceName} sarted. Press any key to stop.");
            Console.ReadLine();

            await endpointInstance.Stop();
        }
    }
}
