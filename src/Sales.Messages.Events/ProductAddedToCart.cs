using NServiceBus.AttributeConventions.Contracts;
using System;

namespace Sales.Messages.Events
{
    [Event]
    public interface ProductAddedToCart
    {
        Guid CartId { get; set; }
        int ProductId { get; set; }
    }
}
