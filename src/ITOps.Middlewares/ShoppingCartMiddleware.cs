using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace ITOps.Middlewares
{
    public class ShoppingCartMiddleware
    {
        private readonly RequestDelegate _next;

        public ShoppingCartMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Cookies.ContainsKey("cart-id"))
            {
                context.Response.Cookies.Append("cart-id", Guid.NewGuid().ToString());
            }

            await _next(context);
        }
    }
}
