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

            var client = httpClientFactory.CreateClient("marketing-api");
            var response = await client.GetAsync($"product-details/product/{id}");

            dynamic details = await response.Content.AsExpando();
            var vm = request.GetComposedResponseModel();
            vm.ProductName = details.Name;
            vm.ProductDescription = details.Description;
        }
    }
}
