using System;
using SimpleKit.Domain.Events;

namespace ProductMgt.Domain.Events
{
    public class CategoryAddedEvent : IDomainEvent
    {
        public Guid EventId { get; set; }
    }
}