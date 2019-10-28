using System;
using System.Collections.Generic;

namespace Sales.ViewModelComposition.Messages
{
    public class AddToCartRequest
    {
        public Dictionary<string, string> RequestData { get; set; }
        public string RequestId { get; set; }
        public Guid CartId { get; set; }
    }
}