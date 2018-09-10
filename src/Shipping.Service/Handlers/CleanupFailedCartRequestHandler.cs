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
                var requestWasHandled = await db.ShoppingCarts
                    .Where(o => o.Items.Any(i => i.RequestId == message.RequestId))
                    .AnyAsync();

                if (requestWasHandled)
                {
                    var cart = db.ShoppingCarts
                        .Include(c => c.Items)
                        .Where(c => c.Id == message.CartId)
                        .Single();

                    var itemToRemove = cart.Items.Single(item => item.RequestId == message.RequestId);
                    cart.Items.Remove(itemToRemove);
                    await db.SaveChangesAsync();
                }
            }
        }
    }
}
