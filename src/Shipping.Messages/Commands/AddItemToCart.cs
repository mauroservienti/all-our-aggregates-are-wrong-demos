using System;

namespace Shipping.Messages.Commands
{
    public class AddItemToCart
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public Guid CartId { get; set; }
        public string RequestId { get; set; }
    }
}
