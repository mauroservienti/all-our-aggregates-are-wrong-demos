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
            //Obsolete: won't be ever invoked
            throw new System.NotImplementedException();
        }
        
        public Task Preview(HttpRequest request)
        {
            var viewModel = request.GetComposedResponseModel();
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