using Microsoft.EntityFrameworkCore;
using NServiceBus;
using Sales.Data;
using Sales.Data.Models;
using Sales.Messages.Commands;
using Sales.Messages.Events;
using System.Linq;
using System.Threading.Tasks;

namespace Sales.Service.Handlers
{
    class AddItemToCartHandler : IHandleMessages<AddItemToCart>
    {
        public async Task Handle(AddItemToCart message, IMessageHandlerContext context)
        {
            using (var db = SalesContext.Create())
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

                    var product = db.ProductsPrices
                        .Where(o => o.Id == message.ProductId)
                        .Single();

                    cart.Items.Add(new ShoppingCartItem()
                    {
                        CartId = message.CartId,
                        RequestId = message.RequestId,
                        ProductId = message.ProductId,
                        CurrentPrice = product.Price,
                        LastPrice = product.Price,
                        Quantity = message.Quantity
                    });

                    await context.Publish<ProductAddedToCart>(e =>
                    {
                        e.CartId = message.CartId;
                        e.ProductId = message.ProductId;
                    });

                    await db.SaveChangesAsync();
                }
            }
        }
    }
}
