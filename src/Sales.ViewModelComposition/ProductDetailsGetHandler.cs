using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;
using System.Net.Http;
using System.Threading.Tasks;
using JsonUtils;

namespace Sales.ViewModelComposition
{
    class ProductDetailsGetHandler : IHandleRequests
    {
        public bool Matches(RouteData routeData, string httpVerb, HttpRequest request)
        {
            var controller = (string)routeData.Values["controller"];
            var action = (string)routeData.Values["action"];

            return HttpMethods.IsGet(httpVerb)
                   && controller.ToLowerInvariant() == "products"
                   && action.ToLowerInvariant() == "details"
                   && routeData.Values.ContainsKey("id");
        }

        public async Task Handle(string requestId, dynamic vm, RouteData routeData, HttpRequest request)
        {
            var id = (string)routeData.Values["id"];

            var url = $"http://localhost:5001/api/prices/product/{id}";
            var client = new HttpClient();
            var response = await client.GetAsync(url);

            dynamic productPrice = await response.Content.AsExpando();

            vm.ProductPrice = productPrice.Price;
        }
    }
}
