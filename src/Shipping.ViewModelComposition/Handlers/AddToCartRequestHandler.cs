using NServiceBus;
using Sales.ViewModelComposition.Messages;
using Shipping.Messages.Commands;
using System.Threading.Tasks;

namespace Shipping.ViewModelComposition.Handlers
{
    class AddToCartRequestHandler : IHandleMessages<AddToCartRequest>
    {
        public Task Handle(AddToCartRequest message, IMessageHandlerContext context)
        {
            return context.Send("Shipping.Service", new AddItemToCart()
            {
                CartId = message.CartId,
                RequestId = message.RequestId,
                ProductId = int.Parse(message.RequestData["shipping-product-id"]),
                Quantity = int.Parse(message.RequestData["shipping-quantity"]),
            });
        }
    }
}