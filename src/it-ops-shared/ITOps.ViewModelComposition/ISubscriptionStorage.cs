using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;

namespace ITOps.ViewModelComposition
{
    public interface ISubscriptionStorage
    {
        void Subscribe<T>(Func<string, dynamic, T, RouteData, HttpRequest, Task> subscription);
    }
}
