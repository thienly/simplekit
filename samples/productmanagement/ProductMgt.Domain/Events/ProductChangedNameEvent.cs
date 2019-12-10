using System;
using SimpleKit.Domain.Events;

namespace ProductMgt.Domain.Events
{
    public class ProductChangedNameEvent : IDomainEvent
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
        public string OldName { get; set; }
        public string NewName { get; set; }
    }
}