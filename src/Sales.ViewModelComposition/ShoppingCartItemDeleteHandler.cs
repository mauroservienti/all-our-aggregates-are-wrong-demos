using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sales.ViewModelComposition
{
    class ShoppingCartItemDeleteHandler : IHandleRequests
    {
        public bool Matches(RouteData routeData, string httpVerb, HttpRequest request)
        {
            var controller = (string)routeData.Values["controller"];
            var action = (string)routeData.Values["action"];

            return HttpMethods.IsGet(httpVerb)
                   && controller.ToLowerInvariant() == "shoppingcart"
                   && action.ToLowerInvariant() == "delete"
                   && !routeData.Values.ContainsKey("id");
        }

        public async Task Handle(string requestId, dynamic vm, RouteData routeData, HttpRequest request)
        {
            var cartId = request.Cookies["cart-id"];
            var itemId = (string)routeData.Values["id"];

            var url = $"http://localhost:5001/api/shopping-cart/{cartId}/item/{itemId}";

            var client = new HttpClient();
            var response = await client.DeleteAsync(url);

            response.EnsureSuccessStatusCode();
        }
    }
}
