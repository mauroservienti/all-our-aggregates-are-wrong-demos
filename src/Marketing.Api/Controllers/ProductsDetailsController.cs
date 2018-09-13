using Marketing.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Marketing.Api.Controllers
{
    [Route("api/product-details")]
    [ApiController]
    public class ProductsDetailsController : ControllerBase
    {
        [HttpGet]
        [Route("product/{id}")]
        public dynamic Get(int id)
        {
            using (var db = MarketingContext.Create())
            {
                var item = db.ProductsDetails
                    .Where(o => o.Id == id)
                    .SingleOrDefault();

                return item;
            }
        }

        [HttpGet]
        [Route("products/{ids}")]
        public IEnumerable<dynamic> Get(string ids)
        {
            using (var db = MarketingContext.Create())
            {
                var productIds = ids.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(s => int.Parse(s)).ToArray();
                var items = db.ProductsDetails
                    .Where(status => productIds.Any(id => id == status.Id))
                    .ToArray();

                return items;
            }
        }
    }
}
