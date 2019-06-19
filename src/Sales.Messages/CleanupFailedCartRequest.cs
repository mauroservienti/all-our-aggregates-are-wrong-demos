using NServiceBus.AttributeConventions.Contracts;
using NServiceBus.AttributeRouting.Contracts;
using System;

namespace Sales.Messages
{
    [Message, RouteTo("Sales.Service")]
    public class CleanupFailedCartRequest
    {
        public Guid CartId { get; set; }
        public string RequestId { get; set; }
    }
}
