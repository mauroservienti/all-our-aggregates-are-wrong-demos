using System;
using System.Collections.Generic;

namespace Sales.ViewModelComposition.Events
{
    public class ShoppingCartItemsLoaded
    {
        public Guid CartId { get; set; }
        public IDictionary<dynamic, dynamic> CartItemsViewModel { get; set; }
    }
}
