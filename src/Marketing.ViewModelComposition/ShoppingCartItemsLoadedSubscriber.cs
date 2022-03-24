using JsonUtils;
using Sales.ViewModelComposition.Events;
using ServiceComposer.AspNetCore;
using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;

namespace Marketing.ViewModelComposition
{
    public class ShoppingCartItemsLoadedSubscriber : ICompositionEventsSubscriber
    {
        [HttpGet("/ShoppingCart")]
        public void Subscribe(ICompositionEventsPublisher publisher)
        {
            publisher.Subscribe<ShoppingCartItemsLoaded>(async (@event, request) =>
            {
                var ids = String.Join(",", @event.CartItemsViewModel.Keys);

                var url = $"http://localhost:5032/api/product-details/products/{ids}";
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
