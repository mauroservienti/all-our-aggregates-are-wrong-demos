using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Sales.ViewModelComposition.Events;
using ServiceComposer.AspNetCore;

namespace Shipping.ViewModelComposition
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
            publisher.Subscribe<AddItemToCartRequested>(async (requestId, pageViewModel, @event, rd, req) =>
            {
                @event.RequestData.Add("shipping-product-id", (string)rd.Values["id"]);
                @event.RequestData.Add("shipping-quantity", req.Form["quantity"][0]);
            });
        }
    }
}
