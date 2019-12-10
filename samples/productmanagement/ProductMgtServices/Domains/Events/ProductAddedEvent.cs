using System;
using SimpleKit.Domain.Events;

namespace ProductMgtServices.Domains.Events
{
    public class ProductAddedEvent : IDomainEvent
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
        public Product Product { get; set; }
    }

    public class ProductChangedNameEvent : IDomainEvent
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
        public string OldName { get; set; }
        public string NewName { get; set; }
    }

    public class ProductAssignedCategoryEvent : IDomainEvent
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
        public long CategoryId { get; set; }
    }
}