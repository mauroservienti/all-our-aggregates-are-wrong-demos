using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ITOps.ViewModelComposition
{
    public interface IInterceptRoutes
    {
        bool Matches(RouteData routeData, string httpVerb, HttpRequest request);
    }
}
