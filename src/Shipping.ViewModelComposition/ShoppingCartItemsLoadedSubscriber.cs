using JsonUtils;
using Sales.ViewModelComposition.Events;
using ServiceComposer.AspNetCore;
using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;

namespace Shipping.ViewModelComposition
{
    public class ShoppingCartItemsLoadedSubscriber(IHttpClientFactory httpClientFactory) : ICompositionEventsSubscriber
    {
        [HttpGet("/ShoppingCart")]
        public void Subscribe(ICompositionEventsPublisher publisher)
        {
            publisher.Subscribe<ShoppingCartItemsLoaded>(async (@event, request) =>
            {
                var ids = String.Join(",", @event.CartItemsViewModel.Keys);

                var client = httpClientFactory.CreateClient("shipping-api");
                var response = await client.GetAsync($"shopping-cart/products/{ids}");

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
