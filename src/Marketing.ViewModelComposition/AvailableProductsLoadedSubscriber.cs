using JsonUtils;
using Marketing.ViewModelComposition.Events;
using ServiceComposer.AspNetCore;
using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;

namespace Marketing.ViewModelComposition
{
    class AvailableProductsLoadedSubscriber(IHttpClientFactory httpClientFactory) : ICompositionEventsSubscriber
    {
        [HttpGet("/")]
        public void Subscribe(ICompositionEventsPublisher publisher)
        {
            publisher.Subscribe<AvailableProductsLoaded>(async (@event, request) =>
            {
                var ids = String.Join(",", @event.AvailableProductsViewModel.Keys);

                var client = httpClientFactory.CreateClient("marketing-api");
                var response = await client.GetAsync($"product-details/products/{ids}");

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
