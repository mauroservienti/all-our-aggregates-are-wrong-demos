﻿using Microsoft.EntityFrameworkCore;
using NServiceBus;
using Sales.Messages.Events;
using Shipping.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Console = Colorful.Console;

namespace Shipping.Service.Handlers
{
    class ShoppingCartGotInactiveHandler : IHandleMessages<ShoppingCartGotInactive>
    {
        public async Task Handle(ShoppingCartGotInactive message, IMessageHandlerContext context)
        {
            Console.WriteLine($"Ready to wipe cart {message.CartId}.", Color.Yellow);

            using (var db = new ShippingContext())
            {
                var cartItems = await db.ShoppingCartItems
                    .Where(o => o.CartId == message.CartId)
                    .ToListAsync(context.CancellationToken);

                db.ShoppingCartItems.RemoveRange(cartItems);
                await db.SaveChangesAsync(context.CancellationToken);
            }

            Console.WriteLine($"Cart {message.CartId} wiped.", Color.Green);
        }
    }
}