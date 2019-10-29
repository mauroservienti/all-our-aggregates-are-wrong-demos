using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NServiceBus;
using Sales.ViewModelComposition.Events;
using Sales.ViewModelComposition.Messages;
using ServiceComposer.AspNetCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sales.ViewModelComposition
{
    class ShoppingCartAddPostHandler : IHandleRequests
    {
        IMessageSession messageSession;

        public ShoppingCartAddPostHandler(IMessageSession messageSession)
        {
            this.messageSession = messageSession;
        }

        public bool Matches(RouteData routeData, string httpVerb, HttpRequest request)
        {
            var controller = (string)routeData.Values["controller"];
            var action = (string)routeData.Values["action"];

            return HttpMethods.IsPost(httpVerb)
                   && controller.ToLowerInvariant() == "shoppingcart"
                   && action.ToLowerInvariant() == "add"
                   && routeData.Values.ContainsKey("id");
        }

        public async Task Handle(string requestId, dynamic vm, RouteData routeData, HttpRequest request)
        {
            var requestData = new Dictionary<string, string>() 
            {
                { "sales-product-id", (string)routeData.Values["id"] },
                { "sales-quantity", request.Form["quantity"][0] },
            };

            await vm.RaiseEvent(new AddItemToCartRequested() 
            {
                CartId = request.Cookies["cart-id"],
                RequestId = requestId,
                RequestData = requestData 
            });

            await messageSession.SendLocal(new AddToCartRequest() 
            {
                RequestId = requestId,
                CartId = new Guid(request.Cookies["cart-id"]),
                RequestData = requestData });
        }
    }
}
