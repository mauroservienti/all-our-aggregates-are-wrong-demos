using JsonUtils;
using Marketing.ViewModelComposition.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;
using System;
using System.Net.Http;

namespace Marketing.ViewModelComposition
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

        public void Subscribe(IPublishCompositionEvents publisher)
        {
            publisher.Subscribe<AvailableProductsLoaded>(async (requestId, pageViewModel, @event, rd, req) =>
            {
                var ids = String.Join(",", @event.AvailableProductsViewModel.Keys);

                var url = $"http://localhost:5002/api/product-details/products/{ids}";
                var client = new HttpClient();

                var response = await client.GetAsync(url);

                dynamic[] productDetails = await response.Content.AsExpandoArray();

                foreach (dynamic detail in productDetails)
                {
                    @event.AvailableProductsViewModel[(int)detail.Id].ProductName = detail.Name;
                    @event.AvailableProductsViewModel[(int)detail.Id].ProductDescription = detail.Description;
                }
            });
        }
    }
}
