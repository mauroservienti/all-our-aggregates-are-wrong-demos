using NServiceBus.IntegrationTesting.Agent;
using Warehouse.Data;
using Warehouse.Service;
using Warehouse.Service.Testing.Scenarios;

await using var ctx = new WarehouseContext();
ctx.Database.EnsureCreated();

await IntegrationTestingBootstrap.RunAsync(
    "Warehouse.Service",
    WarehouseServiceConfig.Create,
    scenarios: [new AddItemToCartScenario()]);
