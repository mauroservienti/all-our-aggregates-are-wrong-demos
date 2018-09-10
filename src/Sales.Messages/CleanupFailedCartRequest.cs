using System;

namespace Sales.Messages
{
    public class CleanupFailedCartRequest
    {
        public int ProductId { get; set; }
        public Guid CartId { get; set; }
        public string RequestId { get; set; }
    }
}
