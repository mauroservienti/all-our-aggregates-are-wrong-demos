using Heyi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NServiceBus;
using Sales.Messages.Commands;
using Sales.ViewModelComposition.Events;
using ServiceComposer.AspNetCore;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Sales.ViewModelComposition
{
    class ShoppingCartAddPostHandler : ICompositionRequestsHandler
    {
        IOutgoingMessages messages;

        public ShoppingCartAddPostHandler(IOutgoingMessages messages)
        {
            this.messages = messages;
        }

        [HttpPost("shoppingcart/add/{id}")]
        public async Task Handle(HttpRequest request)
        {
            var compositionContext = request.GetCompositionContext();

            await request.GetCompositionContext().RaiseEvent(new AddItemToCartRequested()
            {
                CartId = request.Cookies["cart-id"],
                RequestId = compositionContext.RequestId,
            });

            var options = new SendOptions();
            options.SetDestination("Sales.Service");
            await messages.Send(new AddItemToCart()
            {
                RequestId = compositionContext.RequestId,
                CartId = new Guid(request.Cookies["cart-id"]),
                ProductId = int.Parse((string)request.HttpContext.GetRouteValue("id")),
                Quantity = int.Parse(request.Form["quantity"][0]),
            }, options, request.HttpContext.RequestAborted);
        }
    }
}
