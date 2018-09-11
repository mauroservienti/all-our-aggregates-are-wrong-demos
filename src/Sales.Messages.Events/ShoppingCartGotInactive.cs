using System;

namespace Sales.Messages.Events
{
    public interface ShoppingCartGotInactive
    {
        Guid CartId { get; set; }
    }
}
