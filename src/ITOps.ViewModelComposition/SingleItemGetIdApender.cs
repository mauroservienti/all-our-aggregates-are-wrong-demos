using ServiceComposer.ViewModelComposition;
using ServiceComposer.ViewModelComposition.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Net.Http;
using System.Threading.Tasks;

namespace Warehouse.ViewModelComposition
{
    class SingleItemGetIdApender : IHandleRequests
    {
        public bool Matches(RouteData routeData, string httpVerb, HttpRequest request)
        {
            return HttpMethods.IsGet(httpVerb)
                   && routeData.Values.ContainsKey("id");
        }

        public Task Handle(string requestId, dynamic vm, RouteData routeData, HttpRequest request)
        {
            var id = (string)routeData.Values["id"];
            vm.Id = id;

            return Task.CompletedTask;
        }
    }
}
