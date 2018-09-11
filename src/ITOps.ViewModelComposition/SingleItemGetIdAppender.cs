using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.ViewModelComposition;
using System.Threading.Tasks;

namespace Warehouse.ViewModelComposition
{
    class SingleItemGetIdAppender : IHandleRequests
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
