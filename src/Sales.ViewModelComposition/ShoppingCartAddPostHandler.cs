using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using NServiceBus;
using Sales.Messages;
using ServiceComposer.ViewModelComposition;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sales.ViewModelComposition
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

        public async Task Handle(string requestId, dynamic vm, RouteData routeData, HttpRequest request)
        {
            var postData = new
            {
                ProductId = (string)routeData.Values["id"],
                Quantity = int.Parse(request.Form["quantity"][0]),
                CartId = request.Cookies["cart-id"]
            };

            var url = $"http://localhost:5001/api/shopping-cart";
            var client = new HttpClient();

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(
                    JsonConvert.SerializeObject(postData), 
                    Encoding.UTF8, 
                    "application/json")
            };
            requestMessage.Headers.Add("request-id", requestId);

            var response = await client.SendAsync(requestMessage);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(response.ReasonPhrase);
            }
        }

        public Task OnRequestError(string requestId, Exception ex, dynamic vm, RouteData routeData, HttpRequest request)
        {
            return messageSession.Send("Sales.Service", new CleanupFailedCartRequest()
            {
                CartId = new Guid(request.Cookies["cart-id"]),
                RequestId = requestId
            });
        }
    }
}
