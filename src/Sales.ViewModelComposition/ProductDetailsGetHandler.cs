using JsonUtils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Sales.ViewModelComposition
{
    class ProductDetailsGetHandler(IHttpClientFactory httpClientFactory) : ICompositionRequestsHandler
    {
        [HttpGet("products/details/{id}")]
        public async Task Handle(HttpRequest request)
        {
            var id = (string)request.HttpContext.GetRouteData().Values["id"];

            var client = httpClientFactory.CreateClient("sales-api");
            var response = await client.GetAsync($"prices/product/{id}");

            dynamic productPrice = await response.Content.AsExpando();
            var vm = request.GetComposedResponseModel();
            vm.ProductPrice = productPrice.Price;
        }
    }
}
