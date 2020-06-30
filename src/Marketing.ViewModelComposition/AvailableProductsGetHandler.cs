using JsonUtils;
using Marketing.ViewModelComposition.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Marketing.ViewModelComposition
{
    class AvailableProductsGetHandler : ICompositionRequestsHandler
    {
        public bool Matches(RouteData routeData, string httpVerb, HttpRequest request)
        {
            var controller = (string)routeData.Values["controller"];
            var action = (string)routeData.Values["action"];

            return HttpMethods.IsGet(httpVerb)
                   && controller.ToLowerInvariant() == "home"
                   && action.ToLowerInvariant() == "index"
                   && !routeData.Values.ContainsKey("id");
        }

        public async Task Handle(HttpRequest request)
        {
            var url = $"http://localhost:5002/api/available/products";
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
