using System;
using ITOps.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using ServiceComposer.AspNetCore;

namespace WebApp;

static class WebAppConfig
{
    public static WebApplicationBuilder CreateBuilder(string[] args, Action<WebApplicationBuilder> configure = null)
    {
        var builder = WebApplication.CreateBuilder(args);
        configure?.Invoke(builder);

        builder.Configuration.AddEnvironmentVariables();

        builder.Services.AddControllersWithViews();
        builder.Services.AddViewModelComposition(options =>
        {
            options.EnableCompositionOverControllers(true);
        });

        builder.Services.AddHttpClient("marketing-api")
            .ConfigureHttpClient(client =>
            {
                var baseAddress = builder.Configuration["MARKETING_API_BASE_ADDRESS"];
                client.BaseAddress = new Uri(baseAddress);
            });
        builder.Services.AddHttpClient("sales-api")
            .ConfigureHttpClient(client =>
            {
                var baseAddress = builder.Configuration["SALES_API_BASE_ADDRESS"];
                client.BaseAddress = new Uri(baseAddress);
            });
        builder.Services.AddHttpClient("shipping-api")
            .ConfigureHttpClient(client =>
            {
                var baseAddress = builder.Configuration["SHIPPING_API_BASE_ADDRESS"];
                client.BaseAddress = new Uri(baseAddress);
            });
        builder.Services.AddHttpClient("warehouse-api")
            .ConfigureHttpClient(client =>
            {
                var baseAddress = builder.Configuration["WAREHOUSE_API_BASE_ADDRESS"];
                client.BaseAddress = new Uri(baseAddress);
            });
        
        return builder;
    }

    public static EndpointConfiguration CreateNServiceBusConfiguration(HostBuilderContext ctx)
    {
        var pgCs = Environment.GetEnvironmentVariable("POSTGRES_WEBAPP_CONNECTION_STRING")
                   ?? ctx.Configuration?["NServiceBus:WebAppDatabase"]
                   ?? "Host=localhost;Port=7432;Username=db_user;Password=P@ssw0rd;Database=webapp_database";

        var config = new EndpointConfiguration("WebApp");
        config.ApplyCommonConfigurationWithPersistence(pgCs, "WebApp");
        config.AssemblyScanner()
            .ExcludeAssemblies(
                "Sales.Service.dll",
                "Shipping.Service.dll",
                "Warehouse.Service.dll",
                "Marketing.Service.dll");

        return config;
    }

    public static void ConfigurePipeline(WebApplication app)
    {
        app.UseBrowserLink();
        app.UseStaticFiles();
        app.UseMiddleware<ShoppingCartMiddleware>();
        app.UseMiddleware<TransactionalSessionMiddleware>();
        app.MapControllers();
        app.MapCompositionHandlers();
    }
}