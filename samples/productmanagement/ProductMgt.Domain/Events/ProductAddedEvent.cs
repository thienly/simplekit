using System;
using SimpleKit.Domain.Events;

namespace ProductMgt.Domain.Events
{
    public class ProductAddedEvent : IntegrationEvent, IDomainEvent
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
        public long ProductId { get; set; }
    }
}