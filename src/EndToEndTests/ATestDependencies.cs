using NServiceBus.IntegrationTesting;

namespace EndToEndTests;

public class ATestDependencies : IAsyncLifetime
{
    public TestEnvironment? Environment { get; private set; }

    public async Task InitializeAsync()
    {
        Environment = await new TestEnvironmentBuilder()
            .WithDockerfileDirectory(TestEnvironmentBuilder.FindRootByFile("*.sln"))
            .UseRabbitMQ()
            .AddWebApp()
            .AddWarehouse()
            .AddMarketing()
            .AddSales()
            .AddShipping()
            .StartAsync();
    }

    public Task DisposeAsync() => Environment?.DisposeAsync().AsTask() ?? Task.CompletedTask;
}