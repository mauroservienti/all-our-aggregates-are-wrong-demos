using Marketing.Data;
using NServiceBus.IntegrationTesting.Agent;
using Marketing.Service;

await using var ctx = new MarketingContext();
ctx.Database.EnsureCreated();

await IntegrationTestingBootstrap.RunAsync(
    "Marketing.Service",
    MarketingServiceConfig.Create,
    scenarios: []);
