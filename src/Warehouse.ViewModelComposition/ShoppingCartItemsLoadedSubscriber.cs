using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Sales.ViewModelComposition.Events;
using ServiceComposer.ViewModelComposition;
using ServiceComposer.ViewModelComposition.Json;
using System;
using System.Net.Http;

namespace Warehouse.ViewModelComposition
{
    public class ShoppingCartItemsLoadedSubscriber : ISubscribeToCompositionEvents
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

        public void Subscribe(ISubscriptionStorage subscriptionStorage, RouteData routeData, HttpRequest request)
        {
            subscriptionStorage.Subscribe<ShoppingCartItemsLoaded>(async (requestId, pageViewModel, @event, rd, req) =>
            {
                var ids = String.Join(",", @event.CartItemsViewModel.Keys);

                var url = $"http://localhost:5003/api/shopping-cart/products/{ids}";
                var client = new HttpClient();

                var response = await client.GetAsync(url);

                dynamic[] inventoryDetails = await response.Content.AsExpandoArray();

                foreach (dynamic detail in inventoryDetails)
                {
                    @event.CartItemsViewModel[detail.ProductId].Inventory = detail.Inventory;
                }
            });
        }
    }
}
