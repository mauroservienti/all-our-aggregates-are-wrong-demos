using JsonUtils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Sales.ViewModelComposition.Events;
using ServiceComposer.AspNetCore;
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

        public void Subscribe(IPublishCompositionEvents publisher)
        {
            publisher.Subscribe<ShoppingCartItemsLoaded>(async (requestId, pageViewModel, @event, rd, req) =>
            {
                var ids = String.Join(",", @event.CartItemsViewModel.Keys);

                var url = $"http://localhost:5003/api/shopping-cart/products/{ids}";
                var client = new HttpClient();

                var response = await client.GetAsync(url);

                dynamic[] inventoryDetails = await response.Content.AsExpandoArray();
                if (inventoryDetails == null || inventoryDetails.Length == 0)
                {
                    //eventual consitency is making fun of us
                    foreach (var item in @event.CartItemsViewModel.Values)
                    {
                        item.Inventory = "evaulation in progress";
                    }
                }
                else
                {
                    foreach (dynamic detail in inventoryDetails)
                    {
                        @event.CartItemsViewModel[detail.ProductId].Inventory = $"{detail.Inventory} item(s) left in stock";
                    }
                }
            });
        }
    }
}
