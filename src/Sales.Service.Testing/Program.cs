using NServiceBus.IntegrationTesting.Agent;
using Sales.Data;
using Sales.Service;
using Sales.Service.Policies;
using Sales.Service.Testing.Scenarios;

await using var ctx = new SalesContext();
ctx.Database.EnsureCreated();

await IntegrationTestingBootstrap.RunAsync(
    "Sales.Service",
    SalesServiceConfig.Create,
    scenarios: [new AddItemToCartScenario()],
    timeoutRules:
    [
        TimeoutRule.For<CartGettingStaleTimeout>(TimeSpan.FromSeconds(3)),
        TimeoutRule.For<CartWipeTimeout>(TimeSpan.FromSeconds(5))
    ]);
