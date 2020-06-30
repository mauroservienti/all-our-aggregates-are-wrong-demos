using JsonUtils;
using Marketing.ViewModelComposition.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;
using System;
using System.Net.Http;

namespace Sales.ViewModelComposition
{
    class AvailableProductsLoadedSubscriber : ICompositionEventsSubscriber
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

        public void Subscribe(ICompositionEventsPublisher publisher)
        {
            publisher.Subscribe<AvailableProductsLoaded>(async (@event, request) =>
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
