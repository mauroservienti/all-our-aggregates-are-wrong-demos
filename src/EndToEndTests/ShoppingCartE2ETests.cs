using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using Sales.Data;
using Shipping.Data;
using System.Net;
using System.Text;
using Testcontainers.PostgreSql;
using Warehouse.Data;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace EndToEndTests;

/// <summary>
/// End-to-end tests for the shopping-cart "add item" flow.
///
/// The test starts the full application stack in-process on real Kestrel ports and
/// drives it through real HTTP:
///   1. POST /shoppingcart/add/{id}  (WebApp, port 5030)
///       → TransactionalSessionMiddleware opens an ITransactionalSession
///       → ShoppingCartAddPostHandler + Shipping/Warehouse subscribers each call
///         transactionalSession.Send() to enqueue commands for their backend services
///       → Middleware commits the session; all three commands land in the outbox atomically
///   2. NServiceBus dispatches commands from the outbox table
///       → Sales.Service processes AddItemToCart and saves the item to the Sales database
///       → Shipping.Service processes AddItemToCart and saves the delivery estimate
///       → Warehouse.Service processes AddItemToCart and saves the inventory snapshot
///   3. GET /ShoppingCart (WebApp) polls until the composed result is visible
///       → ShoppingCartGetHandler calls Sales.Api which reads the Sales database
///       → ShoppingCartItemsLoadedSubscriber calls Shipping.Api and Warehouse.Api
///         which read the Shipping and Warehouse databases respectively
///
/// Because the test requires Docker (Testcontainers) it runs only on Linux in CI.
/// </summary>
public class ShoppingCartE2ETests : IAsyncLifetime
{
    // Each database container uses a fixed host port matching the connection string
    // hardcoded in the corresponding DbContext / Service Program.cs.
    readonly PostgreSqlContainer _salesDb = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .WithPortBinding(7432, 5432)
        .WithDatabase("sales_database")
        .WithUsername("db_user")
        .WithPassword("P@ssw0rd")
        .Build();

    readonly PostgreSqlContainer _shippingDb = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .WithPortBinding(8432, 5432)
        .WithDatabase("shipping_database")
        .WithUsername("db_user")
        .WithPassword("P@ssw0rd")
        .Build();

    readonly PostgreSqlContainer _warehouseDb = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .WithPortBinding(9432, 5432)
        .WithDatabase("warehouse_database")
        .WithUsername("db_user")
        .WithPassword("P@ssw0rd")
        .Build();

    // WebApp NServiceBus DB: dynamic port; connection string injected via configuration.
    readonly PostgreSqlContainer _webAppDb = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .Build();

    IHost? _salesServiceHost;
    IHost? _shippingServiceHost;
    IHost? _warehouseServiceHost;
    IHost? _salesApiHost;
    IHost? _shippingApiHost;
    IHost? _warehouseApiHost;
    IHost? _webAppHost;

    // Marketing.Api stub: not under test, but the composition layer calls it for product names.
    readonly List<HttpListener> _stubListeners = [];
    readonly List<Task> _stubLoops = [];

