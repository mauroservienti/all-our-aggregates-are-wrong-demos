using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NServiceBus;
using Shipping.Data;
using Shipping.Data.Models;
using Shipping.Messages.Commands;

namespace Shipping.Service.Handlers
{
    class AddItemToCartHandler : IHandleMessages<AddItemToCart>
    {
        public async Task Handle(AddItemToCart message, IMessageHandlerContext context)
        {
            using (var db = ShippingContext.Create())
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

                    var shippingOptions = db.ProductShippingOptions
                        .Where(o => o.ProductId == message.ProductId)
                        .Single();

                    var shortest = shippingOptions.Options.Min(o => o.EstimatedMinDeliveryDays);
                    var longest = shippingOptions.Options.Max(o => o.EstimatedMaxDeliveryDays);
                    var estimate = "";
                    if (shortest == int.MaxValue && longest == int.MaxValue)
                    {
                        estimate = "ah ah ah ah ah ah";
                    }
                    else
                    {
                        estimate = $"between {shortest} and {longest} days";
                    }

                    cart.Items.Add(new ShoppingCartItem()
                    {
                        CartId = message.CartId,
                        RequestId = message.RequestId,
                        ProductId = message.ProductId,
                        DeliveryEstimate = estimate,
                        Quantity = message.Quantity
                    });

                    await db.SaveChangesAsync();
                }
            }
        }
    }
}
