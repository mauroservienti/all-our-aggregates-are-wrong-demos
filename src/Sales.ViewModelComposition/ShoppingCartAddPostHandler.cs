using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NServiceBus;
using Sales.ViewModelComposition.Events;
using Sales.ViewModelComposition.Messages;
using ServiceComposer.AspNetCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Sales.ViewModelComposition
{
    class ShoppingCartAddPostHandler : ICompositionRequestsHandler
    {
        IMessageSession messageSession;

        public ShoppingCartAddPostHandler(IMessageSession messageSession)
        {
            this.messageSession = messageSession;
        }

        [HttpPost("shoppingcart/add/{id}")]
        public async Task Handle(HttpRequest request)
        {
            var requestData = new Dictionary<string, string>()
            {
                { "sales-product-id", (string)request.HttpContext.GetRouteValue("id") },
                { "sales-quantity", request.Form["quantity"][0] },
            };
            var compositionContext = request.GetCompositionContext();
            var vm = request.GetComposedResponseModel();
            await request.GetCompositionContext().RaiseEvent(new AddItemToCartRequested()
            {
                CartId = request.Cookies["cart-id"],
                RequestId = compositionContext.RequestId,
                RequestData = requestData
            });

            await messageSession.SendLocal(new AddToCartRequest()
            {
                RequestId = compositionContext.RequestId,
                CartId = new Guid(request.Cookies["cart-id"]),
                RequestData = requestData });
        }
    }
}
