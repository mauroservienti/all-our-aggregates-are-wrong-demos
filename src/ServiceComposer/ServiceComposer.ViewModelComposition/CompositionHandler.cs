using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceComposer.ViewModelComposition
{
    public class CompositionHandler
    {
        public static async Task<(dynamic ViewModel, int StatusCode)> HandleRequest(string requestId, HttpContext context)
        {
            var pending = new List<Task>();
            var routeData = context.GetRouteData();
            var request = context.Request;
            var vm = new DynamicViewModel(requestId, routeData, request);
            var interceptors = context.RequestServices.GetServices<IInterceptRoutes>();

            try
            {
                //matching interceptors could be cached by URL
                var matching = interceptors
                    .Where(a => a.Matches(routeData, request.Method, request))
                    .ToArray();

                foreach (var subscriber in matching.OfType<ISubscribeToCompositionEvents>())
                {
                    subscriber.Subscribe(vm, routeData, request);
                }

                foreach (var handler in matching.OfType<IHandleRequests>())
                {
                    pending.Add
                    (
                        handler.Handle(requestId, vm, routeData, request)
                    );
                }

                if (pending.Count == 0)
                {
                    return (null, StatusCodes.Status404NotFound);
                }
                else
                {
                    try
                    {
                        await Task.WhenAll(pending);
                    }
                    catch (Exception ex)
                    {
                        var errorHandlers = matching.OfType<IHandleRequestsErrors>();
                        if (errorHandlers.Any())
                        {
                            foreach (var handler in errorHandlers)
                            {
                                await handler.OnRequestError(requestId, ex, vm, routeData, request);
                            }
                        }

                        throw;
                    }
                }

                return (vm, StatusCodes.Status200OK);
            }
            finally
            {
                vm.CleanupSubscribers();
            }
        }
    }
}