using JsonUtils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Warehouse.ViewModelComposition
{
    class ProductDetailsGetHandler(IHttpClientFactory httpClientFactory) : ICompositionRequestsHandler
    {
        [HttpGet("products/details/{id}")]
        public async Task Handle(HttpRequest request)
        {
            var id = (string)request.HttpContext.GetRouteData().Values["id"];

            var client = httpClientFactory.CreateClient("warehouse-api");
            var response = await client.GetAsync($"inventory/product/{id}");

            dynamic stockItem = await response.Content.AsExpando();
            var vm = request.GetComposedResponseModel();
            vm.ProductInventory = stockItem.Inventory;
            vm.ProductOutOfStock = stockItem.Inventory == 0;
        }
    }
}
