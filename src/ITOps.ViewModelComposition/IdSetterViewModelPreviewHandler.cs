using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;
using System.Threading.Tasks;

namespace ITOps.ViewModelComposition
{
    class IdSetterViewModelPreviewHandler : IViewModelPreviewHandler
    {
        Task IViewModelPreviewHandler.Preview(dynamic viewModel)
        {
            throw new System.NotImplementedException();
        }
        
        public Task Preview(HttpRequest request, dynamic viewModel)
        {
            var routeData = request.HttpContext.GetRouteData();
            if (routeData.Values.ContainsKey("id"))
            {
                var id = (string)routeData.Values["id"];
                viewModel.Id = id;
            }

            return Task.CompletedTask;
        }
    }
}