using NServiceBus;
using Sales.Messages.Commands;
using Sales.ViewModelComposition.Messages;
using System.Threading.Tasks;

namespace Sales.ViewModelComposition.Handlers
{
    class AddToCartRequestHandler : IHandleMessages<AddToCartRequest>
    {
        public Task Handle(AddToCartRequest message, IMessageHandlerContext context)
        {
            return context.Send("Sales.Service", new AddItemToCart()
            {
                RequestId = message.RequestId,
                CartId = message.CartId,
                ProductId = int.Parse(message.RequestData["sales-product-id"]),
                Quantity = int.Parse(message.RequestData["sales-quantity"]),
            });
        }
    }
}
