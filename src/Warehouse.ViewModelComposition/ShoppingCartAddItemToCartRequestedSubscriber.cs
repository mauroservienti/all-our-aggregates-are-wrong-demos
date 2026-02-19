using Microsoft.AspNetCore.Routing;
using NServiceBus;
using NServiceBus.TransactionalSession;
using Sales.ViewModelComposition.Events;
using ServiceComposer.AspNetCore;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Messages.Commands;

namespace Warehouse.ViewModelComposition
{
    public class ShoppingCartAddItemToCartRequestedSubscriber : ICompositionEventsSubscriber
    {
        ITransactionalSession transactionalSession;

        public ShoppingCartAddItemToCartRequestedSubscriber(ITransactionalSession transactionalSession)
        {
            this.transactionalSession = transactionalSession;
        }

        [HttpPost("shoppingcart/add/{id}")]
        public void Subscribe(ICompositionEventsPublisher publisher)
        {
            publisher.Subscribe<AddItemToCartRequested>(async (@event, request) =>
            {
                var options = new SendOptions();
                options.SetDestination("Warehouse.Service");
                await transactionalSession.Send(new AddItemToCart()
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
