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
            using (var db = new SalesContext())
            {
                var requestAlreadyHandled = await db.ShoppingCarts
                    .Where(o => o.Items.Any(i => i.RequestId == message.RequestId))
                    .AnyAsync(context.CancellationToken);

                if (!requestAlreadyHandled)
                {
                    var cart = db.ShoppingCarts
                        .Include(c => c.Items)
                        .SingleOrDefault(o => o.Id == message.CartId);

                    if (cart == null)
                    {
                        cart = db.ShoppingCarts.Add(new ShoppingCart()
                        {
                            Id = message.CartId
                        }).Entity;
                    }

                    var product = db.ProductsPrices
                        .Single(o => o.Id == message.ProductId);

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

                    await db.SaveChangesAsync(context.CancellationToken);
                }
            }
        }
    }
}
