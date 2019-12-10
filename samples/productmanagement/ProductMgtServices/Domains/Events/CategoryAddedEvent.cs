using System;
using SimpleKit.Domain.Events;

namespace ProductMgtServices.Domains.Events
{
    public class CategoryAddedEvent : IDomainEvent
    {
        public Guid EventId { get; set; }
    }
}