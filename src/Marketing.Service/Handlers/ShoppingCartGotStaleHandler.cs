using NServiceBus;
using Sales.Messages.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketing.Service.Handlers
{
    class ShoppingCartGotStaleHandler : IHandleMessages<ShoppingCartGotStale>
    {
        public Task Handle(ShoppingCartGotStale message, IMessageHandlerContext context)
        {
            Console.WriteLine("Cart got stale, let's annoy user with an email :-)");

            return Task.CompletedTask;
        }
    }
}
