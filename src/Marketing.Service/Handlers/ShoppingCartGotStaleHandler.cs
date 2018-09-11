using NServiceBus;
using Sales.Messages.Events;
using System.Drawing;
using System.Threading.Tasks;
using Console = Colorful.Console;

namespace Marketing.Service.Handlers
{
    class ShoppingCartGotStaleHandler : IHandleMessages<ShoppingCartGotStale>
    {
        public Task Handle(ShoppingCartGotStale message, IMessageHandlerContext context)
        {
            Console.WriteLine("Cart got stale, let's annoy user with an email :-)", Color.Yellow);

            return Task.CompletedTask;
        }
    }
}
