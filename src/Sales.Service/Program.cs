using NServiceBus;
using System;
using System.Threading.Tasks;

namespace Sales.Service
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = new EndpointConfiguration("Sales.Service");
            config.ApplyCommonConfiguration();

            var endpointInstance = await Endpoint.Start(config);

            Console.WriteLine("Sales.Service sarted. Press any key to stop.");
            Console.ReadLine();

            await endpointInstance.Stop();
        }
    }
}
