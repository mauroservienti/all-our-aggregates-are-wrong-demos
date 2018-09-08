using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ITOps.ViewModelComposition
{
    public interface ISubscribeToCompositionEvents : IInterceptRoutes
    {
        void Subscribe(ISubscriptionStorage subscriptionStorage, RouteData routeData, HttpRequest request);
    }
}
