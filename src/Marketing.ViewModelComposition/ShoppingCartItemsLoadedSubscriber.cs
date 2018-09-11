using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Sales.ViewModelComposition.Events;
using ServiceComposer.ViewModelComposition;
using ServiceComposer.ViewModelComposition.Json;
using System;
using System.Net.Http;

namespace Marketing.ViewModelComposition
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

                var url = $"http://localhost:5002/api/product-details/products/{ids}";
                var client = new HttpClient();

                var response = await client.GetAsync(url).ConfigureAwait(false);

                dynamic[] productDetails = await response.Content.AsExpandoArray().ConfigureAwait(false);

                foreach (dynamic detail in productDetails)
                {
                    @event.CartItemsViewModel[detail.Id].ProductName = detail.Name;
                    @event.CartItemsViewModel[detail.Id].ProductDescription = detail.Description;
                }
            });
        }
    }
}
