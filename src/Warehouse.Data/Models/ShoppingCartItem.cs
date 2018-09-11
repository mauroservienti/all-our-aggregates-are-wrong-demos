using System;

namespace Warehouse.Data.Models
{
    public class ShoppingCartItem
    {
        public int Id { get; set; }
        public Guid CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int Inventory { get; set; }
        public string RequestId { get; set; }
    }
}
