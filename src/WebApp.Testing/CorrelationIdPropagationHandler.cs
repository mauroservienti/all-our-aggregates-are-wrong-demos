using NServiceBus.IntegrationTesting.Agent;

class CorrelationIdPropagationHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var id = IntegrationTestingBootstrap.GetCorrelationId();
        if (id is not null)
            request.Headers.TryAddWithoutValidation("X-IntegrationTesting-Correlation-Id", id);

        return base.SendAsync(request, cancellationToken);
    }
}