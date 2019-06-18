using NServiceBus.AttributeConventions.Contracts;
using System;

namespace Sales.Messages.Events
{
    [Event]
    public interface ShoppingCartGotStale
    {
        Guid CartId { get; set; }
    }
}
