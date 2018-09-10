using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shipping.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shipping.Api.Controllers
{
    [Route("api/shipping-options")]
    [ApiController]
    public class ProductShippingOptionsController : ControllerBase
    {
        [HttpGet]
        [Route("product/{id}")]
        public dynamic Get(int id)
        {
            using (var db = ShippingContext.Create())
            {
                var item = db.ProductShippingOptions
                    .Include(pso => pso.Options)
                    .Where(o => o.ProductId == id)
                    .SingleOrDefault();

                return item;
            }
        }

        [HttpGet]
        [Route("products/{ids}")]
        public IEnumerable<dynamic> Get(string ids)
        {
            using (var db = ShippingContext.Create())
            {
                var productIds = ids.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(s => int.Parse(s)).ToArray();
                var items = db.ProductShippingOptions
                    .Include(pso => pso.Options)
                    .Where(status => productIds.Any(id => id == status.ProductId))
                    .ToArray();

                return items;
            }
        }
    }
}
