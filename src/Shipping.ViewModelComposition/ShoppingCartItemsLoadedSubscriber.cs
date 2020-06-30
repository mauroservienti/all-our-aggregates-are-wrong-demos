using JsonUtils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Sales.ViewModelComposition.Events;
using ServiceComposer.AspNetCore;
using System;
using System.Net.Http;

namespace Shipping.ViewModelComposition
{
    public class ShoppingCartItemsLoadedSubscriber : ICompositionEventsSubscriber
    {
        public bool Matches(RouteData routeData, string httpVerb, HttpRequest request)
        {
            var controller = (string)routeData.Values["controller"];
            var action = (string)routeData.Values["action"];

            return HttpMethods.IsGet(httpVerb)
                   && controller.ToLowerInvariant() == "shoppingcart"
                   && action.ToLowerInvariant() == "index"
                   && !routeData.Values.ContainsKey("id");
        }

        public void Subscribe(ICompositionEventsPublisher publisher)
        {
            publisher.Subscribe<ShoppingCartItemsLoaded>(async (@event, request) =>
            {
                var ids = String.Join(",", @event.CartItemsViewModel.Keys);

                var url = $"http://localhost:5004/api/shopping-cart/products/{ids}";
                var client = new HttpClient();

                var response = await client.GetAsync(url);

                dynamic[] shippingDetails = await response.Content.AsExpandoArray();
                if (shippingDetails == null || shippingDetails.Length == 0)
                {
                    //eventual consitency is making fun of us
                    foreach (var item in @event.CartItemsViewModel.Values)
                    {
                        item.DeliveryEstimate = "not yet available";
                    }
                }
                else
                {
                    foreach (dynamic detail in shippingDetails)
                    {
                        @event.CartItemsViewModel[detail.ProductId].DeliveryEstimate = detail.DeliveryEstimate;
                    }
                }
            });
        }
    }
}
