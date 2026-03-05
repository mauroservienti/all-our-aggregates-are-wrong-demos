using NServiceBus.IntegrationTesting;
using Xunit.Abstractions;

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

public class ATest(ITestOutputHelper testOutput, ATestDependencies dependencies) : IClassFixture<ATestDependencies> //, IAsyncLifetime 
{
    readonly TestEnvironment environment = dependencies.Environment ?? throw new NullReferenceException(nameof(dependencies.Environment));
    bool _testPassed;

    // public Task InitializeAsync() => Task.CompletedTask;
    //
    // public async Task DisposeAsync()
    // {
    //     if (_testPassed) return;
    //
    //     var (sampleStdout, sampleStderr) = await environment.GetEndpointContainerLogsAsync("SampleEndpoint");
    //     testOutput.WriteLine("=== SampleEndpoint container stdout ===");
    //     testOutput.WriteLine(sampleStdout);
    //     testOutput.WriteLine("=== SampleEndpoint container stderr ===");
    //     testOutput.WriteLine(sampleStderr);
    //
    //     var (anotherStdout, anotherStderr) = await environment.GetEndpointContainerLogsAsync("AnotherEndpoint");
    //     testOutput.WriteLine("=== AnotherEndpoint container stdout ===");
    //     testOutput.WriteLine(anotherStdout);
    //     testOutput.WriteLine("=== AnotherEndpoint container stderr ===");
    //     testOutput.WriteLine(anotherStderr);
    // }

    [Fact]
    public Task ATest_that_tests_something()
    {
        try
        {
            var webApp = environment!.GetEndpoint("WebApp");
            var webAppBaseUrl = webApp.GetBaseUrl(5030);

            _testPassed = true;

            return Task.CompletedTask;
        }
        catch (Exception exception)
        {
            return Task.FromException(exception);
        }
    }
}

static class BuilderExtensions
{
    extension(TestEnvironmentBuilder testEnvironmentBuilder)
    {
        public TestEnvironmentBuilder UsePostgreSqlForService(string discriminator)
        {
            return testEnvironmentBuilder
                .UsePostgreSql(
                    containerOptions: options =>
                    {
                        options.Username = "db_user";
                        options.Password = "P@ssw0rd";
                        options.Database = $"{discriminator}_database";
                        options.Key = $"postgres-{discriminator}";
                    },
                    containerBuilder: b => b
                        .WithDatabase($"{discriminator}_database")
                        .WithUsername("db_user")
                        .WithPassword("P@ssw0rd"));
        }

        public TestEnvironmentBuilder AddWebApp()
        {
            return testEnvironmentBuilder.UsePostgreSqlForService("webapp")
                .AddEndpoint("WebApp", "WebApp.Testing/Dockerfile",
                    containerOptions: options =>
                    {
                        options.EnvironmentVariables.Add("MARKETING_API_BASE_ADDRESS", "http://marketing-api:8080/api");
                        options.EnvironmentVariables.Add("SALES_API_BASE_ADDRESS", "http://sales-api:8080/api");
                        options.EnvironmentVariables.Add("SHIPPING_API_BASE_ADDRESS", "http://shipping-api:8080/api");
                        options.EnvironmentVariables.Add("WAREHOUSE_API_BASE_ADDRESS", "http://warehouse-api:8080/api");
                    },
                    containerBuilder: b => b
                        //.WithLogger(logger)
                        .WithPortBinding(5030, assignRandomHostPort: true));
        }

        public TestEnvironmentBuilder AddSales()
        {
            return testEnvironmentBuilder.UsePostgreSqlForService("sales")
                .AddEndpoint("Sales.Service", "Sales.Service.Testing/Dockerfile")
                .AddContainer("Sales.Api", "Sales.Api.Testing/Dockerfile", options => options.NetworkAlias = "sales-api");
        }

        public TestEnvironmentBuilder AddShipping()
        {
            return testEnvironmentBuilder.UsePostgreSqlForService("shipping")
                .AddEndpoint("Shipping.Service", "Shipping.Service.Testing/Dockerfile")
                .AddContainer("Shipping.Api", "Shipping.Api.Testing/Dockerfile", options => options.NetworkAlias = "shipping-api");
        }

        public TestEnvironmentBuilder AddMarketing()
        {
            return testEnvironmentBuilder.UsePostgreSqlForService("marketing")
                .AddEndpoint("Marketing.Service", "Marketing.Service.Testing/Dockerfile")
                .AddContainer("Marketing.Api", "Marketing.Api.Testing/Dockerfile", options => options.NetworkAlias = "marketing-api");
        }

        public TestEnvironmentBuilder AddWarehouse()
        {
            return testEnvironmentBuilder.UsePostgreSqlForService("warehouse")
                .AddEndpoint("Warehouse.Service", "Warehouse.Service.Testing/Dockerfile")
                .AddContainer("Warehouse.Api", "Warehouse.Api.Testing/Dockerfile", options => options.NetworkAlias = "warehouse-api");
        }
    }
}