using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using Sales.Data;
using System.Net;
using System.Text;
using Testcontainers.PostgreSql;

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
///   3. GET /ShoppingCart (WebApp) polls until the item is visible
///       → ShoppingCartGetHandler calls Sales.Api which reads the Sales database
///
/// Because the test requires Docker (Testcontainers) it runs only on Linux in CI.
/// </summary>
public class ShoppingCartE2ETests : IAsyncLifetime
{
    // Sales DB: fixed host port 7432 to match the connection string hardcoded in SalesContext
    // and in Sales.Service/Program.cs.
    readonly PostgreSqlContainer _salesDb = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .WithPortBinding(7432, 5432)
        .WithDatabase("sales_database")
        .WithUsername("db_user")
        .WithPassword("P@ssw0rd")
        .Build();

    // WebApp NServiceBus DB: dynamic port; connection string injected via configuration.
    readonly PostgreSqlContainer _webAppDb = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .Build();

    IHost? _salesServiceHost;
    IHost? _salesApiHost;
    IHost? _webAppHost;

    readonly List<HttpListener> _stubListeners = [];
    readonly List<Task> _stubLoops = [];

    public async Task InitializeAsync()
    {
        // ── 1. Start databases ────────────────────────────────────────────────────
        await Task.WhenAll(_salesDb.StartAsync(), _webAppDb.StartAsync());

        // ── 2. Create EF Core schema + seed data (ProductPrices) in Sales DB ─────
        //      Use EnsureCreated so that HasData seed records are applied.
        await using (var db = new SalesContext())
        {
            await db.Database.EnsureCreatedAsync();
        }

        // ── 3. Start Sales.Service NServiceBus endpoint ───────────────────────────
        //      The endpoint is started via the Generic Host so its lifetime is
        //      managed cleanly alongside the other hosts.
        _salesServiceHost = Host.CreateDefaultBuilder()
            .UseNServiceBus(_ =>
            {
                var config = new EndpointConfiguration("Sales.Service");

                // Use the same connection string that SalesContext uses so that
                // NServiceBus persistence tables land in the same database.
                config.ApplyCommonConfigurationWithPersistence(
                    @"Host=localhost;Port=7432;Username=db_user;Password=P@ssw0rd;Database=sales_database");

                // Restrict scanning to the Sales.Service assembly and its direct
                // dependencies so that WebApp / composition handlers are not
                // accidentally registered in this endpoint.
                config.AssemblyScanner().ExcludeAssemblies(
                    "EndToEndTests.dll",
                    "WebApp.dll",
                    "Sales.Api.dll",
                    "ITOps.Middlewares.dll",
                    "ITOps.ViewModelComposition.dll",
                    "Marketing.ViewModelComposition.dll",
                    "Sales.ViewModelComposition.dll",
                    "Shipping.ViewModelComposition.dll",
                    "Warehouse.ViewModelComposition.dll");

                return config;
            })
            .Build();

        await _salesServiceHost.StartAsync();

        // ── 4. Start Sales.Api on its well-known port ─────────────────────────────
        //      WebApp's ShoppingCartGetHandler calls http://localhost:5031/api/...
        _salesApiHost = Sales.Api.Program.CreateHostBuilder([])
            .ConfigureAppConfiguration((_, cfg) =>
                cfg.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["urls"] = "http://localhost:5031"
                }))
            .Build();

        await _salesApiHost.StartAsync();

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

        // ── 6. Start stub HTTP listeners for the APIs that are not under test ─────
        //      Marketing.Api (5032): returns a product name so the cart view renders
        //      Shipping.Api (5034): returns [] (eventual consistency, "not yet available")
        //      Warehouse.Api (5033): returns [] (eventual consistency, "evaluation in progress")
        StartStubListener(5032, path =>
        {
            if (path.StartsWith("/api/product-details/products/", StringComparison.Ordinal))
                return "[{\"Id\":1,\"Name\":\"Demo Product\",\"Description\":\"A product\"}]";
            return "[]";
        });
        StartStubListener(5034, _ => "[]");
        StartStubListener(5033, _ => "[]");
    }

    public async Task DisposeAsync()
    {
        if (_webAppHost != null) await _webAppHost.StopAsync();
        if (_salesApiHost != null) await _salesApiHost.StopAsync();
        if (_salesServiceHost != null) await _salesServiceHost.StopAsync();

        foreach (var l in _stubListeners) { l.Stop(); l.Close(); }
        await Task.WhenAll(_stubLoops);

        _webAppHost?.Dispose();
        _salesApiHost?.Dispose();
        _salesServiceHost?.Dispose();

        await _salesDb.DisposeAsync();
        await _webAppDb.DisposeAsync();
    }

    /// <summary>
    /// Posting to /shoppingcart/add/{id} must eventually cause the item to appear on the
    /// /ShoppingCart page.  The test verifies the complete vertical slice:
    ///   WebApp (transactional session) → LearningTransport → Sales.Service → Sales.Api → WebApp composition
    /// </summary>
    [Fact]
    public async Task Adding_an_item_to_the_cart_eventually_shows_it_on_the_shopping_cart_page()
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

        // ── Assert: poll the shopping-cart page until the item appears ────────────
        //   NServiceBus dispatches from the outbox and Sales.Service processes the
        //   message asynchronously, so we retry with a generous timeout.
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        string pageContent = string.Empty;
        while (!cts.IsCancellationRequested)
        {
            var getResponse = await client.GetAsync("http://localhost:5030/ShoppingCart", cts.Token);
            pageContent = await getResponse.Content.ReadAsStringAsync(cts.Token);

            if (pageContent.Contains("Q.ty: 2"))
                break;

            await Task.Delay(TimeSpan.FromMilliseconds(500), cts.Token);
        }

        // Product 1 has seed price $10.00 → total for qty 2 is $20.00
        Assert.Contains("Q.ty: 2, Item $ 10.00, Total $ 20.00", pageContent);
        Assert.Contains("Shipping estimate: not yet available", pageContent);
        Assert.Contains("Availability: evaluation in progress", pageContent);
    }

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
