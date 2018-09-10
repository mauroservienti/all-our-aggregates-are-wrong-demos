using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NServiceBus;
using Sales.Data;
using Sales.Data.Models;
using Sales.Messages.Events;

namespace Sales.Api.Controllers
{
    [Route("api/shopping-cart")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        IMessageSession messageSession;
        public ShoppingCartController(IMessageSession messageSession)
        {
            this.messageSession = messageSession;
        }

        [HttpPost]
        [Route("/")]
        public async Task<IActionResult> AddToCart(dynamic data)
        {
            var cartId = new Guid(data.CartId);
            var itemId = (int)data.ItemId;
            var quantity = (int)data.Quantity;
            var requestId = Request.Headers["request-id"].Single();

            if (quantity <= 0)
            {
                return BadRequest();
            }

            using (var db = SalesContext.Create())
            {
                var requestAlreadyHandled = await db.ShoppingCarts
                    .Where(o => o.Items.Any(i => i.RequestId == requestId))
                    .AnyAsync();

                if (!requestAlreadyHandled)
                {
                    var cart = db.ShoppingCarts
                        .Include(c => c.Items)
                        .Where(o => o.Id == cartId)
                        .SingleOrDefault();

                    if (cart == null)
                    {
                        cart = db.ShoppingCarts.Add(new ShoppingCart()
                        {
                            Id = data.CartId
                        }).Entity;
                    }

                    var product = db.ProductsPrices
                        .Where(o => o.Id == itemId)
                        .Single();

                    cart.Items.Add(new ShoppingCartItem()
                    {
                        CartId = cartId,
                        RequestId = requestId,
                        ItemId = itemId,
                        CurrentPrice = product.Price,
                        LastPrice = product.Price,
                        Quantity = quantity
                    });

                    await messageSession.Publish<ItemAddedToCart>(e =>
                    {
                        e.CartId = cartId;
                        e.ItemId = itemId;
                    });

                    await db.SaveChangesAsync();
                }                
            }

            return StatusCode(200);
        }
    }
}