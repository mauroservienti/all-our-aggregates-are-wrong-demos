using System.Collections.Generic;

namespace Sales.ViewModelComposition.Events
{
    public class AddItemToCartRequested
    {
        public string RequestId { get; set; }
        public string CartId { get; set; }
        public Dictionary<string, string> RequestData { get; set; }
    }
}