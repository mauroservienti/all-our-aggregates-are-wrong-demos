using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NServiceBus.Persistence;
using NServiceBus.TransactionalSession;
using System.Net;
using System.Text;
using WebApp;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace WebApp.Tests;

public class CompositionIntegrationTests : IClassFixture<TestWebApplicationFactory>, IClassFixture<StubApiServers>
{
    readonly TestWebApplicationFactory factory;

    public CompositionIntegrationTests(TestWebApplicationFactory factory, StubApiServers _)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task Get_root_composes_available_products_from_services()
    {
        using var client = factory.CreateClient();

        var response = await client.GetStringAsync("/");

        Assert.Contains("Available products", response);
        Assert.Contains("/products/details/1", response);
        Assert.Contains("Contoso Product", response);
        Assert.Contains("A demo product", response);
        Assert.Contains("Price 12.34 $", response);
    }

    [Fact]
    public async Task Get_shopping_cart_composes_items_and_handles_missing_shipping_inventory_details()
    {
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("Cookie", "cart-id=aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        var response = await client.GetStringAsync("/ShoppingCart");

        Assert.Contains("Shopping Cart", response);
        Assert.Contains("Contoso Product", response);
        Assert.Contains("Q.ty: 2, Item $ 12.34, Total $ 24.68", response);
        Assert.Contains("Shipping estimate: not yet available", response);
        Assert.Contains("Availability: evaluation in progress", response);
    }

    [Fact]
    public async Task Get_product_details_composes_product_data_and_projects_route_id()
    {
        using var client = factory.CreateClient();

        var response = await client.GetStringAsync("/products/details/42");

        Assert.Contains("<h1>Contoso Product</h1>", response);
        Assert.Contains("<h2>A demo product</h2>", response);
        Assert.Contains("Price: 12.34 $", response);
        Assert.Contains("Out of stock", response);
        Assert.Contains("Shipping: Standard, Express", response);
        Assert.Contains("Product #42", response);
    }
}

public class StubApiServers : IAsyncLifetime
{
    readonly List<HttpListener> listeners = [];
    readonly List<Task> listenerLoops = [];

    public Task InitializeAsync()
    {
        StartServer(5031, HandleSalesApiRequest);
        StartServer(5032, HandleMarketingApiRequest);
        StartServer(5033, HandleWarehouseApiRequest);
        StartServer(5034, HandleShippingApiRequest);

        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        foreach (var listener in listeners)
        {
            listener.Stop();
            listener.Close();
        }

        await Task.WhenAll(listenerLoops);
    }

    void StartServer(int port, Func<string, string> responseProvider)
    {
        var listener = new HttpListener();
        listener.Prefixes.Add($"http://localhost:{port}/");
        listener.Start();

        listeners.Add(listener);

        listenerLoops.Add(Task.Run(async () =>
        {
            while (listener.IsListening)
            {
                try
                {
                    var context = await listener.GetContextAsync();
                    var payload = responseProvider(context.Request.Url?.AbsolutePath ?? string.Empty);

                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    context.Response.ContentType = "application/json";

                    var bytes = Encoding.UTF8.GetBytes(payload);
                    context.Response.ContentLength64 = bytes.Length;
                    await context.Response.OutputStream.WriteAsync(bytes);
                    context.Response.Close();
                }
                catch (HttpListenerException)
                {
                    break;
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
            }
        }));
    }

    static string HandleMarketingApiRequest(string path)
    {
        if (path == "/api/available/products")
        {
            return "[1]";
        }

        if (path == "/api/product-details/product/42")
        {
            return "{\"Id\":42,\"Name\":\"Contoso Product\",\"Description\":\"A demo product\"}";
        }

        if (path.StartsWith("/api/product-details/products/", StringComparison.Ordinal))
        {
            return "[{\"Id\":1,\"Name\":\"Contoso Product\",\"Description\":\"A demo product\"}]";
        }

        return "{}";
    }

    static string HandleSalesApiRequest(string path)
    {
        if (path == "/api/prices/product/42")
        {
            return "{\"Id\":42,\"Price\":12.34}";
        }

        if (path.StartsWith("/api/prices/products/", StringComparison.Ordinal))
        {
            return "[{\"Id\":1,\"Price\":12.34}]";
        }

        if (path == "/api/shopping-cart/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")
        {
            return "{\"CartId\":\"aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa\",\"Items\":[{\"ProductId\":1,\"CurrentPrice\":12.34,\"LastPrice\":10.00,\"Quantity\":2}]}";
        }

        return "{}";
    }

    static string HandleShippingApiRequest(string path)
    {
        if (path == "/api/shipping-options/product/42")
        {
            return "{\"Options\":[{\"Option\":\"Standard\"},{\"Option\":\"Express\"}]}";
        }

        if (path.StartsWith("/api/shopping-cart/products/", StringComparison.Ordinal))
        {
            return "[]";
        }

        return "{}";
    }

    static string HandleWarehouseApiRequest(string path)
    {
        if (path == "/api/inventory/product/42")
        {
            return "{\"Inventory\":0}";
        }

        if (path.StartsWith("/api/shopping-cart/products/", StringComparison.Ordinal))
        {
            return "[]";
        }

        return "{}";
    }
}

public class TestWebApplicationFactory : WebApplicationFactory<Startup>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.TryAddScoped<ITransactionalSession, NoOpTransactionalSession>();
        });
    }
}

class NoOpTransactionalSession : ITransactionalSession
{
    public ISynchronizedStorageSession SynchronizedStorageSession => throw new NotSupportedException();
    public string SessionId => string.Empty;

    public Task Open(OpenSessionOptions options, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task Commit(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task Send(object message, SendOptions options, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task Send<T>(Action<T> messageConstructor, SendOptions options, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task Publish(object message, PublishOptions options, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task Publish<T>(Action<T> messageConstructor, PublishOptions options, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public void Dispose() { }
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
