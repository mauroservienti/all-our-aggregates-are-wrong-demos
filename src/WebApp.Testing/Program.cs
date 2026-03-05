using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus.IntegrationTesting.Agent;
using WebApp;

var builder = WebAppConfig.CreateBuilder(args);
builder.Host.UseNServiceBus(ctx =>
{
    var config = WebAppConfig.CreateNServiceBusConfiguration(ctx);
    IntegrationTestingBootstrap.Configure("WebApp", config, builder.Services);
    return config;
});

builder.Services.AddTransient<CorrelationIdPropagationHandler>();
builder.Services.AddHttpClient("marketing-api")
    .AddHttpMessageHandler<CorrelationIdPropagationHandler>();
builder.Services.AddHttpClient("sales-api")                                                                                                                                                                                                                                                                                                                             
    .AddHttpMessageHandler<CorrelationIdPropagationHandler>();
builder.Services.AddHttpClient("shipping-api")                                                                                                                                                                                                                                                                                                                             
    .AddHttpMessageHandler<CorrelationIdPropagationHandler>();
builder.Services.AddHttpClient("warehouse-api")                                                                                                                                                                                                                                                                                                                             
    .AddHttpMessageHandler<CorrelationIdPropagationHandler>();

var app = builder.Build();
app.UseMiddleware<CorrelationIdMiddleware>();

WebAppConfig.ConfigurePipeline(app);

await app.RunAsync();