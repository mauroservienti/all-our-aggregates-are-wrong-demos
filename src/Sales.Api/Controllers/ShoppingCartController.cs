using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NServiceBus;
using Sales.Data;
using Sales.Data.Models;
using Sales.Messages.Events;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

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
        [Route("")]
        public async Task<IActionResult> AddToCart(JsonElement data)
        {
            var cartId = data.GetProperty("CartId").GetGuid();
            var productId = int.Parse(data.GetProperty("ProductId").GetString());
            var quantity = data.GetProperty("Quantity").GetInt32();
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
                            Id = cartId
                        }).Entity;
                    }

                    var product = db.ProductsPrices
                        .Where(o => o.Id == productId)
                        .Single();

                    cart.Items.Add(new ShoppingCartItem()
                    {
                        CartId = cartId,
                        RequestId = requestId,
                        ProductId = productId,
                        CurrentPrice = product.Price,
                        LastPrice = product.Price,
                        Quantity = quantity
                    });

                    await messageSession.Publish<ProductAddedToCart>(e =>
                    {
                        e.CartId = cartId;
                        e.ProductId = productId;
                    });

                    await db.SaveChangesAsync();
                }
            }

            return StatusCode(200);
        }

        [HttpGet]
        [Route("{id}")]
        public dynamic GetCart(Guid id)
        {
            using (var db = SalesContext.Create())
            {
                var cartItems = db.ShoppingCarts
                    .Include(c => c.Items)
                    .Where(o => o.Id == id)
                    .SelectMany(cart => cart.Items)
                    .ToArray()
                    .GroupBy(cartItem => cartItem.ProductId)
                    .Select(group => new
                    {
                        ProductId = group.Key,
                        Quantity = group.Sum(cartItem => cartItem.Quantity),
                        CurrentPrice = group.FirstOrDefault()?.CurrentPrice,
                        LastPrice = group.FirstOrDefault()?.LastPrice,
                    })
                    .ToArray();

                return new
                {
                    CartId = id,
                    Items = cartItems
                };
            }
        }
    }
}