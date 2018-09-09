using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;

namespace ITOps.ViewModelComposition
{
    class Subscription<T> : Subscription
    {
        private Func<string, dynamic, T, RouteData, HttpRequest, Task> subscription;

        public Subscription(Func<string, dynamic, T, RouteData, HttpRequest, Task> subscription)
        {
            this.subscription = subscription;
        }

        public override Task Invoke(string requestId, dynamic viewModel, object @event, RouteData routeData, HttpRequest request) => subscription(requestId, viewModel, (T)@event, routeData, request);
    }
}
