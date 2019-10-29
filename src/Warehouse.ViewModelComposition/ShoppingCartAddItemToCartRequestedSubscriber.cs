using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Sales.ViewModelComposition.Events;
using ServiceComposer.AspNetCore;
using System.Threading.Tasks;

namespace Warehouse.ViewModelComposition
{
    public class ShoppingCartAddItemToCartRequestedSubscriber : ISubscribeToCompositionEvents
    {
        public bool Matches(RouteData routeData, string httpVerb, HttpRequest request)
        {
            var controller = (string)routeData.Values["controller"];
            var action = (string)routeData.Values["action"];

            return HttpMethods.IsPost(httpVerb)
                   && controller.ToLowerInvariant() == "shoppingcart"
                   && action.ToLowerInvariant() == "add"
                   && routeData.Values.ContainsKey("id");
        }

        public void Subscribe(IPublishCompositionEvents publisher)
        {
            publisher.Subscribe<AddItemToCartRequested>((requestId, pageViewModel, @event, rd, req) =>
            {
                @event.RequestData.Add("warehouse-product-id", (string)rd.Values["id"]);
                @event.RequestData.Add("warehouse-quantity", req.Form["quantity"][0]);

                return Task.CompletedTask;
            });
        }
    }
}
