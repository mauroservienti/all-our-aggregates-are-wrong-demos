﻿using NServiceBus.AttributeConventions.Contracts;
using NServiceBus.AttributeRouting.Contracts;
using System;

namespace Warehouse.Messages.Commands
{
    [Command, RouteTo("Warehouse.Service")]
    public class AddItemToCart
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public Guid CartId { get; set; }
        public string RequestId { get; set; }
    }
}
