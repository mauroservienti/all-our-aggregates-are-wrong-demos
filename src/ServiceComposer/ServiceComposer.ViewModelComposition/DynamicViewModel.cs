using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace ServiceComposer.ViewModelComposition
{
    class DynamicViewModel : DynamicObject, ISubscriptionStorage
    {
        string requestId;
        RouteData routeData;
        HttpRequest request;
        IDictionary<Type, IList<Subscription>> subscriptions = new Dictionary<Type, IList<Subscription>>();
        IDictionary<string, object> properties = new Dictionary<string, object>();

        public DynamicViewModel(string requestId, RouteData routeData, HttpRequest request)
        {
            this.requestId = requestId;
            this.routeData = routeData;
            this.request = request;
        }

        public void CleanupSubscribers() => subscriptions.Clear();

        public void Subscribe<T>(Func<string, dynamic, T, RouteData, HttpRequest, Task> subscription)
        {
            if (!subscriptions.TryGetValue(typeof(T), out IList<Subscription> subscribers))
            {
                subscribers = new List<Subscription>();
                subscriptions.Add(typeof(T), subscribers);
            }

            subscribers.Add(new Subscription<T>(subscription));
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result) => properties.TryGetValue(binder.Name, out result);

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            properties[binder.Name] = value;
            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = null;

            if (binder.Name == "RaiseEvent")
            {
                result = this.RaiseEvent(args[0]);
                return true;
            }

            return false;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            foreach (var item in properties.Keys)
            {
                yield return item;
            }

            yield return "RaiseEvent";
        }

        public Task RaiseEvent(object @event)
        {
            if (subscriptions.TryGetValue(@event.GetType(), out IList<Subscription> subscribers))
            {
                var tasks = new List<Task>();
                foreach (var subscriber in subscribers)
                {
                    tasks.Add(subscriber.Invoke(requestId, this, @event, routeData, request));
                }

                return Task.WhenAll(tasks);
            }

            return Task.CompletedTask;
        }
    }
}
