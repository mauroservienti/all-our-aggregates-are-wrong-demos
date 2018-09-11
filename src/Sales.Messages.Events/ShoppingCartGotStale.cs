using System;

namespace Sales.Messages.Events
{
    public interface ShoppingCartGotStale
    {
        Guid CartId { get; set; }
    }
}
