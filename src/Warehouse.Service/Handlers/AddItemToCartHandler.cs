using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NServiceBus;
using Warehouse.Data;
using Warehouse.Data.Models;
using Warehouse.Messages.Commands;

namespace Warehouse.Service.Handlers
{
    class AddItemToCartHandler : IHandleMessages<AddItemToCart>
    {
        public async Task Handle(AddItemToCart message, IMessageHandlerContext context)
        {
            using (var db = WarehouseContext.Create())
            {
                var requestAlreadyHandled = await db.ShoppingCarts
                    .Where(o => o.Items.Any(i => i.RequestId == message.RequestId))
                    .AnyAsync();

                if (!requestAlreadyHandled)
                {
                    var cart = db.ShoppingCarts
                        .Include(c => c.Items)
                        .Where(o => o.Id == message.CartId)
                        .SingleOrDefault();

                    if (cart == null)
                    {
                        cart = db.ShoppingCarts.Add(new ShoppingCart()
                        {
                            Id = message.CartId
                        }).Entity;
                    }

                    var stockItem = db.StockItems
                        .Where(o => o.ProductId == message.ItemId)
                        .Single();

                    cart.Items.Add(new ShoppingCartItem()
                    {
                        CartId = message.CartId,
                        RequestId = message.RequestId,
                        ItemId = message.ItemId,
                        Inventory = stockItem.Inventory,
                        Quantity = message.Quantity
                    });

                    await db.SaveChangesAsync();
                }
            }
        }
    }
}
