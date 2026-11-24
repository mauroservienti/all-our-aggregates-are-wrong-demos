using Heyi;
using Microsoft.AspNetCore.Routing;
using NServiceBus;
using Sales.ViewModelComposition.Events;
using ServiceComposer.AspNetCore;
using Shipping.Messages.Commands;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Shipping.ViewModelComposition
{
    public class ShoppingCartAddItemToCartRequestedSubscriber : ICompositionEventsSubscriber
    {
        IOutgoingMessages messages;

        public ShoppingCartAddItemToCartRequestedSubscriber(IOutgoingMessages messages)
        {
            this.messages = messages;
        }

        [HttpPost("shoppingcart/add/{id}")]
        public void Subscribe(ICompositionEventsPublisher publisher)
        {
            publisher.Subscribe<AddItemToCartRequested>(async (@event, request) =>
            {
                var options = new SendOptions();
                options.SetDestination("Shipping.Service");
                await messages.Send(new AddItemToCart()
                {
                    CartId = new Guid(@event.CartId),
                    RequestId = @event.RequestId,
                    ProductId = int.Parse((string)request.HttpContext.GetRouteValue("id")),
                    Quantity = int.Parse(request.Form["quantity"][0]),
                }, options, request.HttpContext.RequestAborted);
            });
        }
    }
}
