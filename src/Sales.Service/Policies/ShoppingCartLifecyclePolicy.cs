using NServiceBus;
using Sales.Messages.Events;
using System;
using System.Threading.Tasks;

namespace Sales.Service.Policies
{
    class ShoppingCartLifecyclePolicy : 
        Saga<ShoppingCartLifecyclePolicy.ShoppingCartLifecyclePolicyData>,
        IAmStartedByMessages<ProductAddedToCart>,
        IHandleTimeouts<CartGettingStaleTimeout>,
        IHandleTimeouts<CartWipeTimeout>
    {
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<ShoppingCartLifecyclePolicyData> mapper)
        {
            mapper.MapSaga(data => data.CartId).ToMessage<ProductAddedToCart>(msg => msg.CartId);
        }

        public class ShoppingCartLifecyclePolicyData : ContainSagaData
        {
            public Guid CartId { get; set; }
            public DateTime LastTouched { get; set; }
        }

        public async Task Handle(ProductAddedToCart message, IMessageHandlerContext context)
        {
            Data.LastTouched = DateTime.Now;

            await RequestTimeout(context, Data.LastTouched.AddSeconds(30), new CartGettingStaleTimeout()
            {
                LastTouched = Data.LastTouched
            });

            await RequestTimeout(context, Data.LastTouched.AddSeconds(60), new CartWipeTimeout()
            {
                LastTouched = Data.LastTouched
            });
        }

        public async Task Timeout(CartGettingStaleTimeout state, IMessageHandlerContext context)
        {
            if (Data.LastTouched <= state.LastTouched)
            {
                await context.Publish<ShoppingCartGotStale>(e => e.CartId = Data.CartId);
            }
        }

        public async Task Timeout(CartWipeTimeout state, IMessageHandlerContext context)
        {
            if (Data.LastTouched <= state.LastTouched)
            {
                await context.Publish<ShoppingCartGotInactive>(e => e.CartId = Data.CartId);
                MarkAsComplete();
            }
        }
    }

    class CartGettingStaleTimeout
    {
        public DateTime LastTouched { get; set; }
    }

    class CartWipeTimeout
    {
        public DateTime LastTouched { get; set; }
    }
}
