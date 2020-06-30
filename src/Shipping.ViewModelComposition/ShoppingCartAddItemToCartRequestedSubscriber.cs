using Microsoft.AspNetCore.Routing;
using Sales.ViewModelComposition.Events;
using ServiceComposer.AspNetCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Shipping.ViewModelComposition
{
    public class ShoppingCartAddItemToCartRequestedSubscriber : ICompositionEventsSubscriber
    {
        [HttpPost("shoppingcart/add/{id}")]
        public void Subscribe(ICompositionEventsPublisher publisher)
        {
            publisher.Subscribe<AddItemToCartRequested>((@event, request) =>
            {
                @event.RequestData.Add("shipping-product-id", (string)request.HttpContext.GetRouteValue("id"));
                @event.RequestData.Add("shipping-quantity", request.Form["quantity"][0]);

                return Task.CompletedTask;
            });
        }
    }
}
