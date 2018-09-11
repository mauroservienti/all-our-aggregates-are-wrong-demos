using Microsoft.EntityFrameworkCore;
using NServiceBus;
using Sales.Data;
using Sales.Messages.Events;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Console = Colorful.Console;

namespace Sales.Service.Handlers
{
    class ShoppingCartGotInactiveHandler : IHandleMessages<ShoppingCartGotInactive>
    {
        public async Task Handle(ShoppingCartGotInactive message, IMessageHandlerContext context)
        {
            Console.WriteLine($"Ready to wipe cart {message.CartId}.", Color.Yellow);

            using (var db = SalesContext.Create())
            {
                var cart = await db.ShoppingCarts
                    .Where(o => o.Id == message.CartId)
                    .SingleOrDefaultAsync();
                if (cart != null)
                {
                    db.ShoppingCarts.Remove(cart);
                    await db.SaveChangesAsync();
                }
            }

            Console.WriteLine($"Cart {message.CartId} wiped.", Color.Green);
        }
    }
}