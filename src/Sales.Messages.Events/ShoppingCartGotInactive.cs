using NServiceBus.AttributeConventions.Contracts;
using System;

namespace Sales.Messages.Events
{
    [Event]
    public interface ShoppingCartGotInactive
    {
        Guid CartId { get; set; }
    }
}
