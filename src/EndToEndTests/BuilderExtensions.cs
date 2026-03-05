using DotNet.Testcontainers.Builders;
using NServiceBus.IntegrationTesting;

namespace EndToEndTests;

static class BuilderExtensions
{
    static string[] _services = ["Sales", "Shipping", "Marketing", "Warehouse"];
    extension(TestEnvironment testEnvironment)
    {
        public async Task DropAndRecreateDatabases()
        {
            foreach (var discriminator in _services)
            {
                await testEnvironment.ResetService(discriminator);
            }

            await testEnvironment.ResetWebApp();
        }

        async Task ResetWebApp()
        {
            var discriminator = "WebApp";
            
            var serviceEndpoint = testEnvironment.GetEndpoint(discriminator);
            await serviceEndpoint.StopAsync();
            
            var postgreSql = testEnvironment.GetInfrastructure($"postgres-{discriminator.ToLowerInvariant()}");
            await postgreSql.ExecAsync(["dropdb", "-U", "db_user", $"{discriminator.ToLowerInvariant()}_database"]);
            await postgreSql.ExecAsync(["createdb", "-U", "db_user", $"{discriminator.ToLowerInvariant()}_database"]);

            await serviceEndpoint.StartAsync();
        }

        async Task ResetService(string discriminator)
        {
            //var apiEndpoint = testEnvironment.GetContainer($"{discriminator}.Api");
            var serviceEndpoint = testEnvironment.GetEndpoint($"{discriminator}.Service");
            //await apiEndpoint.StopAsync();
            await serviceEndpoint.StopAsync();

            var postgreSql = testEnvironment.GetInfrastructure($"postgres-{discriminator.ToLowerInvariant()}");
            await postgreSql.ExecAsync(["dropdb", "-U", "db_user", $"{discriminator.ToLowerInvariant()}_database"]);
            await postgreSql.ExecAsync(["createdb", "-U", "db_user", $"{discriminator.ToLowerInvariant()}_database"]);
            
            //await apiEndpoint.StartAsync();
            await serviceEndpoint.StartAsync();
        }
    }
    
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
                        //WebApp is configured to listen on port 5030/HTTP
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