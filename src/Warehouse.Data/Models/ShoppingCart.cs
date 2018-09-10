using System;
using System.Collections.Generic;

namespace Warehouse.Data.Models
{
    public class ShoppingCart
    {
        public Guid Id { get; set; }
        public List<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();
    }

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
