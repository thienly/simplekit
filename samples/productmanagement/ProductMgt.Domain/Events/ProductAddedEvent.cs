using System;
using SimpleKit.Domain.Events;

namespace ProductMgt.Domain.Events
{
    public class ProductAddedEvent : IDomainEvent
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
        public Guid ProductId { get; set; }
    }
}