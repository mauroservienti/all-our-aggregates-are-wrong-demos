using NServiceBus.AttributeConventions.Contracts;
using NServiceBus.AttributeRouting.Contracts;
using System;

namespace Warehouse.Messages
{
    [Message, RouteTo("Warehouse.Service")]
    public class CleanupFailedCartRequest
    {
        public Guid CartId { get; set; }
        public string RequestId { get; set; }
    }
}
