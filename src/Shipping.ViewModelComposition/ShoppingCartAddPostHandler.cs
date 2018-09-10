using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NServiceBus;
using Shipping.Messages;
using ServiceComposer.ViewModelComposition;
using System;
using System.Threading.Tasks;
using Shipping.Messages.Commands;

namespace Shipping.ViewModelComposition
{
    class ShoppingCartAddPostHandler : IHandleRequests, IHandleRequestsErrors
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

        public Task Handle(string requestId, dynamic vm, RouteData routeData, HttpRequest request)
        {
            return messageSession.Send("Shipping.Service", new AddItemToCart()
            {
                ProductId = int.Parse((string)routeData.Values["id"]),
                Quantity = int.Parse(request.Form["quantity"][0]),
                CartId = new Guid(request.Cookies["cart-id"]),
                RequestId= requestId,
            });
        }

        public Task OnRequestError(string requestId, Exception ex, dynamic vm, RouteData routeData, HttpRequest request)
        {
            return messageSession.Send("Shipping.Service", new CleanupFailedCartRequest()
            {
                CartId = new Guid(request.Cookies["cart-id"]),
                RequestId = requestId
            });
        }
    }
}
