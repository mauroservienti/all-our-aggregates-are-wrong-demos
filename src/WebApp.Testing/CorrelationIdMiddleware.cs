using Microsoft.AspNetCore.Http;
using NServiceBus.IntegrationTesting.Agent;

class CorrelationIdMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("X-IntegrationTesting-Correlation-Id", out var id))
            IntegrationTestingBootstrap.SetCorrelationId(id.ToString());

        await next(context);
    }
}