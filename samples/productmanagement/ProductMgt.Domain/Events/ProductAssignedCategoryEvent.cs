using System;
using SimpleKit.Domain.Events;

namespace ProductMgt.Domain.Events
{
    public class ProductAssignedCategoryEvent : IDomainEvent
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
        public long CategoryId { get; set; }
    }
}