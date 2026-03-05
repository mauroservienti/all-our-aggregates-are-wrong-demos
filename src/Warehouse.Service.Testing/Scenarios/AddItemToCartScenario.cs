using NServiceBus;
using NServiceBus.IntegrationTesting.Agent;
using Warehouse.Messages.Commands;

namespace Warehouse.Service.Testing.Scenarios;

public class AddItemToCartScenario : Scenario
{
    public override string Name => "AddItemToCart";

    public override async Task Execute(
        IMessageSession session,
        Dictionary<string, string> args,
        CancellationToken cancellationToken = default)
    {
        var options = new SendOptions();
        options.SetDestination("Warehouse.Service");

        await session.Send(new AddItemToCart
        {
            CartId = Guid.Parse(args["CartId"]),
            ProductId = int.Parse(args["ProductId"]),
            Quantity = int.Parse(args["Quantity"]),
            RequestId = args["RequestId"]
        }, options, cancellationToken);
    }
}
