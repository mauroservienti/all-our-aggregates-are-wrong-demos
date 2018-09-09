using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;

namespace ServiceComposer.ViewModelComposition
{
    abstract class Subscription
    {
        public abstract Task Invoke(string requestId, dynamic viewModel, object @event, RouteData routeData, HttpRequest request);
    }
}
