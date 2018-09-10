using ServiceComposer.ViewModelComposition;
using ServiceComposer.ViewModelComposition.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Net.Http;
using System.Threading.Tasks;
using System;

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
            if(!request.Cookies.ContainsKey("cart-id"))
            {
                request.HttpContext.Response.Cookies.Append("cart-id", Guid.NewGuid().ToString());
            }

            return Task.CompletedTask;
        }
    }
}
