using JsonUtils;
using Marketing.ViewModelComposition.Events;
using Microsoft.AspNetCore.Http;
using ServiceComposer.AspNetCore;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Marketing.ViewModelComposition
{
    class AvailableProductsGetHandler : ICompositionRequestsHandler
    {
        [HttpGet("/")]
        public async Task Handle(HttpRequest request)
        {
            var url = $"http://localhost:5032/api/available/products";
            var client = new HttpClient();
            var response = await client.GetAsync(url);

            var availableProducts = await response.Content.As<int[]>();
            var availableProductsViewModel = MapToDictionary(availableProducts);
            var vm = request.GetComposedResponseModel();
            await vm.RaiseEvent(new AvailableProductsLoaded()
            {
                AvailableProductsViewModel = availableProductsViewModel
            });

            vm.AvailableProducts = availableProductsViewModel.Values.ToList();
        }

        IDictionary<int, dynamic> MapToDictionary(IEnumerable<int> availableProducts)
        {
            var availableProductsViewModel = new Dictionary<int, dynamic>();

            foreach (var id in availableProducts)
            {
                dynamic vm = new ExpandoObject();
                vm.Id = id;

                availableProductsViewModel[id] = vm;
            }

            return availableProductsViewModel;
        }
    }
}
