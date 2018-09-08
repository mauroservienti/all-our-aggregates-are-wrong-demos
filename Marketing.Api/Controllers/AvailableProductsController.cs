using Marketing.Data;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Marketing.Api.Controllers
{
    [Route("api/available")]
    [ApiController]
    public class AvailableProductsController : ControllerBase
    {
        [HttpGet]
        [Route("products")]
        public IEnumerable<int> Get()
        {
            using (var db = MarketingContext.Create())
            {
                var all = db.ProductsDetails
                    .Select(p => p.Id)
                    .ToArray();

                return all;
            }
        }
    }
}
