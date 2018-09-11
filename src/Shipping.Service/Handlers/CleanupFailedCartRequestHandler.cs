using Microsoft.EntityFrameworkCore;
using NServiceBus;
using System.Linq;
using System.Threading.Tasks;
using Shipping.Data;
using Shipping.Messages;

namespace Shipping.Service.Handlers
{
    class CleanupFailedCartRequestHandler : IHandleMessages<CleanupFailedCartRequest>
    {
        public async Task Handle(CleanupFailedCartRequest message, IMessageHandlerContext context)
        {
            using (var db = ShippingContext.Create())
            {
                var requestItem = await db.ShoppingCartItems
                    .Where(o => o.RequestId == message.RequestId)
                    .SingleOrDefaultAsync();

                if (requestItem != null)
                {
                    db.ShoppingCartItems.Remove(requestItem);
                    await db.SaveChangesAsync();
                }
            }
        }
    }
}
