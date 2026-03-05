using NServiceBus.IntegrationTesting.Agent;
using Shipping.Data;
using Shipping.Service;
using Shipping.Service.Testing.Scenarios;

await using var ctx = new ShippingContext();
ctx.Database.EnsureCreated();

await IntegrationTestingBootstrap.RunAsync(
    "Shipping.Service",
    ShippingServiceConfig.Create,
    scenarios: [new AddItemToCartScenario()]);
