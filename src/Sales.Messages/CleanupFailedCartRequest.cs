using System;

namespace Sales.Messages
{
    public class CleanupFailedCartRequest
    {
        public Guid CartId { get; set; }
        public string RequestId { get; set; }
    }
}
