using ITOps.ViewModelComposition;
using ITOps.ViewModelComposition.Json;
using Marketing.ViewModelComposition.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Net.Http;

namespace Sales.ViewModelComposition
{
    class AvailableProductsLoadedSubscriber : ISubscribeToCompositionEvents
    {
        public bool Matches(RouteData routeData, string httpVerb, HttpRequest request)
        {
            var controller = (string)routeData.Values["controller"];
            var action = (string)routeData.Values["action"];

            return HttpMethods.IsGet(httpVerb)
                   && controller.ToLowerInvariant() == "home"
                   && action.ToLowerInvariant() == "index"
                   && !routeData.Values.ContainsKey("id");
        }

        public void Subscribe(ISubscriptionStorage subscriptionStorage, RouteData routeData, HttpRequest request)
        {
            subscriptionStorage.Subscribe<AvailableProductsLoaded>(async (requestId, pageViewModel, @event, rd, req) =>
            {
                var ids = String.Join(",", @event.AvailableProductsViewModel.Keys);

                var url = $"http://localhost:5001/api/prices/products/{ids}";
                var client = new HttpClient();

                var response = await client.GetAsync(url);

                dynamic[] productPrices = await response.Content.AsExpandoArray();

                foreach (dynamic productPrice in productPrices)
                {
                    @event.AvailableProductsViewModel[(int)productPrice.Id].ProductPrice = productPrice.Price;
                }
            });
        }
    }
}
