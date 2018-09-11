using System;

namespace Sales.Messages.Events
{
    public interface ProductAddedToCart
    {
        Guid CartId { get; set; }
        int ProductId { get; set; }
    }
}
