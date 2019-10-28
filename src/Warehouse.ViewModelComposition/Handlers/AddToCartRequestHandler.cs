using NServiceBus;
using Sales.ViewModelComposition.Messages;
using System.Threading.Tasks;
using Warehouse.Messages.Commands;

namespace Warehouse.ViewModelComposition.Handlers
{
    class AddToCartRequestHandler : IHandleMessages<AddToCartRequest>
    {
        public Task Handle(AddToCartRequest message, IMessageHandlerContext context)
        {
            return context.Send("Warehouse.Service", new AddItemToCart()
            {
                CartId = message.CartId,
                RequestId = message.RequestId,
                ProductId = int.Parse(message.RequestData["warehouse-product-id"]),
                Quantity = int.Parse(message.RequestData["warehouse-quantity"]),                
            });
        }
    }
}
