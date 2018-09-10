using System;

namespace Sales.Messages.Events
{
    public interface ItemAddedToCart
    {
        Guid CartId { get; set; }
        int ItemId { get; set; }
    }
}
