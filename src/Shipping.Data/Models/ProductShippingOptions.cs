using System.Collections.Generic;

namespace Shipping.Data.Models
{
    public class ProductShippingOptions
    {
        public ProductShippingOptions()
        {
            this.Options = new List<ShippingOption>();
        }

        public int Id { get; set; }

        public List<ShippingOption> Options { get; set; }
    }

    public class ShippingOption
    {
        public int Id { get; set; }

        public int ProductShippingOptionsId { get; set; }

        public string Option { get; set; }
    }
}
