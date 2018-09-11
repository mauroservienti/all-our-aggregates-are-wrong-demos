using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.ViewModelComposition;
using System;
using System.Threading.Tasks;

namespace Warehouse.ViewModelComposition
{
    class CartIdAppender : IHandleRequests
    {
        public bool Matches(RouteData routeData, string httpVerb, HttpRequest request)
        {
            return true;
        }

        public Task Handle(string requestId, dynamic vm, RouteData routeData, HttpRequest request)
        {
            if (!request.Cookies.ContainsKey("cart-id"))
            {
                request.HttpContext.Response.Cookies.Append("cart-id", Guid.NewGuid().ToString());
            }

            return Task.CompletedTask;
        }
    }
}