    public async Task InitializeAsync()
    {
        // ── 1. Start all databases in parallel ───────────────────────────────────
        await Task.WhenAll(
            _salesDb.StartAsync(),
            _shippingDb.StartAsync(),
            _warehouseDb.StartAsync(),
            _webAppDb.StartAsync());

        // ── 2. Create EF Core schema + seed data in each service database ─────────
        //      EnsureCreated applies the HasData seed records defined in each context.
        await using (var salesCtx = new SalesContext())
        await using (var shippingCtx = new ShippingContext())
        await using (var warehouseCtx = new WarehouseContext())
        {
            await Task.WhenAll(
                salesCtx.Database.EnsureCreatedAsync(),
                shippingCtx.Database.EnsureCreatedAsync(),
                warehouseCtx.Database.EnsureCreatedAsync());
        }

        // ── 3. Start NServiceBus backend service endpoints ────────────────────────
        _salesServiceHost = BuildServiceHost("Sales.Service",
            @"Host=localhost;Port=7432;Username=db_user;Password=P@ssw0rd;Database=sales_database");
        _shippingServiceHost = BuildServiceHost("Shipping.Service",
            @"Host=localhost;Port=8432;Username=db_user;Password=P@ssw0rd;Database=shipping_database");
        _warehouseServiceHost = BuildServiceHost("Warehouse.Service",
            @"Host=localhost;Port=9432;Username=db_user;Password=P@ssw0rd;Database=warehouse_database");

        await Task.WhenAll(
            _salesServiceHost.StartAsync(),
            _shippingServiceHost.StartAsync(),
            _warehouseServiceHost.StartAsync());

        // ── 4. Start API hosts on their well-known ports ──────────────────────────
        _salesApiHost     = BuildApiHost(Sales.Api.Program.CreateHostBuilder([]),     "http://localhost:5031");
        _shippingApiHost  = BuildApiHost(Shipping.Api.Program.CreateHostBuilder([]),  "http://localhost:5034");
        _warehouseApiHost = BuildApiHost(Warehouse.Api.Program.CreateHostBuilder([]), "http://localhost:5033");

        await Task.WhenAll(
            _salesApiHost.StartAsync(),
            _shippingApiHost.StartAsync(),
            _warehouseApiHost.StartAsync());

        // ── 5. Start WebApp on its well-known port ────────────────────────────────
        _webAppHost = WebApp.Program.CreateHostBuilder([])
            .ConfigureAppConfiguration((_, cfg) =>
                cfg.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    // Inject the Testcontainers connection string so that
                    // WebApp/Program.cs picks up the real SQL persistence path.
                    ["NServiceBus:WebAppDatabase"] = _webAppDb.GetConnectionString(),
                    ["urls"] = "http://localhost:5030"
                }))
            .Build();

        await _webAppHost.StartAsync();

        // ── 6. Start stub for Marketing.Api (not under test) ─────────────────────
        //      ShoppingCartItemsLoadedSubscriber calls it for product names.
        StartStubListener(5032, path =>
        {
            if (path.StartsWith("/api/product-details/products/", StringComparison.Ordinal))
                return "[{\"Id\":1,\"Name\":\"Demo Product\",\"Description\":\"A product\"}]";
            return "[]";
        });
    }

    public async Task DisposeAsync()
    {
        if (_webAppHost != null) await _webAppHost.StopAsync();
        if (_salesApiHost != null) await _salesApiHost.StopAsync();
        if (_shippingApiHost != null) await _shippingApiHost.StopAsync();
        if (_warehouseApiHost != null) await _warehouseApiHost.StopAsync();
        if (_salesServiceHost != null) await _salesServiceHost.StopAsync();
        if (_shippingServiceHost != null) await _shippingServiceHost.StopAsync();
        if (_warehouseServiceHost != null) await _warehouseServiceHost.StopAsync();

        foreach (var l in _stubListeners) { l.Stop(); l.Close(); }
        await Task.WhenAll(_stubLoops);

        _webAppHost?.Dispose();
        _salesApiHost?.Dispose();
        _shippingApiHost?.Dispose();
        _warehouseApiHost?.Dispose();
        _salesServiceHost?.Dispose();
        _shippingServiceHost?.Dispose();
        _warehouseServiceHost?.Dispose();

        await Task.WhenAll(
            _salesDb.DisposeAsync().AsTask(),
            _shippingDb.DisposeAsync().AsTask(),
            _warehouseDb.DisposeAsync().AsTask(),
            _webAppDb.DisposeAsync().AsTask());
    }

    /// <summary>
    /// Posting to /shoppingcart/add/{id} must eventually cause the item to appear on the
    /// /ShoppingCart page with real data composed from all three backend services:
    ///   - Sales.Api: item price and quantity
    ///   - Shipping.Api: delivery estimate ("between 1 and 12 days" from seed data for product 1)
    ///   - Warehouse.Api: inventory snapshot ("4 item(s) left in stock" from seed data for product 1)
    /// </summary>
    [Fact]
    public async Task Adding_an_item_to_the_cart_eventually_shows_composed_data_from_all_services()
    {
        var cartId = Guid.NewGuid().ToString();

        using var handler = new HttpClientHandler { AllowAutoRedirect = true };
        using var client = new HttpClient(handler);
        client.DefaultRequestHeaders.Add("Cookie", $"cart-id={cartId}");

        // ── Act: add product 1 (qty 2) to the cart ───────────────────────────────
        var formData = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("quantity", "2")
        ]);

        var postResponse = await client.PostAsync("http://localhost:5030/shoppingcart/add/1", formData);
        postResponse.EnsureSuccessStatusCode();

        // ── Assert: poll until all three services' data is composed ───────────────
        //   NServiceBus dispatches the outbox messages asynchronously; each backend
        //   service processes its command independently before the API returns data.
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        string pageContent = string.Empty;
        while (!cts.IsCancellationRequested)
        {
            var getResponse = await client.GetAsync("http://localhost:5030/ShoppingCart", cts.Token);
            pageContent = await getResponse.Content.ReadAsStringAsync(cts.Token);

            // Wait until all three services have processed their commands and all
            // composed data is present on the page.
            if (pageContent.Contains("Q.ty: 2") &&
                pageContent.Contains("between 1 and 12 days") &&
                pageContent.Contains("4 item(s) left in stock"))
                break;

            await Task.Delay(TimeSpan.FromMilliseconds(500), cts.Token);
        }

        // Sales.Api: product 1 has seed price $10.00 -> total for qty 2 is $20.00
        Assert.Contains("Q.ty: 2, Item $ 10.00, Total $ 20.00", pageContent);

        // Shipping.Api: product 1 seed data -> options min 1 day, max 12 days
        Assert.Contains("between 1 and 12 days", pageContent);

        // Warehouse.Api: product 1 seed data -> inventory 4
        Assert.Contains("4 item(s) left in stock", pageContent);
    }

    /// <summary>Builds an NServiceBus Generic Host for a backend service endpoint.</summary>
    static IHost BuildServiceHost(string endpointName, string connectionString)
    {
        return Host.CreateDefaultBuilder()
            .UseNServiceBus(_ =>
            {
                var config = new EndpointConfiguration(endpointName);
                config.ApplyCommonConfigurationWithPersistence(connectionString);

                // Exclude assemblies that don't belong in this endpoint so that
                // composition handlers and other services' handlers are not registered.
                config.AssemblyScanner().ExcludeAssemblies(
                    "EndToEndTests.dll",
                    "WebApp.dll",
                    "Sales.Api.dll",
                    "Shipping.Api.dll",
                    "Warehouse.Api.dll",
                    "ITOps.Middlewares.dll",
                    "ITOps.ViewModelComposition.dll",
                    "Marketing.ViewModelComposition.dll",
                    "Sales.ViewModelComposition.dll",
                    "Shipping.ViewModelComposition.dll",
                    "Warehouse.ViewModelComposition.dll");

                return config;
            })
            .Build();
    }

    /// <summary>Builds a Kestrel host for an API, binding it to the given URL.</summary>
    static IHost BuildApiHost(IHostBuilder hostBuilder, string url) =>
        hostBuilder
            .ConfigureAppConfiguration((_, cfg) =>
                cfg.AddInMemoryCollection(new Dictionary<string, string?> { ["urls"] = url }))
            .Build();

    void StartStubListener(int port, Func<string, string> responseProvider)
    {
        var listener = new HttpListener();
        listener.Prefixes.Add($"http://localhost:{port}/");
        listener.Start();
        _stubListeners.Add(listener);

        _stubLoops.Add(Task.Run(async () =>
        {
            while (listener.IsListening)
            {
                try
                {
                    var ctx = await listener.GetContextAsync();
                    var body = responseProvider(ctx.Request.Url?.AbsolutePath ?? string.Empty);
                    ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                    ctx.Response.ContentType = "application/json";
                    var bytes = Encoding.UTF8.GetBytes(body);
                    ctx.Response.ContentLength64 = bytes.Length;
                    await ctx.Response.OutputStream.WriteAsync(bytes);
                    ctx.Response.Close();
                }
                catch (HttpListenerException) { break; }
                catch (ObjectDisposedException) { break; }
            }
        }));
    }
}
