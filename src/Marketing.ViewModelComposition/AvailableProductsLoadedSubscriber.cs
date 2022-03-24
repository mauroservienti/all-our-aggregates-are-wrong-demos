using JsonUtils;
using Marketing.ViewModelComposition.Events;
using ServiceComposer.AspNetCore;
using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;

namespace Marketing.ViewModelComposition
{
    class AvailableProductsLoadedSubscriber : ICompositionEventsSubscriber
    {
        [HttpGet("/")]
        public void Subscribe(ICompositionEventsPublisher publisher)
        {
            publisher.Subscribe<AvailableProductsLoaded>(async (@event, request) =>
            {
                var ids = String.Join(",", @event.AvailableProductsViewModel.Keys);

                var url = $"http://localhost:5032/api/product-details/products/{ids}";
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
