using JsonUtils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Warehouse.ViewModelComposition
{
    class ProductDetailsGetHandler : ICompositionRequestsHandler
    {
        [HttpGet("products/details/{id}")]
        public async Task Handle(HttpRequest request)
        {
            var id = (string)request.HttpContext.GetRouteData().Values["id"];

            var url = $"http://localhost:5033/api/inventory/product/{id}";
            var client = new HttpClient();
            var response = await client.GetAsync(url);

            dynamic stockItem = await response.Content.AsExpando();
            var vm = request.GetComposedResponseModel();
            vm.ProductInventory = stockItem.Inventory;
            vm.ProductOutOfStock = stockItem.Inventory == 0;
        }
    }
}
