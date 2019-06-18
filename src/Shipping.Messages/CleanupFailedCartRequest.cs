using NServiceBus.AttributeConventions.Contracts;
using System;

namespace Shipping.Messages
{
    [Message]
    public class CleanupFailedCartRequest
    {
        public Guid CartId { get; set; }
        public string RequestId { get; set; }
    }
}
