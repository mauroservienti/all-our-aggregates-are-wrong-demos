using System;

namespace Shipping.Data.Models
{
    public class ShoppingCartItem
    {
        public int Id { get; set; }
        public Guid CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string DeliveryEstimate { get; set; }
        public string RequestId { get; set; }
    }
}
