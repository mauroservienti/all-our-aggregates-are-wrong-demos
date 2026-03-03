using NServiceBus;
using System;
using System.Threading.Tasks;

namespace Warehouse.Service
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceName = typeof(Program).Namespace;
            Console.Title = serviceName;

            var config = WarehouseServiceConfig.Create();

            var endpointInstance = await Endpoint.Start(config);

            Console.WriteLine($"{serviceName} started. Press any key to stop.");
            Console.ReadLine();

            await endpointInstance.Stop();
        }
    }
}
