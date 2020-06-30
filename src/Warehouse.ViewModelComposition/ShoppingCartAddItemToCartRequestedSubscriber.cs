using Microsoft.AspNetCore.Routing;
using Sales.ViewModelComposition.Events;
using ServiceComposer.AspNetCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Warehouse.ViewModelComposition
{
    public class ShoppingCartAddItemToCartRequestedSubscriber : ICompositionEventsSubscriber
    {
        [HttpPost("shoppingcart/add/{id}")]
        public void Subscribe(ICompositionEventsPublisher publisher)
        {
            publisher.Subscribe<AddItemToCartRequested>((@event, request) =>
            {
                @event.RequestData.Add("warehouse-product-id", (string)request.HttpContext.GetRouteValue("id"));
                @event.RequestData.Add("warehouse-quantity", request.Form["quantity"][0]);

                return Task.CompletedTask;
            });
        }
    }
}
