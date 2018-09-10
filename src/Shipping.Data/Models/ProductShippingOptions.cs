using System.Collections.Generic;

namespace Shipping.Data.Models
{
    public class ProductShippingOptions
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public List<ShippingOption> Options { get; set; } = new List<ShippingOption>();
    }

    public class ShippingOption
    {
        public int Id { get; set; }
        public int ProductShippingOptionsId { get; set; }
        public string Option { get; set; }
        public int EstimatedMinDeliveryDays { get; set; }
        public int EstimatedMaxDeliveryDays { get; set; }
    }
}
