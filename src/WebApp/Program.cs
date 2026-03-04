using System;
using ITOps.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using ServiceComposer.AspNetCore;

namespace WebApp;

public class Program
{
    public static void Main(string[] args) => Build(args).Run();

    public static WebApplication Build(string[] args, Action<WebApplicationBuilder> configure = null)
    {
        var builder = WebApplication.CreateBuilder(args);
        configure?.Invoke(builder);

        builder.Services.AddControllersWithViews();
        builder.Services.AddViewModelComposition(options =>
        {
            options.EnableCompositionOverControllers(true);
        });

        builder.Host.UseNServiceBus(ctx =>
        {
            var endpointConfiguration = WebAppConfig.Create(ctx.Configuration);
            return endpointConfiguration;
        });

        var app = builder.Build();

        app.UseBrowserLink();
        app.UseStaticFiles();
        app.UseMiddleware<ShoppingCartMiddleware>();
        app.UseMiddleware<TransactionalSessionMiddleware>();
        app.MapControllers();
        app.MapCompositionHandlers();

        return app;
    }
}
